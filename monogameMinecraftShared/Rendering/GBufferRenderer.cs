using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
 
using monogameMinecraftShared.Rendering.Particle;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.World;
using System.Collections.Concurrent;
using monogameMinecraftShared.Core;

namespace monogameMinecraftShared.Rendering
{
    public class GBufferRenderer
    {

        // public RenderTarget2D renderTargetPositionDepth;
        public RenderTarget2D renderTargetProjectionDepth;
        //    public RenderTarget2D renderTargetNormal;
        public RenderTarget2D renderTargetNormalWS;
        public RenderTarget2D renderTargetAlbedo;
        public RenderTarget2D renderTargetMER;



        public RenderTarget2D renderTargetProjectionDepthTrans0;
        //    public RenderTarget2D renderTargetNormal;
        public RenderTarget2D renderTargetNormalWSTrans0;
        public RenderTarget2D renderTargetAlbedoTrans0;
        public RenderTarget2D renderTargetMERTrans0;


        public RenderTarget2D renderTargetProjectionDepthTrans1;
     
        public RenderTarget2D renderTargetNormalWSTrans1;
        public RenderTarget2D renderTargetAlbedoTrans1;
        public RenderTarget2D renderTargetMERTrans1;

        public RenderTarget2D renderTargetProjectionDepthTrans2;

        public RenderTarget2D renderTargetNormalWSTrans2;
        public RenderTarget2D renderTargetAlbedoTrans2;
        public RenderTarget2D renderTargetMERTrans2;
        public RenderTargetBinding[] binding;
        public RenderTargetBinding[] bindingTrans0;
        public RenderTargetBinding[] bindingTrans1;
        public RenderTargetBinding[] bindingTrans2;

        public GraphicsDevice graphicsDevice;
        public IGamePlayer player;
        public VertexBuffer quadVertexBuffer;
        public ChunkRenderer chunkRenderer;
        public IEntityRenderer entityRenderer;
        public ParticleRenderer particleRenderer;
        public IGBufferDrawableRenderer optionalRenderer;
        public IGBufferDrawableRenderer optionalRenderer1;
        public Effect gBufferEffect;
        public Effect gBufferDepthPeelingEffect;
        public Effect gBufferEntityEffect;
        public bool chunksOnly = false;
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
        public GBufferRenderer(GraphicsDevice device, Effect gBufferEffect, Effect gBufferEntityEffect, Effect gBufferDepthPeelingEffect, IGamePlayer player, ChunkRenderer cr, IEntityRenderer er, ParticleRenderer pr, bool chunksOnly=false,IGBufferDrawableRenderer optionalRenderer=null, IGBufferDrawableRenderer optionalRenderer1 = null)
        {
            graphicsDevice = device;
            this.gBufferEffect = gBufferEffect;
            this.gBufferEntityEffect = gBufferEntityEffect;
            this.gBufferDepthPeelingEffect= gBufferDepthPeelingEffect;
            this.player = player;
            this.optionalRenderer= optionalRenderer;
            chunkRenderer = cr;
            entityRenderer = er;
            particleRenderer = pr;
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            //       this.renderTargetPositionDepth = new RenderTarget2D(this.graphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            renderTargetProjectionDepth = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.Depth24);
            renderTargetMER = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedo = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWS = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            binding = new RenderTargetBinding[4];

            binding[0] = new RenderTargetBinding(renderTargetProjectionDepth);
            binding[1] = new RenderTargetBinding(renderTargetNormalWS);
            binding[2] = new RenderTargetBinding(renderTargetAlbedo);
            binding[3] = new RenderTargetBinding(renderTargetMER);


            renderTargetProjectionDepthTrans0 = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.Depth24);
            renderTargetMERTrans0 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedoTrans0 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWSTrans0 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            bindingTrans0 = new RenderTargetBinding[4];

            bindingTrans0[0] = new RenderTargetBinding(renderTargetProjectionDepthTrans0);
            bindingTrans0[1] = new RenderTargetBinding(renderTargetNormalWSTrans0);
            bindingTrans0[2] = new RenderTargetBinding(renderTargetAlbedoTrans0);
            bindingTrans0[3] = new RenderTargetBinding(renderTargetMERTrans0);



            renderTargetProjectionDepthTrans1 = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.Depth24);
            renderTargetMERTrans1 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedoTrans1 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWSTrans1 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            bindingTrans1 = new RenderTargetBinding[4];

            bindingTrans1[0] = new RenderTargetBinding(renderTargetProjectionDepthTrans1);
            bindingTrans1[1] = new RenderTargetBinding(renderTargetNormalWSTrans1);
            bindingTrans1[2] = new RenderTargetBinding(renderTargetAlbedoTrans1);
            bindingTrans1[3] = new RenderTargetBinding(renderTargetMERTrans1);

            renderTargetProjectionDepthTrans2 = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.Depth24);
            renderTargetMERTrans2 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedoTrans2 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWSTrans2 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            bindingTrans2 = new RenderTargetBinding[4];

            bindingTrans2[0] = new RenderTargetBinding(renderTargetProjectionDepthTrans2);
            bindingTrans2[1] = new RenderTargetBinding(renderTargetNormalWSTrans2);
            bindingTrans2[2] = new RenderTargetBinding(renderTargetAlbedoTrans2);
            bindingTrans2[3] = new RenderTargetBinding(renderTargetMERTrans2);
            InitializeVertices();

            quadIndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.None);
            quadIndexBuffer.SetData(quadIndices);
            quadVertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), 4, BufferUsage.None);
            quadVertexBuffer.SetData(quadVertices);
            this.chunksOnly = chunksOnly;
            this.optionalRenderer1=optionalRenderer1;
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


            renderTargetProjectionDepthTrans0 = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.Depth24);
            renderTargetMERTrans0 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedoTrans0 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWSTrans0 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            bindingTrans0 = new RenderTargetBinding[4];

            bindingTrans0[0] = new RenderTargetBinding(renderTargetProjectionDepthTrans0);
            bindingTrans0[1] = new RenderTargetBinding(renderTargetNormalWSTrans0);
           
            bindingTrans0[2] = new RenderTargetBinding(renderTargetAlbedoTrans0);
            bindingTrans0[3] = new RenderTargetBinding(renderTargetMERTrans0);



            renderTargetProjectionDepthTrans1 = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.Depth24);
            renderTargetMERTrans1 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedoTrans1 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWSTrans1 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            bindingTrans1 = new RenderTargetBinding[4];

            bindingTrans1[0] = new RenderTargetBinding(renderTargetProjectionDepthTrans1);
            bindingTrans1[1] = new RenderTargetBinding(renderTargetNormalWSTrans1);

            bindingTrans1[2] = new RenderTargetBinding(renderTargetAlbedoTrans1);
            bindingTrans1[3] = new RenderTargetBinding(renderTargetMERTrans1);

            renderTargetProjectionDepthTrans2 = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.Depth24);
            renderTargetMERTrans2 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTargetAlbedoTrans2 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            renderTargetNormalWSTrans2 = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.HalfVector4, DepthFormat.Depth24);
            bindingTrans2 = new RenderTargetBinding[4];

            bindingTrans2[0] = new RenderTargetBinding(renderTargetProjectionDepthTrans2);
            bindingTrans2[1] = new RenderTargetBinding(renderTargetNormalWSTrans2);

            bindingTrans2[2] = new RenderTargetBinding(renderTargetAlbedoTrans2);
            bindingTrans2[3] = new RenderTargetBinding(renderTargetMERTrans2);
        }
        RasterizerState rasterizerState=new RasterizerState { DepthClipEnable = false,CullMode = CullMode.CullCounterClockwiseFace};
        RasterizerState rasterizerState1 = new RasterizerState{DepthClipEnable = false, CullMode = CullMode.None };
        public void Draw(ConcurrentDictionary<Vector2Int, IRenderableChunkBuffers> RenderingChunks)
        {
            graphicsDevice.DepthStencilState=DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.SetRenderTargets(binding);
            graphicsDevice.Clear(Color.Transparent);
            chunkRenderer.RenderAllChunksGBuffer(RenderingChunks, player, gBufferEffect);
            if (!chunksOnly)
            {
              
              
               
            }

            if (entityRenderer != null)
            {
                entityRenderer.DrawGBuffer();
            }
        
            particleRenderer.DrawGBuffer();
            if (optionalRenderer != null)
            {
                optionalRenderer.DrawGBuffer();
            }
            if (optionalRenderer1 != null)
            {
                optionalRenderer1.DrawGBuffer();
            }
            graphicsDevice.SetRenderTargets(null);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.SetRenderTargets(bindingTrans0);
            graphicsDevice.Clear(Color.Transparent);
            chunkRenderer.RenderAllChunksGBufferDepthPeeling(RenderingChunks, player, gBufferDepthPeelingEffect,renderTargetProjectionDepth);
            graphicsDevice.SetRenderTargets(null);

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.SetRenderTargets(bindingTrans1);
            graphicsDevice.Clear(Color.Transparent);
            chunkRenderer.RenderAllChunksGBufferDepthPeeling(RenderingChunks, player, gBufferDepthPeelingEffect, renderTargetProjectionDepth, renderTargetProjectionDepthTrans0);
            graphicsDevice.SetRenderTargets(null);

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.SetRenderTargets(bindingTrans2);
            graphicsDevice.Clear(Color.Transparent);
            chunkRenderer.RenderAllChunksGBufferDepthPeeling(RenderingChunks, player, gBufferDepthPeelingEffect, renderTargetProjectionDepth, renderTargetProjectionDepthTrans0, renderTargetProjectionDepthTrans1);
            graphicsDevice.SetRenderTargets(null);


            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.Clear(Color.CornflowerBlue);


        }
    }
}
