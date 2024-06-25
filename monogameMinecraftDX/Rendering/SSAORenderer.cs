using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftDX.Utility;
using System;
using System.Collections.Generic;


namespace monogameMinecraftDX.Rendering
{
    public class SSAORenderer : FullScreenQuadRenderer

    {
        public Effect ssaoEffect;
        /*     public Effect gBufferEffect;
             public ChunkRenderer chunkRenderer;
             public RenderTarget2D renderTargetPositionDepth;
             public RenderTarget2D renderTargetProjectionDepth;
             public RenderTarget2D renderTargetNormal;
             public RenderTargetBinding[] binding;*/
        public GBufferRenderer gBufferRenderer;
        public GraphicsDevice graphicsDevice;
        public GamePlayer player;
        public List<Vector3> ssaoKernel = new List<Vector3>();
        public List<Color> ssaoNoise = new List<Color>();
        public Texture2D ssaoNoiseTexture;

        public RenderTarget2D ssaoTarget;

        public Texture2D randNormal;
        public Camera cam;
        public SSAORenderer(Effect ssaoEffect, GBufferRenderer gBufferRenderer, ChunkRenderer chunkRenderer, GraphicsDevice gd, GamePlayer player, Texture2D randNormal)
        {
            graphicsDevice = gd;
            this.ssaoEffect = ssaoEffect;
            this.gBufferRenderer = gBufferRenderer;
            cam = player.cam;
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            ssaoTarget = new RenderTarget2D(graphicsDevice, width / 2, height / 2, false, SurfaceFormat.Color, DepthFormat.Depth24);

            this.player = player;
            InitializeVertices();
            InitializeQuadBuffers(graphicsDevice);
            this.randNormal = randNormal;
            ssaoKernel.Clear();
            for (int i = 0; i < 32; ++i)
            {
                Vector3 sample = new Vector3(random.NextSingle() * 2f - 1f, random.NextSingle() * 2f - 1f, random.NextSingle());
                sample.Normalize();
                sample *= random.NextSingle();

                /*    float scale = (float)i /32f;
                    scale = MathHelper.Lerp(0.1f,1f, scale * scale);
                    sample *= scale;*/
                ssaoKernel.Add(sample);
            }
            for (int i = 0; i < 16; i++)
            {
                Vector3 noise = new Vector3(
         random.NextSingle() * 2.0f - 1.0f,
         random.NextSingle() * 2.0f - 1.0f,
         0.0f);
                ssaoNoise.Add(new Color(noise));
            }
            ssaoNoiseTexture = new Texture2D(graphicsDevice, 4, 4, false, SurfaceFormat.Color);
            ssaoNoiseTexture.SetData(ssaoNoise.ToArray());
        }
        Random random = new Random();



        public void Draw()
        {

            if (GameOptions.renderSSAO == false)
            {
                RenderQuad(graphicsDevice, ssaoTarget, ssaoEffect, true);
                return;
            }

            /*  ssaoKernel.Clear();
              for (int i = 0; i < 32; ++i)
              {
                  Vector3 sample = new Vector3(random.NextSingle() * 2f - 1f, random.NextSingle() * 2f - 1f, random.NextSingle());
                  sample.Normalize();
                  sample *= random.NextSingle();

                  float scale = (float)i / 32f;
                  scale = MathHelper.Lerp(0.1f, 1.0f, scale * scale);
                  sample *= scale;
                  ssaoKernel.Add(sample);
              }*/
            SetCameraFrustum(cam, ssaoEffect);
            /*  graphicsDevice.SetRenderTargets(renderTargetPositionDepth,renderTargetProjectionDepth,renderTargetNormal);

              chunkRenderer.RenderAllChunksGBuffer(ChunkManager.chunks, player, this.gBufferEffect);
              graphicsDevice.SetRenderTargets(null);
              graphicsDevice.Clear(Color.CornflowerBlue);*/
            /*        //     ssaoEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
                    ssaoEffect.Parameters["ProjectionDepthTex"].SetValue(this.renderTargetProjectionDepth);
               //     
                    ssaoEffect.Parameters["invProjection"].SetValue(Matrix.Invert(player.cam.projectionMatrix));
               //     ssaoEffect.Parameters["projection"].SetValue(player.cam.projectionMatrix);
                   
                    ssaoEffect.Parameters["NormalTex"].SetValue(this.renderTargetNormal);*/

            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            if (ssaoEffect.Parameters["PixelSize"] != null) { ssaoEffect.Parameters["PixelSize"].SetValue(new Vector2(1f / width, 1f / height)); }
            if (ssaoEffect.Parameters["NormalTex"] != null) { ssaoEffect.Parameters["NormalTex"].SetValue(gBufferRenderer.renderTargetNormalWS); }
            //   if (ssaoEffect.Parameters["samples"] != null) {ssaoEffect.Parameters["samples"].SetValue(ssaoKernel.ToArray());}
            if (ssaoEffect.Parameters["NoiseTex"] != null) { ssaoEffect.Parameters["NoiseTex"].SetValue(RandomTextureGenerator.instance.randomTex); }
            if (ssaoEffect.Parameters["ProjectionDepthTex"] != null) { ssaoEffect.Parameters["ProjectionDepthTex"].SetValue(gBufferRenderer.renderTargetProjectionDepth); }
            if (ssaoEffect.Parameters["AlbedoTex"] != null) { ssaoEffect.Parameters["AlbedoTex"].SetValue(gBufferRenderer.renderTargetAlbedo); }
            //       if (ssaoEffect.Parameters["PositionWSTex"] != null) { ssaoEffect.Parameters["PositionWSTex"].SetValue(gBufferRenderer.renderTargetPositionWS); }
            if (ssaoEffect.Parameters["View"] != null) { ssaoEffect.Parameters["View"].SetValue(cam.viewMatrix); }
            if (ssaoEffect.Parameters["NormalView"] != null) { ssaoEffect.Parameters["NormalView"].SetValue(cam.viewMatrixOrigin); }
            if (ssaoEffect.Parameters["CameraPos"] != null) { ssaoEffect.Parameters["CameraPos"].SetValue(cam.position); }
            if (ssaoEffect.Parameters["ViewProjection"] != null) { ssaoEffect.Parameters["ViewProjection"].SetValue(cam.viewMatrix * cam.projectionMatrix); }
            /*        
                           ssaoEffect.Parameters["param_normalMap"].SetValue(gBufferRenderer.renderTargetNormalWS);
                   ssaoEffect.Parameters["transposeInverseView"].SetValue(Matrix.Transpose(Matrix.Invert(player.cam.viewMatrix)));
                                ssaoEffect.Parameters["param_randomMap"].SetValue(this.ssaoNoiseTexture);
                      // ssaoEffect.Parameters["ViewProjection"].SetValue(player.cam.projectionMatrix*player.cam.viewMatrix);
                          ssaoEffect.Parameters["param_intensity"].SetValue(0.7f);
                       ssaoEffect.Parameters["param_scale"].SetValue(1f);
                       ssaoEffect.Parameters["param_sampleRadius"].SetValue(0.2f);
                       ssaoEffect.Parameters["param_randomSize"].SetValue(0.001f);
                   ssaoEffect.Parameters["param_screenSize"].SetValue(30f);
                   ssaoEffect.Parameters["g_matInvProjection"].SetValue(Matrix.Invert(player.cam.projectionMatrix ));*/

            RenderQuad(graphicsDevice, ssaoTarget, ssaoEffect);
        }

    }
}
