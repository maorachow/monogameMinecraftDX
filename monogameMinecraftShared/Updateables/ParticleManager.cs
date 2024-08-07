using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Physics;
using monogameMinecraftShared.World;

namespace monogameMinecraftShared.Updateables
{
    public class ParticleManager
    {
        public static ParticleManager instance;



        public ParticleManager()
        {
            instance = this;
        }

        public object allParticlesLock = new object();
        public IParticle[] allParticles = new IParticle[200];

        public Dictionary<Vector3Int, BoundingBox> cachedBlockColliders;
        public void Initialize()
        {
            lock (allParticlesLock)
            {
                allParticles = new IParticle[200];
                cachedBlockColliders = new Dictionary<Vector3Int, BoundingBox>();
            }
           
        }

        public void ReleaseResources()
        {
            lock (allParticlesLock)
            {
                allParticles = null;
                cachedBlockColliders = null;
            }
        
        }
        public BoundingBox GetOrFetchBoundingBox(Vector3Int pos,BlockData? blockData)
        {
            if (cachedBlockColliders.ContainsKey(pos))
            {
                return cachedBlockColliders[pos];
            }
            else
            {
                if (blockData == null)
                {
                    return new BoundingBox();
                }
                if (blockData.Value.blockID != 0 && Chunk.blockInfosNew.ContainsKey(blockData.Value.blockID) &&
                    BlockBoundingBoxUtility.IsBlockWithBoundingBox(Chunk.blockInfosNew[blockData.Value.blockID].shape) ==
                    true)
                {
                    cachedBlockColliders.Add(pos, BlockBoundingBoxUtility.GetBoundingBox(pos.x, pos.y, pos.z, blockData.Value));
                    return cachedBlockColliders[pos];
                }
            }

            return new BoundingBox();
        }

        public void Update(float deltaTime)
        {
            lock (allParticlesLock)
            {
            cachedBlockColliders.Clear();
            //    Debug.WriteLine(allParticles[0]?.position);
            for (int i = 0; i < allParticles.Length; i++)
            {
                if (allParticles[i] != null && allParticles[i].isAlive == true)
                {
                    allParticles[i].Update(deltaTime);
                }

            }
            }
          
            // RemoveDeadParticles();
        }

        public void SpawnNewParticleTexturedGravity(Vector3 position, float size, Vector2 uvCorner, Vector2 uvWidth, float lifeTime,
            Vector3 initalMotionVector, float friction)
        {
            lock (allParticlesLock)
            {
            TexturedGravityParticle particle = new TexturedGravityParticle(position, size, uvCorner, uvWidth, lifeTime, initalMotionVector, friction);
            int firstUnusedParticle = FindFirstDeadParticle();
            if (firstUnusedParticle != -1)
            {
                allParticles[firstUnusedParticle] = particle;
            }

            }
           
        }

        public int FindFirstDeadParticle()
        {
            for (int i = 0; i < allParticles.Length; i++)
            {
                if (allParticles[i] == null || allParticles[i] != null && allParticles[i].isAlive == false)
                {
                    return i;
                }
            }
            return -1;
        }
        /* public void RemoveDeadParticles()
         {
             for (int i = 0; i < allParticles.Length; i++)
             {
                 if (allParticles[i].isAlive == false)
                 {
                     allParticles.RemoveAt(i);
                     i--;
                 } 
             }
         }*/
    }
}
