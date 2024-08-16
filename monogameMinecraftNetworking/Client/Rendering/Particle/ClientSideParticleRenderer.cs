using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Rendering.Particle;
using monogameMinecraftShared.Updateables;

namespace monogameMinecraftNetworking.Client.Rendering.Particle
{
    public class ClientSideParticleRenderer:ParticleRenderer
    {
        public ClientSideParticleRenderer(Texture2D atlas, Texture2D atlasNormal, Texture2D atlasMER, GraphicsDevice device, Effect gBufferParticleEffect, IGamePlayer gamePlayer, bool isEnabled): base(atlas, atlasNormal, atlasMER, device, gBufferParticleEffect, gamePlayer, isEnabled)
        {
            instancingBufferGravityTextured = new VertexBuffer(device, typeof(VertexMatrix4x4UVScale),
              300, BufferUsage.WriteOnly);
        }

        public override void DrawGBuffer()
        {
            if (isEnabled == false)
            {
                return;
            }
            instancingDataGravityTextured = new List<VertexMatrix4x4UVScale>();
            //   Debug.WriteLine(ParticleManager.instance.allParticles.Count);
            foreach (var item in ParticleManager.instance.allParticles.Values)
            {
                if (item is ClientSideTexturedGravityParticle)
                {

                    ClientSideTexturedGravityParticle? item1 = item as ClientSideTexturedGravityParticle?;
                    if (item1 != null && item1.Value.isAlive == true)
                    {
                        VertexMatrix4x4UVScale vertex = new VertexMatrix4x4UVScale();
                        item1.Value.GetInstancingElement(gamePlayer, out vertex);
                        //     Debug.WriteLine(vertex.row3);
                        if (instancingDataGravityTextured.Count < 200)
                        {
                            instancingDataGravityTextured.Add(vertex);
                        }
                       
                    }


                }
            }
            //    Debug.WriteLine(instancingDataGravityTextured.Count);
            if (instancingDataGravityTextured.Count <= 0)
            {
                return;
            }
         //   instancingBufferGravityTextured?.Dispose();
          //  instancingBufferGravityTextured=new VertexBuffer()
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
