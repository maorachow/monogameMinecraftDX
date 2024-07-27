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
        public void InitRenderPipeline();
        public void Resize();

        public WalkablePathVisualizationRenderer walkablePathVisualizationRenderer { get; set; }

        public BoundingBoxVisualizationRenderer boundingBoxVisualizationRenderer { get; set;}

        public ChunkRenderer chunkRenderer { get; set; }

        public ParticleRenderer particleRenderer { get; set; }

   
    }
}
