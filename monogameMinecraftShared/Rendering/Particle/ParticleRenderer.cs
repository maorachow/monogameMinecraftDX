using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;


namespace monogameMinecraftShared.Rendering.Particle
{
    public class ParticleRenderer
    {
        public Texture2D atlas;
        public Texture2D atlasNormal;
        public Texture2D atlasMER;
        public GraphicsDevice device;
        public Effect gBufferParticleEffect;

        public VertexBuffer instancingBufferGravityTextured;
        public VertexBuffer quadVertexBuffer;
        public IndexBuffer quadIndexBuffer;
     public bool isEnabled=true;
        public IGamePlayer gamePlayer;

        public ushort[] quadIndices =
        {
            0, 1, 2,
            2, 3, 0
        };
        public bool isVertsInited = false;
        public bool isQuadBuffersInited = false;

        public VertexPositionNormalTangentTextureVertID[] quadVertices;
        public List<VertexMatrix4x4UVScale> instancingDataGravityTextured;
        public void InitializeVertices()
        {
            if (isVertsInited == true) { return; }
            quadVertices = new VertexPositionNormalTangentTextureVertID[4];

            quadVertices[0].Position = new Vector3(-0.5f, 0.5f, 0);
            quadVertices[0].Normal = new Vector3(0, 0, -1);
            quadVertices[0].Tangent = new Vector3(0, 1, 0);
            quadVertices[0].TextureCoordinate = new Vector2(0, 0);
            quadVertices[0].vertID = 0;
            quadVertices[0].vertID1 = 0;

            quadVertices[1].Position = new Vector3(0.5f, 0.5f, 0);
            quadVertices[1].TextureCoordinate = new Vector2(1, 0);
            quadVertices[1].Normal = new Vector3(0, 0, -1);
            quadVertices[1].Tangent = new Vector3(0, 1, 0);
            quadVertices[1].vertID = 1;
            quadVertices[1].vertID1 = 0;

            quadVertices[2].Position = new Vector3(0.5f, -0.5f, 0);
            quadVertices[2].TextureCoordinate = new Vector2(1, 1);
            quadVertices[2].Normal = new Vector3(0, 0, -1);
            quadVertices[2].Tangent = new Vector3(0, 1, 0);
            quadVertices[2].vertID = 2;
            quadVertices[2].vertID1 = 0;

            quadVertices[3].Position = new Vector3(-0.5f, -0.5f, 0);
            quadVertices[3].TextureCoordinate = new Vector2(0, 1);
            quadVertices[3].Normal = new Vector3(0, 0, -1);
            quadVertices[3].Tangent = new Vector3(0, 1, 0);
            quadVertices[3].vertID = 3;
            quadVertices[3].vertID1 = 0;
            isVertsInited = true;
        }

        public void SetTexture(Texture2D atlas, Texture2D atlasNormal, Texture2D atlasMER)
        {
            this.atlas = atlas;
            this.atlasNormal = atlasNormal;
            this.atlasMER = atlasMER;
        }
        public void InitializeQuadBuffers(GraphicsDevice device)
        {
            if (isQuadBuffersInited == true)
            {
                quadIndexBuffer.Dispose();
                quadVertexBuffer.Dispose();
                return;
            }
            quadVertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTangentTextureVertID), 6, BufferUsage.None);

            quadVertexBuffer.SetData(quadVertices);
            quadIndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.None);
            quadIndexBuffer.SetData(quadIndices);
            isQuadBuffersInited = true;
        }


        public ParticleRenderer(Texture2D atlas, Texture2D atlasNormal, Texture2D atlasMER, GraphicsDevice device, Effect gBufferParticleEffect, IGamePlayer gamePlayer,bool isEnabled)
        {
            this.atlas = atlas;
            this.device = device;
            this.atlasNormal = atlasNormal;
            this.atlasMER = atlasMER;
            this.gBufferParticleEffect = gBufferParticleEffect;
            InitializeVertices();
            InitializeQuadBuffers(this.device);
      this.isEnabled=isEnabled;
            instancingBufferGravityTextured = new VertexBuffer(device, typeof(VertexMatrix4x4UVScale),
               1, BufferUsage.WriteOnly);
            this.gamePlayer = gamePlayer;
        }

        RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None };
        public void DrawGBuffer()
        {
            if (isEnabled==false)
            {
                return;
            }
            instancingDataGravityTextured = new List<VertexMatrix4x4UVScale>();
            //   Debug.WriteLine(ParticleManager.instance.allParticles.Count);
            foreach (var item in ParticleManager.instance.allParticles)
            {
                if (item is TexturedGravityParticle)
                {

                    TexturedGravityParticle? item1 = item as TexturedGravityParticle?;
                    if (item1 != null && item1.Value.isAlive == true)
                    {
                        VertexMatrix4x4UVScale vertex = new VertexMatrix4x4UVScale();
                        item1.Value.GetInstancingElement(gamePlayer, out vertex);
                        //     Debug.WriteLine(vertex.row3);
                        instancingDataGravityTextured.Add(vertex);
                    }


                }
            }
            //    Debug.WriteLine(instancingDataGravityTextured.Count);
            if (instancingDataGravityTextured.Count <= 0)
            {
                return;
            }
            instancingBufferGravityTextured?.Dispose();
            instancingBufferGravityTextured = new VertexBuffer(device, typeof(VertexMatrix4x4UVScale),
                instancingDataGravityTextured.Count, BufferUsage.WriteOnly);
            instancingBufferGravityTextured.SetData(instancingDataGravityTextured.ToArray());

            device.SetVertexBuffers(new VertexBufferBinding(instancingBufferGravityTextured, 0, 1), new VertexBufferBinding(quadVertexBuffer, 0));
            device.Indices = quadIndexBuffer;
            // TODO: Add your drawing code here



            device.RasterizerState = rasterizerState;
            //     basicEffect.Projection = gamePlayer.cam.projectionMatrix;
            //   basicEffect.View = view;
            //     basicEffect.World = world;

            gBufferParticleEffect.Parameters["View"].SetValue(gamePlayer.cam.viewMatrix);
            gBufferParticleEffect.Parameters["Projection"].SetValue(gamePlayer.cam.projectionMatrix);
            gBufferParticleEffect.Parameters["Texture"]?.SetValue(atlas);
            gBufferParticleEffect.Parameters["TextureNormal"]?.SetValue(atlasNormal);
            gBufferParticleEffect.Parameters["TextureMER"]?.SetValue(atlasMER);
            foreach (EffectPass pass in gBufferParticleEffect.CurrentTechnique.Passes)

            {
                pass.Apply();
                device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 6, instancingDataGravityTextured.Count);

            }
        }
    }
}
