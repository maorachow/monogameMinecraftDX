using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.Timers;
//using monogameMinecraftDX.Updateables;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftShared.Rendering
{
    public class SSRRenderer : FullScreenQuadRenderer
    {

        public GraphicsDevice graphicsDevice;
        public IGamePlayer player;
        public GBufferRenderer gBufferRenderer;
        public RenderTarget2D renderTargetSSR;

        public RenderTarget2D renderTargetSSRPrev;
        public MotionVectorRenderer motionVectorRenderer;
        public DeferredShadingRenderer deferredShadingRenderer;
        public Effect SSREffect;
        public Effect textureCopyEffect;
        public bool binarySearch = true;
        public HiZBufferRenderer hiZBufferRenderer;
        public HDRCubemapRenderer hdrCubemapRenderer;
        public GameTimeManager gameTimeManager;
        public SSRRenderer(GraphicsDevice graphicsDevice, IGamePlayer player, GBufferRenderer gBufferRenderer, Effect sSREffect, DeferredShadingRenderer deferredShadingRenderer, Effect textureCopyEffect, MotionVectorRenderer motionVectorRenderer, HiZBufferRenderer hiZBufferRenderer, HDRCubemapRenderer hdrCubemapRenderer, GameTimeManager gameTimeManager)
        {
            this.graphicsDevice = graphicsDevice;
            this.player = player;
            this.gBufferRenderer = gBufferRenderer;
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            this.hiZBufferRenderer = hiZBufferRenderer;
            renderTargetSSR = new RenderTarget2D(graphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetSSRPrev = new RenderTarget2D(graphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            SSREffect = sSREffect;
            this.deferredShadingRenderer = deferredShadingRenderer;
            InitializeVertices();
            InitializeQuadBuffers(graphicsDevice);
            this.textureCopyEffect = textureCopyEffect;
            this.motionVectorRenderer = motionVectorRenderer;
            this.hdrCubemapRenderer = hdrCubemapRenderer;
            this.gameTimeManager = gameTimeManager;
        }
        public bool preIsKeyDown;
        public void Draw(GameTime gameTime)
        {
            if (GameOptions.renderSSR == false)
            {
                return;
            }
            //   System.Diagnostics.Stopwatch sw=new Stopwatch();
            //   sw.Start();
            //    if(Keyboard.GetState().IsKeyDown(Keys.B)&& preIsKeyDown==false)
            //      {

            //        binarySearch = !binarySearch;
            //    Debug.WriteLine("binSearch:" + binarySearch);
            //  }
            //   SSREffect.Parameters["ProjectionDepthTex"].SetValue(gBufferRenderer.renderTargetProjectionDepth);
            int width = hiZBufferRenderer.hiZBufferTargetMips[0].Width;
            int height = hiZBufferRenderer.hiZBufferTargetMips[0].Height;
            SetCameraFrustum(player.cam, SSREffect);
            SSREffect.Parameters["GameTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            SSREffect.Parameters["PrevSSRTexture"]?.SetValue(renderTargetSSRPrev);
            SSREffect.Parameters["MotionVectorTex"]?.SetValue(motionVectorRenderer.renderTargetMotionVector);
            //       SSREffect.Parameters["PositionWSTex"]?.SetValue(gBufferRenderer.renderTargetPositionWS);
            SSREffect.Parameters["ProjectionDepthTex"]?.SetValue(gBufferRenderer.renderTargetProjectionDepth);
            SSREffect.Parameters["TextureMER"]?.SetValue(gBufferRenderer.renderTargetMER);
            SSREffect.Parameters["PixelSize"]?.SetValue(new Vector2(1f / width, 1f / height));
            SSREffect.Parameters["TextureSize"]?.SetValue(new Vector2(width,height));
            SSREffect.Parameters["ProjectionDepthTexMip0"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[0]);
            SSREffect.Parameters["ProjectionDepthTexMip1"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[1]);
            SSREffect.Parameters["ProjectionDepthTexMip2"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[2]);
            SSREffect.Parameters["ProjectionDepthTexMip3"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[3]);
            SSREffect.Parameters["ProjectionDepthTexMip4"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[4]);
            SSREffect.Parameters["ProjectionDepthTexMip5"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[5]);
            SSREffect.Parameters["ProjectionDepthTexMip6"]?.SetValue(hiZBufferRenderer.hiZBufferTargetMips[6]);
            SSREffect.Parameters["HDRPrefilteredTex"]?.SetValue(hdrCubemapRenderer.resultCubeCollection.resultSpecularCubemapMip0);
            SSREffect.Parameters["HDRPrefilteredTexNight"]?.SetValue(hdrCubemapRenderer.resultCubeCollectionNight.resultSpecularCubemapMip0);
            SSREffect.Parameters["mixValue"]?.SetValue(gameTimeManager.skyboxMixValue);
            SSREffect.Parameters["NormalTex"]?.SetValue(gBufferRenderer.renderTargetNormalWS);

            SSREffect.Parameters["ViewProjection"]?.SetValue(player.cam.viewMatrix * player.cam.projectionMatrix);
            SSREffect.Parameters["LumTex"]?.SetValue(deferredShadingRenderer.renderTargetLumAllDiffuse);
            SSREffect.Parameters["AlbedoTex"]?.SetValue(gBufferRenderer.renderTargetAlbedo);
            SSREffect.Parameters["LUTTex"]?.SetValue(BRDFLUTRenderer.instance.renderTargetLUT);
            SSREffect.Parameters["binarySearch"]?.SetValue(binarySearch);
         //   SSREffect.Parameters["metallic"]?.SetValue(GlobalMaterialParamsManager.instance.metallic);
        //    SSREffect.Parameters["roughness"]?.SetValue(GlobalMaterialParamsManager.instance.roughness);
            if (SSREffect.Parameters["NoiseTex"] != null) { SSREffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }
            //  SSREffect.Parameters["matInverseView"].SetValue(Matrix.Invert(player.cam.viewMatrix));
            //  SSREffect.Parameters["matInverseProjection"].SetValue(Matrix.Invert(player.cam.projectionMatrix));
            //  SSREffect.Parameters["matView"].SetValue((player.cam.viewMatrix));
            //  SSREffect.Parameters["matProjection"].SetValue((player.cam.projectionMatrix));
            SSREffect.Parameters["View"]?.SetValue((player.cam.viewMatrix));
            SSREffect.Parameters["Projection"]?.SetValue((player.cam.projectionMatrix));
            SSREffect.Parameters["matTransposeView"]?.SetValue(Matrix.Transpose( player.cam.viewMatrix));
            SSREffect.Parameters["ViewOrigin"]?.SetValue((player.cam.viewMatrixOrigin));
         //   Debug.WriteLine("transpose:"+Matrix.Transpose(player.cam.viewMatrix));
        //    Debug.WriteLine("normal:"+(player.cam.viewMatrix));
            SSREffect.Parameters["CameraPos"]?.SetValue(player.cam.position);
            //   SSREffect.Parameters["RoughnessMap"].SetValue(gBufferRenderer.renderTargetPositionWS);
            RenderQuad(graphicsDevice, renderTargetSSR, SSREffect);
            textureCopyEffect.Parameters["useBkgColor"]?.SetValue(false);
            textureCopyEffect.Parameters["backgroundCol"]?.SetValue(new Vector3(0f, 0f, 0f));
            textureCopyEffect.Parameters["TextureCopy"]?.SetValue(renderTargetSSR);
            RenderQuad(graphicsDevice, renderTargetSSRPrev, textureCopyEffect, false, false, true);
            // sw.Stop();
            //Debug.WriteLine(sw.Elapsed.TotalMilliseconds);
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
