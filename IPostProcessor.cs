using Microsoft.Xna.Framework.Graphics;

namespace monogameMinecraftDX
{
    public interface IPostProcessor
    {
        public RenderTarget2D processedImage { get; set; }
        public void ProcessImage(in RenderTarget2D rt);
    }
}
