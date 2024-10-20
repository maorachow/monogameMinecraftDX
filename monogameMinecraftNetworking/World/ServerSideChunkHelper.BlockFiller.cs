﻿using monogameMinecraftShared.Core;
using monogameMinecraftShared.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace monogameMinecraftNetworking.World
{
    public partial class ServerSideChunkHelper
    {
        static Dictionary<Vector2Int, ServerSideChunk> tempFillChunks = new Dictionary<Vector2Int, ServerSideChunk>();
        public static void FillBlocks(BlockData[,,] blockData, Vector3Int origin, int worldID, BlockFillMode fillmode = BlockFillMode.Default, bool rebuildChunk = true, bool saveChunks = false)
        {
            tempFillChunks.Clear();
            for (int x = origin.x; x < origin.x + blockData.GetLength(0); x++)
            {
                for (int z = origin.z; z < origin.z + blockData.GetLength(2); z++)
                {
                    ServerSideChunk c = GetChunk(Vec3ToChunkPos(new Vector3(x, 0, z)),worldID);
                    if (c == null)
                    {
                        continue;
                    }
                    if (!tempFillChunks.ContainsKey(c.chunkPos))
                    {
                        tempFillChunks.Add(c.chunkPos, c);
                    }
                }
            }

            foreach (var c in tempFillChunks)
            {
                FillBlocksSingleChunk(blockData, origin, c.Value, worldID, fillmode, saveChunks);
                if ( rebuildChunk)
                {
                //    c.Value.BuildChunk();
                //resend chunk data to clients
                }
            }

        }

        public static void FillBlocksSingleChunk(BlockData[,,] blockData, Vector3Int origin, ServerSideChunk c,int worldID, BlockFillMode fillmode, bool isSavingChunk = false, params short[] optionalVal)
        {
            Vector3Int chunkOffset = new Vector3Int(c.chunkPos.x, 0, c.chunkPos.y) - origin;
            for (int i = 0; i < Chunk.chunkWidth; i++)
            {


                for (int k = 0; k < Chunk.chunkWidth; k++)
                {
                    Vector2Int posInData = new Vector2Int(chunkOffset.x + i, chunkOffset.z + k);
                    if (posInData.x < 0 || posInData.x >= blockData.GetLength(0) || posInData.y < 0 ||
                        posInData.y >= blockData.GetLength(2))
                    {
                        continue;
                    }
                    for (int j = origin.y; j < origin.y + blockData.GetLength(1); j++)
                    {
                        if (j < 0 || j >= Chunk.chunkHeight)
                        {
                            continue;
                        }

                        switch (fillmode)
                        {
                            case BlockFillMode.Default:
                                c.map[i, j, k] = blockData[posInData.x, j - origin.y, posInData.y];
                                break;
                            case BlockFillMode.ReplaceAir:
                                c.map[i, j, k] = c.map[i, j, k] == 0 ? blockData[posInData.x, j - origin.y, posInData.y] : c.map[i, j, k];
                                break;
                            case BlockFillMode.ReplaceNonSolid:
                                c.map[i, j, k] = c.map[i, j, k] == 0 || (c.map[i, j, k] != 0 && Chunk.blockInfosNew[c.map[i, j, k]].shape != BlockShape.Solid) ? blockData[posInData.x, j - origin.y, posInData.y] : c.map[i, j, k];
                                break;
                            case BlockFillMode.DontReplaceCustomTypes:

                                if (optionalVal == null || optionalVal.Length == 0)
                                {
                                    c.map[i, j, k] = blockData[posInData.x, j - origin.y, posInData.y];
                                }

                                bool isPlacing = true;
                                foreach (var v in optionalVal)
                                {
                                    if (c.map[i, j, k] == v)
                                    {
                                        isPlacing = false;
                                    }
                                }

                                if (isPlacing)
                                {
                                    c.map[i, j, k] = blockData[posInData.x, j - origin.y, posInData.y];
                                }
                                break;

                            case BlockFillMode.ReplaceCustomTypes:
                                if (optionalVal == null || optionalVal.Length == 0)
                                {
                                    break;
                                }
                                if (blockData[posInData.x, j - origin.y, posInData.y].blockID == 0)
                                {
                                    break;
                                }
                                bool isPlacing1 = false;
                                foreach (var v in optionalVal)
                                {
                                    if (c.map[i, j, k] == v)
                                    {
                                        isPlacing1 = true;
                                        break;
                                    }
                                }

                                if (isPlacing1)
                                {
                                    c.map[i, j, k] = blockData[posInData.x, j - origin.y, posInData.y];
                                }
                                break;
                        }

                    }
                }

            }

            if (isSavingChunk == true)
            {
                c.isModifiedInGame = true;
            }

        }
        static Dictionary<Vector2Int, ServerSideChunk> tempReadChunks = new Dictionary<Vector2Int, ServerSideChunk>();
        public static BlockData[,,] GetBlocks(Vector3Int origin, int lengthX, int lengthY, int lengthZ,int worldID)
        {
            lock (ServerSideVoxelWorld.voxelWorlds[worldID].deleteChunkThreadLock)
            {
                try
                {
                    tempReadChunks.Clear();
                    BlockData[,,] blockData = new BlockData[lengthX, lengthY, lengthZ];
                    for (int x = origin.x; x < origin.x + blockData.GetLength(0); x++)
                    {
                        for (int z = origin.z; z < origin.z + blockData.GetLength(2); z++)
                        {
                            ServerSideChunk c = GetChunk(Vec3ToChunkPos(new Vector3(x, 0, z)), worldID);
                            if (c == null)
                            {
                                continue;
                            }
                            tempReadChunks.TryAdd(c.chunkPos, c);
                        }
                    }
                    foreach (var c in tempReadChunks)
                    {
                        GetBlocksSingleChunk(origin, ref blockData, c.Value);

                    }
                    return blockData;
                }
                catch (Exception e)
                {
                    Console.WriteLine("get block failed:0"+e);
                    return  new BlockData[lengthX, lengthY, lengthZ];
                }
               
            }
        
        }


     /*   public static int[,] GetAreaHeightMap(Vector3Int origin, int lengthX, int lengthZ)
        {
            tempReadChunks.Clear();
            int[,] blockData = new int[lengthX, lengthZ];
            for (int x = origin.x; x < origin.x + blockData.GetLength(0); x++)
            {
                for (int z = origin.z; z < origin.z + blockData.GetLength(1); z++)
                {
                    Chunk c = GetChunk(Vec3ToChunkPos(new Vector3(x, 0, z)));
                    if (c == null)
                    {
                        continue;
                    }
                    tempReadChunks.TryAdd(c.chunkPos, c);
                }
            }
            foreach (var c in tempReadChunks)
            {
                GetAreaHeightMapSingleChunk(origin, ref blockData, c.Value);

            }
            return blockData;
        }*/

        public static void GetAreaHeightMapSingleChunk(Vector3Int origin, ref int[,] heightMapIn, ServerSideChunk c)
        {

            Vector3Int chunkOffset = new Vector3Int(c.chunkPos.x, 0, c.chunkPos.y) - origin;
            for (int i = 0; i < Chunk.chunkWidth; i++)
            {


                for (int k = 0; k < Chunk.chunkWidth; k++)
                {
                    Vector2Int posInData = new Vector2Int(chunkOffset.x + i, chunkOffset.z + k);
                    if (posInData.x < 0 || posInData.x >= heightMapIn.GetLength(0) || posInData.y < 0 ||
                        posInData.y >= heightMapIn.GetLength(1))

                    {
                        continue;
                    }

                    heightMapIn[posInData.x, posInData.y] = GetSingleChunkLandingPoint(c, i, k);

                }

            }
        }
        public static void GetBlocksSingleChunk(Vector3Int origin, ref BlockData[,,] blockDataIn, ServerSideChunk c)
        {
            Vector3Int chunkOffset = new Vector3Int(c.chunkPos.x, 0, c.chunkPos.y) - origin;
           

            for (int i = 0; i < Chunk.chunkWidth; i++)
            {


                for (int k = 0; k < Chunk.chunkWidth; k++)
                {
                    Vector2Int posInData = new Vector2Int(chunkOffset.x + i, chunkOffset.z + k);
                    if (posInData.x < 0 || posInData.x >= blockDataIn.GetLength(0) || posInData.y < 0 ||
                        posInData.y >= blockDataIn.GetLength(2))

                    {
                        continue;
                    }
                    for (int j = origin.y; j < origin.y + blockDataIn.GetLength(1); j++)
                    {
                        if (j < 0 || j >= Chunk.chunkHeight)
                        {
                            continue;
                        }

                        blockDataIn[posInData.x, j - origin.y, posInData.y] = c.map[i, j, k];



                    }
                }

            }
        }
    }
}
