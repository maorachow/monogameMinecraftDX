using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftDX.Core;

namespace monogameMinecraftDX.World
{
    public class WorldUpdater
    {
        public VoxelWorld world;
        public WorldUpdater(VoxelWorld world)
        {
         this.world= world;
        }

        public void Init(MinecraftGame game)
        {
            onUpdated = () =>
            {
                game.gamePlayer.GetBlocksAround(game.gamePlayer.bounds);
            //    Debug.WriteLine("on updated");
            };
            this.queuedChunkUpdatePoints = new Queue<IChunkUpdateOperation>();
            this.chunksNeededRebuild = new List<Chunk>();
            tryUpdateWorldBlocksThread = new Thread(UpdateWorldBlocksThread);
            tryUpdateWorldBlocksThread.IsBackground=true;
            tryUpdateWorldBlocksThread.Start();
        }
        public object chunksNeededRebuildListLock= new object();
        public void UpdateWorldBlocksThread()
        {

            while (true)
            {
                if (world.isThreadsStopping||VoxelWorld.currentWorld.worldID!=world.worldID)
                {
                    Debug.WriteLine("quit update world block thread");
                    return;
                }
                Thread.Sleep(25);

            //    Debug.WriteLine("sleep");
                
                        lock (chunksNeededRebuildListLock)
                        {
                    //       Debug.WriteLine("count: "+queuedChunkUpdatePoints.Count);
                            if (queuedChunkUpdatePoints.Count > 0)
                            {
                                IChunkUpdateOperation updateOper = queuedChunkUpdatePoints.Dequeue();
                                updateOper.Update();
                                chunksNeededRebuild.Add(ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos((Vector3)updateOper.position)));
                            }
                        }
                      
                      

                
            }
         




        }
        public Queue<IChunkUpdateOperation> queuedChunkUpdatePoints;
        public Thread tryUpdateWorldBlocksThread;

        public List<Chunk> chunksNeededRebuild;

        
        public readonly float maxDelayedTime = 0.2f;
        public float delayedTime = 0f;

        public delegate void OnChunkUpdated();

        private OnChunkUpdated onUpdated;

        public OnChunkUpdated onUpdatedOneShot;
        public void MainThreadUpdate(float deltaTime)
        {
            delayedTime += deltaTime;
        if (delayedTime > maxDelayedTime)
            {
                delayedTime = 0f;
                lock (chunksNeededRebuildListLock)
                {
                    if (chunksNeededRebuild.Count > 0)
                    {
                   //     Debug.WriteLine("rebuild");
                        foreach (var chunk in chunksNeededRebuild)
                        {
                            if (chunk != null && chunk.isReadyToRender == true)
                            {
                                chunk.BuildChunkAsync();
                              





                                ChunkHelper. GetChunk(new Vector2Int(chunk.chunkPos.x - Chunk.chunkWidth, chunk.chunkPos.y))?.BuildChunkAsync();


                                // if (chunkNeededUpdate.rightChunk != null && chunkNeededUpdate.rightChunk.isMapGenCompleted == true)

                                //  chunkNeededUpdate.rightChunk.BuildChunk();
                                ChunkHelper.GetChunk(new Vector2Int(chunk.chunkPos.x + Chunk.chunkWidth, chunk.chunkPos.y))?.BuildChunkAsync();

                                //  if (chunkNeededUpdate.backChunk != null && chunkNeededUpdate.backChunk.isMapGenCompleted == true)
                                //       {
                                //         chunkNeededUpdate.backChunk.BuildChunk();
                                //     }
                                ChunkHelper.GetChunk(new Vector2Int(chunk.chunkPos.x, chunk.chunkPos.y - Chunk.chunkWidth))?.BuildChunkAsync();

                                //   if (chunkNeededUpdate.frontChunk != null && chunkNeededUpdate.frontChunk.isMapGenCompleted == true)
                                //     {
                                //      chunkNeededUpdate.frontChunk.BuildChunk();
                                //     }

                                ChunkHelper.GetChunk(new Vector2Int(chunk.chunkPos.x, chunk.chunkPos.y + Chunk.chunkWidth))?.BuildChunkAsync();
                               
                            }
                         
                        }
                     
                       
                            onUpdated();

                     
                            

                    }

                    chunksNeededRebuild.Clear();
                }
            }
            if (onUpdatedOneShot != null)
            {
                onUpdatedOneShot();
                Delegate[] dels = onUpdatedOneShot.GetInvocationList();
                foreach (var del in dels)
                {
                    onUpdatedOneShot -= del as OnChunkUpdated;
                }
            }
        }

        public void StopAllThreads()
        {
            tryUpdateWorldBlocksThread.Join();
        }
    }
}
