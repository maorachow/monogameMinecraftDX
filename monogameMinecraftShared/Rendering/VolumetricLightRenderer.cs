﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using monogameMinecraftDX.Updateables;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
namespace monogameMinecraftShared.Rendering
{
    public class VolumetricLightRenderer : FullScreenQuadRenderer
    {
        public GraphicsDevice device;
        public GBufferRenderer gBufferRenderer;
        public RenderTarget2D blendVolumetricMap;
        public RenderTarget2D renderTargetLum;
        public SpriteBatch spriteBatch;
        public Effect blendEffect;
        public Effect lightShaftEffect;
        public RenderTarget2D lightShaftTarget;
        public IGamePlayer player;
        int width;
        int height;
        public GameTimeManager gameTimeManager;
       // public EntityRenderer entityRenderer;

        public VolumetricLightRenderer(GraphicsDevice device, GBufferRenderer gBufferRenderer, SpriteBatch sb, Effect blendEffect, Effect lightShaftEffect, IGamePlayer player, GameTimeManager gameTimeManager)
        {
            this.device = device;
            this.gBufferRenderer = gBufferRenderer;
            width = device.PresentationParameters.BackBufferWidth;
            height = device.PresentationParameters.BackBufferHeight;

            blendVolumetricMap = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetLum = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            lightShaftTarget = new RenderTarget2D(device, (int)(width / 2f), (int)(height / 2f), false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            this.lightShaftEffect = lightShaftEffect;
            spriteBatch = sb;
            this.blendEffect = blendEffect;
            this.player = player;
            this.gameTimeManager = gameTimeManager;
            InitializeVertices();
            InitializeQuadBuffers(device);

        }
        public void LightShafts(
          RenderTarget2D RenderTargetMask,

          Vector2 LightPos,
          float Density,
          float Decay,
          float Weight,
          float Exposure,
          int numSamples)
        {

            SetCameraFrustum(player.cam, lightShaftEffect);
            Effect effect = lightShaftEffect;
            //       effect.CurrentTechnique = effect.Techniques[0];
            //     effect.Parameters["ShadowmapTex"].SetValue(shadowRenderer.shadowMapTarget);
            //      effect.Parameters["ShadowmapFarTex"].SetValue(shadowRenderer.shadowMapTargetFar);
            //     effect.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
            //    effect.Parameters["LightSpaceMatFar"].SetValue(shadowRenderer.lightSpaceMatFar);
            //     effect.Parameters["ShadowmapFarTex"].SetValue(shadowRenderer.shadowMapTargetFar);
            //      effect.Parameters["DepthTex"].SetValue(gBufferRenderer.renderTargetProjectionDepth);
            effect.Parameters["maskTex"]?.SetValue(RenderTargetMask);
            effect.Parameters["gScreenLightPos"]?.SetValue(LightPos);
            effect.Parameters["gDensity"]?.SetValue(Density);
            effect.Parameters["gDecay"]?.SetValue(Decay);
            effect.Parameters["gWeight"]?.SetValue(Weight);
            effect.Parameters["gExposure"]?.SetValue(Exposure);
            effect.Parameters["NoiseTex"]?.SetValue(RandomTextureGenerator.instance.randomTex);
            //    effect.Parameters["gNumSamples"].SetValue(numSamples);
            RenderQuad(device, lightShaftTarget, effect);




        }
        public void Draw()
        {
            if (GameOptions.renderLightShaft == false)
            {
                return;
            }
            Vector4 vecZero = new Vector4(player.cam.position + gameTimeManager.sunDir, 1f);


            Vector4 projectionPos = Vector4.Transform(vecZero, player.cam.viewMatrix * player.cam.projectionMatrix);
            Vector4 screenSpaceLightPos = new Vector4(projectionPos.X / projectionPos.W, projectionPos.Y / projectionPos.W, projectionPos.Z / projectionPos.W, projectionPos.W / projectionPos.W);
            screenSpaceLightPos.Y = -screenSpaceLightPos.Y;
            screenSpaceLightPos.X *= 0.5f;
            screenSpaceLightPos.Y *= 0.5f;
            screenSpaceLightPos += new Vector4(0.5f, 0.5f, 0f, 0f);

            if (screenSpaceLightPos.Z > 1f)
            {
                screenSpaceLightPos = new Vector4(-10f, -10f, 0f, 0f);
            }


            if (gameTimeManager.sunX >= 180f && gameTimeManager.sunX < 360f)
            {
                blendEffect.Parameters["lightColor"].SetValue(Color.CornflowerBlue.ToVector4());
            }
            else
            {
                blendEffect.Parameters["lightColor"].SetValue(Color.LightYellow.ToVector4());
            }
            blendEffect.Parameters["maskTex"].SetValue(gBufferRenderer.renderTargetAlbedo);
            //  blendEffect.Parameters["backgroundTex"].SetValue(renderTargetLum);
            blendEffect.Parameters["screenSpaceLightPos"].SetValue(new Vector2(screenSpaceLightPos.X, screenSpaceLightPos.Y));
            blendEffect.Parameters["flareWeight"].SetValue(2f);
            RenderQuad(device, blendVolumetricMap, blendEffect);

            LightShafts(blendVolumetricMap, new Vector2(screenSpaceLightPos.X, screenSpaceLightPos.Y), 1.15f, 0.986f, 0.391f, 0.051f, 400);
        }



        /* public void RenderQuad(RenderTarget2D target, Effect quadEffect, bool isPureWhite = false,bool drawModel=false)
         {
             device.SetRenderTarget(target);

             if (isPureWhite)
             {
                 device.Clear(Color.White);
                 device.SetRenderTarget(null);
                 device.Clear(Color.CornflowerBlue);
                 return;
             }
             device.Clear(new Color(0,0,0,0));   
             device.SetVertexBuffer(gBufferRenderer.quadVertexBuffer);
             device.Indices = gBufferRenderer.quadIndexBuffer;
             RasterizerState rasterizerState = new RasterizerState();
             rasterizerState.CullMode = CullMode.None;
             device.RasterizerState = rasterizerState;
             foreach (var pass in quadEffect.CurrentTechnique.Passes)
             {
                 pass.Apply();
                 device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4);
             }


             //    graphicsDevice.Clear(Color.White);
             device.SetRenderTarget(null);
             device.Clear(Color.CornflowerBlue);
         }*/
    }
}
