using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace monogameMinecraftDX.Utility
{
 
  
    //using Microsoft.Xna.Framework;

    
        /// <summary>
        /// This class creates a encapsulated renderable line to aid in visualizing a line in 3d.
        /// </summary>
        public class VisualizationLine
        {
            public VertexPositionNormalTexture[] vertices;
            public int[] indices;

            public Texture2D texture;
            public BasicEffect basicEffect;

            public Matrix World { set { basicEffect.World = value; } get { return basicEffect.World; } }
            public Matrix View { set { basicEffect.View = value; } get { return basicEffect.View; } }
            public Matrix Projection { set { basicEffect.Projection = value; } get { return basicEffect.Projection; } }
            public Texture2D Texture { set { basicEffect.Texture = value; } get { return basicEffect.Texture; } }

            public void SetUpBasicEffect(GraphicsDevice device, Texture2D texture, Matrix view, Matrix proj)
            {
                basicEffect = new BasicEffect(device);
                basicEffect.VertexColorEnabled = false;
                basicEffect.TextureEnabled = true;
                World = Matrix.Identity;
                basicEffect.View = view;
                basicEffect.Projection = proj;
                basicEffect.Texture = texture;
            }

            public VisualizationLine(Texture2D t, Vector3 start, Vector3 end, float thickness, Color c)
            {
                List<VertexPositionNormalTexture> nverts = new List<VertexPositionNormalTexture>();
                List<int> nindices = new List<int>();
                texture = t;
                int sides = 4; // define the number of sides of the cylinder.   
                int lineVerts = sides * 2; // the number of vertices
                int lineIndices = sides * 6; // the number of indices
                for (int k = 0; k < lineVerts; k++)
                    nverts.Add(new VertexPositionNormalTexture());
                for (int j = 0; j < lineIndices; j++)
                    nindices.Add(0);
                vertices = nverts.ToArray();
                indices = nindices.ToArray();
                ReCreateVisualLine(t, start, end, thickness, c);
            }

            public void ReCreateVisualLine(Texture2D t, Vector3 start, Vector3 end, float thickness, Color c)
            {
                // verts
                texture = t;
                int sides = 4; // define the number of sides of the cylinder.       
                var dir = end - start;
                var axis = Vector3.Normalize(dir);
                float radMult = 6.28318f / (sides + .02f);
                int currentVert = 0;
                for (int k = 0; k < sides; k++)
                {
                    var m = Matrix.CreateFromAxisAngle(axis, (float)(k) * radMult);
                    vertices[currentVert + 0] = new VertexPositionNormalTexture() { Position = (m.Right * thickness + start), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 0f) };
                    vertices[currentVert + 1] = new VertexPositionNormalTexture() { Position = (m.Right * thickness + end), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                    currentVert += 2;
                }
                // indices
                for (int quadindex = 0; quadindex < sides; quadindex++)
                {
                    int offsetVertice = quadindex * 2;
                    int offsetIndice = quadindex * 6;
                    if (quadindex != sides - 1)
                    {
                        indices[offsetIndice + 0] = offsetVertice + 0;
                        indices[offsetIndice + 1] = offsetVertice + 1;
                        indices[offsetIndice + 2] = offsetVertice + 2;
                        indices[offsetIndice + 3] = offsetVertice + 2;
                        indices[offsetIndice + 4] = offsetVertice + 1;
                        indices[offsetIndice + 5] = offsetVertice + 3;
                    }
                    else // the last face wraps around well sort of manually attach this.
                    {
                        indices[offsetIndice + 0] = offsetVertice + 0;
                        indices[offsetIndice + 1] = offsetVertice + 1;
                        indices[offsetIndice + 2] = 0;
                        indices[offsetIndice + 3] = 0;
                        indices[offsetIndice + 4] = offsetVertice + 1;
                        indices[offsetIndice + 5] = 1;
                    }
                }
            }

            public void Draw(GraphicsDevice gd)
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionNormalTexture.VertexDeclaration);
                }
            }

            public void Draw(GraphicsDevice gd, Effect effect)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionNormalTexture.VertexDeclaration);
                }
            }
        }
    }

