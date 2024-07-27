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

        public IParticle[] allParticles = new IParticle[200];

        public Dictionary<Vector3Int, BoundingBox> cachedBlockColliders;
        public void Initialize()
        {
            allParticles = new IParticle[200];
            cachedBlockColliders = new Dictionary<Vector3Int, BoundingBox>();
        }

        public void ReleaseResources()
        {
            allParticles = null;
            cachedBlockColliders = null;
        }
        public BoundingBox GetOrFetchBoundingBox(Vector3Int pos)
        {
            if (cachedBlockColliders.ContainsKey(pos))
            {
                return cachedBlockColliders[pos];
            }
            else
            {
                BlockData blockData = ChunkHelper.GetBlockData(pos);
                if (blockData.blockID != 0 && Chunk.blockInfosNew.ContainsKey(blockData.blockID) &&
                    BlockBoundingBoxUtility.IsBlockWithBoundingBox(Chunk.blockInfosNew[blockData.blockID].shape) ==
                    true)
                {
                    cachedBlockColliders.Add(pos, BlockBoundingBoxUtility.GetBoundingBox(pos.x, pos.y, pos.z, blockData));
                    return cachedBlockColliders[pos];
                }
            }

            return new BoundingBox();
        }

        public void Update(float deltaTime)
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
            // RemoveDeadParticles();
        }

        public void SpawnNewParticleTexturedGravity(Vector3 position, float size, Vector2 uvCorner, Vector2 uvWidth, float lifeTime,
            Vector3 initalMotionVector, float friction)
        {

            TexturedGravityParticle particle = new TexturedGravityParticle(position, size, uvCorner, uvWidth, lifeTime, initalMotionVector, friction);
            int firstUnusedParticle = FindFirstDeadParticle();
            if (firstUnusedParticle != -1)
            {
                allParticles[firstUnusedParticle] = particle;
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
