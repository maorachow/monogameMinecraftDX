using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.World
{
    public class ServerSideChunk : IDisposable
    {

        public BlockData[,,] map;//16*256*16*3=?
        //65536*3=

        public Vector2Int chunkPos = new Vector2Int(0, 0);

        public bool isChunkDataSavedInDisk = false;

        public bool isUnused = false;
        public bool isModifiedInGame=false;
        public ServerSideVoxelWorld curWorld;
        public object chunkBuildingLock=new object();
        public ServerSideChunk(Vector2Int chunkPos, ServerSideVoxelWorld world)
        {

            curWorld=world;
            this.chunkPos = chunkPos;
            if (world.chunks.ContainsKey(chunkPos))
            {
                Debug.WriteLine("dispose upon launch");
                Dispose();
                return;
            }
            world.chunks.TryAdd(chunkPos, this);
           

            BuildChunk();

        }

        public void BuildChunk()
        {
            lock (chunkBuildingLock)
            {
                InitMap(chunkPos);
            }
           
        }

        public ChunkData ChunkToChunkData()
        {
            return new ChunkData(this.map, this.chunkPos);
        }
        public static int[,] GenerateChunkBiomeMap(Vector2Int pos,int worldID)
        {
            //   float[,] biomeMap=new float[Chunk.chunkWidth/8+2,Chunk.chunkWidth/8+2];//插值算法
            //      int[,] chunkBiomeMap=GenerateChunkBiomeMap(pos);
            int[,] biomeMapInter = new int[Chunk.chunkWidth / 8 + 2, Chunk.chunkWidth / 8 + 2];
            for (int i = 0; i < Chunk.chunkWidth / 8 + 2; i++)
            {
                for (int j = 0; j < Chunk.chunkWidth / 8 + 2; j++)
                {
                    //           Debug.DrawLine(new Vector3(pos.x+(i-1)*8,60f,pos.y+(j-1)*8),new Vector3(pos.x+(i-1)*8,150f,pos.y+(j-1)*8),Color.green,1f);
                    //    if(RandomGenerator3D.GenerateIntFromVec3(new Vector3Int()))

                    biomeMapInter[i, j] = (int)(1f + ServerSideVoxelWorld.voxelWorlds[worldID].biomeNoiseGenerator.GetSimplex(pos.x + (i - 1) * 8, pos.y + (j - 1) * 8) * 3f);
                }
            }//32,32



            return biomeMapInter;
        }
        public static float[,] GenerateChunkHeightmap(Vector2Int pos,int worldID)
        {
            float[,] heightMap = new float[Chunk.chunkWidth / 8 + 2, Chunk.chunkWidth / 8 + 2];//插值算法
            int[,] chunkBiomeMap = GenerateChunkBiomeMap(pos,worldID);

            for (int i = 0; i < Chunk.chunkWidth / 8 + 2; i++)
            {
                for (int j = 0; j < Chunk.chunkWidth / 8 + 2; j++)
                {
                    //           Debug.DrawLine(new Vector3(pos.x+(i-1)*8,60f,pos.y+(j-1)*8),new Vector3(pos.x+(i-1)*8,150f,pos.y+(j-1)*8),Color.green,1f);
                    //    if(RandomGenerator3D.GenerateIntFromVec3(new Vector3Int()))

                    heightMap[i, j] = Chunk.chunkSeaLevel + ServerSideVoxelWorld.voxelWorlds[worldID].noiseGenerator.GetSimplex(pos.x + (i - 1) * 8, pos.y + (j - 1) * 8) * 20f + chunkBiomeMap[i, j] * 25f;
                }

            }//32,32
            int interMultiplier = 8;
            float[,] heightMapInterpolated = new float[(Chunk.chunkWidth / 8 + 2) * interMultiplier, (Chunk.chunkWidth / 8 + 2) * interMultiplier];
            for (int i = 0; i < (Chunk.chunkWidth / 8 + 2) * interMultiplier; ++i)
            {
                for (int j = 0; j < (Chunk.chunkWidth / 8 + 2) * interMultiplier; ++j)
                {
                    int x = i;
                    int y = j;
                    float x1 = (i / interMultiplier) * interMultiplier;
                    float x2 = (i / interMultiplier) * interMultiplier + interMultiplier;
                    float y1 = (j / interMultiplier) * interMultiplier;
                    float y2 = (j / interMultiplier) * interMultiplier + interMultiplier;
                    int x1Ori = (i / interMultiplier);
                    // Debug.Log(x1Ori);
                    int x2Ori = (i / interMultiplier) + 1;
                    x2Ori = Math.Clamp(x2Ori, 0, (Chunk.chunkWidth / 8 + 2) - 1);
                    //   Debug.Log(x2Ori);
                    int y1Ori = (j / interMultiplier);
                    //   Debug.Log(y1Ori);
                    int y2Ori = (j / interMultiplier) + 1;
                    y2Ori = Math.Clamp(y2Ori, 0, (Chunk.chunkWidth / 8 + 2) - 1);
                    //     Debug.Log(y2Ori);

                    float q11 = heightMap[x1Ori, y1Ori];
                    float q12 = heightMap[x1Ori, y2Ori];
                    float q21 = heightMap[x2Ori, y1Ori];
                    float q22 = heightMap[x2Ori, y2Ori];
                    float fxy1 = (float)(x2 - x) / (x2 - x1) * q11 + (float)(x - x1) / (x2 - x1) * q21;
                    float fxy2 = (float)(x2 - x) / (x2 - x1) * q12 + (float)(x - x1) / (x2 - x1) * q22;
                    float fxy = (float)(y2 - y) / (y2 - y1) * fxy1 + (float)(y - y1) / (y2 - y1) * fxy2;
                    heightMapInterpolated[x, y] = fxy;
                    //       Debug.Log(fxy);
                    //    Debug.Log(x1);
                    //  Debug.Log(x2);

                }
            }

            return heightMapInterpolated;
        }

        public float[,] thisHeightMap;
      
        public void InitMap(Vector2Int chunkPos)
        {

            //   semaphore.Wait();

            //     Thread.Sleep(400);
            this.chunkPos = chunkPos;
            thisHeightMap = GenerateChunkHeightmap(chunkPos,curWorld.worldID);

            //    map = new BlockData[Chunk.chunkWidth,chunkHeight,Chunk.chunkWidth];
         /*   verticesOpq = new List<VertexPositionNormalTangentTexture>();
            verticesNS = new List<VertexPositionNormalTangentTexture>();
            verticesWT = new List<VertexPositionNormalTangentTexture>();
            indicesOpq = new List<ushort>();
            indicesNS = new List<ushort>();
            indicesWT = new List<ushort>();
            verticesOpqLOD = new List<VertexPositionNormalTangentTexture>();
            indicesOpqLOD = new List<ushort>();
            frontLeftChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x - Chunk.chunkWidth, chunkPos.y + Chunk.chunkWidth));
            frontRightChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x + Chunk.chunkWidth, chunkPos.y + Chunk.chunkWidth));
            backLeftChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x - Chunk.chunkWidth, chunkPos.y - Chunk.chunkWidth));
            backRightChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x + Chunk.chunkWidth, chunkPos.y - Chunk.chunkWidth));
            backChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x, chunkPos.y - Chunk.chunkWidth));
            frontChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x, chunkPos.y + Chunk.chunkWidth));
            leftChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x - Chunk.chunkWidth, chunkPos.y));

            rightChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x + Chunk.chunkWidth, chunkPos.y));
            if (frontLeftChunk?.isUnused == true)
            {
                frontLeftChunk = null;
            }
            if (frontRightChunk?.isUnused == true)
            {
                frontRightChunk = null;
            }
            if (backLeftChunk?.isUnused == true)
            {
                backLeftChunk = null;
            }
            if (backRightChunk?.isUnused == true)
            {
                backRightChunk = null;
            }
            if (backChunk?.isUnused == true)
            {
                backChunk = null;
            }
            if (frontChunk?.isUnused == true)
            {
                frontChunk = null;
            }
            if (leftChunk?.isUnused == true)
            {
                leftChunk = null;
            }
            if (rightChunk?.isUnused == true)
            {
                rightChunk = null;
            }

            if (frontChunk != null)
            {
                frontChunk.usedByOthersCount += 1;

            }
            if (backChunk != null)
            {
                backChunk.usedByOthersCount += 1;

            }
            if (leftChunk != null)
            {
                leftChunk.usedByOthersCount += 1;

            }
            if (rightChunk != null)
            {
                rightChunk.usedByOthersCount += 1;

            }



            if (frontRightChunk != null)
            {
                frontRightChunk.usedByOthersCount += 1;

            }
            if (backLeftChunk != null)
            {
                backLeftChunk.usedByOthersCount += 1;

            }
            if (frontLeftChunk != null)
            {
                frontLeftChunk.usedByOthersCount += 1;

            }
            if (backRightChunk != null)
            {
                backRightChunk.usedByOthersCount += 1;

            }*/
            if (isMapGenCompleted == true)
            {

             //   this.chunkBounds = this.CalculateBounds();
         //       GenerateHeightMap();
             //   GenerateMesh(verticesOpq, verticesNS, verticesWT, indicesOpq, indicesNS, indicesWT, generateRenderBuffers);
            //    ReleaseChunkUsage();

                //     semaphore.Release();

                return;
            }
            if (curWorld.chunkDataReadFromDisk.ContainsKey(chunkPos))
            {
                isChunkDataSavedInDisk = true;
                map = (BlockData[,,])curWorld.chunkDataReadFromDisk[chunkPos].map.Clone();
            //    this.chunkBounds = this.CalculateBounds();
          //      GenerateHeightMap();
           //     GenerateMesh(verticesOpq, verticesNS, verticesWT, indicesOpq, indicesNS, indicesWT, generateRenderBuffers);
                isMapGenCompleted = true;
            //    ReleaseChunkUsage();

                //      semaphore.Release();

                return;
            }




            FreshGenMap(chunkPos);
      //      GenerateHeightMap();
            isMapGenCompleted = true;
         //   GenerateMesh(verticesOpq, verticesNS, verticesWT, indicesOpq, indicesNS, indicesWT);
        //    ReleaseChunkUsage();

            //    semaphore.Release();

       /*     void ReleaseChunkUsage()
            {
                if (frontChunk != null)
                {
                    frontChunk.usedByOthersCount -= 1;

                }
                if (backChunk != null)
                {
                    backChunk.usedByOthersCount -= 1;

                }
                if (leftChunk != null)
                {
                    leftChunk.usedByOthersCount -= 1;

                }
                if (rightChunk != null)
                {
                    rightChunk.usedByOthersCount -= 1;

                }


                if (frontLeftChunk != null)
                {
                    frontLeftChunk.usedByOthersCount -= 1;

                }
                if (frontRightChunk != null)
                {
                    frontRightChunk.usedByOthersCount -= 1;

                }
                if (backLeftChunk != null)
                {
                    backLeftChunk.usedByOthersCount -= 1;

                }
                if (backRightChunk != null)
                {
                    backRightChunk.usedByOthersCount -= 1;

                }



                if (this.backChunk != null)
                {
                    this.backChunk = null;
                }
                if (this.frontChunk != null)
                {
                    this.frontChunk = null;
                }
                if (this.leftChunk != null)
                {
                    this.leftChunk = null;
                }
                if (this.rightChunk != null)
                {
                    this.rightChunk = null;
                }

                if (this.backLeftChunk != null)
                {
                    this.backLeftChunk = null;
                }
                if (this.backRightChunk != null)
                {
                    this.backRightChunk = null;
                }
                if (this.frontLeftChunk != null)
                {
                    this.frontLeftChunk = null;
                }
                if (this.frontRightChunk != null)
                {
                    this.frontRightChunk = null;
                }
            }*/

      /*      void GenerateHeightMap()
            {

                thisAccurateHeightMap = new int[Chunk.chunkWidth, Chunk.chunkWidth];
                for (int x = 0; x < Chunk.chunkWidth; x++)
                {
                    for (int z = 0; z < Chunk.chunkWidth; z++)
                    {
                        thisAccurateHeightMap[x, z] = ChunkHelper.GetSingleChunkLandingPoint(this, x, z);
                    }
                }
                isAccurateHeightMapGenerated = true;
            }*/
            void FreshGenMap(Vector2Int pos)
            {

                map = new BlockData[Chunk.chunkWidth,Chunk.chunkHeight, Chunk.chunkWidth];
                if (curWorld.worldGenType == 0)
                {
                    bool isFrontLeftChunkUpdated = false;
                    bool isFrontRightChunkUpdated = false;
                    bool isBackLeftChunkUpdated = false;
                    bool isBackRightChunkUpdated = false;
                    bool isLeftChunkUpdated = false;
                    bool isRightChunkUpdated = false;
                    bool isFrontChunkUpdated = false;
                    bool isBackChunkUpdated = false;
                    //    System.Random random=new System.Random(pos.x+pos.y);
                    int treeCount = 10;

                    int[,] chunkBiomeMap = GenerateChunkBiomeMap(pos,curWorld.worldID);

                    int interMultiplier = 8;
                    int[,] chunkBiomeMapInterpolated = new int[(Chunk.chunkWidth / 8 + 2) * interMultiplier, (Chunk.chunkWidth / 8 + 2) * interMultiplier];
                    for (int i = 0; i < (Chunk.chunkWidth / 8 + 2) * interMultiplier; ++i)
                    {
                        for (int j = 0; j < (Chunk.chunkWidth / 8 + 2) * interMultiplier; ++j)
                        {
                            int x = i;
                            int y = j;
                            float x1 = (i / interMultiplier) * interMultiplier;
                            float x2 = (i / interMultiplier) * interMultiplier + interMultiplier;
                            float y1 = (j / interMultiplier) * interMultiplier;
                            float y2 = (j / interMultiplier) * interMultiplier + interMultiplier;
                            int x1Ori = (i / interMultiplier);
                            // Debug.Log(x1Ori);
                            int x2Ori = (i / interMultiplier) + 1;
                            x2Ori = Math.Clamp(x2Ori, 0, (Chunk.chunkWidth / 8 + 2) - 1);
                            //   Debug.Log(x2Ori);
                            int y1Ori = (j / interMultiplier);
                            //   Debug.Log(y1Ori);
                            int y2Ori = (j / interMultiplier) + 1;
                            y2Ori = Math.Clamp(y2Ori, 0, (Chunk.chunkWidth / 8 + 2) - 1);
                            //     Debug.Log(y2Ori);

                            float q11 = chunkBiomeMap[x1Ori, y1Ori];
                            float q12 = chunkBiomeMap[x1Ori, y2Ori];
                            float q21 = chunkBiomeMap[x2Ori, y1Ori];
                            float q22 = chunkBiomeMap[x2Ori, y2Ori];
                            float fxy1 = (float)(x2 - x) / (x2 - x1) * q11 + (float)(x - x1) / (x2 - x1) * q21;
                            float fxy2 = (float)(x2 - x) / (x2 - x1) * q12 + (float)(x - x1) / (x2 - x1) * q22;
                            int fxy = (int)((int)(y2 - y) / (y2 - y1) * fxy1 + (int)(y - y1) / (y2 - y1) * fxy2);
                            chunkBiomeMapInterpolated[x, y] = fxy;
                            //       Debug.Log(fxy);
                            //    Debug.Log(x1);
                            //  Debug.Log(x2);

                        }
                    }
                    for (int i = 0; i < Chunk.chunkWidth; i++)
                    {
                        for (int j = 0; j < Chunk.chunkWidth; j++)
                        {
                            //  float noiseValue=200f*Mathf.PerlinNoise(pos.x*0.01f+i*0.01f,pos.y*0.01f+j*0.01f);
                            float noiseValue = thisHeightMap[i + 8, j + 8];
                            for (int k = 0; k < Chunk.chunkHeight; k++)
                            {
                                if (noiseValue > k + 3)
                                {
                                    map[i, k, j] = 1;
                                }
                                else if (noiseValue > k)
                                {
                                    if (chunkBiomeMapInterpolated[i + 8, j + 8] == 3)
                                    {
                                        map[i, k, j] = 1;
                                    }
                                    else
                                    {
                                        map[i, k, j] = 3;
                                    }

                                }


                            }
                        }
                    }


                    for (int i = 0; i < Chunk.chunkWidth; i++)
                    {
                        for (int j = 0; j < Chunk.chunkWidth; j++)
                        {

                            for (int k = Chunk.chunkHeight - 1; k >= 0; k--)
                            {

                                if (map[i, k, j] == 3 && k >= Chunk.chunkSeaLevel - 1)
                                {
                                    map[i, k, j] = 4;
                                    break;
                                }


                                if (k < Chunk.chunkSeaLevel && map[i, k, j] == 0)
                                {
                                    map[i, k, j] = 100;
                                }

                            }
                        }
                    }


                    List<Vector3Int> treePoints = new List<Vector3Int>();
                    List<Vector3Int> treeLeafPoints = new List<Vector3Int>();
                    for (int x = 0; x < 32; x++)
                    {
                        for (int z = 0; z < 32; z++)
                        {
                            Vector3Int point = new Vector3Int(x, (int)thisHeightMap[x, z] + 1, z);
                            if (point.y < Chunk.chunkSeaLevel)
                            {
                                continue;
                            }
                            Vector3Int pointTransformed2 = point - new Vector3Int(8, 0, 8);
                            if (chunkBiomeMapInterpolated[x, z] == 3)
                            {
                                continue;
                            }
                            if (pointTransformed2.x >= 0 && pointTransformed2.x < Chunk.chunkWidth && pointTransformed2.y >= 0 && pointTransformed2.y < Chunk.chunkHeight && pointTransformed2.z >= 0 && pointTransformed2.z < Chunk.chunkWidth)
                            {
                                if (map[pointTransformed2.x, pointTransformed2.y - 1, pointTransformed2.z] == 1)
                                {
                                    continue;
                                }
                            }
                            if (RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(point.x, point.y, point.z) + new Vector3Int(chunkPos.x, 0, chunkPos.y)) > 98)
                            {
                                treePoints.Add(point);
                                treePoints.Add(point + new Vector3Int(0, 1, 0));
                                treePoints.Add(point + new Vector3Int(0, 2, 0));
                                treePoints.Add(point + new Vector3Int(0, 3, 0));
                                treePoints.Add(point + new Vector3Int(0, 4, 0));
                                treePoints.Add(point + new Vector3Int(0, 5, 0));
                                for (int i = -2; i < 3; i++)
                                {
                                    for (int j = -2; j < 3; j++)
                                    {
                                        for (int k = 3; k < 5; k++)
                                        {
                                            Vector3Int pointLeaf = point + new Vector3Int(i, k, j);
                                            treeLeafPoints.Add(pointLeaf);
                                        }
                                    }
                                }
                                treeLeafPoints.Add(point + new Vector3Int(0, 5, 0));
                                treeLeafPoints.Add(point + new Vector3Int(0, 6, 0));
                                treeLeafPoints.Add(point + new Vector3Int(1, 5, 0));
                                treeLeafPoints.Add(point + new Vector3Int(1, 6, 0));
                                treeLeafPoints.Add(point + new Vector3Int(0, 5, 1));
                                treeLeafPoints.Add(point + new Vector3Int(0, 6, 1));
                                treeLeafPoints.Add(point + new Vector3Int(-1, 5, 0));
                                treeLeafPoints.Add(point + new Vector3Int(-1, 6, 0));
                                treeLeafPoints.Add(point + new Vector3Int(0, 5, -1));
                                treeLeafPoints.Add(point + new Vector3Int(0, 6, -1));
                            }
                        }
                    }
                    //  Debug.WriteLine(treePoints[0].x +" "+ treePoints[0].y+" " + treePoints[0].z);
                    foreach (var point1 in treeLeafPoints)
                    {
                        Vector3Int pointTransformed1 = point1 - new Vector3Int(8, 0, 8);
                        //   Debug.WriteLine(pointTransformed.x + " "+pointTransformed.y + " "+pointTransformed.z);
                        if (pointTransformed1.x >= 0 && pointTransformed1.x < Chunk.chunkWidth && pointTransformed1.y >= 0 && pointTransformed1.y < Chunk.chunkHeight && pointTransformed1.z >= 0 && pointTransformed1.z < Chunk.chunkWidth)
                        {
                            if (map[pointTransformed1.x, pointTransformed1.y, pointTransformed1.z] == 0)
                            {
                                map[pointTransformed1.x, pointTransformed1.y, pointTransformed1.z] = 9;
                            }

                        }
                    }
                    foreach (var point in treePoints)
                    {
                        Vector3Int pointTransformed = point - new Vector3Int(8, 0, 8);
                        //   Debug.WriteLine(pointTransformed.x + " "+pointTransformed.y + " "+pointTransformed.z);
                        if (pointTransformed.x >= 0 && pointTransformed.x < Chunk.chunkWidth && pointTransformed.y >= 0 && pointTransformed.y < Chunk.chunkHeight && pointTransformed.z >= 0 && pointTransformed.z < Chunk.chunkWidth)
                        {
                            map[pointTransformed.x, pointTransformed.y, pointTransformed.z] = 7;
                        }
                    }

                    for (int i = 0; i < Chunk.chunkWidth; i++)
                    {
                        for (int j = 0; j < Chunk.chunkWidth; j++)
                        {

                            for (int k = Chunk.chunkHeight - 1; k >= 0; k--)
                            {
                                if (k > Chunk.chunkSeaLevel && map[i, k, j] == 0 && map[i, k - 1, j] == 4 && RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(i, k, j)) > 80)
                                {
                                    map[i, k, j] = 101;
                                }
                            }
                        }
                    }
                    /*   for (int i = 0; i < Chunk.chunkWidth; i++)
                       {
                           for (int j = 0; j < Chunk.chunkWidth; j++)
                           {

                               for (int k = chunkHeight - 1; k >= 0; k--)
                               {
                               if (k > chunkSeaLevel && map[i, k, j] == 0 && map[i, k - 1, j] ==4  &&RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(i,k,j))>80)
                                   {
                                       map[i, k, j] = 101;
                                  }
                                   if (k > chunkSeaLevel && map[i, k, j] == 0 && map[i, k - 1, j] == 4 && map[i, k - 1, j] != 100)
                                   {
                                       if (treeCount > 0)
                                       {
                                                    //Debug.WriteLine(RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(i, k, j)));
                                           if (RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(i, k, j)+new Vector3Int(chunkPos.x,0,chunkPos.y)) > 96)
                                           {


                                               for (int x = -2; x < 3; x++)
                                               {
                                                   for (int y = 3; y < 5; y++)
                                                   {
                                                       for (int z = -2; z < 3; z++)
                                                       {
                                                           if (x + i < 0 || x + i >= Chunk.chunkWidth || z + j < 0 || z + j >= Chunk.chunkWidth)
                                                           {



                                                               if (x + i < 0)
                                                               {
                                                                   if (z + j >= 0 && z + j < Chunk.chunkWidth)
                                                                   {
                                                                       if (leftChunk != null&&leftChunk.disposed==false)
                                                                       {

                                                                           leftChunk.additiveMap[Chunk.chunkWidth + (x + i), y + k, z + j] = 9;

                                                                           isLeftChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(leftChunk,0);
                                                                           //         leftChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                                   else if (z + j < 0)
                                                                   {
                                                                       if (backLeftChunk != null && backLeftChunk.disposed == false)
                                                                       {
                                                                           backLeftChunk.additiveMap[Chunk.chunkWidth + (x + i), y + k, Chunk.chunkWidth - 1 + (z + j)] = 9;

                                                                           isBackLeftChunkUpdated = true;

                                                                           //    WorldManage r.chunkLoadingQueue.UpdatePriority(backLeftChunk,0);
                                                                           //               backLeftChunk.isChunkMapUpdated=true;
                                                                       }

                                                                   }
                                                                   else if (z + j >= Chunk.chunkWidth)
                                                                   {
                                                                       if (frontLeftChunk != null && frontLeftChunk.disposed == false)
                                                                       {
                                                                           frontLeftChunk.additiveMap[Chunk.chunkWidth + (x + i), y + k, (z + j) - Chunk.chunkWidth] = 9;

                                                                           isFrontLeftChunkUpdated = true;

                                                                           //     WorldManager.chunkLoadingQueue.UpdatePriority(frontLeftChunk,0);
                                                                           //                 frontLeftChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }

                                                               }
                                                               else
                                                               if (x + i >= Chunk.chunkWidth)
                                                               {
                                                                   if (z + j >= 0 && z + j < Chunk.chunkWidth)
                                                                   {
                                                                       if (rightChunk != null && rightChunk.disposed == false)
                                                                       {

                                                                           rightChunk.additiveMap[(x + i) - Chunk.chunkWidth, y + k, z + j] = 9;

                                                                           isRightChunkUpdated = true;

                                                                           //   WorldManager.chunkLoadingQueue.UpdatePriority(rightChunk,0);
                                                                           //      rightChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                                   else if (z + j < 0)
                                                                   {
                                                                       if (backRightChunk != null)
                                                                       {
                                                                           backRightChunk.additiveMap[(x + i) - Chunk.chunkWidth, y + k, Chunk.chunkWidth + (z + j)] = 9;

                                                                           isBackRightChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(backRightChunk,0);
                                                                           //         backRightChunk.isChunkMapUpdated=true;
                                                                       }

                                                                   }
                                                                   else if (z + j >= Chunk.chunkWidth)
                                                                   {
                                                                       if (frontRightChunk != null && frontRightChunk.disposed == false)
                                                                       {
                                                                           frontRightChunk.additiveMap[(x + i) - Chunk.chunkWidth, y + k, (z + j) - Chunk.chunkWidth] = 9;

                                                                           isFrontRightChunkUpdated = true;

                                                                           //     WorldManager.chunkLoadingQueue.UpdatePriority(frontRightChunk,0);
                                                                           //          frontRightChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                               }
                                                               else
                                                               if (z + j < 0)
                                                               {

                                                                   if (x + i >= 0 && x + i < Chunk.chunkWidth)
                                                                   {
                                                                       if (backChunk != null)
                                                                       {

                                                                           backChunk.additiveMap[x + i, y + k, Chunk.chunkWidth + (z + j)] = 9;

                                                                           isBackChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(backChunk,0);
                                                                           //         backChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                                   else if (x + i < 0)
                                                                   {
                                                                       if (backLeftChunk != null && backLeftChunk.disposed == false)
                                                                       {
                                                                           backLeftChunk.additiveMap[Chunk.chunkWidth + (x + i), y + k, Chunk.chunkWidth - 1 + (z + j)] = 9;

                                                                           isBackLeftChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(backLeftChunk,0);
                                                                           //            backLeftChunk.isChunkMapUpdated=true;
                                                                       }

                                                                   }
                                                                   else if (x + i >= Chunk.chunkWidth)
                                                                   {
                                                                       if (backRightChunk != null)
                                                                       {
                                                                           backRightChunk.additiveMap[(x + i) - Chunk.chunkWidth, y + k, Chunk.chunkWidth - 1 + (z + j)] = 9;

                                                                           isBackRightChunkUpdated = true;

                                                                           //       WorldManager.chunkLoadingQueue.UpdatePriority(backRightChunk,0);
                                                                           //      backRightChunk.isChunkMapUpdated=true;    
                                                                       }
                                                                   }

                                                               }
                                                               else
                                                               if (z + j >= Chunk.chunkWidth)
                                                               {

                                                                   if (x + i >= 0 && x + i < Chunk.chunkWidth)
                                                                   {
                                                                       if (frontChunk != null && frontChunk.disposed == false)
                                                                       {

                                                                           frontChunk.additiveMap[x + i, y + k, (z + j) - Chunk.chunkWidth] = 9;

                                                                           isFrontChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(frontChunk,0);
                                                                           //   frontChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                                   else if (x + i < 0)
                                                                   {
                                                                       if (frontLeftChunk != null && frontLeftChunk.disposed == false)
                                                                       {
                                                                           frontLeftChunk.additiveMap[Chunk.chunkWidth + (x + i), y + k, (z + j) - Chunk.chunkWidth] = 9;

                                                                           isBackLeftChunkUpdated = true;

                                                                           //        WorldManager.chunkLoadingQueue.UpdatePriority(frontLeftChunk,0);
                                                                           //    frontLeftChunk.isChunkMapUpdated=true;
                                                                       }

                                                                   }
                                                                   else if (x + i >= Chunk.chunkWidth)
                                                                   {
                                                                       if (frontRightChunk != null && frontRightChunk.disposed == false)
                                                                       {
                                                                           frontRightChunk.additiveMap[(x + i) - Chunk.chunkWidth, y + k, (z + j) - Chunk.chunkWidth] = 9;

                                                                           isFrontRightChunkUpdated = true;

                                                                           //  WorldManager.chunkLoadingQueue.UpdatePriority(frontRightChunk,0);
                                                                           //      frontRightChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                               }


                                                           }
                                                           else
                                                           {
                                                               map[x + i, y + k, z + j] = 9;
                                                           }
                                                       }
                                                   }
                                               }
                                               map[i, k, j] = 7;
                                               map[i, k + 1, j] = 7;
                                               map[i, k + 2, j] = 7;
                                               map[i, k + 3, j] = 7;
                                               map[i, k + 4, j] = 7;
                                               map[i, k + 5, j] = 9;
                                               map[i, k + 6, j] = 9;

                                               if (i + 1 < Chunk.chunkWidth)
                                               {
                                                   map[i + 1, k + 5, j] = 9;
                                                   map[i + 1, k + 6, j] = 9;

                                               }
                                               else
                                               {
                                                   if (rightChunk != null)
                                                   {
                                                       rightChunk.additiveMap[0, k + 5, j] = 9;
                                                       rightChunk.additiveMap[0, k + 6, j] = 9;

                                                       //      rightChunk.isChunkMapUpdated=true;
                                                   }
                                               }

                                               if (i - 1 >= 0)
                                               {
                                                   map[i - 1, k + 5, j] = 9;
                                                   map[i - 1, k + 6, j] = 9;

                                               }
                                               else
                                               {
                                                   if (leftChunk != null)
                                                   {
                                                       leftChunk.additiveMap[Chunk.chunkWidth - 1, k + 5, j] = 9;
                                                       leftChunk.additiveMap[Chunk.chunkWidth - 1, k + 6, j] = 9;

                                                       // leftChunk.isChunkMapUpdated=true;
                                                   }
                                               }
                                               if (j + 1 < Chunk.chunkWidth)
                                               {
                                                   map[i, k + 5, j + 1] = 9;
                                                   map[i, k + 6, j + 1] = 9;

                                               }
                                               else
                                               {
                                                   if (frontChunk != null)
                                                   {
                                                       frontChunk.additiveMap[i, k + 5, 0] = 9;
                                                       frontChunk.additiveMap[i, k + 6, 0] = 9;

                                                       //   frontChunk.isChunkMapUpdated=true;
                                                   }
                                               }

                                               if (j - 1 >= 0)
                                               {
                                                   map[i, k + 5, j - 1] = 9;
                                                   map[i, k + 6, j - 1] = 9;

                                               }
                                               else
                                               {
                                                   if (backChunk != null)
                                                   {
                                                       backChunk.additiveMap[i, k + 5, Chunk.chunkWidth - 1] = 9;
                                                       backChunk.additiveMap[i, k + 6, Chunk.chunkWidth - 1] = 9;

                                                       //  backChunk.isChunkMapUpdated=true;
                                                   }
                                               }

                                               treeCount--;
                                           }
                                       }
                                   }

                               }
                           }
                     } */
                    for (int i = 0; i < Chunk.chunkWidth; i++)
                    {
                        for (int j = 0; j < Chunk.chunkWidth; j++)
                        {
                            for (int k = 0; k < Chunk.chunkHeight / 4; k++)
                            {

                                if (0 < k && k < 12)
                                {
                                    if (RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(pos.x, 0, pos.y) + new Vector3Int(i, k, j)) > 96)
                                    {

                                        map[i, k, j] = 10;

                                    }

                                }

                            }

                        }
                    }
                    for (int i = 0; i < Chunk.chunkWidth; i++)
                    {
                        for (int j = 0; j < Chunk.chunkWidth; j++)
                        {
                            map[i, 0, j] = 5;
                        }
                    }
                    /*    if (isBackChunkUpdated)
                        {
                            if (backChunk != null&&backChunk.isReadyToRender)
                            {
                                backChunk.BuildChunk();
                            } }
                        if (isLeftChunkUpdated)
                        {
                        if (leftChunk != null && leftChunk.isReadyToRender)
                            {
                                leftChunk.BuildChunk();
                            }
                        }
                        if (isFrontChunkUpdated && frontChunk.isReadyToRender)
                        {
                        if (frontChunk != null)
                            {
                                frontChunk.BuildChunk();
                            }
                        }
                        if (isRightChunkUpdated && rightChunk.isReadyToRender)
                        {
                        if (rightChunk != null)
                            {
                                rightChunk.BuildChunk();
                            }
                        }*/

                    isMapGenCompleted = true;
                }
                else if (curWorld.worldGenType == 1)
                {
                    for (int i = 0; i < Chunk.chunkWidth; i++)
                    {
                        for (int j = 0; j < Chunk.chunkWidth; j++)
                        {
                            //  float noiseValue=200f*Mathf.PerlinNoise(pos.x*0.01f+i*0.01f,pos.z*0.01f+j*0.01f);
                            for (int k = 0; k < Chunk.chunkHeight / 4; k++)
                            {

                                map[i, k, j] = 1;

                            }
                        }
                    }
                }
                else if (curWorld.worldGenType == 2)
                {
                    for (int i = 0; i < Chunk.chunkWidth; i++)
                    {
                        for (int j = 0; j < Chunk.chunkWidth; j++)
                        {
                            //  float noiseValue=200f*Mathf.PerlinNoise(pos.x*0.01f+i*0.01f,pos.z*0.01f+j*0.01f);
                            for (int k = 0; k < Chunk.chunkHeight / 2; k++)
                            {
                                float yLerpValue = MathHelper.Lerp(-1, 1, (MathF.Abs(k - Chunk.chunkSeaLevel)) / 40f);
                                float xzLerpValue = MathHelper.Lerp(-1, 1, (new Vector3(chunkPos.x + i, 0, chunkPos.y + j).Length() / 384f));
                                float xyzLerpValue = MathHelper.Max(xzLerpValue, yLerpValue);
                                if (curWorld. frequentNoiseGenerator.GetSimplex(i + chunkPos.x, k, j + chunkPos.y) > xyzLerpValue)
                                {
                                    map[i, k, j] = 12;
                                }


                            }
                        }
                    }
                }
            //    this.chunkBounds = this.CalculateBounds();
                generatedStructure.Clear();
                for (int i = 0; i < VoxelWorld.currentWorld.worldStructures.Count; i++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            CalculateChunkStructurePoint(this, i, VoxelWorld.currentWorld.worldStructures[i].genParams, chunkPos + new Vector2Int(x * Chunk.chunkWidth, z * Chunk.chunkWidth));
                        }
                    }

                }

                foreach (var item in generatedStructure)
                {
                    if (VoxelWorld.currentWorld.worldStructures.Count <= item.Item1)
                    {
                        continue;
                    }

                    Vector3Int testPoint = item.Item2;
                    BlockData data = ChunkHelper.GetBlockData(testPoint + new Vector3Int(0, -1, 0));
                    if (testPoint.y < 0)
                    {
                        continue;
                    }
                    bool isIgnored = false;
                    foreach (short ig in VoxelWorld.currentWorld.worldStructures[item.Item1].genParams
                                 .ignorePlacingBlockTypes)
                    {
                        if (data.blockID == ig)
                        {
                            isIgnored = true;
                            break;
                        }
                    }

                    if (isIgnored)
                    {
                        continue;
                    }
                    ServerSideChunkHelper.FillBlocksSingleChunk(
                        VoxelWorld.currentWorld.worldStructures[item.Item1].data,
                        item.Item2 -
                        new Vector3Int(
                            VoxelWorld.currentWorld.worldStructures[item.Item1].data.GetLength(0) / 2, 0,
                            VoxelWorld.currentWorld.worldStructures[item.Item1].data.GetLength(2) / 2),
                        this,curWorld.worldID ,BlockFillMode.ReplaceCustomTypes, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 100, 101);
                    ServerSideChunkHelper.SetBlockWithoutUpdate(testPoint, (short)9, curWorld.worldID);
                }

              
                isMapGenCompleted = true;
            }
        }
        public List<(int, Vector3Int)> generatedStructure = new List<(int, Vector3Int)>();


        public static void CalculateChunkStructurePoint(ServerSideChunk c, int structureID, StructureGeneratingParam param, Vector2Int chunkPos)
        {

            switch (param.type)
            {
                case StructureGeneratingType.Random:
                    if (RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(chunkPos.x, 0,
                            chunkPos.y)) > 100 - param.additionalParam1)
                    {

                        c.generatedStructure.Add(
                            new(structureID, new Vector3Int(chunkPos.x + 8, ChunkHelper.PredictChunkLandingPoint(chunkPos.x + 8, chunkPos.y + 8), chunkPos.y + 8)));
                    }
                    break;
                case StructureGeneratingType.FixedSpacing:
                    break;
            }
        }
        public static int PredictBlockType3D(int x, int y, int z,int worldID)
        {
            float yLerpValue = MathHelper.Lerp(-1, 1, (MathF.Abs(y - Chunk.chunkSeaLevel)) / 40f);
            float xzLerpValue = MathHelper.Lerp(-1, 1, (new Vector3(x, 0, z).Length() / 384f));
            float xyzLerpValue = MathHelper.Max(xzLerpValue, yLerpValue);
            float noiseValue = ServerSideVoxelWorld.voxelWorlds[worldID].frequentNoiseGenerator.GetSimplex(x, y, z);
            if (noiseValue > xyzLerpValue)
            {
                return 1;
            }
            else
            {

                return 0;
            }
            // return 0;
        }
        public bool isMapGenCompleted = false;
        public void Dispose()
        {
            
        }

        public void SaveSingleChunk()
        {

            if (!isModifiedInGame)
            {

                return;
            }
            if (curWorld.chunkDataReadFromDisk.ContainsKey(chunkPos))
            {
                curWorld.chunkDataReadFromDisk.Remove(chunkPos);
                BlockData[,,] worldDataMap = map;
                ChunkData wd = new ChunkData(chunkPos);
                wd.map = worldDataMap;

                curWorld.chunkDataReadFromDisk.Add(chunkPos, wd);
            }
            else
            {
                BlockData[,,] worldDataMap = map;
                ChunkData wd = new ChunkData(chunkPos);
                wd.map = worldDataMap;

                curWorld.chunkDataReadFromDisk.Add(chunkPos, wd);
            }
        }
    }
}
