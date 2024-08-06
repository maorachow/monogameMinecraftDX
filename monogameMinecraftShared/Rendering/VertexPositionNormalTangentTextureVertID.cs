using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace monogameMinecraftShared.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalTangentTextureVertID : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 TextureCoordinate;
        public ushort vertID;
        public ushort vertID1;

        public static readonly VertexDeclaration VertexDeclaration;
        public VertexPositionNormalTangentTextureVertID(Vector3 position, Vector3 normal, Vector3 Tangent, Vector2 textureCoordinate, ushort vertID, ushort vertID1)
        {
            Position = position;
            Normal = normal;
            this.Tangent = Tangent;
            TextureCoordinate = textureCoordinate;
            this.vertID=vertID;
            this.vertID1 = vertID1;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        static VertexPositionNormalTangentTextureVertID()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float)*3, VertexElementFormat.Vector3, VertexElementUsage.Normal,0),
                new VertexElement(sizeof(float)*6, VertexElementFormat.Vector3, VertexElementUsage.Tangent,0),
                new VertexElement(sizeof(float)*9, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float)*11, VertexElementFormat.Short2 , VertexElementUsage.TextureCoordinate,8)
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }
}
