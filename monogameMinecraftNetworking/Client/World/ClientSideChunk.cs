using MessagePack;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Client.World
{
    public class ClientSideChunk : IDisposable,IChunkFaceBuildingChecks,IRenderableChunkBuffers
    {
        public static int chunkWidth => Chunk.chunkWidth;
        public static int chunkHeight => Chunk.chunkHeight;
        public static int chunkSeaLevel => Chunk.chunkSeaLevel;

        public static FastNoise noiseGenerator { get { return ClientSideVoxelWorld.singleInstance.noiseGenerator; } set { } }

        public static FastNoise biomeNoiseGenerator { get { return ClientSideVoxelWorld.singleInstance.biomeNoiseGenerator; } set { } }

        public static FastNoise frequentNoiseGenerator { get { return ClientSideVoxelWorld.singleInstance.frequentNoiseGenerator; } }

        public bool isMapGenCompleted => isMapDataFetchedFromServer;
        public bool isMapDataFetchedFromServer=false;
        public BlockData[,,] map;
        public Vector2Int chunkPos { get; set; }
        public bool isReadyToRender { get; set; } = false;
        public bool disposed { get; set; }

        [IgnoreMember]
        public volatile ClientSideChunk frontChunk;
        [IgnoreMember]
        public volatile ClientSideChunk backChunk;
        [IgnoreMember]
        public volatile ClientSideChunk leftChunk;
        [IgnoreMember]
        public volatile ClientSideChunk rightChunk;
        [IgnoreMember]
        public volatile ClientSideChunk frontLeftChunk;
        [IgnoreMember]
        public volatile ClientSideChunk frontRightChunk;
        [IgnoreMember]
        public volatile ClientSideChunk backLeftChunk;
        [IgnoreMember]
        public volatile ClientSideChunk backRightChunk;
        public int usedByOthersCount = 0;
        public float unusedSeconds = 0f;
       

        public GraphicsDevice device;
        public bool isUnused { get; set; }
        public bool isTaskCompleted;

        public object chunkBuildingLock=new object();
        public bool _lockWasTaken=false;
        public ClientSideChunk(Vector2Int chunkPos, GraphicsDevice device, ClientSideVoxelWorld world)
        {

            this.device = device;
            this.chunkPos = chunkPos;
            if (world.chunks.ContainsKey(chunkPos))
            {
                Debug.WriteLine("dispose upon launch");
                Dispose();
                return;
            }
            world.chunks.TryAdd(chunkPos, this);
            isReadyToRender = false;
            isTaskCompleted = true;
            //   BuildChunk();

        }
        public List<VertexPositionNormalTangentTexture> verticesOpq;//= new List<VertexPositionNormalTexture>();
        public List<ushort> indicesOpq;
        public List<VertexPositionNormalTangentTexture> verticesOpqLOD;//= new List<VertexPositionNormalTexture>();
        public List<ushort> indicesOpqLOD;
        public List<VertexPositionNormalTangentTexture> verticesNS; //= new List<VertexPositionNormalTexture>();
        public List<ushort> indicesNS;
        public List<VertexPositionNormalTangentTexture> verticesWT;// = new List<VertexPositionNormalTexture>();
        public List<ushort> indicesWT;




        public VertexPositionNormalTangentTexture[] verticesOpqArray
        {
            get
            {
                return _verticesOpqArray;
            }
            set
            {
                _verticesOpqArray = value;
            }
        }
        public VertexPositionNormalTangentTexture[] verticesOpqLOD1Array
        {
            get
            {
                return _verticesOpqLOD1Array;
            }
            set
            {
                _verticesOpqLOD1Array = value;
            }
        }
        public VertexPositionNormalTangentTexture[] verticesNSArray
        {
            get
            {
                return _verticesNSArray;
            }
            set
            {
                _verticesNSArray = value;
            }
        }
        public VertexPositionNormalTangentTexture[] verticesWTArray
        {
            get
            {
                return _verticesWTArray;
            }
            set
            {
                _verticesWTArray = value;
            }
        }
        public ushort[] indicesOpqArray
        {
            get
            {
                return _indicesOpqArray;
            }
            set
            {
                _indicesOpqArray = value;
            }
        }
        public ushort[] indicesOpqLOD1Array
        {
            get
            {
                return _indicesOpqLOD1Array;
            }
            set
            {
                _indicesOpqLOD1Array = value;
            }
        }
        public ushort[] indicesNSArray
        {
            get
            {
                return _indicesNSArray;
            }
            set
            {
                _indicesNSArray = value;
            }
        }
        public ushort[] indicesWTArray
        {
            get
            {
                return _indicesWTArray;
            }
            set
            {
                _indicesWTArray = value;
            }
        }



        public VertexPositionNormalTangentTexture[] _verticesOpqArray;
        public VertexPositionNormalTangentTexture[] _verticesOpqLOD1Array;
        public VertexPositionNormalTangentTexture[] _verticesNSArray;
        public VertexPositionNormalTangentTexture[] _verticesWTArray;
        public ushort[] _indicesOpqArray;
        public ushort[] _indicesOpqLOD1Array;
        public ushort[] _indicesNSArray;
        public ushort[] _indicesWTArray;
        public IndexBuffer IBOpq
        {
            get { return _IBOpq; }
            set
            {
                _IBOpq = value;
            }
        }
        public VertexBuffer VBOpq
        {
            get { return _VBOpq; }
            set
            {
                _VBOpq = value;
            }
        }
        public IndexBuffer IBOpqLOD1
        {
            get { return _IBOpqLOD1; }
            set
            {
                _IBOpqLOD1 = value;
            }
        }
        public VertexBuffer VBOpqLOD1
        {
            get { return _VBOpqLOD1; }
            set
            {
                _VBOpqLOD1 = value;
            }
        }
        public IndexBuffer IBWT
        {
            get { return _IBWT; }
            set
            {
                _IBWT = value;
            }
        }
        public VertexBuffer VBWT
        {
            get { return _VBWT; }
            set
            {
                _VBWT = value;
            }
        }
        public IndexBuffer IBNS
        {
            get { return _IBNS; }
            set
            {
                _IBNS = value;
            }
        }
        public VertexBuffer VBNS
        {
            get { return _VBNS; }
            set
            {
                _VBNS = value;
            }
        }


        public IndexBuffer _IBOpq;
        public VertexBuffer _VBOpq;
        public IndexBuffer _IBOpqLOD1;
        public VertexBuffer _VBOpqLOD1;
        public IndexBuffer _IBWT;
        public VertexBuffer _VBWT;
        public IndexBuffer _IBNS;
        public VertexBuffer _VBNS;
        public BoundingBox chunkBounds { get; set; }
        public BoundingBox CalculateBounds()
        {
            return new BoundingBox(new Vector3(chunkPos.x, 0, chunkPos.y), new Vector3(chunkPos.x + chunkWidth, GetHighestPoint(), chunkPos.y + chunkWidth));
        }
        public int GetHighestPoint()
        {
            int returnValue = 0;
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    for (int y = chunkHeight - 1; y > 0; y--)
                    {
                        if (map[x, y, z] != 0)
                        {
                            if (y > returnValue) returnValue = y;
                        }
                    }
                }
            }
            return returnValue;
        }
        public void InitMap(Vector2Int chunkPos, bool generateRenderBuffers = true)
        {
            this.chunkPos = chunkPos;
            thisHeightMap = GenerateChunkHeightmap(chunkPos);

            //    map = new BlockData[chunkWidth,chunkHeight,chunkWidth];
            verticesOpq = new List<VertexPositionNormalTangentTexture>();
            verticesNS = new List<VertexPositionNormalTangentTexture>();
            verticesWT = new List<VertexPositionNormalTangentTexture>();
            indicesOpq = new List<ushort>();
            indicesNS = new List<ushort>();
            indicesWT = new List<ushort>();
            verticesOpqLOD = new List<VertexPositionNormalTangentTexture>();
            indicesOpqLOD = new List<ushort>();
            frontLeftChunk = ClientSideChunkHelper.GetChunk(new Vector2Int(chunkPos.x - chunkWidth, chunkPos.y + chunkWidth));
            frontRightChunk = ClientSideChunkHelper.GetChunk(new Vector2Int(chunkPos.x + chunkWidth, chunkPos.y + chunkWidth));
            backLeftChunk = ClientSideChunkHelper.GetChunk(new Vector2Int(chunkPos.x - chunkWidth, chunkPos.y - chunkWidth));
            backRightChunk = ClientSideChunkHelper.GetChunk(new Vector2Int(chunkPos.x + chunkWidth, chunkPos.y - chunkWidth));
            backChunk = ClientSideChunkHelper.GetChunk(new Vector2Int(chunkPos.x, chunkPos.y - chunkWidth));
            frontChunk = ClientSideChunkHelper.GetChunk(new Vector2Int(chunkPos.x, chunkPos.y + chunkWidth));
            leftChunk = ClientSideChunkHelper.GetChunk(new Vector2Int(chunkPos.x - chunkWidth, chunkPos.y));

            rightChunk = ClientSideChunkHelper.GetChunk(new Vector2Int(chunkPos.x + chunkWidth, chunkPos.y));
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
           //     Monitor.Enter(frontChunk.chunkBuildingLock);
            }
            if (backChunk != null)
            {
                backChunk.usedByOthersCount += 1;
          //      Monitor.Enter(backChunk.chunkBuildingLock);
            }
            if (leftChunk != null)
            {
                leftChunk.usedByOthersCount += 1;
        //        Monitor.Enter(leftChunk.chunkBuildingLock);
            }
            if (rightChunk != null)
            {
                rightChunk.usedByOthersCount += 1;
         //       Monitor.Enter(rightChunk.chunkBuildingLock);
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

            }
            if (isMapGenCompleted == true)
            {

                this.chunkBounds = this.CalculateBounds();
                GenerateHeightMap();
                GenerateMesh(verticesOpq, verticesNS, verticesWT, indicesOpq, indicesNS, indicesWT, generateRenderBuffers);
                ReleaseChunkUsage();

                //     semaphore.Release();

                return;
            }
           




          
        //    GenerateHeightMap();
        //    isMapGenCompleted = true;
          //  GenerateMesh(verticesOpq, verticesNS, verticesWT, indicesOpq, indicesNS, indicesWT);
            ReleaseChunkUsage();

            //    semaphore.Release();

            void ReleaseChunkUsage()
            {
                if (frontChunk != null)
                {
                    frontChunk.usedByOthersCount -= 1;
            //        Monitor.Exit(frontChunk.chunkBuildingLock);
                }
                if (backChunk != null)
                {
                    backChunk.usedByOthersCount -= 1;
           //         Monitor.Exit(backChunk.chunkBuildingLock);
                }
                if (leftChunk != null)
                {
                    leftChunk.usedByOthersCount -= 1;
         //           Monitor.Exit(leftChunk.chunkBuildingLock);
                }
                if (rightChunk != null)
                {
                    rightChunk.usedByOthersCount -= 1;
             //       Monitor.Exit(rightChunk.chunkBuildingLock);
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
            }

            void GenerateHeightMap()
            {

                thisAccurateHeightMap = new int[chunkWidth, chunkWidth];
                for (int x = 0; x < chunkWidth; x++)
                {
                    for (int z = 0; z < chunkWidth; z++)
                    {
                        thisAccurateHeightMap[x, z] = ClientSideChunkHelper.GetSingleChunkLandingPoint(this, x, z);
                    }
                }
              
            }
        }
        public void GenerateMesh(List<VertexPositionNormalTangentTexture> OpqVerts, List<VertexPositionNormalTangentTexture> NSVerts, List<VertexPositionNormalTangentTexture> WTVerts, List<ushort> OpqIndices, List<ushort> NSIndices, List<ushort> WTIndices, bool generateRenderingBuffers = true)
        {
           // lightPoints = new List<Vector3>();

            for (int x = 0; x < chunkWidth; x++)
            {

                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int z = 0; z < chunkWidth; z++)
                    { 

                        if (map == null)
                        {
                            return;
                        }
                        BlockMeshBuildingHelper.BuildSingleBlock(this, x, y, z, this.map[x, y, z], ref OpqVerts, ref NSVerts, ref WTVerts, ref OpqIndices, ref NSIndices, ref WTIndices);
                        continue;
                

                    }
                }
            }
            GenerateMeshOpqLOD(verticesOpqLOD, indicesOpqLOD, ref _verticesOpqLOD1Array, ref _indicesOpqLOD1Array, ref _VBOpqLOD1, ref _IBOpqLOD1, 2);
            verticesNSArray = verticesNS.ToArray();
            verticesWTArray = verticesWT.ToArray();
            verticesOpqArray = verticesOpq.ToArray();
            indicesOpqArray = indicesOpq.ToArray();
            indicesNSArray = indicesNS.ToArray();
            indicesWTArray = indicesWT.ToArray();


        
            if (generateRenderingBuffers)
            {
                GenerateRenderBuffers();
            }

            /*  this.verticesOpq = null;
              this.verticesNS = null;
              this.verticesWT = null;
              this.indicesOpq = null;
              this.indicesNS = null;
              this.indicesWT = null;*/

        }


        public void GenerateRenderBuffers()
        {
            isReadyToRender = false;
            isVertexBufferDirty = true;
            //      Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
            isNSBuffersValid = false;
            isWTBuffersValid = false;
            isOpqBuffersValid = false;

            VBOpq?.Dispose();
            IBOpq?.Dispose();
            if (verticesOpqArray.Length > 0 || indicesOpqArray.Length > 0)
            {

                if (verticesOpqArray.Length > 0)
                {
                    VBOpq = new VertexBuffer(this.device, typeof(VertexPositionNormalTangentTexture), verticesOpqArray.Length + 1, BufferUsage.WriteOnly);
                    //   }

                    VBOpq.SetData(verticesOpqArray);
                }

                if (indicesOpqArray.Length > 0)
                {
                    IBOpq = new DynamicIndexBuffer(this.device, IndexElementSize.SixteenBits, indicesOpqArray.Length, BufferUsage.WriteOnly);
                    IBOpq.SetData(indicesOpqArray);
                }
                isOpqBuffersValid = true;
            }

            //  if(IBOpq == null)
            //  {


            VBWT?.Dispose();
            IBWT?.Dispose();
            if (verticesWTArray.Length > 0 || indicesWTArray.Length > 0)
            {

                if (verticesWTArray.Length > 0)
                {
                    VBWT = new VertexBuffer(this.device, typeof(VertexPositionNormalTangentTexture), verticesWTArray.Length + 1, BufferUsage.WriteOnly);
                    VBWT.SetData(verticesWTArray);
                }



                if (indicesWTArray.Length > 0)
                {
                    IBWT = new DynamicIndexBuffer(this.device, IndexElementSize.SixteenBits, indicesWTArray.Length, BufferUsage.WriteOnly);
                    IBWT.SetData(indicesWTArray);
                }
                isWTBuffersValid = true;
            }




            VBNS?.Dispose();
            IBNS?.Dispose();
            if (verticesNSArray.Length > 0 || indicesNSArray.Length > 0)
            {


                if (verticesNSArray.Length > 0)
                {
                    VBNS = new VertexBuffer(this.device, typeof(VertexPositionNormalTangentTexture), verticesNSArray.Length + 1, BufferUsage.WriteOnly);
                    VBNS.SetData(verticesNSArray);
                }




                if (indicesNSArray.Length > 0)
                {
                    IBNS = new DynamicIndexBuffer(this.device, IndexElementSize.SixteenBits, indicesNSArray.Length, BufferUsage.WriteOnly);
                    IBNS.SetData(indicesNSArray);
                }

                isNSBuffersValid = true;
            }



            isVertexBufferDirty = false;
        }
        public void BuildChunk()
        {
            isTaskCompleted = false;


            InitMap(chunkPos);
        

            isTaskCompleted = true;
            //     GenerateMesh(verticesOpq, verticesNS, verticesWT);
            //  Debug.WriteLine(verticesOpqArray.Length);
            //Debug.WriteLine(verticesWTArray.Length);
            isReadyToRender = true;
        }

        public async void BuildChunkAsync()
        {
            if (isTaskCompleted == false)
            {
                return;
            }
            isTaskCompleted = false;

            //     Stopwatch sw = new Stopwatch();
            //     sw.Start(); 
            Task.Run(() =>
            {

                InitMap(chunkPos, false);



            }).GetAwaiter().OnCompleted(() =>
            {
                

                    GenerateRenderBuffers();

                    isTaskCompleted = true;
                    //     GenerateMesh(verticesOpq, verticesNS, verticesWT);
                    //  Debug.WriteLine(verticesOpqArray.Length);
                    //Debug.WriteLine(verticesWTArray.Length);
                    isReadyToRender = true;

                
             
            });

            //  sw.Stop();
            //   Debug.WriteLine(sw.ElapsedMilliseconds);







            //      t.RunSynchronously();



        }



        public static int[,] GenerateChunkBiomeMap(Vector2Int pos)
        {
            //   float[,] biomeMap=new float[chunkWidth/8+2,chunkWidth/8+2];//插值算法
            //      int[,] chunkBiomeMap=GenerateChunkBiomeMap(pos);
            int[,] biomeMapInter = new int[chunkWidth / 8 + 2, chunkWidth / 8 + 2];
            for (int i = 0; i < chunkWidth / 8 + 2; i++)
            {
                for (int j = 0; j < chunkWidth / 8 + 2; j++)
                {
                    //           Debug.DrawLine(new Vector3(pos.x+(i-1)*8,60f,pos.y+(j-1)*8),new Vector3(pos.x+(i-1)*8,150f,pos.y+(j-1)*8),Color.green,1f);
                    //    if(RandomGenerator3D.GenerateIntFromVec3(new Vector3Int()))

                    biomeMapInter[i, j] = (int)(1f + biomeNoiseGenerator.GetSimplex(pos.x + (i - 1) * 8, pos.y + (j - 1) * 8) * 3f);
                }
            }//32,32



            return biomeMapInter;
        }
        public float[,] thisHeightMap;
        public int[,] thisAccurateHeightMap;
        public bool isVertexBufferDirty;
        public bool isNSBuffersValid { get; set; }
        public bool isWTBuffersValid { get; set; }
        public bool isOpqBuffersValid {get; set; }



    public void GenerateMeshOpqLOD(List<VertexPositionNormalTangentTexture> verts, List<ushort> indices, ref VertexPositionNormalTangentTexture[] vertsArray, ref ushort[] indicesArray, ref VertexBuffer vb, ref IndexBuffer ib, int lodBlockSkipCount = 2)
        {

          


            for (int x = 0; x < chunkWidth; x += lodBlockSkipCount)
            {
                for (int y = 0; y < chunkHeight; y += 1)
                {
                    for (int z = 0; z < chunkWidth; z += lodBlockSkipCount)
                    {//new int[chunkwidth,chunkheiight,chunkwidth]
                     //     BuildBlock(x, y, z, verts, uvs, tris, vertsNS, uvsNS, trisNS);


                        BlockData typeid = this.map[x, y, z];
                        //    Array.Clear(typeIDs, 0, lodBlockSkipCount * 1 * lodBlockSkipCount);
                        //    Array.Clear(typeIDWeights, 0, lodBlockSkipCount * 1 * lodBlockSkipCount);
                        Vector3Int blockCheckPos = new Vector3Int(x, y, z);
                        BlockMeshBuildingHelper.BuildSingleBlockLOD(lodBlockSkipCount, this, x, y, z, typeid, ref verts, ref indices);
                   
                    }
                }
            }

            vertsArray = verts.ToArray();
            indicesArray = indices.ToArray();
            vb?.Dispose();

            if (vertsArray.Length > 0)
            {
                vb = new VertexBuffer(this.device, typeof(VertexPositionNormalTangentTexture), vertsArray.Length + 1, BufferUsage.WriteOnly);
                //   }

                vb.SetData(vertsArray);
            }

            //  if(IBOpq == null)
            //  {
            ib?.Dispose();
            if (indicesArray.Length > 0)
            {
                ib = new IndexBuffer(this.device, IndexElementSize.SixteenBits, indicesArray.Length, BufferUsage.WriteOnly);
                ib.SetData(indicesArray);
            }
        }

        public static float[,] GenerateChunkHeightmap(Vector2Int pos)
        {
            float[,] heightMap = new float[chunkWidth / 8 + 2, chunkWidth / 8 + 2];//插值算法
            int[,] chunkBiomeMap = GenerateChunkBiomeMap(pos);

            for (int i = 0; i < chunkWidth / 8 + 2; i++)
            {
                for (int j = 0; j < chunkWidth / 8 + 2; j++)
                {
                    //           Debug.DrawLine(new Vector3(pos.x+(i-1)*8,60f,pos.y+(j-1)*8),new Vector3(pos.x+(i-1)*8,150f,pos.y+(j-1)*8),Color.green,1f);
                    //    if(RandomGenerator3D.GenerateIntFromVec3(new Vector3Int()))

                    heightMap[i, j] = chunkSeaLevel + noiseGenerator.GetSimplex(pos.x + (i - 1) * 8, pos.y + (j - 1) * 8) * 20f + chunkBiomeMap[i, j] * 25f;
                }

            }//32,32
            int interMultiplier = 8;
            float[,] heightMapInterpolated = new float[(chunkWidth / 8 + 2) * interMultiplier, (chunkWidth / 8 + 2) * interMultiplier];
            for (int i = 0; i < (chunkWidth / 8 + 2) * interMultiplier; ++i)
            {
                for (int j = 0; j < (chunkWidth / 8 + 2) * interMultiplier; ++j)
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
                    x2Ori = Math.Clamp(x2Ori, 0, (chunkWidth / 8 + 2) - 1);
                    //   Debug.Log(x2Ori);
                    int y1Ori = (j / interMultiplier);
                    //   Debug.Log(y1Ori);
                    int y2Ori = (j / interMultiplier) + 1;
                    y2Ori = Math.Clamp(y2Ori, 0, (chunkWidth / 8 + 2) - 1);
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
     

        public static int PredictBlockType(float noiseValue, int y)
        {
            if (noiseValue > y)
            {
                return 1;
            }
            else
            {
                if (y < chunkSeaLevel && y > noiseValue)
                {
                    return 100;
                }
                return 0;
            }
            // return 0;
        }
        public static int PredictBlockType3D(int x, int y, int z)
        {
            float yLerpValue = MathHelper.Lerp(-1, 1, (MathF.Abs(y - chunkSeaLevel)) / 40f);
            float xzLerpValue = MathHelper.Lerp(-1, 1, (new Vector3(x, 0, z).Length() / 384f));
            float xyzLerpValue = MathHelper.Max(xzLerpValue, yLerpValue);
            float noiseValue = frequentNoiseGenerator.GetSimplex(x, y, z);
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
        public static int PredictBlockType3DLOD(int x, int y, int z, int LODBlockSkipCount = 4)
        {
            float yLerpValue = MathHelper.Lerp(-1, 1, (MathF.Abs(y - chunkSeaLevel)) / 40f);
            float xzLerpValue = MathHelper.Lerp(-1, 1, (new Vector3(x, 0, z).Length() / 384f));
            float xyzLerpValue = MathF.Max(xzLerpValue, yLerpValue);
            float noiseValue = frequentNoiseGenerator.GetSimplex((int)(x / LODBlockSkipCount) * LODBlockSkipCount, y, (int)(z / LODBlockSkipCount) * LODBlockSkipCount);
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
        public int GetChunkBlockTypeLOD(int x, int y, int z, int LODSkipBlockCount = 2)
        {
            if (y < 0 || y > chunkHeight - 1)
            {
                return 0;
            }

            if ((x < 0) || (z < 0) || (x >= chunkWidth) || (z >= chunkWidth))
            {
                if (ClientSideVoxelWorld.singleInstance.worldGenType == 0)
                {
                    if (x >= chunkWidth)
                    {
                        return PredictBlockType(thisHeightMap[x - chunkWidth + 25, z + 8], y);

                    }
                    else if (z >= chunkWidth)
                    {
                        return PredictBlockType(thisHeightMap[x + 8, z - chunkWidth + 25], y);
                    }
                    else if (x < 0)
                    {
                        return PredictBlockType(thisHeightMap[8 + x, z + 8], y);
                    }
                    else if (z < 0)
                    {
                        return PredictBlockType(thisHeightMap[x + 8, 8 + z], y);
                    }
                }
                else if (ClientSideVoxelWorld.singleInstance.worldGenType == 2)
                {
                    if (x >= chunkWidth)
                    {
                        if (rightChunk != null && rightChunk.isMapGenCompleted == true && rightChunk.disposed == false)
                        {
                            return PredictBlockType3DLOD(chunkPos.x + x, y, chunkPos.y + z, LODSkipBlockCount);

                        }
                        else return PredictBlockType3DLOD(chunkPos.x + x, y, chunkPos.y + z, LODSkipBlockCount);

                    }
                    else if (z >= chunkWidth)
                    {
                        if (frontChunk != null && frontChunk.isMapGenCompleted == true && frontChunk.disposed == false)
                        {
                            return PredictBlockType3DLOD(chunkPos.x + x, y, chunkPos.y + z, LODSkipBlockCount);



                        }
                        else return PredictBlockType3DLOD(chunkPos.x + x, y, chunkPos.y + z, LODSkipBlockCount);
                    }
                    else if (x < 0)
                    {
                        if (leftChunk != null && leftChunk.isMapGenCompleted == true && leftChunk.disposed == false)
                        {
                            return PredictBlockType3DLOD(chunkPos.x + x, y, chunkPos.y + z, LODSkipBlockCount);

                        }
                        else return PredictBlockType3DLOD(chunkPos.x + x, y, chunkPos.y + z, LODSkipBlockCount);
                    }
                    else if (z < 0)
                    {
                        if (backChunk != null && backChunk.isMapGenCompleted == true && backChunk.disposed == false)
                        {
                            return PredictBlockType3DLOD(chunkPos.x + x, y, chunkPos.y + z, LODSkipBlockCount);

                        }
                        else return PredictBlockType3DLOD(chunkPos.x + x, y, chunkPos.y + z, LODSkipBlockCount);
                    }
                }
                else
                {
                    return 1;
                }


            }
            return map[x, y, z];
        }
        public int GetChunkBlockType(int x, int y, int z)
        {
            if (y < 0 || y > chunkHeight - 1)
            {
                return 0;
            }

            try
            {
                if ((x < 0) || (z < 0) || (x >= chunkWidth) || (z >= chunkWidth))
                {

                    if (ClientSideVoxelWorld.singleInstance.worldGenType == 0)
                    {
                        if (x >= chunkWidth)
                        {
                            if (rightChunk != null && rightChunk.isMapGenCompleted == true &&
                                rightChunk.disposed == false)
                            {
                                return rightChunk.map[0, y, z];
                            }
                            else return PredictBlockType(thisHeightMap[x - chunkWidth + 25, z + 8], y);

                        }
                        else if (z >= chunkWidth)
                        {
                            if (frontChunk != null && frontChunk.isMapGenCompleted == true &&
                                frontChunk.disposed == false)
                            {
                                return frontChunk.map[x, y, 0];
                            }
                            else return PredictBlockType(thisHeightMap[x + 8, z - chunkWidth + 25], y);
                        }
                        else if (x < 0)
                        {
                            if (leftChunk != null && leftChunk.isMapGenCompleted == true && leftChunk.disposed == false)
                            {
                                return leftChunk.map[chunkWidth - 1, y, z];
                            }
                            else return PredictBlockType(thisHeightMap[8 + x, z + 8], y);
                        }
                        else if (z < 0)
                        {
                            if (backChunk != null && backChunk.isMapGenCompleted == true && backChunk.disposed == false)
                            {
                                return backChunk.map[x, y, chunkWidth - 1];
                            }
                            else return PredictBlockType(thisHeightMap[x + 8, 8 + z], y);
                        }
                    }
                    else if (ClientSideVoxelWorld.singleInstance.worldGenType == 2)
                    {

                        if (x >= chunkWidth)
                        {
                            if (rightChunk != null && rightChunk.isMapGenCompleted == true)
                            {
                                if (rightChunk.isMapGenCompleted == true)
                                {

                                    return rightChunk.map[0, y, z];
                                }
                                else return PredictBlockType3D(chunkPos.x + x, y, chunkPos.y + z);

                            }
                            else return PredictBlockType3D(chunkPos.x + x, y, chunkPos.y + z);

                        }
                        else if (z >= chunkWidth)
                        {
                            if (frontChunk != null && frontChunk.isMapGenCompleted == true)
                            {
                                if (frontChunk.isMapGenCompleted == true)
                                {

                                    return frontChunk.map[x, y, 0];
                                }
                                else return PredictBlockType3D(chunkPos.x + x, y, chunkPos.y + z);



                            }
                            else return PredictBlockType3D(chunkPos.x + x, y, chunkPos.y + z);
                        }
                        else if (x < 0)
                        {
                            if (leftChunk != null && leftChunk.isMapGenCompleted == true)
                            {
                                if (leftChunk.isMapGenCompleted == true)
                                {

                                    return leftChunk.map[chunkWidth - 1, y, z];
                                }
                                else return PredictBlockType3D(chunkPos.x + x, y, chunkPos.y + z);

                            }
                            else return PredictBlockType3D(chunkPos.x + x, y, chunkPos.y + z);
                        }
                        else if (z < 0)
                        {
                            if (backChunk != null && backChunk.isMapGenCompleted == true)
                            {
                                if (backChunk.isMapGenCompleted == true)
                                {
                                    return backChunk.map[x, y, chunkWidth - 1];
                                }
                                else return PredictBlockType3D(chunkPos.x + x, y, chunkPos.y + z);

                            }
                            else return PredictBlockType3D(chunkPos.x + x, y, chunkPos.y + z);
                        }

                    }


                }
            }
            catch (Exception e)
            {
                return 1;
            }
           
           
            return map[x, y, z];
        }

        public bool CheckNeedBuildFace(int x, int y, int z, BlockData curBlock, int LODSkipBlockCount)
        {
            // return true;
            if (y < 0) return false;
            var type = GetChunkBlockTypeLOD(x, y, z, LODSkipBlockCount);
            bool isNonSolid = false;
            if (type == 0 && curBlock.blockID != 0)
            {
                return true;
            }
            if (type != 0 && curBlock.blockID == 0)
            {
                return true;
            }
            if (type == 0 && curBlock.blockID == 0)
            {
                return false;
            }
            BlockShape shape = Chunk.blockInfosNew[curBlock.blockID].shape;
            BlockShape shape1 = Chunk.blockInfosNew[type].shape;
            if (shape == BlockShape.Solid)
            {
                if (shape1 == BlockShape.Solid)
                {
                    return false;
                }
                else
                if (shape1 == BlockShape.Water)
                {
                    return true;
                }
                else
                {
                    return true;
                }

            }
            if (shape == BlockShape.Water)
            {
                if (shape1 == BlockShape.Solid)
                {
                    return false;
                }
                else if (shape1 == BlockShape.Water)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            if (shape1 != BlockShape.Solid && shape == BlockShape.Solid)
            {

                return true;
            }
            if (shape1 == BlockShape.Water && shape == BlockShape.Water)
            {
                return false;
            }
            if (shape1 == BlockShape.Water && shape == BlockShape.Solid)
            {
                return false;
            }
            if (shape1 != shape)
            {
                return true;
            }
            return false;

            /*   if (isThisNS == true)
               {
                   switch (type)
                   {
                       case 100:
                           //       Debug.WriteLine("true");
                           return false;
                       case 0:
                           return true;
                       default: return false;
                   }
               }
               else
               {
                   if (type != 0 && blockInfosNew[type].shape != BlockShape.Solid)
                   {
                       isNonSolid = true;
                   }
                   switch (isNonSolid)
                   {
                       case true: return true;
                       case false: break;
                   }
               }


               switch (type)
               {


                   case 0:
                       return true;
                   case 9:
                       return true;
                   default:
                       return false;
               }*/
        }


        public bool CheckNeedBuildFace(int x, int y, int z, BlockData curBlock)
        {
            // return true;
            if (y < 0) return false;
            var type = GetChunkBlockType(x, y, z);
            bool isNonSolid = false;
            if (type == 0 && curBlock.blockID != 0)
            {
                return true;
            }
            if (type != 0 && curBlock.blockID == 0)
            {
                return true;
            }
            if (type == 0 && curBlock.blockID == 0)
            {
                return false;
            }
            BlockShape shape =Chunk. blockInfosNew[curBlock.blockID].shape;
            BlockShape shape1 = Chunk.blockInfosNew[type].shape;
            if (shape == BlockShape.Solid)
            {
                if (shape1 == BlockShape.Solid)
                {
                    return false;
                }
                else
                if (shape1 == BlockShape.Water)
                {
                    return true;
                }
                else
                {
                    return true;
                }

            }
            if (shape == BlockShape.Water)
            {
                if (shape1 == BlockShape.Solid)
                {
                    return false;
                }
                else if (shape1 == BlockShape.Water)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            if (shape1 != BlockShape.Solid && shape == BlockShape.Solid)
            {

                return true;
            }
            if (shape1 == BlockShape.Water && shape == BlockShape.Water)
            {
                return false;
            }
            if (shape1 == BlockShape.Water && shape == BlockShape.Solid)
            {
                return false;
            }
            if (shape1 != shape)
            {
                return true;
            }
            return false;

            /*   if (isThisNS == true)
               {
                   switch (type)
                   {
                       case 100:
                           //       Debug.WriteLine("true");
                           return false;
                       case 0:
                           return true;
                       default: return false;
                   }
               }
               else
               {
                   if (type != 0 && blockInfosNew[type].shape != BlockShape.Solid)
                   {
                       isNonSolid = true;
                   }
                   switch (isNonSolid)
                   {
                       case true: return true;
                       case false: break;
                   }
               }


               switch (type)
               {


                   case 0:
                       return true;
                   case 9:
                       return true;
                   default:
                       return false;
               }*/
        }


        public bool CheckNeedBuildFace(int x, int y, int z, bool isThisNS)
        {
            // return true;
            if (y < 0) return false;
            var type = GetChunkBlockType(x, y, z);
            bool isNonSolid = false;
            if (isThisNS == true)
            {
                switch (type)
                {
                    case 100:
                        //       Debug.WriteLine("true");
                        return false;
                    case 0:
                        return true;
                    default: return false;
                }
            }
            else
            {
                if (type != 0 &&Chunk. blockInfosNew[type].shape != BlockShape.Solid)
                {
                    isNonSolid = true;
                }
                switch (isNonSolid)
                {
                    case true: return true;
                    case false: break;
                }
            }


            switch (type)
            {


                case 0:
                    return true;
                case 9:
                    return true;
                default:
                    return false;
            }
        }
 


        public void Dispose()
        {
            // Debug.WriteLine("dispose");
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        ~ClientSideChunk()
        {

            Dispose(false);
        }

        public void Dispose(bool disposing)
        {

            if (disposed)
            {
                return;
            }
            //    Debug.WriteLine("dispose"+disposing);
            if (disposing)
            {
                if (isTaskCompleted == false)
                {
                    Debug.WriteLine("dispose failed");
                    return;
                }


                this.backChunk = null;

                this.frontChunk = null;

                this.leftChunk = null;


                this.rightChunk = null;



                this.backLeftChunk = null;


                this.backRightChunk = null;


                this.frontLeftChunk = null;


                this.frontRightChunk = null;

                this.map = null;
                this.thisHeightMap = null;

                this.verticesNSArray = null;
                //  this.additiveMap = null;
                this.verticesWTArray = null;
                this.verticesOpqArray = null;
                this.verticesOpq = null;
                this.verticesNS = null;
                this.verticesWT = null;
                this.indicesOpq = null;
                this.indicesNS = null;
                this.indicesWT = null;
                this.verticesNSArray = null;
                this.indicesOpqArray = null;
                this.verticesOpqLOD1Array = null;
                this.indicesOpqLOD1Array = null;
                this.indicesNSArray = null;
                this.indicesWTArray = null;
                if (this.VBOpqLOD1 != null)
                {
                    this.VBOpqLOD1.Dispose();
                }
                if (this.IBOpqLOD1 != null)
                {
                    this.IBOpqLOD1.Dispose();
                }
                if (this.VBOpq != null)
                {
                    this.VBOpq.Dispose();
                }
                if (this.VBOpq != null)
                {
                    this.VBOpq.Dispose();
                }
                if (this.IBOpq != null)
                {
                    this.IBOpq.Dispose();
                }
                if (this.VBWT != null)
                {
                    this.VBWT.Dispose();
                }
                if (this.IBWT != null)
                {
                    this.IBWT.Dispose();
                }
                if (this.VBNS != null)
                {
                    this.VBNS.Dispose();
                }
                if (this.IBNS != null)
                {
                    this.IBNS.Dispose();
                }

                this.VBOpq = null;
                this.IBOpq = null;
                this.VBOpqLOD1 = null;
                this.IBOpqLOD1 = null;
                this.VBWT = null;
                this.IBWT = null;
                this.VBNS = null;
                this.IBNS = null;
                this.device = null;
               
                //   this.semaphore=null;
            }
            disposed = true;






        }
    }
}
