using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;

namespace monogameMinecraftShared.Rendering
{
    public interface IRenderableChunkBuffers
    {

        public Vector2Int chunkPos { get; set; }
        public bool isReadyToRender { get; set; }
        public bool disposed { get; set; }
        public bool isUnused { get; set; }
        public bool isOpqBuffersValid { get; set; }
        public bool isNSBuffersValid { get; set; }
        public bool isWTBuffersValid { get; set; }

        public BoundingBox chunkBounds { get; set; }
        public VertexPositionNormalTangentTexture[] verticesOpqArray { get; }
        public VertexPositionNormalTangentTexture[] verticesOpqLOD1Array { get; }
        public VertexPositionNormalTangentTexture[] verticesNSArray { get; }
        public VertexPositionNormalTangentTexture[] verticesWTArray { get; }
        public ushort[] indicesOpqArray { get; }
        public ushort[] indicesOpqLOD1Array { get; }
        public ushort[] indicesNSArray { get; }
        public ushort[] indicesWTArray { get; }
        public IndexBuffer IBOpq { get; }
        public VertexBuffer VBOpq { get; }
        public IndexBuffer IBOpqLOD1 { get; }
        public VertexBuffer VBOpqLOD1 { get; }
        public IndexBuffer IBWT { get; }
        public VertexBuffer VBWT { get; }
        public IndexBuffer IBNS { get; }
        public VertexBuffer VBNS { get; }
    }
}
