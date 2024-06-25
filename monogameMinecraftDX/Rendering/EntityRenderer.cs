using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftDX.World;
using System;
using System.Collections.Generic;

namespace monogameMinecraftDX.Rendering
{
    public class EntityRenderer
    {
        public Effect basicShader;
        public Effect shadowMapShader;
        public GraphicsDevice device;
        public static Model zombieModel;
        public Model zombieModelRef;
        public GamePlayer player;
        public Texture2D zombieTex;
        public static MinecraftGame game;
        public ShadowRenderer shadowRenderer;
        public GameTimeManager gameTimeManager;
        public EntityRenderer(MinecraftGame game, GraphicsDevice device, GamePlayer player, Effect shader, Model model, Texture2D zombieTex, Model zombieModelRef, Effect shadowmapShader, ShadowRenderer sr, GameTimeManager gameTimeManager)
        {
            this.device = device;
            basicShader = shader;
            zombieModel = model;
            this.zombieModelRef = zombieModelRef;
            this.player = player;
            shadowMapShader = shadowmapShader;
            this.zombieTex = zombieTex;
            shadowRenderer = sr;
            EntityRenderer.game = game;
            this.gameTimeManager = gameTimeManager;
            /*  foreach(var modelMesh in zombieModel.Meshes)
               {
                   foreach(var modelMeshPart in modelMesh.MeshParts)
                   {

                       VertexPositionTexture[] vertices = new VertexPositionTexture[modelMeshPart.VertexBuffer.VertexCount];
                       ushort[] indices=new ushort[modelMeshPart.IndexBuffer.IndexCount];
                       modelMeshPart.IndexBuffer.GetData <ushort>(indices);
                       modelMeshPart.VertexBuffer.GetData<VertexPositionTexture>(vertices);
                       VertexPositionNormalTexture[] verticesNew = new VertexPositionNormalTexture[modelMeshPart.VertexBuffer.VertexCount];
                       for(int j = 0; j < vertices.Length; j++)
                       {
                           verticesNew[j]=new VertexPositionNormalTexture(vertices[j].Position,new Vector3(0, 0, 0), vertices[j].TextureCoordinate);
                       }
                       for(int i=0;i<indices.Length; i+=3)
                       {
                           Vector3 pos1 = verticesNew[indices[i]].Position;
                           Vector3 pos2 = verticesNew[indices[i +1]].Position;
                           Vector3 pos3 = verticesNew[indices[i +2]].Position;
                           Vector3 normal = Vector3.Normalize(Vector3.Cross(pos3 - pos2, pos2 - pos1)); 

                           verticesNew[indices[i]].Normal = normal;
                           verticesNew[indices[i + 1]].Normal = normal;
                           verticesNew[indices[i + 2]].Normal = normal;
                       }
                       VertexBuffer vertexBufferNew = new VertexBuffer(device, typeof(VertexPositionNormalTexture), verticesNew.Length, BufferUsage.WriteOnly);
                       modelMeshPart.VertexBuffer=vertexBufferNew;
                       modelMeshPart.VertexBuffer.SetData(verticesNew);
                      foreach(var vertex in verticesNew)
                       {
                           Debug.WriteLine(vertex.ToString()); 
                       }
                       foreach (var index in indices)
                       {
                           Debug.WriteLine(index);
                       }
                   }
               }*/
        }

        public void Draw()
        {
            //    basicShader.Parameters["View"].SetValue(player.cam.GetViewMatrix());
            //   basicShader.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            //      
            basicShader.Parameters["TextureE"].SetValue(zombieTex);
            basicShader.Parameters["ShadowMapC"]?.SetValue(shadowRenderer.shadowMapTarget);
            basicShader.Parameters["LightSpaceMat"].SetValue(shadowRenderer.lightSpaceMat);
            //          basicShader.Parameters["LightSpaceMatFar"].SetValue(shadowRenderer.lightSpaceMatFar);
            //        basicShader.Parameters["ShadowMapCFar"].SetValue(shadowRenderer.shadowMapTargetFar);
            //      zombieModel.Bones["head"].Transform = Matrix.CreateScale(0.5f);
            if (gameTimeManager.sunX > 160f || gameTimeManager.sunX <= 20f)
            {
                basicShader.Parameters["receiveShadow"].SetValue(false);
            }
            else
            {
                basicShader.Parameters["receiveShadow"].SetValue(true);
            }
            BoundingFrustum frustum = new BoundingFrustum(game.gamePlayer.cam.viewMatrix * game.gamePlayer.cam.projectionMatrix);
            foreach (var entity in EntityBeh.worldEntities)
            {
                if (frustum.Intersects(entity.bounds))
                {
                    switch (entity.typeID)
                    {
                        case 0:
                            DrawZombie(entity);
                            break;
                    }
                }

            }


        }


        public void DrawGBuffer(Effect gBufferShader)
        {
            //    basicShader.Parameters["View"].SetValue(player.cam.GetViewMatrix());
            //   basicShader.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            //      
            gBufferShader.Parameters["TextureE"].SetValue(zombieTex);
            BoundingFrustum frustum = new BoundingFrustum(game.gamePlayer.cam.viewMatrix * game.gamePlayer.cam.projectionMatrix);
            foreach (var entity in EntityBeh.worldEntities)
            {
                if (frustum.Intersects(entity.bounds))
                {
                    switch (entity.typeID)
                    {
                        case 0:
                            //    DrawZombie(entity,gBufferShader);
                            Matrix world = Matrix.CreateTranslation(entity.position);
                            Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
                            {
                                {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.rotationY), -MathHelper.ToRadians(entity.rotationX), 0) },
                                {"body", Matrix.CreateFromQuaternion(entity.bodyQuat)}
                            };

                            entity.animationBlend.DrawAnimatedModel(world, player.cam.viewMatrix, player.cam.projectionMatrix, gBufferShader, optionalParams, () =>
                            {
                                if (entity.isEntityHurt)
                                {


                                    gBufferShader.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());

                                }
                                else
                                {

                                    gBufferShader.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());

                                }
                            });
                            break;
                    }
                }

            }


        }
        public static Matrix[] sharedDrawBoneMatrices;
        public void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (var bone in model.Bones)
            {


                foreach (var mesh in bone.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = basicShader;
                    }


                }
            }
            int count = model.Bones.Count;
            if (sharedDrawBoneMatrices == null || sharedDrawBoneMatrices.Length < count)
            {
                sharedDrawBoneMatrices = new Matrix[count];
            }

            model.CopyAbsoluteBoneTransformsTo(sharedDrawBoneMatrices);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {

                    effect.Parameters["World"].SetValue(sharedDrawBoneMatrices[mesh.ParentBone.Index] * world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                }

                mesh.Draw();
            }
        }
        public void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, Effect shader)
        {
            foreach (var bone in model.Bones)
            {


                foreach (var mesh in bone.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = shader;
                    }


                }
            }
            int count = model.Bones.Count;
            if (sharedDrawBoneMatrices == null || sharedDrawBoneMatrices.Length < count)
            {
                sharedDrawBoneMatrices = new Matrix[count];
            }

            model.CopyAbsoluteBoneTransformsTo(sharedDrawBoneMatrices);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    Matrix normalMat = sharedDrawBoneMatrices[mesh.ParentBone.Index] * world;
                    normalMat.Translation = new Vector3(0, 0, 0);
                    effect.Parameters["NormalMat"]?.SetValue(normalMat);
                    effect.Parameters["World"].SetValue(sharedDrawBoneMatrices[mesh.ParentBone.Index] * world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                }

                mesh.Draw();
            }
        }
        public void DrawModelShadow(Model model, Matrix world, Matrix lightSpaceMat, Effect shadowMapShader)
        {

            foreach (var bone in model.Bones)
            {


                foreach (var mesh in bone.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = shadowMapShader;
                    }


                }
            }
            int count = model.Bones.Count;
            if (sharedDrawBoneMatrices == null || sharedDrawBoneMatrices.Length < count)
            {
                sharedDrawBoneMatrices = new Matrix[count];
            }

            model.CopyAbsoluteBoneTransformsTo(sharedDrawBoneMatrices);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {

                    effect.Parameters["World"].SetValue(sharedDrawBoneMatrices[mesh.ParentBone.Index] * world);

                    effect.Parameters["LightSpaceMat"].SetValue(lightSpaceMat);

                }

                mesh.Draw();
            }
        }
        public void DrawZombieShadow(EntityBeh entity, Matrix lightSpaceMat, Effect shadowMapShader)
        {

            Matrix world = Matrix.CreateTranslation(entity.position);
            //   zombieModel.Bones["head"].Transform = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.rotationY), -MathHelper.ToRadians(entity.rotationX), 0) * zombieModelRef.Bones["head"].Transform;
            //      zombieModel.Bones["body"].Transform = Matrix.CreateFromQuaternion(entity.bodyQuat) * zombieModelRef.Bones["body"].Transform;
            //   zombieModel.Bones["rightLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["rightLeg"].Transform;
            //   zombieModel.Bones["leftLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, -MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["leftLeg"].Transform;
            //  DrawModelShadow(zombieModel, world, lightSpaceMat,shadowMapShader);
            Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
                            {
                                {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.rotationY), -MathHelper.ToRadians(entity.rotationX), 0) },
                                {"body", Matrix.CreateFromQuaternion(entity.bodyQuat)}
                            };

            entity.animationBlend.DrawAnimatedModel(world, player.cam.viewMatrix, player.cam.projectionMatrix, shadowMapShader, optionalParams, () => { shadowMapShader.Parameters["LightSpaceMat"].SetValue(lightSpaceMat); });
        }
        public void DrawZombie(EntityBeh entity)
        {
            // Debug.WriteLine(zombieModelRef.Equals(zombieModel));

            //              Debug.WriteLine(entity.rotationX + "  " + entity.rotationY + "  " + entity.rotationZ);
            foreach (var bone in zombieModel.Bones)
            {


                foreach (var mesh in bone.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = basicShader;
                    }


                }
            }

            if (entity.isEntityHurt)
            {
                foreach (ModelMesh mesh in zombieModel.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {

                        effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                    }

                }
            }
            else
            {
                foreach (ModelMesh mesh in zombieModel.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());
                    }

                }
            }

            Matrix world = Matrix.CreateTranslation(entity.position);

            zombieModel.Bones["head"].Transform = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.rotationY), -MathHelper.ToRadians(entity.rotationX), 0)/* * zombieModelRef.Bones["head"].Transform*/;
            zombieModel.Bones["body"].Transform = Matrix.CreateFromQuaternion(entity.bodyQuat) * zombieModelRef.Bones["body"].Transform;
            zombieModel.Bones["rightLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["rightLeg"].Transform;
            zombieModel.Bones["leftLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, -MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["leftLeg"].Transform;
            DrawModel(zombieModel, world, player.cam.viewMatrix, player.cam.projectionMatrix);

        }







        public void DrawZombie(EntityBeh entity, Effect shader)
        {
            // Debug.WriteLine(zombieModelRef.Equals(zombieModel));

            //              Debug.WriteLine(entity.rotationX + "  " + entity.rotationY + "  " + entity.rotationZ);
            foreach (var bone in zombieModel.Bones)
            {


                foreach (var mesh in bone.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = shader;
                    }


                }
            }

            if (entity.isEntityHurt)
            {


                shader.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());

            }
            else
            {

                shader.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());

            }

            Matrix world = Matrix.CreateTranslation(entity.position);
            //  zombieModel.Root.Transform=world * Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(entity.rotationX), 0);
            zombieModel.Bones["head"].Transform = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.rotationY), MathHelper.ToRadians(entity.rotationX), 0) * zombieModel.Bones["head"].Parent.Transform; /** zombieModelRef.Bones["head"].Transform*/;
            zombieModel.Bones["body"].Transform = Matrix.CreateFromQuaternion(entity.bodyQuat) * zombieModel.Bones["body"].Parent.Transform;
            zombieModel.Bones["rightLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["rightLeg"].Transform;
            zombieModel.Bones["leftLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, -MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["leftLeg"].Transform;
            DrawModel(zombieModel, world, player.cam.viewMatrix, player.cam.projectionMatrix, shader);

        }
        //      basicShader.Parameters["World"].SetValue(world * bone.Transform);

    }
}



