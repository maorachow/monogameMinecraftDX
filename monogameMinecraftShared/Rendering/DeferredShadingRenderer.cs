using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using monogameMinecraftDX.Updateables;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
using System.Collections.Generic;
using System.Diagnostics;

namespace monogameMinecraftShared.Rendering
{
    public class DeferredShadingRenderer : FullScreenQuadRenderer
    {
        public MinecraftGameBase game;

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
        public RenderTarget2D renderTargetLumAllDiffuse;
        public RenderTarget2D renderTargetLumSpec;


        public Effect transparentBlockDeferredEffect;
        public RenderTarget2D renderTargetLumTransparent;
     //   public RenderTarget2D renderTargetLumAllDiffuseTransparent;
        public RenderTarget2D renderTargetLumSpecTransparent;
        public SkyboxRenderer skyboxRenderer;
        public RenderTarget2D finalImage;
        public FXAARenderer fxaaRenderer;
        public MotionBlurRenderer motionBlurRenderer;
        public List<CustomPostProcessor> customPostProcessors;
        public HDRCubemapRenderer hdrCubemapRenderer;
        public DeferredShadingRenderer(GraphicsDevice device, Effect blockDeferredEffect, Effect transparentBlockDeferredEffect, ShadowRenderer shadowRenderer, SSAORenderer sSAORenderer, GameTimeManager gameTimeManager, PointLightUpdater lightUpdater, GBufferRenderer gBufferRenderer, ContactShadowRenderer contactShadowRenderer, SSRRenderer sSRRenderer, SSIDRenderer sSIDRenderer, Effect deferredBlendEffect, SkyboxRenderer skyboxRenderer, FXAARenderer fxaaRenderer, MotionBlurRenderer motionBlurRenderer, HDRCubemapRenderer hdrCubemapRenderer)
        {
            this.device = device;
            this.blockDeferredEffect = blockDeferredEffect;
            this.transparentBlockDeferredEffect= transparentBlockDeferredEffect;
            this.shadowRenderer = shadowRenderer;
            SSAORenderer = sSAORenderer;

            this.gameTimeManager = gameTimeManager;
            this.lightUpdater = lightUpdater;
            this.gBufferRenderer = gBufferRenderer;
            this.contactShadowRenderer = contactShadowRenderer;
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            renderTargetLum = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetLumAllDiffuse = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetLumSpec = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);

            renderTargetLumTransparent = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
          //  renderTargetLumAllDiffuseTransparent = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetLumSpecTransparent = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            finalImage = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            ssrRenderer = sSRRenderer;
            ssidRenderer = sSIDRenderer;
            this.deferredBlendEffect = deferredBlendEffect;
            this.skyboxRenderer = skyboxRenderer;
            InitializeVertices();
            InitializeQuadBuffers(device);
            this.fxaaRenderer = fxaaRenderer;
            this.motionBlurRenderer = motionBlurRenderer;
            this.hdrCubemapRenderer = hdrCubemapRenderer;
        }

        public void Draw(IGamePlayer player)
        {

            SetCameraFrustum(player.cam, blockDeferredEffect);
            blockDeferredEffect.CurrentTechnique = blockDeferredEffect.Techniques["DeferredBlockEffectP"];
            //   blockDeferredEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
            //  blockDeferredEffect.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            blockDeferredEffect.Parameters["fogStart"]?.SetValue(256.0f);
            blockDeferredEffect.Parameters["fogRange"]?.SetValue(1024.0f);
            blockDeferredEffect.Parameters["metallic"]?.SetValue(GlobalMaterialParamsManager.instance.metallic);
      //      blockDeferredEffect.Parameters["roughness"]?.SetValue(GlobalMaterialParamsManager.instance.roughness);
            blockDeferredEffect.Parameters["LightColor"].SetValue(new Vector3(15, 15, 15));
            blockDeferredEffect.Parameters["LightDir"].SetValue(gameTimeManager.sunDir);
            blockDeferredEffect.Parameters["TextureMER"].SetValue(gBufferRenderer.renderTargetMER);
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
            blockDeferredEffect.Parameters["TextureContactShadow"]?.SetValue(contactShadowRenderer.contactShadowRenderTarget);
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
            if (gameTimeManager.sunX > 170f || gameTimeManager.sunX <= 10f)
            {
                blockDeferredEffect.Parameters["receiveShadow"].SetValue(false);

            }
            else
            {
                blockDeferredEffect.Parameters["receiveShadow"].SetValue(true);

            }
            RenderQuad(device, renderTargetLum, renderTargetLumSpec, blockDeferredEffect);


            List<RenderTargetBinding[]> transparentBufferBindings = new List<RenderTargetBinding[]>
                { gBufferRenderer.bindingTrans0, gBufferRenderer.bindingTrans1, gBufferRenderer.bindingTrans2 };
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.None;
            device.SetRenderTargets(renderTargetLumTransparent,renderTargetLumSpecTransparent);
            device.Clear(Color.Transparent);
           
            for (int i = transparentBufferBindings.Count - 1; i >= 0; i--)
            {

                SetCameraFrustum(player.cam, transparentBlockDeferredEffect);
                transparentBlockDeferredEffect.CurrentTechnique = transparentBlockDeferredEffect.Techniques["DeferredBlockEffectPTransparent"];
                //   blockDeferredEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
                //  blockDeferredEffect.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
                transparentBlockDeferredEffect.Parameters["fogStart"]?.SetValue(256.0f);
                transparentBlockDeferredEffect.Parameters["fogRange"]?.SetValue(1024.0f);
                transparentBlockDeferredEffect.Parameters["metallic"]?.SetValue(GlobalMaterialParamsManager.instance.metallic);
                //      blockDeferredEffect.Parameters["roughness"]?.SetValue(GlobalMaterialParamsManager.instance.roughness);
                transparentBlockDeferredEffect.Parameters["LightColor"].SetValue(new Vector3(15, 15, 15));
                transparentBlockDeferredEffect.Parameters["LightDir"].SetValue(gameTimeManager.sunDir);
                transparentBlockDeferredEffect.Parameters["TextureMER"].SetValue(transparentBufferBindings[i][3].RenderTarget);
                //  basicShader.Parameters["LightPos"].SetValue(player.position + new Vector3(10, 50, 30));
                transparentBlockDeferredEffect.Parameters["View"]?.SetValue(player.cam.viewMatrix);
                transparentBlockDeferredEffect.Parameters["viewPos"]?.SetValue(player.cam.position);
                //   if (blockDeferredEffect.Parameters["PositionWSTex"] != null) { blockDeferredEffect.Parameters["PositionWSTex"].SetValue(gBufferRenderer.renderTargetPositionWS); }
                // shadowmapShader.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
                //     RenderShadow(RenderingChunks, player,lightSpaceMat);
                if (transparentBlockDeferredEffect.Parameters["NoiseTex"] != null) { transparentBlockDeferredEffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }
                transparentBlockDeferredEffect.Parameters["TextureAO"]?.SetValue(SSAORenderer.ssaoTarget);
                transparentBlockDeferredEffect.Parameters["TextureNormals"]?.SetValue(transparentBufferBindings[i][1].RenderTarget);
                transparentBlockDeferredEffect.Parameters["TextureAlbedo"]?.SetValue(transparentBufferBindings[i][2].RenderTarget);
                transparentBlockDeferredEffect.Parameters["TextureDepth"]?.SetValue(transparentBufferBindings[i][0].RenderTarget);
                transparentBlockDeferredEffect.Parameters["TextureContactShadow"]?.SetValue(contactShadowRenderer.contactShadowRenderTarget);
                transparentBlockDeferredEffect.Parameters["HDRIrradianceTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultIrradianceCubemap);
                transparentBlockDeferredEffect.Parameters["HDRIrradianceTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultIrradianceCubemap);
                transparentBlockDeferredEffect.Parameters["HDRPrefilteredTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultSpecularCubemapMip0);
                transparentBlockDeferredEffect.Parameters["HDRPrefilteredTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultSpecularCubemapMip0);
                transparentBlockDeferredEffect.Parameters["LUTTex"]?.SetValue(BRDFLUTRenderer.instance.renderTargetLUT);
                transparentBlockDeferredEffect.Parameters["mixValue"]?.SetValue(gameTimeManager.skyboxMixValue);
                //    blockDeferredEffect.Parameters["receiveAO"].SetValue(true);
                transparentBlockDeferredEffect.Parameters["LightSpaceMat"]?.SetValue(shadowRenderer.lightSpaceMat);
                transparentBlockDeferredEffect.Parameters["LightSpaceMatFar"]?.SetValue(shadowRenderer.lightSpaceMatFar);
                transparentBlockDeferredEffect.Parameters["ShadowMap"]?.SetValue(shadowRenderer.shadowMapTarget);
                transparentBlockDeferredEffect.Parameters["ShadowMapFar"]?.SetValue(shadowRenderer.shadowMapTargetFar);
                transparentBlockDeferredEffect.Parameters["shadowBias"]?.SetValue(shadowRenderer.shadowBias);
                transparentBlockDeferredEffect.Parameters["LightPositions"].SetValue(lightUpdater.lights.ToArray());
               
                if (gameTimeManager.sunX > 170f || gameTimeManager.sunX <= 10f)
                {
                    transparentBlockDeferredEffect.Parameters["receiveShadow"].SetValue(false);

                }
                else
                {
                    transparentBlockDeferredEffect.Parameters["receiveShadow"].SetValue(true);

                }
                RenderQuad(device, renderTargetLumTransparent, renderTargetLumSpecTransparent, transparentBlockDeferredEffect,false,false);
            }
            device.SetRenderTarget(null);
            device.Clear(Color.CornflowerBlue);
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
        }

        public void DiffuseBlend(IGamePlayer player)
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




        /*    List<RenderTargetBinding[]> transparentBufferBindings = new List<RenderTargetBinding[]>
                { gBufferRenderer.bindingTrans0, gBufferRenderer.bindingTrans1, gBufferRenderer.bindingTrans2 };
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.None;
            device.SetRenderTargets(renderTargetLumAllDiffuseTransparent);
            device.Clear(Color.Transparent);

            for (int i = transparentBufferBindings.Count - 1; i >= 0; i--)
            {

                SetCameraFrustum(player.cam, transparentBlockDeferredEffect);
                transparentBlockDeferredEffect.CurrentTechnique = transparentBlockDeferredEffect.Techniques["DeferredBlockEffectDiffuseTransparent"];
                transparentBlockDeferredEffect.Parameters["TextureMER"]?.SetValue(transparentBufferBindings[i][3].RenderTarget);
                transparentBlockDeferredEffect.Parameters["TextureAO"]?.SetValue(SSAORenderer.ssaoTarget);
                transparentBlockDeferredEffect.Parameters["TextureNormals"]?.SetValue(transparentBufferBindings[i][1].RenderTarget);
                transparentBlockDeferredEffect.Parameters["TextureAlbedo"]?.SetValue(transparentBufferBindings[i][2].RenderTarget);
                transparentBlockDeferredEffect.Parameters["TextureDepth"]?.SetValue(transparentBufferBindings[i][0].RenderTarget);
                transparentBlockDeferredEffect.Parameters["HDRIrradianceTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultIrradianceCubemap);
                transparentBlockDeferredEffect.Parameters["HDRIrradianceTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultIrradianceCubemap);
                transparentBlockDeferredEffect.Parameters["TextureDeferredLumDirect"].SetValue(renderTargetLumTransparent);

                transparentBlockDeferredEffect.Parameters["TextureIndirectDiffuse"]?.SetValue(ssidRenderer.renderTargetSSID);
                RenderQuad(device, renderTargetLumAllDiffuseTransparent, transparentBlockDeferredEffect, false, false);
            }
            device.SetRenderTarget(null);
            device.Clear(Color.CornflowerBlue);
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;*/
        }

        public void Resize(int width, int height)
        {
            renderTargetLum = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            finalImage = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetLumSpec = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                renderTargetLumTransparent = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                renderTargetLumSpecTransparent = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetLumAllDiffuse = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
        }
        public void FinalBlend(SpriteBatch sb, VolumetricLightRenderer vlr, GraphicsDevice device, IGamePlayer player)
        {
            skyboxRenderer.Draw(finalImage, true);
            SetCameraFrustum(player.cam, deferredBlendEffect);

            //    deferredBlendEffect.Parameters["LightColor"].SetValue(new Vector3(10, 10, 10));
            //   deferredBlendEffect.Parameters["LightDir"].SetValue(gameTimeManager.sunDir);
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.None;
            deferredBlendEffect.Parameters["TextureDepth"]?.SetValue(gBufferRenderer.renderTargetProjectionDepth);
            deferredBlendEffect.Parameters["viewPos"]?.SetValue(player.cam.position);
            deferredBlendEffect.Parameters["TextureNormals"]?.SetValue(gBufferRenderer.renderTargetNormalWS);
            deferredBlendEffect.Parameters["HDRIrradianceTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultIrradianceCubemap);
            deferredBlendEffect.Parameters["HDRPrefilteredTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultSpecularCubemapMip0);
            deferredBlendEffect.Parameters["HDRPrefilteredTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultSpecularCubemapMip0);
            deferredBlendEffect.Parameters["LUTTex"]?.SetValue(BRDFLUTRenderer.instance.renderTargetLUT);
            deferredBlendEffect.Parameters["HDRIrradianceTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultIrradianceCubemap);
            deferredBlendEffect.Parameters["HDRIrradianceTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultIrradianceCubemap);
            deferredBlendEffect.Parameters["mixValue"]?.SetValue(gameTimeManager.skyboxMixValue);
         
            deferredBlendEffect.Parameters["TextureAlbedo"].SetValue(gBufferRenderer.renderTargetAlbedo);
            deferredBlendEffect.Parameters["TextureDeferredLum"].SetValue(renderTargetLumAllDiffuse);

            deferredBlendEffect.Parameters["TextureDeferredLumSpec"].SetValue(renderTargetLumSpec);

            deferredBlendEffect.Parameters["TextureDeferredLumTrans"]?.SetValue(renderTargetLumTransparent);
            deferredBlendEffect.Parameters["TextureDeferredLumSpecTrans"]?.SetValue(renderTargetLumSpecTransparent);
            deferredBlendEffect.Parameters["TextureAO"].SetValue(SSAORenderer.ssaoTarget);
            deferredBlendEffect.Parameters["TextureReflection"]?.SetValue(ssrRenderer.renderTargetSSR);
            deferredBlendEffect.Parameters["TextureIndirectDiffuse"]?.SetValue(ssidRenderer.renderTargetSSID);
            deferredBlendEffect.Parameters["TextureAlbedo"]?.SetValue(gBufferRenderer.renderTargetAlbedo);
            deferredBlendEffect.Parameters["TextureMER"]?.SetValue(gBufferRenderer.renderTargetMER);
            RenderQuad(device, finalImage, deferredBlendEffect, false, false, clearColor: false);
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
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
        }
    }
}
