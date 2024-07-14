using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using SharpDX.MediaFoundation;
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
                if (basicEffect == null)
                {
                    basicEffect = new BasicEffect(device);
            }
              
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
                int sides = 2; // define the number of sides of the cylinder.   
                int lineVerts = sides * 8; // the number of vertices
                int lineIndices = sides *12; // the number of indices
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
            int sides = 2; // define the number of sides of the cylinder.       
            var dir = end - start;
            var axis = (dir);
            float radMult = 6.28318f / (sides + .02f);
            int currentVert = 0;
            for (int k = 0; k < sides; k++)
            {
                var m = Matrix.CreateFromAxisAngle(axis, k);
                vertices[currentVert + 0] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(m.Forward.Length() > 0.001f ? m.Forward : m.Up) * thickness * 0.5f + start), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 0f) };
                vertices[currentVert + 1] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(m.Forward.Length() > 0.001f ? m.Forward : m.Up) * -thickness * 0.5f + start), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                vertices[currentVert + 2] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(m.Forward.Length() > 0.001f ? m.Forward : m.Up) * thickness * 0.5f + end), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                vertices[currentVert + 3] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(m.Forward.Length() > 0.001f ? m.Forward : m.Up) * -thickness * 0.5f + end), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };

                vertices[currentVert + 4] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(m.Forward.Length() > 0.0001f ? m.Forward : m.Up) * thickness * 0.5f + start), Normal = -axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 0f) };
                vertices[currentVert + 5] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(m.Forward.Length() > 0.001f ? m.Forward : m.Up) * -thickness * 0.5f + start), Normal = -axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                vertices[currentVert + 6] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(m.Forward.Length() > 0.001f ? m.Forward : m.Up) * thickness * 0.5f + end), Normal = -axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                vertices[currentVert + 7] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(m.Forward.Length() > 0.001f ? m.Forward : m.Up) * -thickness * 0.5f + end), Normal = -axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                currentVert += 8;
            }
            // indices

            int offsetVertice = 0;
            indices[0 + 0] = offsetVertice + 0;
            indices[0 + 1] = offsetVertice + 3;
            indices[0 + 2] = offsetVertice + 1;
            indices[0 + 3] = offsetVertice + 0;
            indices[0 + 4] = offsetVertice + 2;
            indices[0 + 5] = offsetVertice + 3;


            offsetVertice = 4;
            indices[0 + 12] = offsetVertice + 0;
            indices[0 + 13] = offsetVertice + 1;
            indices[0 + 14] = offsetVertice + 3;
            indices[0 + 15] = offsetVertice + 0;
            indices[0 + 16] = offsetVertice + 3;
            indices[0 + 17] = offsetVertice + 2;
            offsetVertice = 8;
            indices[0 + 6] = offsetVertice + 0;
            indices[0 + 7] = offsetVertice + 3;
            indices[0 + 8] = offsetVertice + 1;
            indices[0 + 9] = offsetVertice + 0;
            indices[0 + 10] = offsetVertice + 2;
            indices[0 + 11] = offsetVertice + 3;






            offsetVertice = 12;
            indices[0 + 18] = offsetVertice + 0;
            indices[0 + 19] = offsetVertice + 1;
            indices[0 + 20] = offsetVertice + 3;
            indices[0 + 21] = offsetVertice + 0;
            indices[0 + 22] = offsetVertice + 3;
            indices[0 + 23] = offsetVertice + 2;

        }
        public void ReCreateVisualLine( Vector3 start, Vector3 end, float thickness,Matrix viewMatrix)
            {

             
            int sides = 2; // define the number of sides of the cylinder.       
            var dir =Vector3.Normalize(end - start);


        
        //    float3x3 TBN = float3x3(tangent, bitangent, dir);
            var axis = (dir);
            float radMult = 6.28318f / (sides + .02f);
            int currentVert = 0;
            for (int k = 0; k < sides; k++)
            {

                Vector3 randomVec =Vector3.Normalize(k>0?new Vector3(-0.3f,1f,-1f): new Vector3(1f, -0f, 1f)); 
                Vector3 tangent = Vector3.Normalize(randomVec - dir * Vector3.Dot(randomVec, dir));
                Vector3 bitangent = Vector3.Cross(dir, tangent);
          
                vertices[currentVert + 0] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(bitangent) * thickness*0.5f + start), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 0f) };
                vertices[currentVert + 1] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(bitangent) * -thickness * 0.5f + start), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                vertices[currentVert + 2] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(bitangent) * thickness * 0.5f + end), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                vertices[currentVert + 3] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(bitangent) * -thickness * 0.5f + end), Normal = axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };

                vertices[currentVert + 4] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(bitangent) * thickness * 0.5f + start), Normal = -axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 0f) };
                vertices[currentVert + 5] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(bitangent) * -thickness * 0.5f + start), Normal = -axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                vertices[currentVert + 6] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(bitangent) * thickness * 0.5f + end), Normal = -axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                vertices[currentVert + 7] = new VertexPositionNormalTexture() { Position = (Vector3.Normalize(bitangent) * -thickness * 0.5f + end), Normal = -axis, TextureCoordinate = new Vector2((float)(k) / (float)(sides - 1), 1f) };
                currentVert += 8;
            }
            // indices
           
                int offsetVertice = 0;
                indices[0 + 0] = offsetVertice + 0;
                indices[0 + 1] = offsetVertice + 3;
                indices[0 + 2] = offsetVertice + 1;
                indices[0 + 3] = offsetVertice + 0;
                indices[0 + 4] = offsetVertice + 2;
                indices[0 + 5] = offsetVertice + 3;


                offsetVertice = 4;
                indices[0 + 12] = offsetVertice + 0;
                indices[0 + 13] = offsetVertice + 1;
                indices[0 + 14] = offsetVertice + 3;
                indices[0 + 15] = offsetVertice + 0;
                indices[0 + 16] = offsetVertice + 3;
                indices[0 + 17] = offsetVertice + 2;
            offsetVertice = 8;
            indices[0 + 6] = offsetVertice + 0;
            indices[0 + 7] = offsetVertice + 3;
            indices[0 + 8] = offsetVertice + 1;
            indices[0 + 9] = offsetVertice + 0;
            indices[0 + 10] = offsetVertice + 2;
            indices[0 + 11] = offsetVertice + 3;



          


            offsetVertice = 12;
            indices[0 + 18] = offsetVertice + 0;
            indices[0 + 19] = offsetVertice + 1;
            indices[0 + 20] = offsetVertice + 3;
            indices[0 + 21] = offsetVertice + 0;
            indices[0 + 22] = offsetVertice + 3;
            indices[0 + 23] = offsetVertice + 2;


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


        public static class BoundingBoxVisualizationUtility
        {
            public static VisualizationLine line;
            public static Texture2D texture;
            public static GraphicsDevice device;
            public static void Initialize(Texture2D tex,GraphicsDevice device)
            {
                texture = tex;
               BoundingBoxVisualizationUtility.device = device;
                line = new VisualizationLine(texture, new Vector3(), new Vector3(), 0.1f, new Color(1, 1, 1));
            }

            public static void VisualizeBoundingBox(BoundingBox bounds,Matrix view,Matrix projection)
            {
            DrawLine(new Vector3(bounds.Max.X, bounds.Max.Y, bounds.Max.Z), new Vector3(bounds.Max.X, bounds.Min.Y, bounds.Max.Z), view,projection);
            DrawLine(new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Min.Z), new Vector3(bounds.Min.X, bounds.Max.Y, bounds.Min.Z), view,projection);
            DrawLine(new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Min.Z), new Vector3(bounds.Max.X, bounds.Min.Y, bounds.Min.Z), view,projection);
            DrawLine(new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Min.Z), new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Max.Z), view,projection);
            DrawLine(new Vector3(bounds.Max.X, bounds.Max.Y, bounds.Max.Z), new Vector3(bounds.Min.X, bounds.Max.Y, bounds.Max.Z), view,projection);
            DrawLine(new Vector3(bounds.Max.X, bounds.Max.Y, bounds.Max.Z), new Vector3(bounds.Max.X, bounds.Max.Y, bounds.Min.Z), view,projection);
            DrawLine(new Vector3(bounds.Max.X, bounds.Min.Y, bounds.Min.Z), new Vector3(bounds.Max.X, bounds.Max.Y, bounds.Min.Z), view,projection);
            DrawLine(new Vector3(bounds.Max.X, bounds.Min.Y, bounds.Min.Z), new Vector3(bounds.Max.X, bounds.Min.Y, bounds.Max.Z), view,projection);
            DrawLine(new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Max.Z), new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Min.Z), view,projection);
            DrawLine(new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Max.Z), new Vector3(bounds.Min.X, bounds.Max.Y, bounds.Max.Z), view,projection);
            DrawLine(new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Max.Z), new Vector3(bounds.Max.X, bounds.Min.Y, bounds.Max.Z), view,projection);
                DrawLine(new Vector3(bounds.Min.X, bounds.Min.Y, bounds.Min.Z), new Vector3(bounds.Max.X, bounds.Min.Y, bounds.Min.Z), view, projection);
                DrawLine(new Vector3(bounds.Min.X, bounds.Max.Y, bounds.Min.Z), new Vector3(bounds.Max.X, bounds.Max.Y, bounds.Min.Z), view, projection);
                DrawLine(new Vector3(bounds.Min.X, bounds.Max.Y, bounds.Min.Z), new Vector3(bounds.Min.X, bounds.Max.Y, bounds.Max.Z), view, projection);
            //  DrawLine(new Vector3(bounds.Min.X,bounds.Min.Y,bounds.Min.Z),new Vector3(bounds.Max.X,bounds.Max.Y,bounds.Max.Z),view,projection);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Matrix view, Matrix projection)
            {
                line.ReCreateVisualLine( start, end, 0.1f,view);
                line.SetUpBasicEffect(device,texture,view,projection);
                line.Draw(device);
            }
        }
    }

