using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using monogameMinecraftDX;
namespace monogameMinecraft
{
    public class SSRRenderer:FullScreenQuadRenderer
    {

        public GraphicsDevice graphicsDevice;
        public GamePlayer player;
        public GBufferRenderer gBufferRenderer;
        public RenderTarget2D renderTargetSSR;

        public RenderTarget2D renderTargetSSRPrev;
        public MotionVectorRenderer motionVectorRenderer;
        public DeferredShadingRenderer deferredShadingRenderer;
        public Effect SSREffect;
        public Effect textureCopyEffect;
        public bool binarySearch = true;

        public SSRRenderer(GraphicsDevice graphicsDevice, GamePlayer player, GBufferRenderer gBufferRenderer, Effect sSREffect, DeferredShadingRenderer deferredShadingRenderer, Effect textureCopyEffect, MotionVectorRenderer motionVectorRenderer)
        {
            this.graphicsDevice = graphicsDevice;
            this.player = player;
            this.gBufferRenderer = gBufferRenderer;
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            this.renderTargetSSR = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            this.renderTargetSSRPrev = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            SSREffect = sSREffect;
            this.deferredShadingRenderer = deferredShadingRenderer;
            InitializeVertices();
            InitializeQuadBuffers(graphicsDevice);
            this.textureCopyEffect = textureCopyEffect;
            this.motionVectorRenderer = motionVectorRenderer;
        }
        public bool preIsKeyDown;
        public void Draw(GameTime gameTime)
        {
            if (GameOptions.renderSSR == false)
            {
                return;
            }
            //    if(Keyboard.GetState().IsKeyDown(Keys.B)&& preIsKeyDown==false)
            //      {

            //        binarySearch = !binarySearch;
            //    Debug.WriteLine("binSearch:" + binarySearch);
            //  }
            //   SSREffect.Parameters["ProjectionDepthTex"].SetValue(gBufferRenderer.renderTargetProjectionDepth);
            SSREffect.Parameters["GameTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            SSREffect.Parameters["PrevSSRTexture"]?.SetValue(renderTargetSSRPrev);
            SSREffect.Parameters["MotionVectorTex"]?.SetValue(motionVectorRenderer.renderTargetMotionVector);
            SSREffect.Parameters["PositionWSTex"]?.SetValue(gBufferRenderer.renderTargetPositionWS);
           SSREffect.Parameters["NormalTex"]?.SetValue(gBufferRenderer.renderTargetNormalWS);

           SSREffect.Parameters["ViewProjection"]?.SetValue(player.cam.viewMatrix * player.cam.projectionMatrix);
             SSREffect.Parameters["LumTex"]?.SetValue(deferredShadingRenderer.renderTargetLum);
            SSREffect.Parameters["AlbedoTex"]?.SetValue(gBufferRenderer.renderTargetAlbedo);
            SSREffect.Parameters["LUTTex"]?.SetValue(BRDFLUTRenderer.instance.renderTargetLUT);
            SSREffect.Parameters["binarySearch"]?.SetValue(binarySearch);
            SSREffect.Parameters["metallic"]?.SetValue(GlobalMaterialParamsManager.instance.metallic);
            SSREffect.Parameters["roughness"]?.SetValue(GlobalMaterialParamsManager.instance.roughness);
            if (SSREffect.Parameters["NoiseTex"] != null) { SSREffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }
            //  SSREffect.Parameters["matInverseView"].SetValue(Matrix.Invert(player.cam.viewMatrix));
            //  SSREffect.Parameters["matInverseProjection"].SetValue(Matrix.Invert(player.cam.projectionMatrix));
            //  SSREffect.Parameters["matView"].SetValue((player.cam.viewMatrix));
            //  SSREffect.Parameters["matProjection"].SetValue((player.cam.projectionMatrix));
            SSREffect.Parameters["View"]?.SetValue(player.cam.viewMatrix);
            SSREffect.Parameters["CameraPos"]?.SetValue(player.cam.position);
         //   SSREffect.Parameters["RoughnessMap"].SetValue(gBufferRenderer.renderTargetPositionWS);
            RenderQuad(graphicsDevice,renderTargetSSR, SSREffect);

            textureCopyEffect.Parameters["TextureCopy"].SetValue(renderTargetSSR);
            RenderQuad(graphicsDevice, renderTargetSSRPrev, textureCopyEffect);
        }

  /*      public void RenderQuad(RenderTarget2D target, Effect quadEffect, bool isPureWhite = false)
        {
            graphicsDevice.SetRenderTarget(target);

            if (isPureWhite)
            {
                graphicsDevice.Clear(Color.White);
                graphicsDevice.SetRenderTarget(null);
                graphicsDevice.Clear(Color.CornflowerBlue);
                return;
            }
            graphicsDevice.Clear(new Color(0,0,0,0));
            graphicsDevice.SetVertexBuffer(gBufferRenderer.quadVertexBuffer);
            graphicsDevice.Indices = gBufferRenderer.quadIndexBuffer;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;
            foreach (var pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4);
            }
            //    graphicsDevice.Clear(Color.White);
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Color.CornflowerBlue);
        }*/
    }
}
