using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Physics;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
using monogameMinecraftShared;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;

namespace monogameMinecraftNetworking.Client.World
{
    public class ClientSideChunkHelper
    {

        public static ClientSideChunk GetChunk(Vector2Int pos) => ClientSideVoxelWorld.singleInstance.GetChunk(pos);
       
        public static Vector3Int Vec3ToBlockPos(Vector3 pos)
        {
            Vector3Int intPos = new Vector3Int(FloatToInt(pos.X), FloatToInt(pos.Y), FloatToInt(pos.Z));
            return intPos;
        }
        public static int FloatToInt(float f)
        {
            if (f >= 0)
            {
                return (int)f;
            }
            else
            {
                return (int)f - 1;
            }
        }
        public static int FloorFloat(float n)
        {
            int i = (int)n;
            return n >= i ? i : i - 1;
        }

        public static int CeilFloat(float n)
        {
            int i = (int)(n + 1);
            return n >= i ? i : i - 1;
        }
        public static Vector2Int Vec3ToChunkPos(Vector3 pos)
        {
            Vector3 tmp = pos;
            tmp.X = MathF.Floor(tmp.X / (float)ClientSideChunk.chunkWidth) * ClientSideChunk.chunkWidth;
            tmp.Z = MathF.Floor(tmp.Z / (float)ClientSideChunk.chunkWidth) * ClientSideChunk.chunkWidth;
            Vector2Int value = new Vector2Int((int)tmp.X, (int)tmp.Z);
            //  mainForm.LogOnTextbox(value.x+" "+value.y+"\n");
            return value;
        }
        public static bool isJsonReadFromDisk { get; set; }
        public static bool isWorldDataSaved { get; private set; }
        public static string gameWorldDataPath = AppDomain.CurrentDomain.BaseDirectory;


       
        public static bool CheckIsPosInChunk(Vector3 pos, ClientSideChunk c)
        {
            if (c == null)
            {
                return false;
            }

            Vector3 chunkSpacePos = pos - new Vector3(c.chunkPos.x, 0, c.chunkPos.y);
            if (chunkSpacePos.X >= 0 && chunkSpacePos.X < ClientSideChunk.chunkWidth && chunkSpacePos.Z >= 0 && chunkSpacePos.Z < ClientSideChunk.chunkWidth)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckIsPosInChunkBorder(Vector3 pos, ClientSideChunk c)
        {
            if (c == null)
            {
                return false;
            }

            Vector3 chunkSpacePos = pos - new Vector3(c.chunkPos.x, 0, c.chunkPos.y);
            if ((chunkSpacePos.X >= 0 && chunkSpacePos.X < 1) || (chunkSpacePos.X >= ClientSideChunk.chunkWidth - 1 && chunkSpacePos.X < ClientSideChunk.chunkWidth) || (chunkSpacePos.Z >= 0 && chunkSpacePos.Z < 1) || (chunkSpacePos.Z >= ClientSideChunk.chunkWidth - 1 && chunkSpacePos.Z < ClientSideChunk.chunkWidth))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    
   /*     public static void RebuildAllChunks()
        {
            lock (VoxelWorld.currentWorld.updateWorldThreadLock)
            {
                lock (VoxelWorld.currentWorld.deleteChunkThreadLock)
                {
                    foreach (ClientSideChunk c in VoxelWorld.currentWorld.chunks.Values)
                    {
                        //  c.semaphore.Wait();
                        if (c.disposed == false && c.isReadyToRender == true)
                        {
                            c.BuildChunk();
                        }


                        //   c.semaphore.Release();

                    }
                }
            }
            GC.Collect();

        }*/
        public static short GetBlock(Vector3 pos)
        {
            Vector3Int intPos = Vector3Int.FloorToIntVec3(pos);
            ClientSideChunk chunkNeededUpdate = GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(pos));

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false || chunkNeededUpdate.isUnused == true)
            {
                return 1;
            }

            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x >= 0 && chunkSpacePos.x < ClientSideChunk.chunkWidth && chunkSpacePos.y < ClientSideChunk.chunkHeight && chunkSpacePos.y >= 0 && chunkSpacePos.z >= 0 && chunkSpacePos.z < ClientSideChunk.chunkWidth)
            {
                return chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            }
            else
            {
                return 0;
            }



        }
        public static short GetBlock(Vector3Int pos)
        {

            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)));

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false || chunkNeededUpdate.isUnused == true)
            {
                return 1;
            }

            Vector3Int chunkSpacePos = pos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x >= 0 && chunkSpacePos.x < ClientSideChunk.chunkWidth && chunkSpacePos.y < ClientSideChunk.chunkHeight && chunkSpacePos.y >= 0 && chunkSpacePos.z >= 0 && chunkSpacePos.z < ClientSideChunk.chunkWidth)
            {
                return chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            }
            else
            {
                return 0;
            }



        }

        public static BlockShape? GetBlockShape(short typeID)
        {
            if (typeID == 0 || !Chunk.blockInfosNew.ContainsKey(typeID))
            {
                return null;
            }

            return Chunk.blockInfosNew[typeID].shape;
        }
        public static BlockData GetBlockData(Vector3 pos)
        {
            Vector3Int intPos = Vector3Int.FloorToIntVec3(pos);
            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(pos));

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false || chunkNeededUpdate.isUnused == true)
            {
                return (BlockData)1;
            }

            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x >= 0 && chunkSpacePos.x < ClientSideChunk.chunkWidth && chunkSpacePos.y < ClientSideChunk.chunkHeight && chunkSpacePos.y >= 0 && chunkSpacePos.z >= 0 && chunkSpacePos.z < ClientSideChunk.chunkWidth)
            {
                return chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            }
            else
            {
                return (BlockData)0;
            }



        }
        public static BlockData GetBlockData(Vector3Int pos)
        {

            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)));

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false || chunkNeededUpdate.isUnused == true)
            {
                return (BlockData)1;
            }

            Vector3Int chunkSpacePos = pos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x >= 0 && chunkSpacePos.x < ClientSideChunk.chunkWidth && chunkSpacePos.y < ClientSideChunk.chunkHeight && chunkSpacePos.y >= 0 && chunkSpacePos.z >= 0 && chunkSpacePos.z < ClientSideChunk.chunkWidth)
            {
                return chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            }
            else
            {
                return (BlockData)0;
            }



        }
        public static short RaycastFirstBlockID(monogameMinecraftShared .Physics.Ray ray, float distance)
        {
            for (float i = 0; i < distance; i += 0.1f)
            {
                Vector3 blockPoint = ray.origin + ray.direction * i;
                short blockID = (short)ClientSideChunkHelper.GetBlock(blockPoint);
                if (blockID != 0)
                {
                    return blockID;
                }
            }
            return 0;
        }
        public static Vector3 RaycastFirstPosition(monogameMinecraftShared.Physics.Ray ray, float distance)
        {
            for (float i = 0; i < distance; i += 0.01f)
            {
                Vector3 blockPoint = ray.origin + ray.direction * i;
                short blockID = ClientSideChunkHelper.GetBlock(blockPoint);
                if (blockID != 0)
                {
                    return blockPoint;
                }
            }
            return new Vector3(1024, 0, 1024);
        }
        public void BreakBlockAtPoint(Vector3 blockPoint)
        {



           // SetBlockWithUpdate(blockPoint, (short)0);

        }


        public static int GetChunkLandingPoint(float x, float z)
        {
            Vector2Int intPos = new Vector2Int((int)x, (int)z);
            ClientSideChunk locChunk = GetChunk(Vec3ToChunkPos(new Vector3(x, 0, z)));
            if (locChunk == null || locChunk.isMapGenCompleted == false)
            {

                return 100;
            }
            Vector2Int chunkSpacePos = intPos - locChunk.chunkPos;
            chunkSpacePos.x = MathHelper.Clamp(chunkSpacePos.x, 0, ClientSideChunk.chunkWidth - 1);
            chunkSpacePos.y = MathHelper.Clamp(chunkSpacePos.y, 0, ClientSideChunk.chunkWidth - 1);
            for (int i = ClientSideChunk.chunkHeight - 1; i > 0; i--)
            {
                if (locChunk.map[chunkSpacePos.x, i - 1, chunkSpacePos.y] != 0)
                {
                    return i;
                }
            }

            return 100;

        }
        public static int GetSingleChunkLandingPoint(ClientSideChunk c, int x, int z)
        {
            if (c == null)
            {
                return -1;
            }
            Vector2Int chunkSpacePos = new Vector2Int(x, z);
            chunkSpacePos.x = MathHelper.Clamp(chunkSpacePos.x, 0, ClientSideChunk.chunkWidth - 1);
            chunkSpacePos.y = MathHelper.Clamp(chunkSpacePos.y, 0, ClientSideChunk.chunkWidth - 1);
            for (int i = ClientSideChunk.chunkHeight - 1; i > 0; i--)
            {
                if (c.map[chunkSpacePos.x, i, chunkSpacePos.y] == 0)
                {
                    continue;
                }
                if (BlockBoundingBoxUtility.IsBlockWithBoundingBox(Chunk.blockInfosNew[c.map[chunkSpacePos.x, i, chunkSpacePos.y]].shape))
                {
                    return i + 1;
                }
            }

            return -1;

        }

        public static Vector2Int GetChunkSpacePos(Vector3 worldPos, ClientSideChunk c)
        {
            Vector2Int intPos = new Vector2Int(FloatToInt(worldPos.X), FloatToInt(worldPos.Z));

            Vector2Int chunkSpacePos = intPos - c.chunkPos;
            return chunkSpacePos;
        }
        public static int PredictChunkLandingPoint(float x, float y)
        {
            if (VoxelWorld.currentWorld.worldGenType == 0)
            {
                int biome = (int)(1f + ClientSideChunk.biomeNoiseGenerator.GetSimplex(x, y) * 3f);
                return (int)(ClientSideChunk.chunkSeaLevel + ClientSideChunk.noiseGenerator.GetSimplex(x, y) * 20f + biome * 25f);
            }
            else if (VoxelWorld.currentWorld.worldGenType == 2)
            {
                for (int i = 200; i > 0; i--)
                {
                    if (ClientSideChunk.PredictBlockType3D((int)x, i, (int)y) > 0)
                    {
                        return i;
                    }
                }
                return -100;
            }
            else
            {
                return -100;
            }
        }
        public static void SetBlockWithUpdate(Vector3 pos, short blockID,Socket dataUploadingSocket)
        {

            Vector3Int intPos = new Vector3Int(ClientSideChunkHelper.FloatToInt(pos.X), ClientSideChunkHelper.FloatToInt(pos.Y), ClientSideChunkHelper.FloatToInt(pos.Z));
            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(pos));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= ClientSideChunk.chunkHeight)
            {
                return;
            }

            BlockData data = chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            BlockData modifyingData= new BlockData(blockID);
            BlockModifyData data1 = new BlockModifyData(intPos.x, intPos.y, intPos.z, modifyingData, data);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.BlockModifyData, MessagePackSerializer.Serialize(data1)), dataUploadingSocket);
     
        }


        public static void SetBlockWithUpdate(Vector3Int pos, short blockID, Socket dataUploadingSocket)
        {

            Vector3Int intPos = pos;
            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= ClientSideChunk.chunkHeight)
            {
                return;
            }

            BlockData data = chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            BlockData modifyingData = new BlockData(blockID);
            BlockModifyData data1 = new BlockModifyData(intPos.x, intPos.y, intPos.z, modifyingData, data);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.BlockModifyData, MessagePackSerializer.Serialize(data1)), dataUploadingSocket);
            /*      chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockID;
                  chunkNeededUpdate.BuildChunk();
          //        chunkNeededUpdate.isModifiedInGame = true;
                  if (Chunk.blockSoundInfo.ContainsKey((int)blockID))
                  {
                      SoundsUtility.PlaySound(MinecraftGameBase.gameposition, new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z + 0.5f), Chunk.blockSoundInfo[+blockID], 20f);
                  }
                  if (chunkSpacePos.x == 0)
                  {

                      GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x - ClientSideChunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunk();

                  }
                  if (chunkSpacePos.x == ClientSideChunk.chunkWidth - 1)
                  {
                      // if (chunkNeededUpdate.rightChunk != null && chunkNeededUpdate.rightChunk.isMapGenCompleted == true)

                      //  chunkNeededUpdate.rightChunk.BuildChunk();
                      GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x + ClientSideChunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunk();
                  }
                  if (chunkSpacePos.z == 0)
                  {
                      //  if (chunkNeededUpdate.backChunk != null && chunkNeededUpdate.backChunk.isMapGenCompleted == true)
                      //       {
                      //         chunkNeededUpdate.backChunk.BuildChunk();
                      //     }
                      GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y - ClientSideChunk.chunkWidth))?.BuildChunk();
                  }
                  if (chunkSpacePos.z == ClientSideChunk.chunkWidth - 1)
                  {
                      //   if (chunkNeededUpdate.frontChunk != null && chunkNeededUpdate.frontChunk.isMapGenCompleted == true)
                      //     {
                      //      chunkNeededUpdate.frontChunk.BuildChunk();
                      //     }

                      GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y + ClientSideChunk.chunkWidth))?.BuildChunk();
                  }


                  //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
                  //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));*/
        }

        public static void SetBlockWithoutUpdate(Vector3Int pos, short blockID, Socket dataUploadingSocket)
        {

            Vector3Int intPos = pos;
            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= ClientSideChunk.chunkHeight)
            {
                return;
            }
            

            BlockData data = chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            BlockData modifyingData = new BlockData(blockID);
            BlockModifyData data1 = new BlockModifyData(intPos.x, intPos.y, intPos.z, modifyingData, data);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.BlockModifyData, MessagePackSerializer.Serialize(data1)), dataUploadingSocket);


            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }
        public static void SetBlockWithoutUpdate(Vector3Int pos, BlockData blockID, Socket dataUploadingSocket)
        {

            Vector3Int intPos = pos;
            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= ClientSideChunk.chunkHeight)
            {
                return;
            }
            BlockData data = chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            BlockData modifyingData = new BlockData(blockID);
            BlockModifyData data1 = new BlockModifyData(intPos.x, intPos.y, intPos.z, modifyingData, data);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.BlockModifyData, MessagePackSerializer.Serialize(data1)), dataUploadingSocket);




            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }

        public static void SendCustomChunkUpdateOperation( ChunkUpdateData data,
            Socket dataUploadingSocket)
        {
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.ChunkUpdateData, MessagePackSerializer.Serialize(data)), dataUploadingSocket);
        }
        public static void SendPlaceBlockOperation(Vector3Int position,int worldID, BlockData data,Socket dataUploadingSocket)
        {
            // VoxelWorld.currentWorld.worldUpdater.queuedChunkUpdatePoints.Enqueue(new PlacingBlockOperation(position, VoxelWorld.currentWorld.worldUpdater, data));
            //Send place block operation to server

            ChunkUpdateData updateData = new ChunkUpdateData((byte)ChunkUpdateDataTypes.PlaceBlockUpdate, position.x, position.y, position.z, data, 0, worldID);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.ChunkUpdateData, MessagePackSerializer.Serialize(updateData)), dataUploadingSocket);
            /*    ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(new Vector3(position.x, position.y, position.z)));
                if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
                {
                    return;
                }
                Vector3Int chunkSpacePos = position - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);*/
            /*    if (Chunk.blockSoundInfo.ContainsKey((int)data.blockID))
                {
                    SoundsUtility.PlaySound(MinecraftGameBase.gameposition, new Vector3(position.x + 0.5f, position.y + 0.5f, position.z + 0.5f), Chunk.blockSoundInfo[data.blockID], 20f);
                }*/
            //wait for server to invoke sound
            /*      if (chunkSpacePos.x == 0)
                  {

                      GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x - ClientSideChunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunkAsync();

                  }
                  if (chunkSpacePos.x == ClientSideChunk.chunkWidth - 1)
                  {
                      // if (chunkNeededUpdate.rightChunk != null && chunkNeededUpdate.rightChunk.isMapGenCompleted == true)

                      //  chunkNeededUpdate.rightChunk.BuildChunk();
                      GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x + ClientSideChunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunkAsync();
                  }
                  if (chunkSpacePos.z == 0)
                  {
                      //  if (chunkNeededUpdate.backChunk != null && chunkNeededUpdate.backChunk.isMapGenCompleted == true)
                      //       {
                      //         chunkNeededUpdate.backChunk.BuildChunk();
                      //     }
                      GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y - ClientSideChunk.chunkWidth))?.BuildChunkAsync();
                  }
                  if (chunkSpacePos.z == ClientSideChunk.chunkWidth - 1)
                  {
                      //   if (chunkNeededUpdate.frontChunk != null && chunkNeededUpdate.frontChunk.isMapGenCompleted == true)
                      //     {
                      //      chunkNeededUpdate.frontChunk.BuildChunk();
                      //     }

                      GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y + ClientSideChunk.chunkWidth))?.BuildChunkAsync();
                  }*/



        }
        
        public static void SendBreakBlockOperation(Vector3Int position,int worldID, Socket dataUploadingSocket)
        {
                BlockData prevData = ClientSideChunkHelper.GetBlockData(position);
        //       VoxelWorld.currentWorld.worldUpdater.queuedChunkUpdatePoints.Enqueue(new BreakBlockOperation(position, VoxelWorld.currentWorld.worldUpdater, prevData));
            //   VoxelWorld.currentWorld.worldUpdater.queuedChunkUpdatePoints.Enqueue(new PlacingBlockOperation(position, VoxelWorld.currentWorld.worldUpdater, 0));
            ChunkUpdateData data=new ChunkUpdateData((byte)ChunkUpdateDataTypes.BreakingBlockUpdate,position.x,position.y,position.z,0, prevData, worldID);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.ChunkUpdateData,MessagePackSerializer.Serialize(data)),dataUploadingSocket);
            //Send break block operation to server


            
            //wait for server to invoke sound
     /*       ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(new Vector3(position.x, position.y, position.z)));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = position - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);*/
    
        }
        /*
        public static void SetBlockOptionalDataWithoutUpdate(Vector3Int pos, byte optionalData)
        {

            Vector3Int intPos = pos;
            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= ClientSideChunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z].optionalDataValue = optionalData;




            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }*/
        public static void SetBlockWithUpdate(Vector3 pos, BlockData blockData, Socket dataUploadingSocket)
        {

            Vector3Int intPos = new Vector3Int(ClientSideChunkHelper.FloatToInt(pos.X), ClientSideChunkHelper.FloatToInt(pos.Y), ClientSideChunkHelper.FloatToInt(pos.Z));
            ClientSideChunk chunkNeededUpdate = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(pos));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= ClientSideChunk.chunkHeight)
            {
                return;
            }
            BlockData data = chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            BlockData modifyingData = blockData;
            BlockModifyData data1 = new BlockModifyData(intPos.x, intPos.y, intPos.z, modifyingData, data);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.BlockModifyData, MessagePackSerializer.Serialize(data1)), dataUploadingSocket);
            /*    chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockData;
                chunkNeededUpdate.BuildChunk();
            //    chunkNeededUpdate.isModifiedInGame = true;
           /*     if (ClientSideChunk.blockSoundInfo.ContainsKey((int)blockData.blockID))
                {
                    SoundsUtility.PlaySound(MinecraftGameBase.gameposition, pos, ClientSideChunk.blockSoundInfo[blockData.blockID], 20f);
                }
                if (chunkSpacePos.x == 0)
                {

                    GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x - ClientSideChunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunk();

                }
                if (chunkSpacePos.x == ClientSideChunk.chunkWidth - 1)
                {
                    // if (chunkNeededUpdate.rightChunk != null && chunkNeededUpdate.rightChunk.isMapGenCompleted == true)

                    //  chunkNeededUpdate.rightChunk.BuildChunk();
                    GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x + ClientSideChunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunk();
                }
                if (chunkSpacePos.z == 0)
                {
                    //  if (chunkNeededUpdate.backChunk != null && chunkNeededUpdate.backChunk.isMapGenCompleted == true)
                    //       {
                    //         chunkNeededUpdate.backChunk.BuildChunk();
                    //     }
                    GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y - ClientSideChunk.chunkWidth))?.BuildChunk();
                }
                if (chunkSpacePos.z == ClientSideChunk.chunkWidth - 1)
                {
                    //   if (chunkNeededUpdate.frontChunk != null && chunkNeededUpdate.frontChunk.isMapGenCompleted == true)
                    //     {
                    //      chunkNeededUpdate.frontChunk.BuildChunk();
                    //     }

                    GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y + ClientSideChunk.chunkWidth))?.BuildChunk();
                }


                //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
                //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));*/
        }

     /*   public static void BreakBlock(Vector3 pos)
        {
            short blockID = GetBlock(pos);
       /*     if (ClientSideChunk.blockSoundInfo.ContainsKey((int)blockID))
            {
                SoundsUtility.PlaySound(MinecraftGameBase.gameposition, pos, ClientSideChunk.blockSoundInfo[+blockID], 20f);
            }
            SetBlockWithUpdate(pos, (short)0);
        }
        public static void BreakBlock(Vector3Int pos)
        {
            short blockID = GetBlock(pos);
            if (ClientSideChunk.blockSoundInfo.ContainsKey((int)blockID))
            {
                SoundsUtility.PlaySound(MinecraftGameBase.gameposition, new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z + 0.5f), ClientSideChunk.blockSoundInfo[+blockID], 20f);
            }
            SetBlockWithUpdate(pos, (short)0);
        }*/
       
    }
}
