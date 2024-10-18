using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace monogameMinecraftShared.Updateables
{
    public static class ParticleEmittingHelper
    {
        public static Random rand = new Random();



        public static Dictionary<string, ParticleEmittingParams> allParticles =
            new Dictionary<string, ParticleEmittingParams>
            {

                {"blockbreaking",new ParticleEmittingParams(ParticleType.TexturedGravityParticle,new Vector4(0,0,0,0),30,0.2f,0.15f,2f,5f,2f)},
                {"blockbreakingclientside",new ParticleEmittingParams(ParticleType.ClientSideTexturedGravityParticle,new Vector4(0,0,0,0),30,0.2f,0.15f,2f,5f,2f)}
            };
        public static void EmitParticleWithParam(Vector3 position, ParticleEmittingParams param)
        {
            switch (param.type)
            {
                case ParticleType.TexturedGravityParticle:
                    for (int i = 0; i < param.particleCount; i++)
                    {
                        Vector3 particlePos = position;
                        Vector3 randPos = new Vector3(rand.NextSingle() * 2f - 1f, rand.NextSingle() * 2f - 1f, rand.NextSingle() * 2f - 1f) * param.radius;
                        if (randPos.Length() > param.radius)
                        {
                            randPos = Vector3.Normalize(randPos) * param.radius;
                        }

                        Vector3 particleMotionVector = Vector3.Normalize(randPos) * param.motionVectorSpeed;
                        ParticleManagerBase.instance.SpawnNewParticleTexturedGravity(position + randPos, param.size, new Vector2(param.uvWidthCorner.X, param.uvWidthCorner.Y), new Vector2(param.uvWidthCorner.Z, param.uvWidthCorner.W), param.lifeTime, particleMotionVector, param.friction);
                    }
                    break;
                default:
                    break;
            }

        }



        public static void EmitParticleWithParamCustomUV(Vector3 position, ParticleEmittingParams param, Vector4 uvCornerWidth, Vector2 randomOffset = new Vector2())
        {
            switch (param.type)
            {
                case ParticleType.TexturedGravityParticle:
                    for (int i = 0; i < param.particleCount; i++)
                    {
                        Vector3 particlePos = position;
                        Vector3 randPos = new Vector3(rand.NextSingle() * 2f - 1f, rand.NextSingle() * 2f - 1f, rand.NextSingle() * 2f - 1f) * param.radius;
                        if (randPos.Length() > param.radius)
                        {
                            randPos = Vector3.Normalize(randPos) * param.radius;
                        }

                        Vector3 particleMotionVector = Vector3.Normalize(randPos) * param.motionVectorSpeed;
                        Vector2 randOffset = new Vector2(rand.NextSingle() % randomOffset.X,
                            rand.NextSingle() % randomOffset.Y);
                        ParticleManagerBase.instance.SpawnNewParticleTexturedGravity(position + randPos, param.size, new Vector2(uvCornerWidth.X + randOffset.X, uvCornerWidth.Y + randOffset.Y), new Vector2(uvCornerWidth.Z, uvCornerWidth.W), param.lifeTime, particleMotionVector, param.friction);
                    }
                    break;
                default:
                    break;
            }

        }
    }
}
