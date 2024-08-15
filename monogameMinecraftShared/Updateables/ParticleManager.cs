using System;
using System.Collections.Concurrent;
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
        public bool isResourcesReleased = false;


        public ParticleManager()
        {
            instance = this;
        }

        public object allParticlesLock = new object();
        public ConcurrentDictionary<int,IParticle> allParticles = new ConcurrentDictionary<int, IParticle>();

        public Dictionary<Vector3Int, BoundingBox> cachedBlockColliders;
        public void Initialize()
        {
            lock (allParticlesLock)
            {
                allParticles = new ConcurrentDictionary<int, IParticle>();
                cachedBlockColliders = new Dictionary<Vector3Int, BoundingBox>();
                isResourcesReleased = false;
            }
           
        }

        public void ReleaseResources()
        {
            lock (allParticlesLock)
            {
                allParticles = null;
                cachedBlockColliders = null;
                isResourcesReleased=true;
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
            
            cachedBlockColliders.Clear();
            //    Debug.WriteLine(allParticles[0]?.position);
            foreach (var particle in allParticles)
            {
                if (particle.Value != null && particle.Value.isAlive == true)
                {
                    particle.Value.Update(deltaTime);
                }

            }
            
          
            // RemoveDeadParticles();
        }
        public Random rand=new Random();
        public void SpawnNewParticleTexturedGravity(Vector3 position, float size, Vector2 uvCorner, Vector2 uvWidth, float lifeTime,
            Vector3 initalMotionVector, float friction)
        {
            
            TexturedGravityParticle particle = new TexturedGravityParticle(position, size, uvCorner, uvWidth, lifeTime, initalMotionVector, friction);
                FindAndRemoveDeadParticle();
          
                allParticles.TryAdd(rand.Next(),particle) ;
           

            
           
        }

        public void FindAndRemoveDeadParticle()
        {
            foreach (var particle in allParticles)
            {
                if (particle.Value != null && particle.Value.isAlive == false)
                {
                    allParticles.Remove(particle.Key, out _);
                }

            }
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
