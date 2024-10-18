using monogameMinecraftShared.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftShared.Rendering
{
    public interface IVoxelWorldWithRenderingChunkBuffers
    {
        public ConcurrentDictionary<Vector2Int, IRenderableChunkBuffers> renderingChunks { get; }
    }
}
