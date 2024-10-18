using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Rendering.Particle;
using monogameMinecraftShared.Utility;

namespace monogameMinecraftShared.Rendering
{
    public interface IRenderPipelineManager
    {
        public MinecraftGameBase game { get; set; }
        public IEffectsManager effectsManager { get; set; }

        public void RenderWorld(GameTime gameTime, SpriteBatch sb);
        public void InitRenderPipeline(Action<IRenderPipelineManager> postRenderingAction =null);
        public void Resize();

        public WalkablePathVisualizationRenderer walkablePathVisualizationRenderer { get; set; }

        public BoundingBoxVisualizationRenderer boundingBoxVisualizationRenderer { get; set;}

        public ChunkRenderer chunkRenderer { get; set; }

        public ParticleRenderer particleRenderer { get; set; }


        public List<IEntityRenderer> entityRenderers { get; set; }
        public List<IPostRenderingRenderer> postRenderingRenderers { get; set; }

        public IVoxelWorldWithRenderingChunkBuffers curRenderingWorld { get; set; }

        public Action optionalPostInitRenderPipelineAction { get; set; }
        public Action<IRenderPipelineManager> optionalPostRenderingAction { get; set; }
        
    }
}
