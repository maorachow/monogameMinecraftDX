﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using monogameMinecraftDX.Updateables;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftShared.Rendering
{
    public class ContactShadowRenderer : FullScreenQuadRenderer
    {

        public GraphicsDevice device;
        public Effect contactShadowEffect;
        public GBufferRenderer gBufferRenderer;
        public GameTimeManager gameTimeManager;
        public IGamePlayer player;
        public RenderTarget2D contactShadowRenderTarget;
        public ContactShadowRenderer(GraphicsDevice device, Effect contactShadowEffect, GBufferRenderer gBufferRenderer, GameTimeManager gameTimeManager, IGamePlayer player)
        {
            this.device = device;
            this.contactShadowEffect = contactShadowEffect;
            this.gBufferRenderer = gBufferRenderer;
            this.gameTimeManager = gameTimeManager;
            InitializeVertices();
            InitializeQuadBuffers(device);
            this.player = player;
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            contactShadowRenderTarget = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
        }


        public void Draw()
        {
            if (GameOptions.renderContactShadow == false)
            {
                RenderQuad(device, contactShadowRenderTarget, null, true, false);
                return;
            }
            SetCameraFrustum(player.cam, contactShadowEffect);
            var cam = player.cam;
            int width = gBufferRenderer. renderTargetProjectionDepth.Width;
            int height = gBufferRenderer.renderTargetProjectionDepth.Height;
            if (contactShadowEffect.Parameters["ProjectionDepthTex"] != null) { contactShadowEffect.Parameters["ProjectionDepthTex"].SetValue(gBufferRenderer.renderTargetProjectionDepth); }
            if (contactShadowEffect.Parameters["NoiseTex"] != null) { contactShadowEffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }
            if (contactShadowEffect.Parameters["NormalTex"] != null) { contactShadowEffect.Parameters["NormalTex"].SetValue(gBufferRenderer.renderTargetNormalWS); }
            contactShadowEffect.Parameters["PixelSize"]?.SetValue(new Vector2(1f / width, 1f / height));
            //   if (contactShadowEffect.Parameters["PositionWSTex"] != null) { contactShadowEffect.Parameters["PositionWSTex"].SetValue(gBufferRenderer.renderTargetPositionWS); }
            if (contactShadowEffect.Parameters["View"] != null) { contactShadowEffect.Parameters["View"].SetValue(cam.viewMatrix); }
            if (contactShadowEffect.Parameters["ViewOrigin"] != null) { contactShadowEffect.Parameters["ViewOrigin"].SetValue(cam.viewMatrixOrigin); }
          
            if (contactShadowEffect.Parameters["CameraPos"] != null) { contactShadowEffect.Parameters["CameraPos"].SetValue(cam.position); }
            if (contactShadowEffect.Parameters["ViewProjection"] != null) { contactShadowEffect.Parameters["ViewProjection"].SetValue(cam.viewMatrix * cam.projectionMatrix); }
            if (contactShadowEffect.Parameters["Projection"] != null) { contactShadowEffect.Parameters["Projection"].SetValue(cam.projectionMatrix); }
            if (contactShadowEffect.Parameters["LightDir"] != null) { contactShadowEffect.Parameters["LightDir"].SetValue(gameTimeManager.sunDir); }
            RenderQuad(device, contactShadowRenderTarget, contactShadowEffect);
        }
    }
}
