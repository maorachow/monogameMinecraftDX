using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftDX.Updateables;


namespace monogameMinecraftDX.Rendering
{
    public class MotionVectorRenderer : FullScreenQuadRenderer
    {
        public GraphicsDevice device;
        public Effect motionVectorEffect;
        public GBufferRenderer gBufferRenderer;
        public RenderTarget2D renderTargetMotionVector;
        public GamePlayer player;
        public Matrix playerPrevProjectionMat;
        public Matrix playerPrevViewMat;

        public MotionVectorRenderer(GraphicsDevice device, Effect motionVectorEffect, GBufferRenderer gBufferRenderer, GamePlayer player)
        {
            this.device = device;
            this.motionVectorEffect = motionVectorEffect;
            this.gBufferRenderer = gBufferRenderer;
            this.player = player;

            InitializeVertices();
            InitializeQuadBuffers(device);
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            renderTargetMotionVector = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
        }
        public void Draw()
        {
            SetCameraFrustum(player.cam, motionVectorEffect);
            //   motionVectorEffect.Parameters["PositionWSTex"]?.SetValue(gBufferRenderer.renderTargetPositionWS);
            motionVectorEffect.Parameters["ProjectionDepthTex"]?.SetValue(gBufferRenderer.renderTargetProjectionDepth);
            motionVectorEffect.Parameters["prevView"]?.SetValue(playerPrevViewMat);
            motionVectorEffect.Parameters["prevProjection"]?.SetValue(playerPrevProjectionMat);
            motionVectorEffect.Parameters["View"]?.SetValue(player.cam.viewMatrix);
            motionVectorEffect.Parameters["Projection"]?.SetValue(player.cam.projectionMatrix);
            RenderQuad(device, renderTargetMotionVector, motionVectorEffect, false, false);
            playerPrevViewMat = player.cam.viewMatrix;
            playerPrevProjectionMat = player.cam.projectionMatrix;
        }

    }
}
