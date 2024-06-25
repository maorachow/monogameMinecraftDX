using Microsoft.Xna.Framework.Graphics;


namespace monogameMinecraftDX.Rendering
{
    public class BRDFLUTRenderer : FullScreenQuadRenderer
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
            renderTargetLUT = new RenderTarget2D(device, 512, 512, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            this.lutEffect = lutEffect;
        }

        public void CalculateLUT()
        {
            lutEffect.Parameters["PI"]?.SetValue(3.14159265359f);
            RenderQuad(device, renderTargetLUT, lutEffect, false, false);

        }

    }
}
