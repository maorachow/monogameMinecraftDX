using Microsoft.Xna.Framework.Graphics;
using monogameMinecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftDX
{
    public class BRDFLUTRenderer:FullScreenQuadRenderer
    {
        public static BRDFLUTRenderer instance;
        public GraphicsDevice device;
        public RenderTarget2D renderTargetLUT;
        public Effect lutEffect;
        public BRDFLUTRenderer(GraphicsDevice device, Effect lutEffect)
        {
            instance = this;
            this.device = device;
            InitializeVertices();
            InitializeQuadBuffers(device);
            renderTargetLUT = new RenderTarget2D(device, 512, 512, false, SurfaceFormat.Vector2, DepthFormat.Depth24);
            this.lutEffect = lutEffect;
        }

        public void CalculateLUT()
        {
            lutEffect.Parameters["PI"]?.SetValue(3.14159265359f);
            RenderQuad(device,renderTargetLUT,lutEffect,false,false);

        }

    }
}
