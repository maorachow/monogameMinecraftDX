using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraft;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftDX
{
    public class SSIDRenderer: FullScreenQuadRenderer
    {


        public GraphicsDevice device;
        public Effect SSIDEffect;
        public Effect textureCopyEffect;
        public GBufferRenderer gBufferRenderer;
        public RenderTarget2D renderTargetSSID;
        public RenderTarget2D renderTargetSSIDPrev;
        public DeferredShadingRenderer deferredShadingRenderer;
        public MotionVectorRenderer motionVectorRenderer;
        public GamePlayer player;
        public SSIDRenderer(GraphicsDevice device, Effect sSIDEffect, GBufferRenderer gBufferRenderer, GamePlayer player, DeferredShadingRenderer deferredShadingRenderer, Effect textureCopyEffect, MotionVectorRenderer motionVectorRenderer)
        {
            this.device = device;
            SSIDEffect = sSIDEffect;
            this.gBufferRenderer = gBufferRenderer;
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            this.renderTargetSSID = new RenderTarget2D(device, width/2, height / 2, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            this.renderTargetSSIDPrev = new RenderTarget2D(device, width / 2, height / 2, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            this.player = player;
            InitializeVertices();
            InitializeQuadBuffers(device);
            this.deferredShadingRenderer = deferredShadingRenderer;
            this.textureCopyEffect = textureCopyEffect;
            this.motionVectorRenderer = motionVectorRenderer;
        }


        public void Draw(GameTime gameTime,SpriteBatch sb)
        {
            if (GameOptions.renderSSID == false)
            {
                return;
            }
            SSIDEffect.Parameters["PrevSSIDTexture"]?.SetValue(renderTargetSSIDPrev);
            SSIDEffect.Parameters["MotionVectorTex"]?.SetValue(motionVectorRenderer.renderTargetMotionVector);
            SSIDEffect.Parameters["GameTime"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            SSIDEffect.Parameters["PositionWSTex"].SetValue(gBufferRenderer.renderTargetPositionWS);
            SSIDEffect.Parameters["NormalTex"].SetValue(gBufferRenderer.renderTargetNormalWS);
            SSIDEffect.Parameters["ViewProjection"].SetValue(player.cam.viewMatrix * player.cam.projectionMatrix);
            SSIDEffect.Parameters["AlbedoTex"].SetValue(gBufferRenderer.renderTargetAlbedo);
            SSIDEffect.Parameters["LumTex"].SetValue(deferredShadingRenderer.renderTargetLum);
            SSIDEffect.Parameters["metallic"].SetValue(GlobalMaterialParamsManager.instance.metallic);
            SSIDEffect.Parameters["roughness"].SetValue(GlobalMaterialParamsManager.instance.roughness);
            if (SSIDEffect.Parameters["NoiseTex"] != null) { SSIDEffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }

            SSIDEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
            SSIDEffect.Parameters["CameraPos"].SetValue(player.cam.position);
            RenderQuad(device,renderTargetSSID, SSIDEffect,false,false);

            textureCopyEffect.Parameters["TextureCopy"].SetValue(renderTargetSSID);
            
            RenderQuad(device, renderTargetSSIDPrev, textureCopyEffect, false, false);
            //  CopyQuad(device, renderTargetSSID, renderTargetSSIDPrev,sb);

        }
    }
}
