using monogameMinecraftShared.Core;
using monogameMinecraftShared.World;
using monogameMinecraftShared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Utility;
using monogameMinecraftNetworking.Protocol;

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
           
            queuedChunkUpdatePoints = new ConcurrentQueue<IChunkUpdateOperation>();
            chunksNeededRebuild = new List<ServerSideChunk>();
            tryUpdateWorldBlocksThread = new Thread(UpdateWorldBlocksThread);
            tryUpdateWorldBlocksThread.IsBackground = true;
            tryUpdateWorldBlocksThread.Start();

            trySendUpdatedChunkDatasThread = new Thread(SendUpdatedChunkDatasThread);
            trySendUpdatedChunkDatasThread.IsBackground = true;
            trySendUpdatedChunkDatasThread.Start();
        }
        public object chunksNeededRebuildListLock = new object();
        public void UpdateWorldBlocksThread()
        {

            while (true)
            {
                if (world.isThreadsStopping)
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
                        IChunkUpdateOperation updateOper;
                        queuedChunkUpdatePoints.TryDequeue(out updateOper);
                        if (updateOper.worldID != world.worldID)
                        {
                            continue;
                        }
                        updateOper.Update();
                        chunksNeededRebuild.Add(ServerSideChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos((Vector3)updateOper.position),world.worldID));
                    }
                }




            }





        }

        public void SendUpdatedChunkDatasThread()
        {
            while (true)
            {
                if (world.isThreadsStopping)
                {
                    Debug.WriteLine("quit update world block thread");
                    return;
                }
                Thread.Sleep(200);

                //    Debug.WriteLine("sleep");

                lock (chunksNeededRebuildListLock)
                {
                    //       Debug.WriteLine("count: "+queuedChunkUpdatePoints.Count);
                /*    if (queuedChunkUpdatePoints.Count > 0)
                    {
                        IChunkUpdateOperation updateOper = queuedChunkUpdatePoints.Dequeue();
                        if (updateOper.worldID != world.worldID)
                        {
                            continue;
                        }
                        updateOper.Update();
                        chunksNeededRebuild.Add(ServerSideChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos((Vector3)updateOper.position), world.worldID));
                    }*/
                foreach (var chunk in chunksNeededRebuild)
                {
                    if (chunk != null&&chunk.map!=null)
                    {
                         chunk.isModifiedInGame=true;
                        NetworkingUtility.CastToAllClients(ServerSideVoxelWorld.serverInstance, new MessageProtocol((byte)MessageCommandType.WorldData, ChunkDataSerializingUtility.SerializeChunk(chunk)));
                    }
                      
                }
                chunksNeededRebuild.Clear();
                }




            }
        }

        public ConcurrentQueue<IChunkUpdateOperation> queuedChunkUpdatePoints;
        public Thread tryUpdateWorldBlocksThread;
        public Thread trySendUpdatedChunkDatasThread;
        public List<ServerSideChunk> chunksNeededRebuild;


        public readonly float maxDelayedTime = 0.2f;
        public float delayedTime = 0f;

        public delegate void OnChunkUpdated();

        private OnChunkUpdated onUpdated;

        public OnChunkUpdated onUpdatedOneShot;
     

        public void StopAllThreads()
        {
            tryUpdateWorldBlocksThread.Join();
            trySendUpdatedChunkDatasThread.Join();
        }
    }
}
