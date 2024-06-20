using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraft;
using System;
namespace monogameMinecraftDX
{
    public class HiZBufferRenderer : FullScreenQuadRenderer
    {


        public GraphicsDevice device;
        public Effect hiZBufferEffect;
        public Effect textureCopyEffect;
        public RenderTarget2D[] hiZBufferTargetMips = new RenderTarget2D[8];

        public GBufferRenderer gBufferRenderer;
        public HiZBufferRenderer(GraphicsDevice device, Effect hiZBufferEffect, GBufferRenderer gBufferRenderer, Effect textureCopyEffect)
        {
            this.device = device;
            this.hiZBufferEffect = hiZBufferEffect;
            ResizeTarget();

            this.textureCopyEffect = textureCopyEffect;
            this.gBufferRenderer = gBufferRenderer;
        }
        public void ResizeTarget()
        {
            int widthO = device.PresentationParameters.BackBufferWidth;
            int heightO = device.PresentationParameters.BackBufferHeight;
            // 把高和宽变换为2的整次幂 然后除以2
            var width = Math.Max((int)Math.Ceiling(Math.Log(widthO, 2) - 1.0f), 1);
            var height = Math.Max((int)Math.Ceiling(Math.Log(heightO, 2) - 1.0f), 1);
            width = 1 << width;
            height = 1 << height;
            //      Debug.WriteLine("width height: "+ width + " " + height);
            for (int i = 0; i < 8; i++)
            {
                this.hiZBufferTargetMips[i] = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector2, DepthFormat.Depth24);
                // generate mipmap
                width = Math.Max(width / 2, 1);
                height = Math.Max(height / 2, 1);
            }
            InitializeVertices();
            InitializeQuadBuffers(device);
        }
        public void Draw()
        {


            //   textureCopyEffect.Parameters["TextureCopy"].SetValue(gBufferRenderer.renderTargetProjectionDepth);
            textureCopyEffect.Parameters["backgroundCol"]?.SetValue(new Vector3(1000, 0f, 0f));
            textureCopyEffect.Parameters["useBkgColor"]?.SetValue(true);
            textureCopyEffect.Parameters["TextureCopy"].SetValue(gBufferRenderer.renderTargetProjectionDepth);
            RenderQuad(device, hiZBufferTargetMips[0], textureCopyEffect);
            for (int i = 1; i < 8; i++)
            {
                hiZBufferEffect.Parameters["PixelSize"].SetValue(new Vector2(1f / hiZBufferTargetMips[i - 1].Width, 1f / hiZBufferTargetMips[i - 1].Height));
                hiZBufferEffect.Parameters["TextureCopy"].SetValue(hiZBufferTargetMips[i - 1]);
                RenderQuad(device, hiZBufferTargetMips[i], hiZBufferEffect);
            }

        }
    }
}
