using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
 
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Updateables;
namespace monogameMinecraftShared.Rendering
{
    public class ChunkRenderer
    {

        public MinecraftGameBase game;
        public GraphicsDevice device;
        //  public AlphaTestEffect basicNSShader;
        public Effect basicShader;
        public Effect deferredShader;
        public Texture2D atlas;
        public Texture2D atlasNormal;
        public Texture2D atlasDepth;
        public Texture2D atlasMER;
        //Dictionary<Vector2Int,Chunk> RenderingChunks

        public ShadowRenderer shadowRenderer;
        public SSAORenderer SSAORenderer;
        public SSRRenderer SSRRenderer;
        public GameTimeManager gameTimeManager;
        public PointLightUpdater lightUpdater;
        public void SetTexture(Texture2D texNormal, Texture2D textureDepth, Texture2D texNoMip, Texture2D texMER)
        {


            // TerrainMipmapGenerator.instance.GenerateMipmap(texNoMip);
            atlas = TerrainMipmapGenerator.instance.GenerateMipmap(texNoMip);
            /*    Color[] atlasMip0= new Color[tex.Width* tex.Height];
                 Color[] atlasMip1 = new Color[tex.Width/2 * tex.Height/2];
                 Color[] atlasMip2 = new Color[tex.Width/4 * tex.Height/4];
                 Color[] atlasMip3 = new Color[tex.Width / 8 * tex.Height / 8];
                 Color[] atlasMip4 = new Color[tex.Width / 16 * tex.Height / 16];
                 Color[] atlasMip5 = new Color[tex.Width / 32 * tex.Height / 32];

                 tex.GetData<Color>(0,null, atlasMip0, 0, tex.Width * tex.Height);
                 tex.GetData<Color>(1, null, atlasMip1, 0, tex.Width/2 * tex.Height / 2);
                 tex.GetData<Color>(2, null, atlasMip2, 0, tex.Width / 4 * tex.Height / 4);
                 tex.GetData<Color>(3, null, atlasMip3, 0, tex.Width /8* tex.Height/8);
                 tex.GetData<Color>(4, null, atlasMip4, 0, tex.Width / 16 * tex.Height /16);
                 tex.GetData<Color>(5, null, atlasMip5, 0, tex.Width / 32 * tex.Height / 32);

                 atlas.SetData<Color>(0, 0,null, atlasMip0, 0, atlas.Width * atlas.Height);
                 atlas.SetData<Color>(1, 0,null, atlasMip1, 0, atlas.Width/2 * atlas.Height / 2);
                 atlas.SetData<Color>(2, 0,null, atlasMip2, 0, atlas.Width / 4 * atlas.Height / 4);
                 atlas.SetData<Color>(3, 0, null, atlasMip3, 0, atlas.Width/8 * atlas.Height/8);
                 atlas.SetData<Color>(4, 0, null, atlasMip4, 0, atlas.Width / 16 * atlas.Height / 16);
                 atlas.SetData<Color>(5, 0, null, atlasMip5, 0, atlas.Width /32 * atlas.Height / 32);*/
            atlasNormal = TerrainMipmapGenerator.instance.GenerateMipmap(texNormal, true);
            atlasMER = texMER;
            atlasDepth = textureDepth;

          //  basicShader.Parameters["Texture"].SetValue(atlas);
            //  basicShader.Parameters["TextureNormal"].SetValue(atlasNormal);
            //    basicShader.Parameters["TextureDepth"].SetValue(atlasDepth);
            //     basicShader.Parameters["TextureAO"].SetValue(SSAORenderer.ssaoTarget);
        }
        public ChunkRenderer(MinecraftGameBase game, GraphicsDevice device, Effect basicSolidShader, ShadowRenderer shadowRenderer, GameTimeManager gameTimeManager)
        {
            this.game = game;
            this.device = device;
            this.shadowRenderer = shadowRenderer;
            basicShader = basicSolidShader;
            device.BlendState = BlendState.NonPremultiplied;
            device.DepthStencilState = DepthStencilState.Default;

            this.gameTimeManager = gameTimeManager;


        }

        public void RenderAllChunksGBuffer(ConcurrentDictionary<Vector2Int, Chunk> RenderingChunks, GamePlayer player, Effect gBufferEffect)
        {

            gBufferEffect.Parameters["blockTex"].SetValue(atlas);
            gBufferEffect.Parameters["normalTex"]?.SetValue(atlasNormal);
            gBufferEffect.Parameters["merTex"]?.SetValue(atlasMER);
            gBufferEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
            gBufferEffect.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            BoundingFrustum frustum = new BoundingFrustum(player.cam.viewMatrix * player.cam.projectionMatrix);

            foreach (var chunk in RenderingChunks)
            {
                Chunk c = chunk.Value;
                if (c == null)
                {
                    continue;
                }

                if (c.isReadyToRender == true && c.disposed == false && c.isUnused == false)
                {
                    if (frustum.Intersects(c.chunkBounds))
                    {
                        if (MathF.Abs(c.chunkPos.x - player.position.X) < 384 && MathF.Abs(c.chunkPos.y - player.position.Z) < 384)
                        {
                            RenderSingleChunkGBuffer(c, player, gBufferEffect, false);
                        }
                        else
                        {
                            RenderSingleChunkGBuffer(c, player, gBufferEffect, true);
                        }
                    }










                }
            }
            foreach (var chunk in RenderingChunks)
            {
                Chunk c = chunk.Value;
                if (c == null)
                {
                    continue;
                }

                if (c.isReadyToRender == true && c.disposed == false && c.isUnused == false)
                {
                    if (MathF.Abs(c.chunkPos.x - player.position.X) < 256 && MathF.Abs(c.chunkPos.y - player.position.Z) < 256)
                    {
                        if (frustum.Intersects(c.chunkBounds))
                        {
                            RenderSingleChunkGBufferAlphaTest(c, player, gBufferEffect);

                        }
                    }





                }
            }
            foreach (var chunk in RenderingChunks)
            {
                Chunk c = chunk.Value;
                if (c == null)
                {
                    continue;
                }

                if (c.isReadyToRender == true && c.disposed == false && c.isUnused == false)
                {

                    if (frustum.Intersects(c.chunkBounds))
                    {
                        RenderSingleChunkGBufferWater(c, player, gBufferEffect);

                    }




                }
            }


        }
        public void RenderSingleChunkGBuffer(Chunk c, GamePlayer player, Effect gBufferEffect, bool isLOD = false)
        {
            if (isLOD)
            {
                if ((c.indicesOpqLOD1Array.Length <= 0 || c.VBOpqLOD1 == null || c.IBOpqLOD1 == null) && c.isOpqBuffersValid == false)
                {
                    return;
                }
                Matrix world = Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y));
                gBufferEffect.Parameters["World"].SetValue(world);
                //   gBufferEffect.Parameters["TransposeInverseView"].SetValue(Matrix.Transpose(Matrix.Invert(world*player.cam.viewMatrix)));
                //  gBufferEffect.Parameters["roughness"].SetValue(0.0f);

                device.SetVertexBuffer(c.VBOpqLOD1);

                device.Indices = c.IBOpqLOD1;

                foreach (EffectPass pass in gBufferEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.indicesOpqLOD1Array.Length / 3);

                }
            }
            else
            {
                if ((c.indicesOpqArray.Length <= 0 || c.VBOpq == null || c.IBOpq == null) && c.isOpqBuffersValid == false)
                {
                    return;
                }

                lock (c.asyncTaskLock)
                {
                    Matrix world = Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y));
                    gBufferEffect.Parameters["World"].SetValue(world);
                    //   gBufferEffect.Parameters["TransposeInverseView"].SetValue(Matrix.Transpose(Matrix.Invert(world*player.cam.viewMatrix)));
                    //  gBufferEffect.Parameters["roughness"].SetValue(0.0f);

                    device.SetVertexBuffer(c.VBOpq);

                    device.Indices = c.IBOpq;

                    foreach (EffectPass pass in gBufferEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.indicesOpqArray.Length / 3);

                    }
                }



            }

        }

        public void RenderSingleChunkGBufferWater(Chunk c, GamePlayer player, Effect gBufferEffect)
        {
            if (c.indicesWTArray.Length > 0 && c.isWTBuffersValid == true)
            {
                Matrix world = Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y));
                gBufferEffect.Parameters["World"].SetValue(world);
                //    gBufferEffect.Parameters["TransposeInverseView"].SetValue(Matrix.Transpose(Matrix.Invert(world * player.cam.viewMatrix)));
                //   gBufferEffect.Parameters["roughness"].SetValue(1f);

                device.SetVertexBuffer(c.VBWT);

                device.Indices = c.IBWT;

                foreach (EffectPass pass in gBufferEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.indicesWTArray.Length / 3);

                }
            }



        }


        public void RenderSingleChunkGBufferAlphaTest(Chunk c, GamePlayer player, Effect gBufferEffect)
        {
            if (c.indicesNSArray.Length > 0 && c.isNSBuffersValid == true)
            {
                Matrix world = Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y));
                gBufferEffect.Parameters["World"].SetValue(world);
                //  gBufferEffect.Parameters["TransposeInverseView"].SetValue(Matrix.Transpose(Matrix.Invert(world * player.cam.viewMatrix)));
                //   gBufferEffect.Parameters["roughness"].SetValue(0f);

                device.SetVertexBuffer(c.VBNS);

                device.Indices = c.IBNS;

                foreach (EffectPass pass in gBufferEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.indicesNSArray.Length / 3);

                }
            }



        }
        public void RenderAllChunksOpq(ConcurrentDictionary<Vector2Int, Chunk> RenderingChunks, GamePlayer player)
        {

            isBusy = true;
            basicShader.Parameters["Texture"].SetValue(atlas);
            basicShader.Parameters["TextureNormal"].SetValue(atlasNormal);
            basicShader.Parameters["TextureDepth"].SetValue(atlasDepth);

            basicShader.Parameters["View"].SetValue(player.cam.viewMatrix);
            basicShader.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            basicShader.Parameters["fogStart"].SetValue(256.0f);
            basicShader.Parameters["fogRange"].SetValue(1024.0f);
            basicShader.Parameters["LightColor"].SetValue(new Vector3(1, 1, 1));
            basicShader.Parameters["LightDir"].SetValue(gameTimeManager.sunDir);
            //  basicShader.Parameters["LightPos"].SetValue(player.position + new Vector3(10, 50, 30));
            basicShader.Parameters["viewPos"].SetValue(player.cam.position);
            // shadowmapShader.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
            //     RenderShadow(RenderingChunks, player,lightSpaceMat);
            basicShader.Parameters["TextureAO"].SetValue(SSAORenderer.ssaoTarget);
            basicShader.Parameters["receiveAO"].SetValue(true);

            BoundingFrustum frustum = new BoundingFrustum(player.cam.viewMatrix * player.cam.projectionMatrix);

            basicShader.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
            basicShader.Parameters["LightSpaceMatFar"].SetValue(shadowRenderer.lightSpaceMatFar);
            basicShader.Parameters["ShadowMap"].SetValue(shadowRenderer.shadowMapTarget);
            basicShader.Parameters["shadowBias"].SetValue(shadowRenderer.shadowBias);

            for (int i = 0; i < lightUpdater.lights.Count; i++)
            {
                basicShader.Parameters["LightPosition" + (i + 1).ToString()].SetValue(lightUpdater.lights[i]);
            }
            Vector3 lightPosition1 = basicShader.Parameters["LightPosition1"].GetValueVector3();
            Vector3 lightPosition2 = basicShader.Parameters["LightPosition2"].GetValueVector3();
            Vector3 lightPosition3 = basicShader.Parameters["LightPosition3"].GetValueVector3();
            Vector3 lightPosition4 = basicShader.Parameters["LightPosition4"].GetValueVector3();
            //    Debug.WriteLine(lightPosition1);
            foreach (var lightD in lightUpdater.lightsDestroying)
            {

                if (lightD.Equals(lightPosition1))
                {
                    basicShader.Parameters["LightPosition1"].SetValue(new Vector3(0, 0, 0));
                    Debug.WriteLine("destroy");
                }
                if (lightD.Equals(lightPosition2))
                {
                    basicShader.Parameters["LightPosition2"].SetValue(new Vector3(0, 0, 0));
                }
                if (lightD.Equals(lightPosition3))
                {
                    basicShader.Parameters["LightPosition3"].SetValue(new Vector3(0, 0, 0));
                }
                if (lightD.Equals(lightPosition4))
                {
                    basicShader.Parameters["LightPosition4"].SetValue(new Vector3(0, 0, 0));
                }
            }
            basicShader.Parameters["receiveReflection"].SetValue(false);
            basicShader.Parameters["receiveBackLight"].SetValue(false);
            if (gameTimeManager.sunX > 160f || gameTimeManager.sunX <= 20f)
            {
                basicShader.Parameters["receiveShadow"].SetValue(false);

            }
            else
            {
                basicShader.Parameters["receiveShadow"].SetValue(true);

            }
            foreach (var chunk in RenderingChunks)
            {
                Chunk c = chunk.Value;
                if (c == null)
                {
                    continue;
                }
                lock (c.renderLock)
                {
                    if (c.isReadyToRender == true && c.disposed == false)
                    {

                        if (frustum.Intersects(c.chunkBounds))
                        {
                            RenderSingleChunkOpq(c, player);

                        }


                    }
                }

            }



            isBusy = false;
        }

        public void RenderAllChunksTransparent(ConcurrentDictionary<Vector2Int, Chunk> RenderingChunks, GamePlayer player)
        {

            isBusy = true;
            basicShader.Parameters["Texture"].SetValue(atlas);
            basicShader.Parameters["View"].SetValue(player.cam.viewMatrix);
            basicShader.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            basicShader.Parameters["fogStart"].SetValue(256.0f);
            basicShader.Parameters["fogRange"].SetValue(1024.0f);
            basicShader.Parameters["LightColor"].SetValue(new Vector3(1, 1, 1));
            basicShader.Parameters["LightDir"].SetValue(new Vector3(20, 40, 30));
            //    basicShader.Parameters["LightPos"].SetValue(player.position + new Vector3(10, 50, 30));

            BoundingFrustum frustum = new BoundingFrustum(player.cam.viewMatrix * player.cam.projectionMatrix);

            basicShader.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
            basicShader.Parameters["LightSpaceMatFar"].SetValue(shadowRenderer.lightSpaceMatFar);
            basicShader.Parameters["ShadowMap"].SetValue(shadowRenderer.shadowMapTarget);
            basicShader.Parameters["ShadowMapFar"].SetValue(shadowRenderer.shadowMapTargetFar);
            basicShader.Parameters["receiveAO"].SetValue(false);
            basicShader.Parameters["TextureReflection"].SetValue(SSRRenderer.renderTargetSSR);

            foreach (var chunk in RenderingChunks)
            {
                Chunk c = chunk.Value;
                if (c == null)
                {
                    continue;
                }
                lock (c.renderLock)
                {


                    if (c.isReadyToRender == true && c.disposed == false)
                    {
                        basicShader.Parameters["receiveReflection"].SetValue(true);
                        basicShader.Parameters["receiveBackLight"].SetValue(false);

                        if (frustum.Intersects(c.chunkBounds))
                        {
                            RenderSingleChunkWater(c, player);

                        }
                        basicShader.Parameters["receiveReflection"].SetValue(false);
                        basicShader.Parameters["receiveBackLight"].SetValue(true);
                        if (MathF.Abs(c.chunkPos.x - player.position.X) < 256 && MathF.Abs(c.chunkPos.y - player.position.Z) < 256)
                        {


                            if (frustum.Intersects(c.chunkBounds))
                            {
                                RenderSingleChunkTransparent(c, player);

                            }


                        }
                    }
                }


            }


            isBusy = false;
        }


        public static bool isBusy = false;
        public void RenderShadow(ConcurrentDictionary<Vector2Int, Chunk> RenderingChunks, GamePlayer player, Matrix lightSpaceMat, Effect shadowmapShader, int maxRenderDistance, bool useFrustumCulling = true)
        {

            shadowmapShader.Parameters["LightSpaceMat"].SetValue(lightSpaceMat);

            BoundingFrustum frustum = new BoundingFrustum(player.cam.viewMatrix * player.cam.projectionMatrix);
            lock (VoxelWorld.currentWorld.worldUpdater.chunksNeededRebuildListLock)
            {
                foreach (var chunk in RenderingChunks)
                {
                    Chunk c = chunk.Value;
                    if (c == null)
                    {
                        continue;
                    }
                    if (MathF.Abs(c.chunkPos.x - player.position.X) < maxRenderDistance && MathF.Abs(c.chunkPos.y - player.position.Z) < maxRenderDistance)
                    {
                        if (frustum.Intersects(c.chunkBounds) || useFrustumCulling == false)
                        {
                            if (c.isReadyToRender == true && c.disposed == false)
                            {
                                RenderSingleChunkShadow(c, shadowmapShader);
                            }
                        }



                    }

                }
            }

            /*      foreach(var entity in EntityBeh.worldEntities)
                  {
                      EntityRenderer.DrawModelShadow(EntityRenderer.zombieModel, Matrix.CreateTranslation(entity.position),lightSpaceMat);
                  }
                  device.SetRenderTarget(null);
                  device.Clear(Color.CornflowerBlue);
                  basicShader.Parameters["LightSpaceMat"].SetValue(lightSpaceMat);*/
        }

        void RenderSingleChunkShadow(Chunk c, Effect shadowmapShader)
        {
            /*     if (c.isTaskCompleted == false)
                 {
                     return;
                 }
                 if (c.indicesOpqArray.Length <= 0|| c.VBOpq==null|| c.IBOpq==null)
                 {
                     return;
                 }

                if (c.VBOpq != null && c.VBOpq.IsDisposed == true)
                 {
                     return;
                 }
                 if (c.IBOpq != null && c.IBOpq.IsDisposed == true)
                 {
                     return;
                 }*/

            if (c.isOpqBuffersValid)
            {
                Matrix world = Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y));
                shadowmapShader.Parameters["World"].SetValue(world);

                device.SetVertexBuffer(c.VBOpq);

                device.Indices = c.IBOpq;

                foreach (EffectPass pass in shadowmapShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.indicesOpqArray.Length / 3);

                }
            }



        }
        void RenderSingleChunkWater(Chunk c, GamePlayer player)
        {
            basicShader.Parameters["World"].SetValue(Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y)));
            //   basicShader.Parameters["receiveReflection"].SetValue(true);
            if (c.verticesWTArray.Length > 0)
            {
                //buffer.SetData(c.verticesWTArray);

                //   bufferIndex.SetData(c.indicesWTArray);

                device.Indices = c.IBWT;
                device.SetVertexBuffer(c.VBWT);
                basicShader.Parameters["Alpha"].SetValue(0.7f);
                basicShader.Parameters["viewPos"].SetValue(player.cam.position);

                foreach (EffectPass pass in basicShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.indicesWTArray.Length / 3);
                }
            }
        }
        void RenderSingleChunkTransparent(Chunk c, GamePlayer player)
        {

            basicShader.Parameters["World"].SetValue(Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y)));

            basicShader.Parameters["viewPos"].SetValue(player.cam.position);
            basicShader.Parameters["Alpha"].SetValue(1.0f);

            if (c.verticesNSArray.Length > 0)
            {
                //      basicShader.Parameters["receiveReflection"].SetValue(false);
                device.SetVertexBuffer(c.VBNS);

                device.Indices = c.IBNS;

                foreach (EffectPass pass in basicShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.indicesNSArray.Length / 3);
                }
            }

        }
        void RenderSingleChunkOpq(Chunk c, GamePlayer player)
        {
            //    basicShader.Parameters["renderShadow"].SetValue(true);

            basicShader.Parameters["World"].SetValue(Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y)));
            //    basicNSShader.World= Matrix.CreateTranslation(new Vector3(c.chunkPos.x, 0, c.chunkPos.y));
            //Debug.WriteLine("render");



            basicShader.Parameters["Alpha"].SetValue(1.0f);

            basicShader.Parameters["viewPos"].SetValue(game.gamePlayer.cam.position);
            //    basicShader.Parameters["fogDensity"].SetValue(0.01f);
            // buffer.SetData(c.verticesOpqArray);
            device.SetVertexBuffer(c.VBOpq);

            // bufferIndex.SetData(c.indicesOpqArray);
            device.Indices = c.IBOpq;
            foreach (EffectPass pass in basicShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.indicesOpqArray.Length / 3);
            }
            //     device.DepthStencilState  =DepthStencilState.None;

            //   device.DepthStencilState = DepthStencilState.Default;
            //   device.DepthStencilState = DepthStencilState.None;

            //    device.DepthStencilState = DepthStencilState.Default;
            //  basicShader.Parameters["Alpha"].SetValue(1.0f);
            //  basicShader.Alpha = 1f;




        }
    }
}
