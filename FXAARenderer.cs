using Microsoft.Xna.Framework.Graphics;
using monogameMinecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace monogameMinecraftDX
{
    public class FXAARenderer:FullScreenQuadRenderer
    {
        public GraphicsDevice device;
        public Effect fxaaEffect;
        public RenderTarget2D renderTargetProcessed;
        public FXAARenderer(GraphicsDevice device, Effect fxaaEffect)
        {
            this.device = device;
            this.fxaaEffect = fxaaEffect;

            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            
            
            this.renderTargetProcessed = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            InitializeVertices();
            InitializeQuadBuffers(device);
        }

        public void Draw(bool isFinalProcess,RenderTarget2D inputImage)
        {

            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            fxaaEffect.Parameters["InputTexture"].SetValue(inputImage);
            Vector2 pixelSize=new Vector2(1f/(float)width, 1f/ (float)height);

            fxaaEffect.Parameters["PixelSize"]?.SetValue(pixelSize);
            if (isFinalProcess)
            {
                RenderQuad(device, null, fxaaEffect, false, true, false);
            }
            else
            {
                RenderQuad(device, renderTargetProcessed, fxaaEffect);
            }
           
        }
    }
}
