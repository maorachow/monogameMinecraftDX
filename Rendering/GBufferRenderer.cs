using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftDX.World;
namespace monogameMinecraftDX.Rendering
{
    public class GBufferRenderer
    {

        // public RenderTarget2D renderTargetPositionDepth;
        public RenderTarget2D renderTargetProjectionDepth;
        //    public RenderTarget2D renderTargetNormal;
        public RenderTarget2D renderTargetNormalWS;
        public RenderTarget2D renderTargetAlbedo;
        public RenderTarget2D renderTargetMER;
        public RenderTargetBinding[] binding;
        public GraphicsDevice graphicsDevice;
        public GamePlayer player;
        public VertexBuffer quadVertexBuffer;
        public ChunkRenderer chunkRenderer;
        public EntityRenderer entityRenderer;
        public Effect gBufferEffect;
        public Effect gBufferEntityEffect;
        public VertexPositionTexture[] quadVertices =
      {

            new VertexPositionTexture(new Vector3(-1.0f,  1.0f, 0.0f),new Vector2(  0.0f, 0.0f)),
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 0.0f),new Vector2(  0.0f, 1.0f)),
            new VertexPositionTexture(new Vector3(1.0f,  1.0f, 0.0f),new Vector2(1.0f, 1.0f)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f, 0.0f),new Vector2(1.0f, 0.0f))
         
        

            


       //     new VertexPositionTexture(new Vector3(-1.0f,  1.0f,0f),new Vector2( 0.0f, 1.0f)), 
             ,
        

        //    new VertexPositionTexture(new Vector3(1.0f, -1.0f,0f),new Vector2(1.0f, 0.0f)) ,  

        
        };


        public ushort[] quadIndices =
        {
                0, 1, 2,
                2, 3, 0
        };
        public void InitializeVertices()
        {
            quadVertices = new VertexPositionTexture[4];

            quadVertices[0].Position = new Vector3(-1, 1, 0);
            quadVertices[0].TextureCoordinate = new Vector2(0, 0);

            quadVertices[1].Position = new Vector3(1, 1, 0);
            quadVertices[1].TextureCoordinate = new Vector2(1, 0);

            quadVertices[2].Position = new Vector3(1, -1, 0);
            quadVertices[2].TextureCoordinate = new Vector2(1, 1);

            quadVertices[3].Position = new Vector3(-1, -1, 0);
            quadVertices[3].TextureCoordinate = new Vector2(0, 1);
        }
        public IndexBuffer quadIndexBuffer;
        public GBufferRenderer(GraphicsDevice device, Effect gBufferEffect, Effect gBufferEntityEffect, GamePlayer player, ChunkRenderer cr, EntityRenderer er)
        {
            graphicsDevice = device;
            this.gBufferEffect = gBufferEffect;
            this.gBufferEntityEffect = gBufferEntityEffect;
            this.player = player;
            chunkRenderer = cr;
            entityRenderer = er;
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            //       this.renderTargetPositionDepth = new RenderTarget2D(this.graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetProjectionDepth = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector2, DepthFormat.Depth24);
            renderTargetMER = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedo = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWS = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            binding = new RenderTargetBinding[4];

            binding[0] = new RenderTargetBinding(renderTargetProjectionDepth);
            binding[1] = new RenderTargetBinding(renderTargetNormalWS);
            binding[2] = new RenderTargetBinding(renderTargetAlbedo);
            binding[3] = new RenderTargetBinding(renderTargetMER);
            InitializeVertices();

            quadIndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.None);
            quadIndexBuffer.SetData(quadIndices);
            quadVertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), 4, BufferUsage.None);
            quadVertexBuffer.SetData(quadVertices);
        }
        public void Resize(int width, int height, GraphicsDevice device)
        {
            renderTargetProjectionDepth = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector2, DepthFormat.Depth24);
            renderTargetMER = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedo = new RenderTarget2D(device, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWS = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            binding = new RenderTargetBinding[4];

            binding[0] = new RenderTargetBinding(renderTargetProjectionDepth);
            binding[1] = new RenderTargetBinding(renderTargetNormalWS);
            binding[2] = new RenderTargetBinding(renderTargetAlbedo);
            binding[3] = new RenderTargetBinding(renderTargetMER);
        }
        public void Draw()
        {

            graphicsDevice.SetRenderTargets(binding);

            chunkRenderer.RenderAllChunksGBuffer(VoxelWorld.currentWorld.chunks, player, gBufferEffect);
            entityRenderer.DrawGBuffer(gBufferEntityEffect);
            graphicsDevice.SetRenderTargets(null);
            graphicsDevice.Clear(Color.CornflowerBlue);


        }
    }
}
