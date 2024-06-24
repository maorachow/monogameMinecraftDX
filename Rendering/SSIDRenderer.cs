using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftDX.Utility;


namespace monogameMinecraftDX.Rendering
{
    public class SSIDRenderer : FullScreenQuadRenderer
    {


        public GraphicsDevice device;
        public Effect SSIDEffect;
        public Effect textureCopyEffect;
        public GBufferRenderer gBufferRenderer;
        public RenderTarget2D renderTargetSSID;
        public RenderTarget2D renderTargetSSIDPrev;
        public DeferredShadingRenderer deferredShadingRenderer;
        public MotionVectorRenderer motionVectorRenderer;
        public HiZBufferRenderer hiZBufferRenderer;
        public GamePlayer player;
        public SSIDRenderer(GraphicsDevice device, Effect sSIDEffect, GBufferRenderer gBufferRenderer, GamePlayer player, DeferredShadingRenderer deferredShadingRenderer, Effect textureCopyEffect, MotionVectorRenderer motionVectorRenderer, HiZBufferRenderer hiZBufferRenderer)
        {
            this.device = device;
            SSIDEffect = sSIDEffect;
            this.gBufferRenderer = gBufferRenderer;
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            renderTargetSSID = new RenderTarget2D(device, width / 2, height / 2, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetSSIDPrev = new RenderTarget2D(device, width / 2, height / 2, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            this.player = player;
            InitializeVertices();
            InitializeQuadBuffers(device);
            this.deferredShadingRenderer = deferredShadingRenderer;
            this.textureCopyEffect = textureCopyEffect;
            this.motionVectorRenderer = motionVectorRenderer;
            this.hiZBufferRenderer = hiZBufferRenderer;
        }


        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            if (GameOptions.renderSSID == false)
            {
                return;
            }

            SetCameraFrustum(player.cam, SSIDEffect);
            SSIDEffect.Parameters["ProjectionDepthTex"]?.SetValue(gBufferRenderer.renderTargetProjectionDepth);
            SSIDEffect.Parameters["ProjectionDepthTexMip0"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[0]);
            SSIDEffect.Parameters["ProjectionDepthTexMip1"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[1]);
            SSIDEffect.Parameters["ProjectionDepthTexMip2"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[2]);
            SSIDEffect.Parameters["ProjectionDepthTexMip3"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[3]);
            SSIDEffect.Parameters["ProjectionDepthTexMip4"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[4]);
            SSIDEffect.Parameters["ProjectionDepthTexMip5"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[5]);

            SSIDEffect.Parameters["PrevSSIDTexture"]?.SetValue(renderTargetSSIDPrev);
            SSIDEffect.Parameters["MotionVectorTex"]?.SetValue(motionVectorRenderer.renderTargetMotionVector);
            SSIDEffect.Parameters["GameTime"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            //    SSIDEffect.Parameters["PositionWSTex"]?.SetValue(gBufferRenderer.renderTargetPositionWS);
            SSIDEffect.Parameters["TextureMER"]?.SetValue(gBufferRenderer.renderTargetMER);
            SSIDEffect.Parameters["NormalTex"]?.SetValue(gBufferRenderer.renderTargetNormalWS);
            SSIDEffect.Parameters["ViewProjection"]?.SetValue(player.cam.viewMatrix * player.cam.projectionMatrix);
            SSIDEffect.Parameters["AlbedoTex"]?.SetValue(gBufferRenderer.renderTargetAlbedo);
            SSIDEffect.Parameters["LumTex"]?.SetValue(deferredShadingRenderer.renderTargetLum);
            SSIDEffect.Parameters["metallic"]?.SetValue(GlobalMaterialParamsManager.instance.metallic);
            SSIDEffect.Parameters["roughness"]?.SetValue(GlobalMaterialParamsManager.instance.roughness);
            if (SSIDEffect.Parameters["NoiseTex"] != null) { SSIDEffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }

            SSIDEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
            SSIDEffect.Parameters["CameraPos"].SetValue(player.cam.position);
            RenderQuad(device, renderTargetSSID, SSIDEffect, false, false);
            textureCopyEffect.Parameters["backgroundCol"]?.SetValue(new Vector3(0f, 0f, 0f));
            textureCopyEffect.Parameters["useBkgColor"]?.SetValue(false);
            textureCopyEffect.Parameters["TextureCopy"].SetValue(renderTargetSSID);

            RenderQuad(device, renderTargetSSIDPrev, textureCopyEffect, false, false);
            //  CopyQuad(device, renderTargetSSID, renderTargetSSIDPrev,sb);

        }
    }
}
