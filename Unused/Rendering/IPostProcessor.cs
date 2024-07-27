using Microsoft.Xna.Framework.Graphics;

namespace monogameMinecraftDX.Rendering
{
    public interface IPostProcessor
    {
        public RenderTarget2D processedImage { get; set; }
        public void ProcessImage(in RenderTarget2D rt);
    }
}
