using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using monogameMinecraftDX;
 
namespace monogameMinecraft
{
    public class DeferredShadingRenderer:FullScreenQuadRenderer
    {
        public MinecraftGame game;
      
        public GraphicsDevice device;
        public Effect blockDeferredEffect;
        public Effect deferredBlendEffect;
        public ShadowRenderer shadowRenderer;
        public SSAORenderer SSAORenderer;
        public SSRRenderer ssrRenderer;
        public SSIDRenderer ssidRenderer;
        public GameTimeManager gameTimeManager;
        public PointLightUpdater lightUpdater;
        public GBufferRenderer gBufferRenderer;
        public ContactShadowRenderer contactShadowRenderer;
        public RenderTarget2D renderTargetLum;
        public DeferredShadingRenderer(GraphicsDevice device, Effect blockDeferredEffect, ShadowRenderer shadowRenderer, SSAORenderer sSAORenderer, GameTimeManager gameTimeManager, PointLightUpdater lightUpdater, GBufferRenderer gBufferRenderer,ContactShadowRenderer contactShadowRenderer,SSRRenderer sSRRenderer,SSIDRenderer sSIDRenderer,Effect deferredBlendEffect)
        {
            this.device = device;
            this.blockDeferredEffect = blockDeferredEffect;
            this.shadowRenderer = shadowRenderer;
            SSAORenderer = sSAORenderer;
           
            this.gameTimeManager = gameTimeManager;
            this.lightUpdater = lightUpdater;
            this.gBufferRenderer = gBufferRenderer;
            this.contactShadowRenderer = contactShadowRenderer;
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            this.renderTargetLum = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            this.ssrRenderer = sSRRenderer;
            this.ssidRenderer=sSIDRenderer;
            this.deferredBlendEffect= deferredBlendEffect;
            InitializeVertices();
            InitializeQuadBuffers(device);
        }

        public void Draw(GamePlayer player)
        {
           
            SetCameraFrustum(player.cam, blockDeferredEffect);
         //   blockDeferredEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
          //  blockDeferredEffect.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            blockDeferredEffect.Parameters["fogStart"].SetValue(256.0f);
            blockDeferredEffect.Parameters["fogRange"].SetValue(1024.0f);
            blockDeferredEffect.Parameters["metallic"].SetValue(GlobalMaterialParamsManager.instance.metallic);
            blockDeferredEffect.Parameters["roughness"].SetValue(GlobalMaterialParamsManager.instance.roughness);
            blockDeferredEffect.Parameters["LightColor"].SetValue(new Vector3(10,10, 10));
            blockDeferredEffect.Parameters["LightDir"].SetValue(gameTimeManager.sunDir);
            //  basicShader.Parameters["LightPos"].SetValue(player.playerPos + new Vector3(10, 50, 30));
            blockDeferredEffect.Parameters["View"]?.SetValue(player.cam.viewMatrix);
            blockDeferredEffect.Parameters["viewPos"].SetValue(player.cam.position);
            if (blockDeferredEffect.Parameters["PositionWSTex"] != null) { blockDeferredEffect.Parameters["PositionWSTex"].SetValue(gBufferRenderer.renderTargetPositionWS); }
            // shadowmapShader.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
            //     RenderShadow(RenderingChunks, player,lightSpaceMat);
            if (blockDeferredEffect.Parameters["NoiseTex"] != null) { blockDeferredEffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }
            blockDeferredEffect.Parameters["TextureAO"]?.SetValue(SSAORenderer.ssaoTarget);
            blockDeferredEffect.Parameters["TextureNormals"].SetValue(gBufferRenderer.renderTargetNormalWS);
            blockDeferredEffect.Parameters["TextureAlbedo"].SetValue(gBufferRenderer.renderTargetAlbedo);
            // blockDeferredEffect.Parameters["TextureDepth"].SetValue(gBufferRenderer.renderTargetProjectionDepth);
            blockDeferredEffect.Parameters["TextureContactShadow"].SetValue(contactShadowRenderer.contactShadowRenderTarget);
           
            //    blockDeferredEffect.Parameters["receiveAO"].SetValue(true);
            blockDeferredEffect.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
            blockDeferredEffect.Parameters["LightSpaceMatFar"].SetValue(shadowRenderer.lightSpaceMatFar);
            blockDeferredEffect.Parameters["ShadowMap"].SetValue(shadowRenderer.shadowMapTarget);
            blockDeferredEffect.Parameters["ShadowMapFar"].SetValue(shadowRenderer.shadowMapTargetFar);
            blockDeferredEffect.Parameters["shadowBias"].SetValue(shadowRenderer.shadowBias);
            for (int i = 0; i < lightUpdater.lights.Count; i++)
            {
                blockDeferredEffect.Parameters["LightPosition" + (i + 1).ToString()].SetValue(lightUpdater.lights[i]);
            }
            Vector3 lightPosition1 = blockDeferredEffect.Parameters["LightPosition1"].GetValueVector3();
            Vector3 lightPosition2 = blockDeferredEffect.Parameters["LightPosition2"].GetValueVector3();
            Vector3 lightPosition3 = blockDeferredEffect.Parameters["LightPosition3"].GetValueVector3();
            Vector3 lightPosition4 = blockDeferredEffect.Parameters["LightPosition4"].GetValueVector3();
            //    Debug.WriteLine(lightPosition1);
            foreach (var lightD in lightUpdater.lightsDestroying)
            {

                if (lightD.Equals(lightPosition1))
                {
                    blockDeferredEffect.Parameters["LightPosition1"].SetValue(new Vector3(0, 0, 0));
                    Debug.WriteLine("destroy");
                }
                if (lightD.Equals(lightPosition2))
                {
                    blockDeferredEffect.Parameters["LightPosition2"].SetValue(new Vector3(0, 0, 0));
                }
                if (lightD.Equals(lightPosition3))
                {
                    blockDeferredEffect.Parameters["LightPosition3"].SetValue(new Vector3(0, 0, 0));
                }
                if (lightD.Equals(lightPosition4))
                {
                    blockDeferredEffect.Parameters["LightPosition4"].SetValue(new Vector3(0, 0, 0));
                }
            }
        //    blockDeferredEffect.Parameters["receiveReflection"].SetValue(false);
        //    blockDeferredEffect.Parameters["receiveBackLight"].SetValue(false);
            if (gameTimeManager.sunX > 160f || gameTimeManager.sunX <= 20f)
            {
                blockDeferredEffect.Parameters["receiveShadow"].SetValue(false);

            }
            else
            {
                blockDeferredEffect.Parameters["receiveShadow"].SetValue(true);

            }
            RenderQuad(device, renderTargetLum, blockDeferredEffect,false,false) ;
            
        }


        public void FinalBlend(SpriteBatch sb,VolumetricLightRenderer vlr,GraphicsDevice device)
        {

            deferredBlendEffect.Parameters["TextureAlbedo"].SetValue(gBufferRenderer.renderTargetAlbedo);
            deferredBlendEffect.Parameters["TextureDeferredLum"].SetValue(renderTargetLum);
            deferredBlendEffect.Parameters["TextureAO"].SetValue(SSAORenderer.ssaoTarget);
            deferredBlendEffect.Parameters["TextureReflection"].SetValue(ssrRenderer.renderTargetSSR);
            deferredBlendEffect.Parameters["TextureIndirectDiffuse"].SetValue(ssidRenderer.renderTargetSSID);
            RenderQuad(device, null, deferredBlendEffect, false, true);
            sb.Begin(blendState:BlendState.Additive);
            sb.Draw(vlr.lightShaftTarget, new Rectangle(0, 0, device.PresentationParameters.BackBufferWidth , device.PresentationParameters.BackBufferHeight), Color.White);
            sb.End();
        }
    }
}
