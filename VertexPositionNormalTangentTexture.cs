using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace monogameMinecraft
{

    public struct VertexPositionNormalTangentTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 TextureCoordinate;
        public static readonly VertexDeclaration VertexDeclaration;
        public VertexPositionNormalTangentTexture(Vector3 position, Vector3 normal, Vector3 Tangent, Vector2 textureCoordinate)
        {
            this.Position = position;
            this.Normal = normal;
            this.Tangent = Tangent;
            this.TextureCoordinate = textureCoordinate;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        static VertexPositionNormalTangentTexture()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float)*3, VertexElementFormat.Vector3, VertexElementUsage.Normal,0),
                new VertexElement(sizeof(float)*6, VertexElementFormat.Vector3, VertexElementUsage.Tangent,0),
                new VertexElement(sizeof(float)*9, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0) };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }

}
