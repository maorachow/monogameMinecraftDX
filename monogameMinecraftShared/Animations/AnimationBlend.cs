using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameMinecraftShared.Animations
{
    
 public class AnimationBlend
    {
        public AnimationState[] animationStates;
        public Model model;
        public AnimationTransformation GetAnimationBoneTransformation(AnimationState animState, string bone)
        {
            var a = animState.stepProgress;
            var fromBone = animState.curStep.GetBoneLocal(bone);
            var toBone = animState.nextStep.GetBoneLocal(bone);
            a = MathHelper.Clamp(a, 0f, 1f);
            AnimationTransformation ret = AnimationTransformation.Lerp(fromBone, toBone, a);
            return ret;
        }

        public AnimationBlend(AnimationState[] animationStates, Model model)
        {
            this.model = model;
            this.animationStates = animationStates;
          
        }


        public static void MultiplyMatrix(Matrix matrix1, Matrix matrix2, out Matrix result)
        {
            float m = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
            float m2 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
            float m3 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
            float m4 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
            float m5 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
            float m6 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
            float m7 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
            float m8 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
            float m9 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
            float m10 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
            float m11 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
            float m12 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
            float m13 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
            float m14 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
            float m15 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
            float m16 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
            result.M11 = m;
            result.M12 = m2;
            result.M13 = m3;
            result.M14 = m4;
            result.M21 = m5;
            result.M22 = m6;
            result.M23 = m7;
            result.M24 = m8;
            result.M31 = m9;
            result.M32 = m10;
            result.M33 = m11;
            result.M34 = m12;
            result.M41 = m13;
            result.M42 = m14;
            result.M43 = m15;
            result.M44 = m16;
        }
        public static Matrix[] sharedDrawBoneMatrices = new Matrix[0];



        public BasicEffect basicEffect;
            public static void DrawModelMesh(ModelMesh modelMesh,GraphicsDevice device,Effect drawingEffect)
            {
                for (int index1 = 0; index1 < modelMesh.MeshParts.Count; ++index1)
                {
                    ModelMeshPart meshPart = modelMesh.MeshParts[index1];
                    
                    if (meshPart.PrimitiveCount > 0)
                    {

                        device.SetVertexBuffer(meshPart.VertexBuffer);
                        device.Indices = meshPart.IndexBuffer;
                        foreach (var pass in drawingEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, meshPart.StartIndex, meshPart.PrimitiveCount);
                        }
                    }
                }
            }
        
        public void DrawAnimatedModel(GraphicsDevice device,Matrix world, Matrix view, Matrix projection, Effect shader, Dictionary<string, Matrix> optionalParams, Action optionalAction = null)
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

            if (optionalAction != null)
            {
                optionalAction();
            }
            Draw(world, view, projection);
            void Draw(Matrix world, Matrix view, Matrix projection)
            {
                int count = model.Bones.Count;

                if (sharedDrawBoneMatrices == null || sharedDrawBoneMatrices.Length < count)
                {
                    sharedDrawBoneMatrices = new Matrix[count];
                }

                CopyAbsoluteBoneTransformsTo(sharedDrawBoneMatrices);
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        Matrix normalMat = sharedDrawBoneMatrices[mesh.ParentBone.Index] *world;
                        normalMat.Translation = new Vector3(0, 0, 0);
                     
                        effect.Parameters["NormalMat"]?.SetValue(normalMat);
                        effect.Parameters["World"]?.SetValue(sharedDrawBoneMatrices[mesh.ParentBone.Index] * world);
                        effect.Parameters["View"]?.SetValue(view);
                        effect.Parameters["Projection"]?.SetValue(projection);
                    }

                    mesh.Draw();
                }
            }

            void CopyAbsoluteBoneTransformsTo(Matrix[] destinationBoneTransforms)
            {
                if (destinationBoneTransforms == null)
                {
                    throw new ArgumentNullException("destinationBoneTransforms");
                }

                if (destinationBoneTransforms.Length < model.Bones.Count)
                {
                    throw new ArgumentOutOfRangeException("destinationBoneTransforms");
                }

                int count = model.Bones.Count;
                for (int i = 0; i < count; i++)
                {
                    ModelBone modelBone = model.Bones[i];
                    Matrix localTransSum = Matrix.Identity;
                    foreach (var animState in animationStates)
                    {
                        Matrix localTrans;
                        if (GetAnimationBoneTransformation(animState, modelBone.Name) != null)
                        {
                            localTrans = GetAnimationBoneTransformation(animState, modelBone.Name).ToMatrix();
                        }
                        else
                        {

                            localTrans = AnimationTransformation.Identity.ToMatrix();

                        }
                        // MultiplyMatrix(localTrans, localTransSum, out localTransSum);
                        localTransSum = localTrans * localTransSum;
                    }

                    if (optionalParams != null)
                    {
                        //    Debug.WriteLine("optional params not null");
                        if (optionalParams.ContainsKey(modelBone.Name))
                        {
                            //     Debug.WriteLine("optional params loaded");
                            localTransSum = localTransSum* optionalParams[modelBone.Name];
                        }

                    }
                  
                    if (modelBone.Parent == null)
                    {
                        destinationBoneTransforms[i] = localTransSum * modelBone.Transform;
                        continue;
                    }

                    int index = modelBone.Parent.Index;
                 Matrix transformedMat;

                    MultiplyMatrix(localTransSum, modelBone.Transform, out transformedMat);
                    MultiplyMatrix(transformedMat, destinationBoneTransforms[index], out destinationBoneTransforms[i]);
                }
            }

        }




        public void Update(float deltaTime, params float[] animationSpeeds)
        {
            for (int i = 0; i < animationStates.Length; i++)
            {
                if (i < animationSpeeds.Length)
                {
                    animationStates[i].Update(deltaTime, animationSpeeds[i], out _, out _);
                }
                else
                {
                    animationStates[i].Update(deltaTime, 1, out _, out _);
                }

            }
        }
    }
    }
   

