using monogameMinecraftShared.Core;
using monogameMinecraftShared.World;
using monogameMinecraftShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace monogameMinecraftNetworking.World
{
    public class ServerSideWorldUpdater
    {
        public ServerSideVoxelWorld world;
        public ServerSideWorldUpdater(ServerSideVoxelWorld world)
        {
            this.world = world;
        }

        public void Init()
        {
           
            queuedChunkUpdatePoints = new Queue<IChunkUpdateOperation>();
            chunksNeededRebuild = new List<ServerSideChunk>();
            tryUpdateWorldBlocksThread = new Thread(UpdateWorldBlocksThread);
            tryUpdateWorldBlocksThread.IsBackground = true;
            tryUpdateWorldBlocksThread.Start();
        }
        public object chunksNeededRebuildListLock = new object();
        public void UpdateWorldBlocksThread()
        {

            while (true)
            {
                if (world.isThreadsStopping || VoxelWorld.currentWorld.worldID != world.worldID)
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
                        chunksNeededRebuild.Add(ServerSideChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos((Vector3)updateOper.position),world.worldID));
                    }
                }




            }





        }
        public Queue<IChunkUpdateOperation> queuedChunkUpdatePoints;
        public Thread tryUpdateWorldBlocksThread;

        public List<ServerSideChunk> chunksNeededRebuild;


        public readonly float maxDelayedTime = 0.2f;
        public float delayedTime = 0f;

        public delegate void OnChunkUpdated();

        private OnChunkUpdated onUpdated;

        public OnChunkUpdated onUpdatedOneShot;
     

        public void StopAllThreads()
        {
            tryUpdateWorldBlocksThread.Join();
        }
    }
}
