using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace monogameMinecraftShared.Rendering
{
    public class DeferredShadingRendererLowDef : FullScreenQuadRenderer
    {
        public MinecraftGameBase game;

        public GraphicsDevice device;
        public Effect blockDeferredEffect;
        public GameTimeManager gameTimeManager;
        public SkyboxRenderer skyboxRenderer;
        public GBufferRenderer gBufferRenderer;
        public RenderTarget2D finalImage;

        public SSAORenderer SSAORenderer;
        /*   public Effect deferredBlendEffect;
            public ShadowRenderer shadowRenderer;
          
            public SSRRenderer ssrRenderer;
            public SSIDRenderer ssidRenderer;

            public PointLightUpdater lightUpdater;
           
            public ContactShadowRenderer contactShadowRenderer;
            public RenderTarget2D renderTargetLum;
            public RenderTarget2D renderTargetLumAllDiffuse;
            public RenderTarget2D renderTargetLumSpec;
         
       
            public FXAARenderer fxaaRenderer;
            public MotionBlurRenderer motionBlurRenderer;
            public List<CustomPostProcessor> customPostProcessors;
            public HDRCubemapRenderer hdrCubemapRenderer;*/
        public DeferredShadingRendererLowDef(GraphicsDevice device, Effect blockDeferredEffect, SSAORenderer sSAORenderer, GameTimeManager gameTimeManager, GBufferRenderer gBufferRenderer, SkyboxRenderer skyboxRenderer)
        {
            this.device = device;
            this.blockDeferredEffect = blockDeferredEffect;
            this.gameTimeManager = gameTimeManager;
            this.skyboxRenderer = skyboxRenderer;
            this.gBufferRenderer = gBufferRenderer;
            SSAORenderer = sSAORenderer;
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            InitializeVertices();
            InitializeQuadBuffers(device);
            finalImage = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            /*     this.shadowRenderer = shadowRenderer;
             


                 this.lightUpdater = lightUpdater;
                 this.gBufferRenderer = gBufferRenderer;
                 this.contactShadowRenderer = contactShadowRenderer;
               
                 renderTargetLum = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                 renderTargetLumAllDiffuse = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                 renderTargetLumSpec = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
               
                 ssrRenderer = sSRRenderer;
                 ssidRenderer = sSIDRenderer;
                 this.deferredBlendEffect = deferredBlendEffect;
              
             
                 this.fxaaRenderer = fxaaRenderer;
                 this.motionBlurRenderer = motionBlurRenderer;
                 this.hdrCubemapRenderer = hdrCubemapRenderer;*/
        }

        public void Draw(IGamePlayer player, SpriteBatch sb)
        {

            SetCameraFrustum(player.cam, blockDeferredEffect);
            blockDeferredEffect.CurrentTechnique = blockDeferredEffect.Techniques["DeferredBlockEffectP"];
            //   blockDeferredEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
            //  blockDeferredEffect.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            blockDeferredEffect.Parameters["fogStart"]?.SetValue(256.0f);
            blockDeferredEffect.Parameters["fogRange"]?.SetValue(1024.0f);
            blockDeferredEffect.Parameters["metallic"]?.SetValue(GlobalMaterialParamsManager.instance.metallic);
            blockDeferredEffect.Parameters["roughness"]?.SetValue(GlobalMaterialParamsManager.instance.roughness);
            blockDeferredEffect.Parameters["LightColor"]?.SetValue(new Vector3(10, 10, 10));
            blockDeferredEffect.Parameters["LightDir"]?.SetValue(gameTimeManager.sunDir);
            blockDeferredEffect.Parameters["TextureMER"]?.SetValue(gBufferRenderer.renderTargetMER);
            //  basicShader.Parameters["LightPos"].SetValue(player.position + new Vector3(10, 50, 30));
            blockDeferredEffect.Parameters["View"]?.SetValue(player.cam.viewMatrix);
            blockDeferredEffect.Parameters["viewPos"]?.SetValue(player.cam.position);
            //   if (blockDeferredEffect.Parameters["PositionWSTex"] != null) { blockDeferredEffect.Parameters["PositionWSTex"].SetValue(gBufferRenderer.renderTargetPositionWS); }
            // shadowmapShader.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
            //     RenderShadow(RenderingChunks, player,lightSpaceMat);
            if (blockDeferredEffect.Parameters["NoiseTex"] != null) { blockDeferredEffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }
            blockDeferredEffect.Parameters["TextureAO"]?.SetValue(SSAORenderer.ssaoTarget);
            blockDeferredEffect.Parameters["TextureNormals"]?.SetValue(gBufferRenderer.renderTargetNormalWS);
            blockDeferredEffect.Parameters["TextureAlbedo"]?.SetValue(gBufferRenderer.renderTargetAlbedo);
            blockDeferredEffect.Parameters["TextureDepth"]?.SetValue(gBufferRenderer.renderTargetProjectionDepth);
       /*     blockDeferredEffect.Parameters["TextureContactShadow"]?.SetValue(contactShadowRenderer.contactShadowRenderTarget);
            blockDeferredEffect.Parameters["HDRIrradianceTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultIrradianceCubemap);
            blockDeferredEffect.Parameters["HDRIrradianceTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultIrradianceCubemap);

            blockDeferredEffect.Parameters["mixValue"]?.SetValue(gameTimeManager.skyboxMixValue);
            //    blockDeferredEffect.Parameters["receiveAO"].SetValue(true);
            blockDeferredEffect.Parameters["LightSpaceMat"]?.SetValue(shadowRenderer.lightSpaceMat);
            blockDeferredEffect.Parameters["LightSpaceMatFar"]?.SetValue(shadowRenderer.lightSpaceMatFar);
            blockDeferredEffect.Parameters["ShadowMap"]?.SetValue(shadowRenderer.shadowMapTarget);
            blockDeferredEffect.Parameters["ShadowMapFar"]?.SetValue(shadowRenderer.shadowMapTargetFar);
            blockDeferredEffect.Parameters["shadowBias"]?.SetValue(shadowRenderer.shadowBias);
            blockDeferredEffect.Parameters["LightPositions"].SetValue(lightUpdater.lights.ToArray());
            /*           for (int i = 0; i < lightUpdater.lights.Count; i++)
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
                       //    blockDeferredEffect.Parameters["receiveBackLight"].SetValue(false);*/
            if (gameTimeManager.sunX > 160f || gameTimeManager.sunX <= 20f)
            {
                blockDeferredEffect.Parameters["receiveShadow"]?.SetValue(false);

            }
            else
            {
                blockDeferredEffect.Parameters["receiveShadow"]?.SetValue(true);

            }

            skyboxRenderer.Draw(finalImage, true);
            RenderQuad(device, finalImage, blockDeferredEffect,false,false,false);
            sb.Begin(blendState: BlendState.Opaque);
            sb.Draw(finalImage, new Rectangle(0, 0, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight), Color.White);
            sb.End();

        }

        /* public void DiffuseBlend(GamePlayer player)
        {
            SetCameraFrustum(player.cam, blockDeferredEffect);
            blockDeferredEffect.CurrentTechnique = blockDeferredEffect.Techniques["DeferredBlockEffectDiffuse"];
            blockDeferredEffect.Parameters["TextureAO"]?.SetValue(SSAORenderer.ssaoTarget);
            blockDeferredEffect.Parameters["TextureNormals"]?.SetValue(gBufferRenderer.renderTargetNormalWS);
            blockDeferredEffect.Parameters["TextureAlbedo"]?.SetValue(gBufferRenderer.renderTargetAlbedo);
            blockDeferredEffect.Parameters["TextureDepth"]?.SetValue(gBufferRenderer.renderTargetProjectionDepth);
            blockDeferredEffect.Parameters["HDRIrradianceTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultIrradianceCubemap);
            blockDeferredEffect.Parameters["HDRIrradianceTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultIrradianceCubemap);
            blockDeferredEffect.Parameters["TextureDeferredLumDirect"].SetValue(renderTargetLum);

            blockDeferredEffect.Parameters["TextureIndirectDiffuse"]?.SetValue(ssidRenderer.renderTargetSSID);
            RenderQuad(device, renderTargetLumAllDiffuse, blockDeferredEffect);
        }
      public void FinalBlend(SpriteBatch sb, VolumetricLightRenderer vlr, GraphicsDevice device, GamePlayer player)
        {
            skyboxRenderer.Draw(finalImage, true);
            SetCameraFrustum(player.cam, deferredBlendEffect);

            //    deferredBlendEffect.Parameters["LightColor"].SetValue(new Vector3(10, 10, 10));
            //   deferredBlendEffect.Parameters["LightDir"].SetValue(gameTimeManager.sunDir);
            deferredBlendEffect.Parameters["TextureDepth"]?.SetValue(gBufferRenderer.renderTargetProjectionDepth);
            deferredBlendEffect.Parameters["viewPos"]?.SetValue(player.cam.position);
            deferredBlendEffect.Parameters["TextureNormals"]?.SetValue(gBufferRenderer.renderTargetNormalWS);
            deferredBlendEffect.Parameters["HDRIrradianceTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultIrradianceCubemap);
            deferredBlendEffect.Parameters["HDRPrefilteredTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultSpecularCubemapMip0);
            deferredBlendEffect.Parameters["HDRPrefilteredTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultSpecularCubemapMip0);
            deferredBlendEffect.Parameters["HDRIrradianceTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultIrradianceCubemap);
            deferredBlendEffect.Parameters["HDRIrradianceTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultIrradianceCubemap);
            deferredBlendEffect.Parameters["mixValue"]?.SetValue(gameTimeManager.skyboxMixValue);
            deferredBlendEffect.Parameters["LUTTex"]?.SetValue(BRDFLUTRenderer.instance.renderTargetLUT);
            deferredBlendEffect.Parameters["TextureAlbedo"].SetValue(gBufferRenderer.renderTargetAlbedo);
            deferredBlendEffect.Parameters["TextureDeferredLum"].SetValue(renderTargetLumAllDiffuse);
            deferredBlendEffect.Parameters["TextureDeferredLumSpec"].SetValue(renderTargetLumSpec);
            deferredBlendEffect.Parameters["TextureAO"].SetValue(SSAORenderer.ssaoTarget);
            deferredBlendEffect.Parameters["TextureReflection"]?.SetValue(ssrRenderer.renderTargetSSR);
            deferredBlendEffect.Parameters["TextureIndirectDiffuse"]?.SetValue(ssidRenderer.renderTargetSSID);
            deferredBlendEffect.Parameters["TextureAlbedo"]?.SetValue(gBufferRenderer.renderTargetAlbedo);
            deferredBlendEffect.Parameters["TextureMER"]?.SetValue(gBufferRenderer.renderTargetMER);
            RenderQuad(device, finalImage, deferredBlendEffect, false, false, clearColor: false);

            motionBlurRenderer.ProcessImage(finalImage);
            // motionBlurRenderer.renderTargetMotionBlur;


            for (int i = 0; i < customPostProcessors.Count; i++)
            {
                customPostProcessors[i].cam = player.cam;
                if (i == 0)
                {
                    customPostProcessors[i].ProcessImage(motionBlurRenderer.processedImage);
                }
                else
                {
                    customPostProcessors[i].ProcessImage(customPostProcessors[i - 1].processedImage);
                }
            }
            fxaaRenderer.Draw(true, customPostProcessors[customPostProcessors.Count - 1].processedImage);
            //   sb.Begin(blendState: BlendState.Opaque);
            //    sb.Draw(finalImage, new Rectangle(0, 0, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight), Color.White);
            //   sb.End();
            sb.Begin(blendState: BlendState.Additive);
            sb.Draw(vlr.lightShaftTarget, new Rectangle(0, 0, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight), Color.White);
            sb.End();
        }*/
    }
}
