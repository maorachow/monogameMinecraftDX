using Microsoft.Xna.Framework.Graphics;
using monogameMinecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftDX
{
    public interface IPostProcessor
    {
        public RenderTarget2D processedImage { get; set; }
        public void ProcessImage(in RenderTarget2D rt);
    }
}
