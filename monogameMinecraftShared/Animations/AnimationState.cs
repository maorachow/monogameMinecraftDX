using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace monogameMinecraftShared.Animations
{
     
 public class AnimationState
    {

        Animation animation;
        //    Dictionary<string, Matrix> _cachedTransforms;

        public AnimationStep curStep;
        public AnimationStep nextStep;

        public float stepDuration => curStep.Duration;
        public float stepProgress => elapsedTimeInStep / stepDuration;


        public float elapsedTimeInStep { get; private set; }

        public int stepsCount => animation.StepsCount;
        public string name => animation.name;

        public int stepIndex;
        public bool repeats;
        public Model model;
        public AnimationState(Animation animation, Model model)
        {

            this.animation = animation;
            elapsedTimeInStep = 0f;
            stepIndex = 0;
            curStep = nextStep = null;
            repeats = animation.repeats;
            //     _cachedTransforms = new Dictionary<string, Matrix>();
            this.model = model;
            Reset();
        }

        public void Reset(int step = 0)
        {
            stepIndex = step;
            elapsedTimeInStep = 0f;
            if (animation.StepsCount > 0)
            {
                curStep = animation.GetStep(stepIndex);
                nextStep = animation.GetStep(stepIndex + 1, repeats);
            }
            //      _cachedTransforms.Clear();
        }

        public AnimationTransformation GetBoneTransformLocal(string bone, bool useAliases = true)
        {
            var a = stepProgress;
            //     Debug.WriteLine(elapsedTimeInStep);
            var fromBone = curStep.GetBoneLocal(bone);
            var toBone = nextStep.GetBoneLocal(bone);

            AnimationTransformation interpolated = AnimationTransformation.Lerp(fromBone, toBone, a);

            var ret = interpolated;
            return ret;
        }


        /*      public AnimationTransformation GetAnimationBoneTransformation(string bone)
              {
                  var a = stepProgress;
                  var fromBone = curStep.GetBoneLocal(bone);
                  var toBone = nextStep.GetBoneLocal(bone);
                  a=MathHelper.Clamp(a, 0f, 1f);
                  AnimationTransformation ret=AnimationTransformation.Lerp(fromBone, toBone, a);
                  return ret;
              }
        */

        public void DrawAnimatedModel(Matrix world, Matrix view, Matrix projection)
        {

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
                        IEffectMatrices obj = (effect as IEffectMatrices) ?? throw new InvalidOperationException();
                        obj.World = sharedDrawBoneMatrices[mesh.ParentBone.Index] * world;
                        obj.View = view;
                        obj.Projection = projection;
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
                    Matrix localTrans;
                    if (GetBoneTransformLocal(modelBone.Name) != null)
                    {
                        localTrans = GetBoneTransformLocal(modelBone.Name).ToMatrix();
                    }
                    else
                    {
                        localTrans = AnimationTransformation.Identity.ToMatrix();
                    }


                    if (modelBone.Parent == null)
                    {
                        destinationBoneTransforms[i] = localTrans * modelBone.Transform;
                        continue;
                    }

                    int index = modelBone.Parent.Index;
                    destinationBoneTransforms[i] = Matrix.Multiply(localTrans * modelBone.Transform, destinationBoneTransforms[index]);
                }
            }

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
        public void DrawAnimatedModel(Matrix world, Matrix view, Matrix projection, Effect shader, Dictionary<string, Matrix> optionalParams, Action optionalAction = null)
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
                        Matrix normalMat = sharedDrawBoneMatrices[mesh.ParentBone.Index] * world;
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
                    Matrix localTrans;
                    if (GetBoneTransformLocal(modelBone.Name) != null)
                    {
                        localTrans = GetBoneTransformLocal(modelBone.Name).ToMatrix();
                    }
                    else
                    {
                        if (optionalParams != null)
                        {
                            //    Debug.WriteLine("optional params not null");
                            if (optionalParams.ContainsKey(modelBone.Name))
                            {
                                //     Debug.WriteLine("optional params loaded");
                                localTrans = optionalParams[modelBone.Name];
                            }
                            else
                            {
                                localTrans = AnimationTransformation.Identity.ToMatrix();
                            }
                        }
                        else
                        {
                            localTrans = AnimationTransformation.Identity.ToMatrix();
                        }
                        //  localTrans = AnimationTransformation.Identity.ToMatrix();
                    }


                    if (modelBone.Parent == null)
                    {
                        destinationBoneTransforms[i] = localTrans * modelBone.Transform;
                        continue;
                    }

                    int index = modelBone.Parent.Index;
                    Matrix transformedMat;
                    MultiplyMatrix(localTrans, modelBone.Transform, out transformedMat);
                    MultiplyMatrix(transformedMat, destinationBoneTransforms[index], out destinationBoneTransforms[i]);
                }
            }

        }

        public void Update(float deltaTime, float animSpeed, out bool didFinish, out int stepsFinished)
        {

            if (animation.StepsCount == 0)
            {
                didFinish = true;
                stepsFinished = 0;
                return;
            }


            if (!repeats && (stepIndex >= animation.StepsCount||stepIndex<0))
            {
                didFinish = true;
                stepsFinished = 0;
                return;
            }


            //      _cachedTransforms.Clear();



            stepsFinished = 0;
            didFinish = false;

            // advance current step
            elapsedTimeInStep += deltaTime * animSpeed;

            // check if finish current step
            while (elapsedTimeInStep >= stepDuration)
            {
                // advance step
                elapsedTimeInStep -= stepDuration;
                stepIndex++;
                stepsFinished++;

                // wrap animation
                if (stepIndex >= animation.StepsCount)
                {
                    didFinish = true;
                    if (!repeats) { return; }
                    stepIndex = 0;
                }

                // get new step
                curStep = animation.GetStep(stepIndex);
                nextStep = animation.GetStep(stepIndex + 1, repeats);
            }

            while (elapsedTimeInStep < 0)
            {
                // advance step
              
                stepIndex--;
                stepsFinished--;
                if (stepIndex < 0)
                {
                    didFinish = true;
                    if (!repeats) { return; }
                    stepIndex = animation.StepsCount - 1;
                }
                curStep = animation.GetStep(stepIndex);
                nextStep = animation.GetStep(stepIndex + 1, repeats);
                elapsedTimeInStep = stepDuration+elapsedTimeInStep;
                // wrap animation
               

                // get new step
                
            }
        }

    }
    }
   

