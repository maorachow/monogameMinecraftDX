using monogameMinecraftShared.Updateables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace monogameMinecraftNetworking.Client.Updateables
{

    public class ClientSideParticleEmittingHelper
    {
        public static void ClientSideSpawnNewParticleTexturedGravity(Vector3 position, float size, Vector2 uvCorner, Vector2 uvWidth, float lifeTime,
            Vector3 initalMotionVector, float friction)
        {
            if (ParticleManager.instance == null)
            {
                Debug.WriteLine("null particle manager");
                return;
            }
           
                ClientSideTexturedGravityParticle particle = new ClientSideTexturedGravityParticle(position, size, uvCorner, uvWidth, lifeTime, initalMotionVector, friction);
                int firstUnusedParticle = ParticleManager.instance.FindFirstDeadParticle();
                if (firstUnusedParticle != -1)
                {
                    ParticleManager.instance.allParticles[firstUnusedParticle] = particle;
                }

            

        }

        public static Random rand = new Random();
        public static void EmitParticleWithParamCustomUV(Vector3 position, ParticleEmittingParams param, Vector4 uvCornerWidth, Vector2 randomOffset = new Vector2())
        {
           
            switch (param.type)
            {
                case ParticleType.ClientSideTexturedGravityParticle:
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
                        ClientSideSpawnNewParticleTexturedGravity(position + randPos, param.size, new Vector2(uvCornerWidth.X + randOffset.X, uvCornerWidth.Y + randOffset.Y), new Vector2(uvCornerWidth.Z, uvCornerWidth.W), param.lifeTime, particleMotionVector, param.friction);
                    }
                    break;
                default:
                    break;
            }

        }
    }
}
