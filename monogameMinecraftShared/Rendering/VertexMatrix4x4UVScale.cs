using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace monogameMinecraftShared.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexMatrix4x4UVScale : IVertexType
    {
        public Vector4 row0;
        public Vector4 row1;
        public Vector4 row2;
        public Vector4 row3;
        public Vector4 uvWidthCorner;
        public float scale;

        public static readonly VertexDeclaration VertexDeclaration;
        public VertexMatrix4x4UVScale(Vector4 r0, Vector4 r1, Vector4 r2, Vector4 r3, Vector4 uvWidthCorner, float scale)
        {
            row0 = r0;
            row1 = r1;
            row2 = r2;
            row3 = r3;
            this.uvWidthCorner = uvWidthCorner;
            this.scale = scale;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        static VertexMatrix4x4UVScale()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(sizeof(float)*4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate,3),
                new VertexElement(sizeof(float)*8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate,4),
                new VertexElement(sizeof(float)*12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5),
                new VertexElement(sizeof(float)*16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 6),
                new VertexElement(sizeof(float)*20, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 7),
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }



}
