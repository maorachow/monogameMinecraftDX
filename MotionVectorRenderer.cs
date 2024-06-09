using Microsoft.Xna.Framework.Graphics;
using monogameMinecraft;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftDX
{
    public class MotionVectorRenderer:FullScreenQuadRenderer
    {
        public GraphicsDevice device;
        public Effect motionVectorEffect;
        public GBufferRenderer gBufferRenderer;
        public RenderTarget2D renderTargetMotionVector;
        public GamePlayer player;
        public Matrix playerPrevProjectionMat;
        public Matrix playerPrevViewMat;
     
        public MotionVectorRenderer(GraphicsDevice device, Effect motionVectorEffect, GBufferRenderer gBufferRenderer,GamePlayer player)
        {
            this.device = device;
            this.motionVectorEffect = motionVectorEffect;
            this.gBufferRenderer = gBufferRenderer;
            this.player = player;

            InitializeVertices();
            InitializeQuadBuffers(device);
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            this.renderTargetMotionVector = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector2, DepthFormat.Depth24);
        }
        public void Draw()
        {
            motionVectorEffect.Parameters["PositionWSTex"]?.SetValue(gBufferRenderer.renderTargetPositionWS);
            motionVectorEffect.Parameters["prevView"]?.SetValue(playerPrevViewMat);
            motionVectorEffect.Parameters["prevProjection"]?.SetValue(playerPrevProjectionMat);
            motionVectorEffect.Parameters["View"]?.SetValue(player.cam.viewMatrix);
            motionVectorEffect.Parameters["Projection"]?.SetValue(player.cam.projectionMatrix);
            RenderQuad(device, renderTargetMotionVector, motionVectorEffect, false, false);
            playerPrevViewMat=player.cam.viewMatrix;
            playerPrevProjectionMat=player.cam.projectionMatrix;    
        }

    }
}
