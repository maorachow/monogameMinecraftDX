﻿using System;
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

    public class ParticleManagerBase
    {
        public static readonly int maxParticlesCount = 1000;
        public static ParticleManagerBase instance;
        public List<IParticle> allParticles = new List<IParticle>();
        public Dictionary<Vector3Int, BoundingBox> cachedBlockColliders;
        public bool isResourcesReleased = false;
        public virtual BoundingBox GetOrFetchBoundingBox(Vector3Int pos, BlockData? blockData)
        {
            throw new NotImplementedException();
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void ReleaseResources()
        {
           
        }

        public virtual void Update(float deltaTime)
        {

        }

        public virtual void SpawnNewParticleTexturedGravity(Vector3 position, float size, Vector2 uvCorner,
            Vector2 uvWidth, float lifeTime,
            Vector3 initalMotionVector, float friction)
        {

        }
    }
    public class ParticleManager: ParticleManagerBase
    {
    
       

        
        public ParticleManager()
        {
            instance = this;
        }

        public object allParticlesLock = new object();
       

      
        public override void Initialize()
        {
            lock (allParticlesLock)
            {
                allParticles = new List< IParticle>();
                cachedBlockColliders = new Dictionary<Vector3Int, BoundingBox>();
                isResourcesReleased = false;
            }
           
        }

        public override void ReleaseResources()
        {
            lock (allParticlesLock)
            {
                allParticles = null;
                cachedBlockColliders = null;
                isResourcesReleased=true;
            }
        
        }
        public override BoundingBox GetOrFetchBoundingBox(Vector3Int pos,BlockData? blockData)
        {
            if (cachedBlockColliders.ContainsKey(pos))
            {
                return cachedBlockColliders[pos];
            }

            blockData ??= ChunkHelper.GetBlockData(pos);


            if (blockData.Value.blockID != 0 && Chunk.blockInfosNew.ContainsKey(blockData.Value.blockID) &&
                BlockBoundingBoxUtility.IsBlockWithBoundingBox(Chunk.blockInfosNew[blockData.Value.blockID].shape) ==
                true)
            {
                cachedBlockColliders.Add(pos, BlockBoundingBoxUtility.GetBoundingBox(pos.x, pos.y, pos.z, blockData.Value));
                return cachedBlockColliders[pos];
            }

            return new BoundingBox();
        }

        public override void Update(float deltaTime)
        {
            
            cachedBlockColliders.Clear();
              
            foreach (var particle in allParticles.ToArray())
            {
                if (particle != null && particle.isAlive == true)
                {
                    particle.Update(deltaTime);
                }

            }
            
          
            // RemoveDeadParticles();
        }
        public Random rand=new Random();
        public override void SpawnNewParticleTexturedGravity(Vector3 position, float size, Vector2 uvCorner, Vector2 uvWidth, float lifeTime,
            Vector3 initalMotionVector, float friction)
        {
            
            TexturedGravityParticle particle = new TexturedGravityParticle(position, size, uvCorner, uvWidth, lifeTime, initalMotionVector, friction);
               bool hasDeadParticles= FindAndRemoveDeadParticle();
               while (hasDeadParticles)
               {
                   hasDeadParticles = FindAndRemoveDeadParticle();
               }

               if (allParticles.Count >= maxParticlesCount-1)
               {
                   return;
               }
               allParticles.Add(particle) ;
           

            
           
        }

        public bool FindAndRemoveDeadParticle()
        {
            foreach (var particle in allParticles)
            {
                if (particle != null && particle.isAlive == false)
                {
                    allParticles.Remove(particle);
                    return true;
                }

            }

            return false;
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
