using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Rendering;

namespace monogameMinecraftShared.World
{

    


        public class Chunk : IDisposable,IChunkFaceBuildingChecks,IRenderableChunkBuffers
    {
        
        public static FastNoise noiseGenerator { get { return VoxelWorld.currentWorld.noiseGenerator; } set { } }
       
        public static FastNoise biomeNoiseGenerator { get { return VoxelWorld.currentWorld.biomeNoiseGenerator; } set { } }
        
        public static FastNoise frequentNoiseGenerator { get { return VoxelWorld.currentWorld.frequentNoiseGenerator; } }
        //   [IgnoreMember]
        //     public static int worldGenType =0;//1 superflat 0 inf
       
        public static int chunkWidth = 16;
      
        public static int chunkHeight = 256;
       
        public BlockData[,,] map;
        //  [IgnoreMember]
        // public short[,,] additiveMap = new short[chunkWidth, chunkHeight, chunkWidth];
         
        public Vector2Int chunkPos { get; set; }
       
        public bool isChunkDataSavedInDisk = false;
        public GraphicsDevice device;
        public Chunk(Vector2Int chunkPos, GraphicsDevice device, VoxelWorld world)
        {

            this.device = device;
            this.chunkPos = chunkPos;
            if (VoxelWorld.currentWorld.chunks.ContainsKey(chunkPos))
            {
                Debug.WriteLine("dispose upon launch");
                Dispose();
                return;
            }
            world.chunks.TryAdd(chunkPos, this);
            isReadyToRender = false;

            BuildChunk();

        }
        public bool isMapGenCompleted = false;
        public static Dictionary<int, SoundEffect> blockSoundInfo = new Dictionary<int, SoundEffect>();
        public static Dictionary<int, BlockInfo> blockInfosNew = new Dictionary<int, BlockInfo>{
            {1,new BlockInfo(new List<Vector2>{new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(0f,0f)},new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {2,new BlockInfo(new List<Vector2>{new Vector2(0.0625f,0f),new Vector2(0.0625f,0f),new Vector2(0.0625f,0f),new Vector2(0.0625f,0f),new Vector2(0.0625f,0f),new Vector2(0.0625f,0f)},new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {3,new BlockInfo(new List<Vector2> { new Vector2(0.125f, 0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {4,new BlockInfo( new List<Vector2> { new Vector2(0.1875f, 0f), new Vector2(0.1875f, 0f), new Vector2(0.125f, 0f), new Vector2(0.0625f, 0f), new Vector2(0.1875f, 0f), new Vector2(0.1875f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {5,new BlockInfo( new List<Vector2> { new Vector2(0.375f, 0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {6,new BlockInfo(new List<Vector2> { new Vector2(0.25f, 0f), new Vector2(0.25f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {7,new BlockInfo(new List<Vector2> { new Vector2(0.3125f, 0f), new Vector2(0.3125f, 0f), new Vector2(0.25f, 0f), new Vector2(0.25f, 0f), new Vector2(0.3125f, 0f), new Vector2(0.3125f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {8,new BlockInfo(new List<Vector2> { new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.25f, 0f), new Vector2(0.25f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {9,new BlockInfo( new List<Vector2> { new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {10,new BlockInfo(new List<Vector2> { new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },


            {11,new BlockInfo( new List<Vector2> { new Vector2(0.625f, 0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {12,new BlockInfo(new List<Vector2> { new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {13,new BlockInfo( new List<Vector2> { new Vector2(0.75f, 0f), new Vector2(0.75f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.8125f, 0f), new Vector2(0.75f, 0f), new Vector2(0.75f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {14,new BlockInfo( new List<Vector2> { new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Slabs) },
            {15,new BlockInfo(new List<Vector2> { new Vector2(0.875f, 0f), new Vector2(0.875f, 0f), new Vector2(0.875f, 0f), new Vector2(0.875f, 0f), new Vector2(0.875f, 0f), new Vector2(0.875f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {16,new BlockInfo( new List<Vector2> { new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
            {17,new BlockInfo(new List<Vector2> { new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Solid) },
           {100,new BlockInfo( new List<Vector2> { new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.Water) },

         {101,new BlockInfo(new List<Vector2> { new Vector2(0.0625f, 0.0625f) },new List<Vector2>{ new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f),new Vector2(0.0625f, 0.0625f) },BlockShape.CrossModel) },
          {102,new BlockInfo(new List<Vector2>{ new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f),
              new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f),
              new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f),
              new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0.03125f) + new Vector2(0.0625f, 0f),
              new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f),
              new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f)},
              new List<Vector2>{  new Vector2(0.0078125f, 0.0390625f),
                   new Vector2(0.0078125f, 0.0390625f),
                  new Vector2(0.0078125f, 0.0078125f),
                  new Vector2(0.0078125f, 0.0078125f),
                  new Vector2(0.0078125f, 0.0390625f),
                  new Vector2(0.0078125f, 0.0390625f) },BlockShape.Torch) },
          {
              103,
              new BlockInfo(
                  new List<Vector2>
                  {
                      new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f),
                      new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f),new Vector2(0.25f, 0.0625f)
                  },
                  new List<Vector2>
                  {
                      new Vector2(0.0625f, 0.0625f), new Vector2(0.0625f, 0.0625f), new Vector2(0.0625f, 0.0625f),
                      new Vector2(0.0625f, 0.0625f), new Vector2(0.0625f, 0.0625f), new Vector2(0.0625f, 0.0625f)
                  }, BlockShape.Fence)
          },
          {
              104,
              new BlockInfo(
                  new List<Vector2>
                  {
                      new Vector2(0.9375f, 0.125f),  new Vector2(0.9375f, 0.125f),  new Vector2(0.9375f, 0.125f),
                      new Vector2(0.9375f, 0.125f), new Vector2(0.9375f, 0.125f),  new Vector2(0.9375f, 0.125f),
                      new Vector2(0.9375f, 0.0625f), new Vector2(0.9375f, 0.0625f),new Vector2(0.9375f, 0.0625f),
                      new Vector2(0.9375f, 0.0625f), new Vector2(0.9375f, 0.0625f),new Vector2(0.9375f, 0.0625f)
                     
                  },
                  new List<Vector2>
                  {
                      new Vector2(0.0625f, 0.0625f), new Vector2(0.0625f, 0.0625f), new Vector2(0.01171875f, 0.0625f),
                      new Vector2(0.01171875f, 0.0625f), new Vector2(0.01171875f, 0.0625f), new Vector2(0.01171875f, 0.0625f),
                      new Vector2(0.0625f, 0.0625f), new Vector2(0.0625f, 0.0625f), new Vector2(0.01171875f, 0.0625f),
                      new Vector2(0.01171875f, 0.0625f), new Vector2(0.01171875f, 0.0625f), new Vector2(0.01171875f, 0.0625f)
                  }, BlockShape.Door)
          },

    };
            [Obsolete]
        public static Dictionary<int, List<Vector2>> blockInfo = new Dictionary<int, List<Vector2>> {
        { 1,new List<Vector2>{new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(0f,0f)} },
     {2,new List<Vector2>{new Vector2(0.0625f,0f),new Vector2(0.0625f,0f),new Vector2(0.0625f,0f),new Vector2(0.0625f,0f),new Vector2(0.0625f,0f),new Vector2(0.0625f,0f)}},
 {3, new List<Vector2> { new Vector2(0.125f, 0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0f) }},
 {4, new List<Vector2> { new Vector2(0.1875f, 0f), new Vector2(0.1875f, 0f), new Vector2(0.125f, 0f), new Vector2(0.0625f, 0f), new Vector2(0.1875f, 0f), new Vector2(0.1875f, 0f) }},
 {100, new List<Vector2> { new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.0625f) }},
 {101, new List<Vector2> { new Vector2(0.0625f, 0.0625f) } },

 {5, new List<Vector2> { new Vector2(0.375f, 0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0f) }},
 {6, new List<Vector2> { new Vector2(0.25f, 0f), new Vector2(0.25f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f) }},
 {7, new List<Vector2> { new Vector2(0.3125f, 0f), new Vector2(0.3125f, 0f), new Vector2(0.25f, 0f), new Vector2(0.25f, 0f), new Vector2(0.3125f, 0f), new Vector2(0.3125f, 0f) }},
 {8, new List<Vector2> { new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.25f, 0f), new Vector2(0.25f, 0f) }},
 {9, new List<Vector2> { new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0f) }},
 {10, new List<Vector2> { new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0f) }},
 {11, new List<Vector2> { new Vector2(0.625f, 0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0f) }},


        {12, new List<Vector2> { new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.6875f, 0f) }},
    {13, new List<Vector2> { new Vector2(0.75f, 0f), new Vector2(0.75f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.8125f, 0f), new Vector2(0.75f, 0f), new Vector2(0.75f, 0f) }},

          {14, new List<Vector2> { new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.1875f, 0.0625f) }},
          {15, new List<Vector2> { new Vector2(0.875f, 0f), new Vector2(0.875f, 0f), new Vector2(0.875f, 0f), new Vector2(0.875f, 0f), new Vector2(0.875f, 0f), new Vector2(0.875f, 0f) }},
        {16, new List<Vector2> { new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f), new Vector2(0.9375f, 0f) }},
        {17, new List<Vector2> { new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.25f, 0.0625f) }},
    };
        //0None 1Stone 2Grass 3Dirt 4Side grass block 5Bedrock 6WoodX 7WoodY 8WoodZ 9Leaves 10Diamond Ore 11Sand 12End Stone 13End Portal 14Sea Lantern 15Iron Block 16Cobblestone 17Wood Planks
        //100Water 101Grass
        //102torch
        //200Leaves
        //0-99solid blocks
        //100-199no hitbox blocks
        //200-299hitbox nonsolid blocks
        //  public static System.Random worldRandomGenerator = new System.Random(0);
        [IgnoreMember]
        public Chunk frontChunk;
        [IgnoreMember]
        public Chunk backChunk;
        [IgnoreMember]
        public Chunk leftChunk;
        [IgnoreMember]
        public Chunk rightChunk;
        [IgnoreMember]
        public Chunk frontLeftChunk;
        [IgnoreMember]
        public Chunk frontRightChunk;
        [IgnoreMember]
        public Chunk backLeftChunk;
        [IgnoreMember]
        public Chunk backRightChunk;
        [IgnoreMember]
        public static int chunkSeaLevel = 63;
        public ChunkData ChunkToChunkData()
        {
            return new ChunkData(this.map, this.chunkPos);
        }
        public int updateCount = 0;
        public bool BFSIsWorking = false;
     //   public bool[,,] mapIsSearched;

        public bool isModifiedInGame;
        /* public void BFSInit(int x, int y, int z, int ignoreSide, int GainedUpdateCount)
        {
            updateCount = GainedUpdateCount;
            mapIsSearched = new bool[chunkWidth + 2, chunkHeight + 2, chunkWidth + 2];
            BFSIsWorking = true;
            Task.Run(() => BFSMapUpdate(x, y, z, ignoreSide));
        }
       public async void BFSMapUpdate(int x, int y, int z, int ignoreSide)
        {
            //left right bottom top back front
            //left x-1 right x+1 top y+1 bottom y-1 back z-1 front z+1
            // Task.Delay(30);
            if (!BFSIsWorking)
            {
                return;
            }
            if (updateCount > 64)
            {

                //       Program.CastToAllClients(new Message("WorldData", MessagePackSerializer.Serialize(this.ChunkToChunkData())));
                BFSIsWorking = false;
                return;
            }

            mapIsSearched[x, y, z] = true;

            try
            {
                if (GetBlock(new Vector3(chunkPos.x + x, y, chunkPos.y + z)) == 101 && GetBlock(new Vector3(chunkPos.x + x, y - 1, chunkPos.y + z)) == 0)
                {
                    BreakBlockAtPoint(new Vector3(chunkPos.x + x, y, chunkPos.y + z));
                }
                if (GetBlock(new Vector3(chunkPos.x + x, y, chunkPos.y + z)) == 100 && GetBlock(new Vector3(chunkPos.x + x, y - 1, chunkPos.y + z)) == 0)
                {
                    SetBlockWithUpdate(new Vector3(chunkPos.x + x, y - 1, chunkPos.y + z), 100);
                }

                if (GetBlock(new Vector3(chunkPos.x + x, y, chunkPos.y + z)) == 100 && GetBlock(new Vector3(chunkPos.x + x - 1, y, chunkPos.y + z)) == 0)
                {
                    SetBlockWithUpdate(new Vector3(chunkPos.x + x - 1, y, chunkPos.y + z), 100);
                }
                if (GetBlock(new Vector3(chunkPos.x + x, y, chunkPos.y + z)) == 100 && GetBlock(new Vector3(chunkPos.x + x + 1, y, chunkPos.y + z)) == 0)
                {
                    SetBlockWithUpdate(new Vector3(chunkPos.x + x + 1, y, chunkPos.y + z), 100);
                }
                if (GetBlock(new Vector3(chunkPos.x + x, y, chunkPos.y + z)) == 100 && GetBlock(new Vector3(chunkPos.x + x, y, chunkPos.y + z - 1)) == 0)
                {
                    SetBlockWithUpdate(new Vector3(chunkPos.x + x, y, chunkPos.y + z - 1), 100);
                }
                if (GetBlock(new Vector3(chunkPos.x + x, y, chunkPos.y + z)) == 100 && GetBlock(new Vector3(chunkPos.x + x, y, chunkPos.y + z + 1)) == 0)
                {
                    SetBlockWithUpdate(new Vector3(chunkPos.x + x, y, chunkPos.y + z + 1), 100);
                }
            }
            catch
            {
                Console.WriteLine("outbound update");
            }

            updateCount++;
            if (!(ignoreSide == 0) && x - 1 >= 0)
            {
                try
                {
                    if (!mapIsSearched[x - 1, y, z] && map[x - 1, y, z] != 0)
                        Task.Run(() => BFSMapUpdate(x - 1, y, z, ignoreSide));
                }
                catch
                {
                    Console.WriteLine("outbound update");
                }

            }
            else if (x - 1 < 0)
            {

                if (leftChunk != null)
                {
                    leftChunk.BFSInit(chunkWidth - 1, y, z, ignoreSide, updateCount);
                }
            }
            if (!(ignoreSide == 1) && x + 1 < chunkWidth)
            {
                try
                {
                    if (!mapIsSearched[x + 1, y, z] && map[x + 1, y, z] != 0)
                        Task.Run(() => BFSMapUpdate(x + 1, y, z, ignoreSide));
                }
                catch
                {
                    Console.WriteLine("outbound update");
                }
            }
            else if (x + 1 >= chunkWidth)
            {
                if (rightChunk != null)
                {
                    rightChunk.BFSInit(0, y, z, ignoreSide, updateCount);
                }
            }
            if (!(ignoreSide == 2) && y - 1 >= 0)
            {
                try
                {
                    if (!mapIsSearched[x, y - 1, z] && map[x, y - 1, z] != 0)
                        Task.Run(() => BFSMapUpdate(x, y - 1, z, ignoreSide));

                }
                catch
                {
                    Console.WriteLine("outbound update");
                }
            }
            if (!(ignoreSide == 3) && y + 1 < chunkHeight)
            {
                try
                {
                    if (!mapIsSearched[x, y + 1, z] && map[x, y + 1, z] != 0)
                        Task.Run(() => BFSMapUpdate(x, y + 1, z, ignoreSide));
                }
                catch
                {
                    Console.WriteLine("outbound update");
                }
            }
            if (!(ignoreSide == 4) && z - 1 >= 0)
            {
                try
                {
                    if (!mapIsSearched[x, y, z - 1] && map[x, y, z - 1] != 0)
                        Task.Run(() => BFSMapUpdate(x, y, z - 1, ignoreSide));
                }
                catch { Console.WriteLine("outbound update"); }
            }
            else if (z - 1 < 0)
            {
                if (backChunk != null)
                {
                    backChunk.BFSInit(x, y, chunkWidth - 1, ignoreSide, updateCount);
                }
            }
            if (!(ignoreSide == 5) && z + 1 < chunkWidth)
            {
                try
                {
                    if (!mapIsSearched[x, y, z + 1] && map[x, y, z + 1] != 0)
                        Task.Run(() => BFSMapUpdate(x, y, z + 1, ignoreSide));
                }
                catch
                {
                    Console.WriteLine("outbound update");
                }
            }
            else if (z + 1 >= chunkWidth)
            {
                if (frontChunk != null)
                {
                    frontChunk.BFSInit(x, y, 0, ignoreSide, updateCount);
                }
            }

        }*/

        public void SaveSingleChunk()
        {

            if (!isModifiedInGame)
            {

                return;
            }
            if (VoxelWorld.currentWorld.chunkDataReadFromDisk.ContainsKey(chunkPos))
            {
                VoxelWorld.currentWorld.chunkDataReadFromDisk.Remove(chunkPos);
                BlockData[,,] worldDataMap = map;
                ChunkData wd = new ChunkData(chunkPos);
                wd.map = worldDataMap;

                VoxelWorld.currentWorld.chunkDataReadFromDisk.Add(chunkPos, wd);
            }
            else
            {
                BlockData[,,] worldDataMap = map;
                ChunkData wd = new ChunkData(chunkPos);
                wd.map = worldDataMap;

                VoxelWorld.currentWorld.chunkDataReadFromDisk.Add(chunkPos, wd);
            }
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


        public object renderLock = new object();
        //  public SemaphoreSlim semaphore = new SemaphoreSlim(1);
        public List<Vector3> lightPoints = new List<Vector3>();
        public int usedByOthersCount = 0;
        public bool isUnused { get; set; } = false;
        public float unusedSeconds = 0f;

        public bool isAccurateHeightMapGenerated = false;
        public void InitMap(Vector2Int chunkPos,bool generateRenderBuffers=true)
        {

            //   semaphore.Wait();

            //     Thread.Sleep(400);
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
            frontLeftChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x - chunkWidth, chunkPos.y + chunkWidth));
            frontRightChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x + chunkWidth, chunkPos.y + chunkWidth));
            backLeftChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x - chunkWidth, chunkPos.y - chunkWidth));
            backRightChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x + chunkWidth, chunkPos.y - chunkWidth));
            backChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x, chunkPos.y - chunkWidth));
            frontChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x, chunkPos.y + chunkWidth));
            leftChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x - chunkWidth, chunkPos.y));

            rightChunk = ChunkHelper.GetChunk(new Vector2Int(chunkPos.x + chunkWidth, chunkPos.y));
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

            }
            if (isMapGenCompleted == true)
            {

                this.chunkBounds = this.CalculateBounds();
                GenerateHeightMap();
                    GenerateMesh(verticesOpq, verticesNS, verticesWT, indicesOpq, indicesNS, indicesWT,generateRenderBuffers);
                ReleaseChunkUsage();

                //     semaphore.Release();

                return;
            }
            if (VoxelWorld.currentWorld.chunkDataReadFromDisk.ContainsKey(chunkPos))
            {
                isChunkDataSavedInDisk = true;
                map = (BlockData[,,])VoxelWorld.currentWorld.chunkDataReadFromDisk[chunkPos].map.Clone();
                this.chunkBounds = this.CalculateBounds();
                GenerateHeightMap();
                    GenerateMesh(verticesOpq, verticesNS, verticesWT, indicesOpq, indicesNS, indicesWT, generateRenderBuffers);
                isMapGenCompleted = true;
                ReleaseChunkUsage();

                //      semaphore.Release();

                return;
            }




            FreshGenMap(chunkPos);
            GenerateHeightMap();
                isMapGenCompleted = true;
            GenerateMesh(verticesOpq, verticesNS, verticesWT, indicesOpq, indicesNS, indicesWT);
            ReleaseChunkUsage();

            //    semaphore.Release();

            void ReleaseChunkUsage()
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
            }

            void GenerateHeightMap()
            {
              
                    thisAccurateHeightMap = new int[chunkWidth, chunkWidth];
                for (int x = 0; x < chunkWidth; x++)
                {
                    for (int z = 0; z < chunkWidth; z++)
                    {
                        thisAccurateHeightMap[x, z] = ChunkHelper.GetSingleChunkLandingPoint(this, x, z);
                    }
                }
                isAccurateHeightMapGenerated = true;
                }
            void FreshGenMap(Vector2Int pos)
            {

                map = new BlockData[chunkWidth, chunkHeight, chunkWidth];
                if (VoxelWorld.currentWorld.worldGenType == 0)
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

                    int[,] chunkBiomeMap = GenerateChunkBiomeMap(pos);

                    int interMultiplier = 8;
                    int[,] chunkBiomeMapInterpolated = new int[(chunkWidth / 8 + 2) * interMultiplier, (chunkWidth / 8 + 2) * interMultiplier];
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
                    for (int i = 0; i < chunkWidth; i++)
                    {
                        for (int j = 0; j < chunkWidth; j++)
                        {
                            //  float noiseValue=200f*Mathf.PerlinNoise(pos.x*0.01f+i*0.01f,pos.y*0.01f+j*0.01f);
                            float noiseValue = thisHeightMap[i + 8, j + 8];
                            for (int k = 0; k < chunkHeight; k++)
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


                    for (int i = 0; i < chunkWidth; i++)
                    {
                        for (int j = 0; j < chunkWidth; j++)
                        {

                            for (int k = chunkHeight - 1; k >= 0; k--)
                            {

                                if (map[i, k, j] == 3 && k >= chunkSeaLevel - 1)
                                {
                                    map[i, k, j] = 4;
                                    break;
                                }


                                if (k < chunkSeaLevel && map[i, k, j] == 0)
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
                            if (point.y < chunkSeaLevel)
                            {
                                continue;
                            }
                            Vector3Int pointTransformed2 = point - new Vector3Int(8, 0, 8);
                            if (chunkBiomeMapInterpolated[x, z] == 3)
                            {
                                continue;
                            }
                            if (pointTransformed2.x >= 0 && pointTransformed2.x < chunkWidth && pointTransformed2.y >= 0 && pointTransformed2.y < chunkHeight && pointTransformed2.z >= 0 && pointTransformed2.z < chunkWidth)
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
                        if (pointTransformed1.x >= 0 && pointTransformed1.x < chunkWidth && pointTransformed1.y >= 0 && pointTransformed1.y < chunkHeight && pointTransformed1.z >= 0 && pointTransformed1.z < chunkWidth)
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
                        if (pointTransformed.x >= 0 && pointTransformed.x < chunkWidth && pointTransformed.y >= 0 && pointTransformed.y < chunkHeight && pointTransformed.z >= 0 && pointTransformed.z < chunkWidth)
                        {
                            map[pointTransformed.x, pointTransformed.y, pointTransformed.z] = 7;
                        }
                    }

                    for (int i = 0; i < chunkWidth; i++)
                    {
                        for (int j = 0; j < chunkWidth; j++)
                        {

                            for (int k = chunkHeight - 1; k >= 0; k--)
                            {
                                if (k > chunkSeaLevel && map[i, k, j] == 0 && map[i, k - 1, j] == 4 && RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(i, k, j)) > 80)
                                {
                                    map[i, k, j] = 101;
                                }
                            }
                        }
                    }
                    /*   for (int i = 0; i < chunkWidth; i++)
                       {
                           for (int j = 0; j < chunkWidth; j++)
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
                                                           if (x + i < 0 || x + i >= chunkWidth || z + j < 0 || z + j >= chunkWidth)
                                                           {



                                                               if (x + i < 0)
                                                               {
                                                                   if (z + j >= 0 && z + j < chunkWidth)
                                                                   {
                                                                       if (leftChunk != null&&leftChunk.disposed==false)
                                                                       {

                                                                           leftChunk.additiveMap[chunkWidth + (x + i), y + k, z + j] = 9;

                                                                           isLeftChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(leftChunk,0);
                                                                           //         leftChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                                   else if (z + j < 0)
                                                                   {
                                                                       if (backLeftChunk != null && backLeftChunk.disposed == false)
                                                                       {
                                                                           backLeftChunk.additiveMap[chunkWidth + (x + i), y + k, chunkWidth - 1 + (z + j)] = 9;

                                                                           isBackLeftChunkUpdated = true;

                                                                           //    WorldManage r.chunkLoadingQueue.UpdatePriority(backLeftChunk,0);
                                                                           //               backLeftChunk.isChunkMapUpdated=true;
                                                                       }

                                                                   }
                                                                   else if (z + j >= chunkWidth)
                                                                   {
                                                                       if (frontLeftChunk != null && frontLeftChunk.disposed == false)
                                                                       {
                                                                           frontLeftChunk.additiveMap[chunkWidth + (x + i), y + k, (z + j) - chunkWidth] = 9;

                                                                           isFrontLeftChunkUpdated = true;

                                                                           //     WorldManager.chunkLoadingQueue.UpdatePriority(frontLeftChunk,0);
                                                                           //                 frontLeftChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }

                                                               }
                                                               else
                                                               if (x + i >= chunkWidth)
                                                               {
                                                                   if (z + j >= 0 && z + j < chunkWidth)
                                                                   {
                                                                       if (rightChunk != null && rightChunk.disposed == false)
                                                                       {

                                                                           rightChunk.additiveMap[(x + i) - chunkWidth, y + k, z + j] = 9;

                                                                           isRightChunkUpdated = true;

                                                                           //   WorldManager.chunkLoadingQueue.UpdatePriority(rightChunk,0);
                                                                           //      rightChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                                   else if (z + j < 0)
                                                                   {
                                                                       if (backRightChunk != null)
                                                                       {
                                                                           backRightChunk.additiveMap[(x + i) - chunkWidth, y + k, chunkWidth + (z + j)] = 9;

                                                                           isBackRightChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(backRightChunk,0);
                                                                           //         backRightChunk.isChunkMapUpdated=true;
                                                                       }

                                                                   }
                                                                   else if (z + j >= chunkWidth)
                                                                   {
                                                                       if (frontRightChunk != null && frontRightChunk.disposed == false)
                                                                       {
                                                                           frontRightChunk.additiveMap[(x + i) - chunkWidth, y + k, (z + j) - chunkWidth] = 9;

                                                                           isFrontRightChunkUpdated = true;

                                                                           //     WorldManager.chunkLoadingQueue.UpdatePriority(frontRightChunk,0);
                                                                           //          frontRightChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                               }
                                                               else
                                                               if (z + j < 0)
                                                               {

                                                                   if (x + i >= 0 && x + i < chunkWidth)
                                                                   {
                                                                       if (backChunk != null)
                                                                       {

                                                                           backChunk.additiveMap[x + i, y + k, chunkWidth + (z + j)] = 9;

                                                                           isBackChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(backChunk,0);
                                                                           //         backChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                                   else if (x + i < 0)
                                                                   {
                                                                       if (backLeftChunk != null && backLeftChunk.disposed == false)
                                                                       {
                                                                           backLeftChunk.additiveMap[chunkWidth + (x + i), y + k, chunkWidth - 1 + (z + j)] = 9;

                                                                           isBackLeftChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(backLeftChunk,0);
                                                                           //            backLeftChunk.isChunkMapUpdated=true;
                                                                       }

                                                                   }
                                                                   else if (x + i >= chunkWidth)
                                                                   {
                                                                       if (backRightChunk != null)
                                                                       {
                                                                           backRightChunk.additiveMap[(x + i) - chunkWidth, y + k, chunkWidth - 1 + (z + j)] = 9;

                                                                           isBackRightChunkUpdated = true;

                                                                           //       WorldManager.chunkLoadingQueue.UpdatePriority(backRightChunk,0);
                                                                           //      backRightChunk.isChunkMapUpdated=true;    
                                                                       }
                                                                   }

                                                               }
                                                               else
                                                               if (z + j >= chunkWidth)
                                                               {

                                                                   if (x + i >= 0 && x + i < chunkWidth)
                                                                   {
                                                                       if (frontChunk != null && frontChunk.disposed == false)
                                                                       {

                                                                           frontChunk.additiveMap[x + i, y + k, (z + j) - chunkWidth] = 9;

                                                                           isFrontChunkUpdated = true;

                                                                           //    WorldManager.chunkLoadingQueue.UpdatePriority(frontChunk,0);
                                                                           //   frontChunk.isChunkMapUpdated=true;
                                                                       }
                                                                   }
                                                                   else if (x + i < 0)
                                                                   {
                                                                       if (frontLeftChunk != null && frontLeftChunk.disposed == false)
                                                                       {
                                                                           frontLeftChunk.additiveMap[chunkWidth + (x + i), y + k, (z + j) - chunkWidth] = 9;

                                                                           isBackLeftChunkUpdated = true;

                                                                           //        WorldManager.chunkLoadingQueue.UpdatePriority(frontLeftChunk,0);
                                                                           //    frontLeftChunk.isChunkMapUpdated=true;
                                                                       }

                                                                   }
                                                                   else if (x + i >= chunkWidth)
                                                                   {
                                                                       if (frontRightChunk != null && frontRightChunk.disposed == false)
                                                                       {
                                                                           frontRightChunk.additiveMap[(x + i) - chunkWidth, y + k, (z + j) - chunkWidth] = 9;

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

                                               if (i + 1 < chunkWidth)
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
                                                       leftChunk.additiveMap[chunkWidth - 1, k + 5, j] = 9;
                                                       leftChunk.additiveMap[chunkWidth - 1, k + 6, j] = 9;

                                                       // leftChunk.isChunkMapUpdated=true;
                                                   }
                                               }
                                               if (j + 1 < chunkWidth)
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
                                                       backChunk.additiveMap[i, k + 5, chunkWidth - 1] = 9;
                                                       backChunk.additiveMap[i, k + 6, chunkWidth - 1] = 9;

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
                    for (int i = 0; i < chunkWidth; i++)
                    {
                        for (int j = 0; j < chunkWidth; j++)
                        {
                            for (int k = 0; k < chunkHeight / 4; k++)
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
                    for (int i = 0; i < chunkWidth; i++)
                    {
                        for (int j = 0; j < chunkWidth; j++)
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
                else if (VoxelWorld.currentWorld.worldGenType == 1)
                {
                    for (int i = 0; i < chunkWidth; i++)
                    {
                        for (int j = 0; j < chunkWidth; j++)
                        {
                            //  float noiseValue=200f*Mathf.PerlinNoise(pos.x*0.01f+i*0.01f,pos.z*0.01f+j*0.01f);
                            for (int k = 0; k < chunkHeight / 4; k++)
                            {

                                map[i, k, j] = 1;

                            }
                        }
                    }
                }
                else if (VoxelWorld.currentWorld.worldGenType == 2)
                {
                    for (int i = 0; i < chunkWidth; i++)
                    {
                        for (int j = 0; j < chunkWidth; j++)
                        {
                            //  float noiseValue=200f*Mathf.PerlinNoise(pos.x*0.01f+i*0.01f,pos.z*0.01f+j*0.01f);
                            for (int k = 0; k < chunkHeight / 2; k++)
                            {
                                float yLerpValue = MathHelper.Lerp(-1, 1, (MathF.Abs(k - chunkSeaLevel)) / 40f);
                                float xzLerpValue = MathHelper.Lerp(-1, 1, (new Vector3(chunkPos.x + i, 0, chunkPos.y + j).Length() / 384f));
                                float xyzLerpValue = MathHelper.Max(xzLerpValue, yLerpValue);
                                if (frequentNoiseGenerator.GetSimplex(i + chunkPos.x, k, j + chunkPos.y) > xyzLerpValue)
                                {
                                    map[i, k, j] = 12;
                                }


                            }
                        }
                    }
                }
                this.chunkBounds = this.CalculateBounds();
                    generatedStructure.Clear();
                for(int i=0;i<VoxelWorld.currentWorld.worldStructures.Count;i++)
                {
                    for (int x = -1; x < 2;x++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            CalculateChunkStructurePoint(this, i, VoxelWorld.currentWorld.worldStructures[i].genParams, chunkPos+new Vector2Int(x*Chunk.chunkWidth, z * Chunk.chunkWidth));
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
                    BlockData data = ChunkHelper.GetBlockData(testPoint+new Vector3Int(0,-1,0));
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
                    ChunkHelper.FillBlocksSingleChunk(
                        VoxelWorld.currentWorld.worldStructures[item.Item1].data,
                        item.Item2 -
                        new Vector3Int(
                            VoxelWorld.currentWorld.worldStructures[item.Item1].data.GetLength(0) / 2, 0,
                            VoxelWorld.currentWorld.worldStructures[item.Item1].data.GetLength(2) / 2),
                        this, BlockFillMode.ReplaceCustomTypes,false,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,100,101);
                    ChunkHelper.SetBlockWithoutUpdate(testPoint, (short)9);
                    }
            
                this.chunkBounds = this.CalculateBounds();
                    isMapGenCompleted = true;
            }
        }

        public bool isReadyToRender { get; set; } = false;
        public List<VertexPositionNormalTangentTexture> verticesOpq;//= new List<VertexPositionNormalTexture>();
        public List<ushort> indicesOpq;
        public List<VertexPositionNormalTangentTexture> verticesOpqLOD;//= new List<VertexPositionNormalTexture>();
        public List<ushort> indicesOpqLOD;
        public List<VertexPositionNormalTangentTexture> verticesNS; //= new List<VertexPositionNormalTexture>();
        public List<ushort> indicesNS;
        public List<VertexPositionNormalTangentTexture> verticesWT;// = new List<VertexPositionNormalTexture>();
        public List<ushort> indicesWT;
        public BoundingBox chunkBounds
        {
            get;
            set;
        }
        public bool isVertexBufferDirty = false;
        // void BuildMesh();
        // void BuildBlocks();
        public BoundingBox CalculateBounds()
        {
            return new BoundingBox(new Vector3(chunkPos.x, 0, chunkPos.y), new Vector3(chunkPos.x + chunkWidth, GetHighestPoint(), chunkPos.y + chunkWidth));
        }


        public List<(int,Vector3Int)> generatedStructure=new List<(int,Vector3Int)> (); 
       
        public static void CalculateChunkStructurePoint(Chunk c,int structureID,StructureGeneratingParam param,Vector2Int chunkPos)
        {
            
            switch (param.type)
            {
                    case StructureGeneratingType.Random:
                        if (RandomGenerator3D.GenerateIntFromVec3(new Vector3Int(chunkPos.x,0,
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
        
        public void GenerateMeshOpqLOD(List<VertexPositionNormalTangentTexture> verts, List<ushort> indices, ref VertexPositionNormalTangentTexture[] vertsArray, ref ushort[] indicesArray, ref VertexBuffer vb, ref IndexBuffer ib, int lodBlockSkipCount = 2)
        {

            //   System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //  sw.Start();

            //   int[] typeIDs = new int[lodBlockSkipCount * 1 * lodBlockSkipCount];
            //  int[] typeIDWeights = new int[lodBlockSkipCount * 1 * lodBlockSkipCount];


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
                            BlockMeshBuildingHelper.BuildSingleBlockLOD(lodBlockSkipCount,this,x,y,z,typeid,ref verts,ref indices);
                        /*     if (lodBlockSkipCount > 1)
                             {
                                 int indx = 0;
                                 for (int x1 = 0; x1 < lodBlockSkipCount; x1++)
                                 {

                                         for (int z1 = 0; z1 < lodBlockSkipCount; z1++)
                                         {


                                             typeIDs[indx] = (this.map[x + x1, y , z + z1]);

                                              if (blockIDWeightDic.ContainsKey((this.map[x + x1, y , z + z1])))
                                             {
                                                 typeIDWeights[indx] = (blockIDWeightDic[(this.map[x + x1, y, z + z1])]);
                                             }
                                             else
                                             {
                                                 typeIDWeights[indx] = 1;
                                             } ;
                                             indx++;
                                         }

                                 }

                                 int maxIndex = MaxIndex(typeIDWeights);
                                 typeid = typeIDs[maxIndex];

                             }*/

                     /*   if (typeid == 0) continue;
                        if (0 < typeid && typeid < 100)
                        {

                            //Left
                            if (CheckNeedBuildFace((blockCheckPos.x - lodBlockSkipCount), blockCheckPos.y, blockCheckPos.z, lodBlockSkipCount))
                                BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 1, 0) * 1, new Vector3(0, 0, 1) * lodBlockSkipCount, false, verts, 0, indices);
                            //Right
                            if (CheckNeedBuildFace((blockCheckPos.x + lodBlockSkipCount), blockCheckPos.y, blockCheckPos.z, lodBlockSkipCount))
                                BuildFace(typeid, new Vector3(x + lodBlockSkipCount, y, z), new Vector3(0, 1, 0) * 1, new Vector3(0, 0, 1) * lodBlockSkipCount, true, verts, 1, indices);

                            //Bottom
                            if (CheckNeedBuildFace(blockCheckPos.x, blockCheckPos.y - 1, blockCheckPos.z, lodBlockSkipCount))
                                BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 0, 1) * lodBlockSkipCount, new Vector3(1, 0, 0) * lodBlockSkipCount, false, verts, 2, indices);
                            //Top
                            if (CheckNeedBuildFace(blockCheckPos.x, blockCheckPos.y + 1, blockCheckPos.z, lodBlockSkipCount))
                                BuildFace(typeid, new Vector3(x, y + 1, z), new Vector3(0, 0, 1) * lodBlockSkipCount, new Vector3(1, 0, 0) * lodBlockSkipCount, true, verts, 3, indices);

                            //Back
                            if (CheckNeedBuildFace(blockCheckPos.x, blockCheckPos.y, (blockCheckPos.z - lodBlockSkipCount), lodBlockSkipCount))
                                BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 1, 0) * 1, new Vector3(1, 0, 0) * lodBlockSkipCount, true, verts, 4, indices);
                            //Front
                            if (CheckNeedBuildFace(blockCheckPos.x, blockCheckPos.y, (blockCheckPos.z + lodBlockSkipCount), lodBlockSkipCount))
                                BuildFace(typeid, new Vector3(x, y, z + lodBlockSkipCount), new Vector3(0, 1, 0) * 1, new Vector3(1, 0, 0) * lodBlockSkipCount, false, verts, 5, indices);





                        }*/
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

        public void GenerateMesh(List<VertexPositionNormalTangentTexture> OpqVerts, List<VertexPositionNormalTangentTexture> NSVerts, List<VertexPositionNormalTangentTexture> WTVerts, List<ushort> OpqIndices, List<ushort> NSIndices, List<ushort> WTIndices,bool generateRenderingBuffers=true)
        {
            lightPoints = new List<Vector3>();

            for (int x = 0; x < chunkWidth; x++)
            {

                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int z = 0; z < chunkWidth; z++)
                    {//new int[chunkwidth,chunkheiight,chunkwidth]
                     //     BuildBlock(x, y, z, verts, uvs, tris, vertsNS, uvsNS, trisNS);

                        if (map == null)
                        {
                            return;
                        }
                        BlockMeshBuildingHelper.BuildSingleBlock(this, x, y, z, this.map[x, y, z], ref OpqVerts, ref NSVerts, ref WTVerts, ref OpqIndices, ref NSIndices, ref WTIndices);
                        continue;
                      /*  int typeid = this.map[x, y, z];
                        //    Debug.WriteLine(typeid);
                        if (typeid == 0) continue;



                        if (typeid == 0) continue;
                        if (0 < typeid && typeid < 100)
                        {
                            if (typeid == 9)
                            {
                                //Left
                                if (CheckNeedBuildFace(x - 1, y, z, false) && GetChunkBlockType(x - 1, y, z) != 9)
                                    BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(0, 0, 1), false, OpqVerts, 0, OpqIndices);
                                //Right
                                if (CheckNeedBuildFace(x + 1, y, z, false) && GetChunkBlockType(x + 1, y, z) != 9)
                                    BuildFace(typeid, new Vector3(x + 1, y, z), new Vector3(0, 1, 0), new Vector3(0, 0, 1), true, OpqVerts, 1, OpqIndices);

                                //Bottom
                                if (CheckNeedBuildFace(x, y - 1, z, false) && GetChunkBlockType(x, y - 1, z) != 9)
                                    BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), false, OpqVerts, 2, OpqIndices);
                                //Top
                                if (CheckNeedBuildFace(x, y + 1, z, false) && GetChunkBlockType(x, y + 1, z) != 9)
                                    BuildFace(typeid, new Vector3(x, y + 1, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), true, OpqVerts, 3, OpqIndices);

                                //Back
                                if (CheckNeedBuildFace(x, y, z - 1, false) && GetChunkBlockType(x, y, z - 1) != 9)
                                    BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(1, 0, 0), true, OpqVerts, 4, OpqIndices);
                                //Front
                                if (CheckNeedBuildFace(x, y, z + 1, false) && GetChunkBlockType(x, y, z + 1) != 9)
                                    BuildFace(typeid, new Vector3(x, y, z + 1), new Vector3(0, 1, 0), new Vector3(1, 0, 0), false, OpqVerts, 5, OpqIndices);

                            }
                            else
                            {
                                if (CheckNeedBuildFace(x - 1, y, z, false))
                                    BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(0, 0, 1), false, OpqVerts, 0, OpqIndices);
                                //Right
                                if (CheckNeedBuildFace(x + 1, y, z, false))
                                    BuildFace(typeid, new Vector3(x + 1, y, z), new Vector3(0, 1, 0), new Vector3(0, 0, 1), true, OpqVerts, 1, OpqIndices);

                                //Bottom
                                if (CheckNeedBuildFace(x, y - 1, z, false))
                                    BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), false, OpqVerts, 2, OpqIndices);
                                //Top
                                if (CheckNeedBuildFace(x, y + 1, z, false))
                                    BuildFace(typeid, new Vector3(x, y + 1, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), true, OpqVerts, 3, OpqIndices);

                                //Back
                                if (CheckNeedBuildFace(x, y, z - 1, false))
                                    BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(1, 0, 0), true, OpqVerts, 4, OpqIndices);
                                //Front
                                if (CheckNeedBuildFace(x, y, z + 1, false))
                                    BuildFace(typeid, new Vector3(x, y, z + 1), new Vector3(0, 1, 0), new Vector3(1, 0, 0), false, OpqVerts, 5, OpqIndices);

                            }



                        }
                        else if (100 <= typeid && typeid < 200)
                        {

                            if (typeid == 100)
                            {



                                //water
                                //left
                                if (CheckNeedBuildFace(x - 1, y, z, true) && GetChunkBlockType(x - 1, y, z) != 100)
                                {
                                    if (GetChunkBlockType(x, y + 1, z) != 100)
                                    {
                                        BuildFace(typeid, new Vector3(x, y, z), new Vector3(0f, 0.8f, 0f), new Vector3(0, 0, 1), false, WTVerts, 0, WTIndices);




                                    }
                                    else
                                    {
                                        BuildFace(typeid, new Vector3(x, y, z), new Vector3(0f, 1f, 0f), new Vector3(0, 0, 1), false, WTVerts, 0, WTIndices);






                                    }

                                }

                                //Right
                                if (CheckNeedBuildFace(x + 1, y, z, true) && GetChunkBlockType(x + 1, y, z) != 100)
                                {
                                    if (GetChunkBlockType(x, y + 1, z) != 100)
                                    {
                                        BuildFace(typeid, new Vector3(x + 1, y, z), new Vector3(0f, 0.8f, 0f), new Vector3(0, 0, 1), true, WTVerts, 1, WTIndices);



                                    }
                                    else
                                    {
                                        BuildFace(typeid, new Vector3(x + 1, y, z), new Vector3(0f, 1f, 0f), new Vector3(0, 0, 1), true, WTVerts, 1, WTIndices);



                                    }

                                }



                                //Bottom
                                if (CheckNeedBuildFace(x, y - 1, z, true) && GetChunkBlockType(x, y - 1, z) != 100)
                                {
                                    BuildFace(typeid, new Vector3(x, y, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), false, WTVerts, 2, WTIndices);




                                }

                                //Top
                                if (CheckNeedBuildFace(x, y + 1, z, true) && GetChunkBlockType(x, y + 1, z) != 100)
                                {
                                    BuildFace(typeid, new Vector3(x, y + 0.8f, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), true, WTVerts, 3, WTIndices);




                                }




                                //Back
                                if (CheckNeedBuildFace(x, y, z - 1, true) && GetChunkBlockType(x, y, z - 1) != 100)
                                {
                                    if (GetChunkBlockType(x, y + 1, z) != 100)
                                    {
                                        BuildFace(typeid, new Vector3(x, y, z), new Vector3(0f, 0.8f, 0f), new Vector3(1, 0, 0), true, WTVerts, 4, WTIndices);




                                    }
                                    else
                                    {
                                        BuildFace(typeid, new Vector3(x, y, z), new Vector3(0f, 1f, 0f), new Vector3(1, 0, 0), true, WTVerts, 4, WTIndices);







                                    }

                                }


                                //Front
                                if (CheckNeedBuildFace(x, y, z + 1, true) && GetChunkBlockType(x, y, z + 1) != 100)
                                {
                                    if (GetChunkBlockType(x, y + 1, z) != 100)
                                    {
                                        BuildFace(typeid, new Vector3(x, y, z + 1), new Vector3(0f, 0.8f, 0f), new Vector3(1, 0, 0), false, WTVerts, 5, WTIndices);


                                    }
                                    else
                                    {
                                        BuildFace(typeid, new Vector3(x, y, z + 1), new Vector3(0f, 1f, 0f), new Vector3(1, 0, 0), false, WTVerts, 4, WTIndices);

                                    }

                                }
                            }

                            if (typeid >= 101 && typeid < 150)
                            {

                                if (typeid == 102)
                                {
                                    //torch

                                    BuildFaceComplex(new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), new Vector2(0.0078125f, 0.0390625f), new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f), false, NSVerts, 0, NSIndices);
                                    //Right

                                    BuildFaceComplex(new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0.125f, 0f, 0f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), new Vector2(0.0078125f, 0.0390625f), new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f), true, NSVerts, 0, NSIndices);

                                    //Bottom

                                    BuildFaceComplex(new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), new Vector2(0.0078125f, 0.0078125f), new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f), false, NSVerts, 0, NSIndices);
                                    //Top

                                    BuildFaceComplex(new Vector3(x, y, z) + new Vector3(0.4375f, 0.625f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), new Vector2(0.0078125f, 0.0078125f), new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0.03125f) + new Vector2(0.0625f, 0f), true, NSVerts, 0, NSIndices);

                                    //Back

                                    BuildFaceComplex(new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), new Vector2(0.0078125f, 0.0390625f), new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f), true, NSVerts, 0, NSIndices);
                                    //Front

                                    BuildFaceComplex(new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0f, 0f, 0.125f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), new Vector2(0.0078125f, 0.0390625f), new Vector2(0.0625f, 0.0625f) + new Vector2(0.02734375f, 0f) + new Vector2(0.0625f, 0f), false, NSVerts, 0, NSIndices);

                                    lightPoints.Add(new Vector3(x, y, z) + new Vector3(0.5f, 0.725f, 0.5f) + new Vector3(chunkPos.x, 0, chunkPos.y));

                                }
                                else
                                {
                                    Vector3 randomCrossModelOffset = new Vector3(0f, 0f, 0f);
                                    BuildFace(typeid, new Vector3(x, y, z) + randomCrossModelOffset, new Vector3(0f, 1f, 0f) + randomCrossModelOffset, new Vector3(1f, 0f, 1f) + randomCrossModelOffset, false, NSVerts, 0, NSIndices);
                                    BuildFace(typeid, new Vector3(x, y, z) + randomCrossModelOffset, new Vector3(0f, 1f, 0f) + randomCrossModelOffset, new Vector3(1f, 0f, 1f) + randomCrossModelOffset, true, NSVerts, 0, NSIndices);






                                    BuildFace(typeid, new Vector3(x, y, z + 1f) + randomCrossModelOffset, new Vector3(0f, 1f, 0f) + randomCrossModelOffset, new Vector3(1f, 0f, -1f) + randomCrossModelOffset, false, NSVerts, 0, NSIndices);
                                    BuildFace(typeid, new Vector3(x, y, z + 1f) + randomCrossModelOffset, new Vector3(0f, 1f, 0f) + randomCrossModelOffset, new Vector3(1f, 0f, -1f) + randomCrossModelOffset, true, NSVerts, 0, NSIndices);




                                }


                            }


                        }*/

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

             
                //   if (VBOpq == null)
                //   {
                /*       VBOpq?.Dispose();

                       if (verticesOpqArray.Length > 0)
                       {
                           VBOpq = new VertexBuffer(this.device, typeof(VertexPositionNormalTangentTexture), verticesOpqArray.Length + 1, BufferUsage.WriteOnly);
                           //   }

                           VBOpq.SetData(verticesOpqArray);
                       }

                       //  if(IBOpq == null)
                       //  {
                       IBOpq?.Dispose();
                       if (indicesOpqArray.Length > 0)
                       {
                           IBOpq = new DynamicIndexBuffer(this.device, IndexElementSize.SixteenBits, indicesOpqArray.Length, BufferUsage.WriteOnly);
                           IBOpq.SetData(indicesOpqArray);
                       }




                       VBWT?.Dispose();

                       if (verticesWTArray.Length > 0)
                       {
                           VBWT = new VertexBuffer(this.device, typeof(VertexPositionNormalTangentTexture), verticesWTArray.Length + 1, BufferUsage.WriteOnly);
                           VBWT.SetData(verticesWTArray);
                       }


                       IBWT?.Dispose();
                       if (indicesWTArray.Length > 0)
                       {
                           IBWT = new DynamicIndexBuffer(this.device, IndexElementSize.SixteenBits, indicesWTArray.Length, BufferUsage.WriteOnly);
                           IBWT.SetData(indicesWTArray);
                       }





                       VBNS?.Dispose();

                       if (verticesNSArray.Length > 0)
                       {
                           VBNS = new VertexBuffer(this.device, typeof(VertexPositionNormalTangentTexture), verticesNSArray.Length + 1, BufferUsage.WriteOnly);
                           VBNS.SetData(verticesNSArray);
                       }

                       IBNS?.Dispose();


                       if (indicesNSArray.Length > 0)
                       {
                           IBNS = new DynamicIndexBuffer(this.device, IndexElementSize.SixteenBits, indicesNSArray.Length, BufferUsage.WriteOnly);
                           IBNS.SetData(indicesNSArray);
                       }*/

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
                    isOpqBuffersValid=true;
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
                    isWTBuffersValid=true;
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

            [Obsolete]
            static void BuildFace(int typeid, Vector3 corner, Vector3 up, Vector3 right, bool reversed, List<VertexPositionNormalTangentTexture> verts, int side, List<ushort> indices)
        {
            VertexPositionNormalTangentTexture vert00 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert01 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert11 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert10 = new VertexPositionNormalTangentTexture();
            short index = (short)verts.Count;
            vert00.Position = corner;
            vert01.Position = corner + up;
            vert11.Position = corner + up + right;
            vert10.Position = corner + right;
            //    verts.Add(vert0);
            //    verts.Add(vert1);
            //    verts.Add(vert2);
            //     verts.Add(vert3);

            Vector2 uvWidth = new Vector2(0.0625f, 0.0625f);
            Vector2 uvCorner = new Vector2(0.00f, 0.00f);

            //uvCorner.x = (float)(typeid - 1) / 16;
            if (blockInfo.ContainsKey(typeid))
            {
                uvCorner = blockInfo[typeid][side];
            }
            vert00.TextureCoordinate = uvCorner;
            vert01.TextureCoordinate = new Vector2(uvCorner.X, uvCorner.Y + uvWidth.Y);
            vert11.TextureCoordinate = new Vector2(uvCorner.X + uvWidth.X, uvCorner.Y + uvWidth.Y);
            vert10.TextureCoordinate = new Vector2(uvCorner.X + uvWidth.X, uvCorner.Y);
            //    uvs.Add(uvCorner);
            //    uvs.Add(new Vector2(uvCorner.x, uvCorner.y + uvWidth.y));
            //    uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y + uvWidth.y));
            //    uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y));


            if (!reversed)
            {


                Vector3 normal = -Vector3.Cross(up, right);
                Vector3 tangent = -right;
                vert00.Normal = normal;
                vert01.Normal = normal;
                vert11.Normal = normal;
                vert10.Normal = normal;

                vert00.Tangent = tangent;
                vert01.Tangent = tangent;
                vert11.Tangent = tangent;
                vert10.Tangent = tangent;
                /*  verts.Add(vert00);
                   verts.Add(vert01);
                   verts.Add(vert11);
                   verts.Add(vert11);
                   verts.Add(vert10);
                   verts.Add(vert00);*/
                indices.Add((ushort)(index + 0));
                indices.Add((ushort)(index + 1));
                indices.Add((ushort)(index + 2));
                indices.Add((ushort)(index + 2));
                indices.Add((ushort)(index + 3));
                indices.Add((ushort)(index + 0));
                //    tris.Add(index + 2);

                //   tris.Add(index + 0);

            }
            else
            {

                Vector3 normal = Vector3.Cross(up, right);
                Vector3 tangent = right;

                vert00.Normal = normal;
                vert01.Normal = normal;
                vert11.Normal = normal;
                vert10.Normal = normal;

                vert00.Tangent = tangent;
                vert01.Tangent = tangent;
                vert11.Tangent = tangent;
                vert10.Tangent = tangent;
                /*    verts.Add(vert01);
                    verts.Add(vert00);
                    verts.Add(vert11);
                    verts.Add(vert10);
                    verts.Add(vert11);
                    verts.Add(vert00);*/
                //     indices.Add()
                indices.Add((ushort)(index + 1));
                indices.Add((ushort)(index + 0));
                indices.Add((ushort)(index + 2));
                indices.Add((ushort)(index + 3));
                indices.Add((ushort)(index + 2));
                indices.Add((ushort)(index + 0));

            }

            verts.Add(vert00);
            verts.Add(vert01);
            verts.Add(vert11);
            verts.Add(vert10);



        }
            [Obsolete]
            static void BuildFaceComplex(Vector3 corner, Vector3 up, Vector3 right, Vector2 uvWidth, Vector2 uvCorner, bool reversed, List<VertexPositionNormalTangentTexture> verts, int side, List<ushort> indices)
        {
            VertexPositionNormalTangentTexture vert00 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert01 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert11 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert10 = new VertexPositionNormalTangentTexture();
            short index = (short)verts.Count;
            vert00.Position = corner;
            vert01.Position = corner + up;
            vert11.Position = corner + up + right;
            vert10.Position = corner + right;
            //    verts.Add(vert0);
            //    verts.Add(vert1);
            //    verts.Add(vert2);
            //     verts.Add(vert3);



            //uvCorner.x = (float)(typeid - 1) / 16;

            vert00.TextureCoordinate = uvCorner;
            vert01.TextureCoordinate = new Vector2(uvCorner.X, uvCorner.Y + uvWidth.Y);
            vert11.TextureCoordinate = new Vector2(uvCorner.X + uvWidth.X, uvCorner.Y + uvWidth.Y);
            vert10.TextureCoordinate = new Vector2(uvCorner.X + uvWidth.X, uvCorner.Y);
            //    uvs.Add(uvCorner);
            //    uvs.Add(new Vector2(uvCorner.x, uvCorner.y + uvWidth.y));
            //    uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y + uvWidth.y));
            //    uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y));


            if (!reversed)
            {


                Vector3 normal = -Vector3.Cross(up, right);
                Vector3 tangent = -right;
                vert00.Normal = normal;
                vert01.Normal = normal;
                vert11.Normal = normal;
                vert10.Normal = normal;

                vert00.Tangent = tangent;
                vert01.Tangent = tangent;
                vert11.Tangent = tangent;
                vert10.Tangent = tangent;
                /*  verts.Add(vert00);
                   verts.Add(vert01);
                   verts.Add(vert11);
                   verts.Add(vert11);
                   verts.Add(vert10);
                   verts.Add(vert00);*/
                indices.Add((ushort)(index + 0));
                indices.Add((ushort)(index + 1));
                indices.Add((ushort)(index + 2));
                indices.Add((ushort)(index + 2));
                indices.Add((ushort)(index + 3));
                indices.Add((ushort)(index + 0));
                //    tris.Add(index + 2);

                //   tris.Add(index + 0);

            }
            else
            {

                Vector3 normal = Vector3.Cross(up, right);
                Vector3 tangent = right;

                vert00.Normal = normal;
                vert01.Normal = normal;
                vert11.Normal = normal;
                vert10.Normal = normal;

                vert00.Tangent = tangent;
                vert01.Tangent = tangent;
                vert11.Tangent = tangent;
                vert10.Tangent = tangent;
                /*    verts.Add(vert01);
                    verts.Add(vert00);
                    verts.Add(vert11);
                    verts.Add(vert10);
                    verts.Add(vert11);
                    verts.Add(vert00);*/
                //     indices.Add()
                indices.Add((ushort)(index + 1));
                indices.Add((ushort)(index + 0));
                indices.Add((ushort)(index + 2));
                indices.Add((ushort)(index + 3));
                indices.Add((ushort)(index + 2));
                indices.Add((ushort)(index + 0));

            }

            verts.Add(vert00);
            verts.Add(vert01);
            verts.Add(vert11);
            verts.Add(vert10);



        }
            [Obsolete]
            bool CheckNeedBuildFace(int x, int y, int z, int LODSkipBlockCount)
        {
            if (y < 0) return false;
            var type = GetChunkBlockTypeLOD(x, y, z, LODSkipBlockCount);
            bool isNonSolid = false;
            if (type != 0 && blockInfosNew[type].shape != BlockShape.Solid)
            {
                isNonSolid = true;
            }
            switch (isNonSolid)
            {
                case true: return true;
                case false: break;
            }
            switch (type)
            {


                case 0:
                    return true;
                case 9:
                    return !(LODSkipBlockCount > 1);
                default:
                    return false;
            }
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
            }
        }

            public bool CheckNeedBuildFace(int x, int y, int z,BlockData curBlock)
            {
                // return true;
                if (y < 0) return false;
                var type = GetChunkBlockType(x, y, z);
                bool isNonSolid = false;
                if(type==0&&curBlock.blockID!=0)
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
                BlockShape shape = blockInfosNew[curBlock.blockID].shape;
                BlockShape shape1 = blockInfosNew[type].shape;
                if (shape == BlockShape.Solid)
                {
                    if(shape1 == BlockShape.Solid)
                    {
                        return false;
                    }else
                    if(shape1== BlockShape.Water)
                    {
                        return true;
                    }
                    else
                    {
                        return true;
                    }

                }
                if(shape==BlockShape.Water) { 
                    if(shape1==BlockShape.Solid)
                    {
                        return false;
                    }else if (shape1 == BlockShape.Water)
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
                if (shape1==BlockShape.Water && shape == BlockShape.Water)
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


            public bool CheckNeedBuildFace(int x, int y, int z, BlockData curBlock,int LODSkipBlockCount)
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
                BlockShape shape = blockInfosNew[curBlock.blockID].shape;
                BlockShape shape1 = blockInfosNew[type].shape;
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
                if (VoxelWorld.currentWorld.worldGenType == 0)
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
                else if (VoxelWorld.currentWorld.worldGenType == 2)
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

            if ((x < 0) || (z < 0) || (x >= chunkWidth) || (z >= chunkWidth))
            {

                if (VoxelWorld.currentWorld.worldGenType == 0)
                {
                    if (x >= chunkWidth)
                    {
                        if (rightChunk != null && rightChunk.isMapGenCompleted == true && rightChunk.disposed == false)
                        {
                            return rightChunk.map[0, y, z];
                        }
                        else return PredictBlockType(thisHeightMap[x - chunkWidth + 25, z + 8], y);

                    }
                    else if (z >= chunkWidth)
                    {
                        if (frontChunk != null && frontChunk.isMapGenCompleted == true && frontChunk.disposed == false)
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
                else if (VoxelWorld.currentWorld.worldGenType == 2)
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
            return map[x, y, z];
        }

        public VertexPositionNormalTangentTexture[] verticesOpqArray
        {
            get
            {
                return _verticesOpqArray;
            } set
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
        public  IndexBuffer IBOpq
        {
            get { return _IBOpq;}
            set
            {
                _IBOpq=value;
            } }
        public  VertexBuffer VBOpq
        {
            get { return _VBOpq; }
            set
            {
                _VBOpq = value;
            }
        }
        public  IndexBuffer IBOpqLOD1
        {
            get { return _IBOpqLOD1; }
            set
            {
                _IBOpqLOD1 = value;
            }
        }
        public  VertexBuffer VBOpqLOD1
        {
            get { return _VBOpqLOD1; }
            set
            {
                _VBOpqLOD1 = value;
            }
        }
        public  IndexBuffer IBWT
        {
            get { return _IBWT; }
            set
            {
                _IBWT = value;
            }
        }
        public  VertexBuffer VBWT
        {
            get { return _VBWT; }
            set
            {
                _VBWT = value;
            }
        }
        public  IndexBuffer IBNS
        {
            get { return _IBNS; }
            set
            {
                _IBNS = value;
            }
        }
        public  VertexBuffer VBNS
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
        //  public bool isTaskCompleted { get { return _isTaskCompleted; } set {  _isTaskCompleted = value;if (_isTaskCompleted == false) { ChunkHelper.buildingChunksCount++; } else {  ChunkHelper.buildingChunksCount--; } } }
        public bool isTaskCompleted;

        public bool isNSBuffersValid { get; set; } = false;

        public bool isWTBuffersValid { get; set; } = false;

        public bool isOpqBuffersValid { get; set; } = false;
            public void BuildChunk()
        {
            isTaskCompleted = false;


            Task t = new Task(() => InitMap(chunkPos));
            t.RunSynchronously();

            isTaskCompleted = true;
            //     GenerateMesh(verticesOpq, verticesNS, verticesWT);
            //  Debug.WriteLine(verticesOpqArray.Length);
            //Debug.WriteLine(verticesWTArray.Length);
            isReadyToRender = true;

        }

        public object asyncTaskLock = new object();
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
                Debug.WriteLine("rebuild chunk thread:"+Thread.CurrentThread.ManagedThreadId);
                isReadyToRender = true;
            });

        //  sw.Stop();
       //   Debug.WriteLine(sw.ElapsedMilliseconds);
             
               
          
            

              

      //      t.RunSynchronously();
      
       

        }

        public void BuildChunkAsyncWithActionOnCompleted(ConcurrentQueue<Action> actionQueue)
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


            }).GetAwaiter().OnCompleted(()=> actionQueue.Enqueue(() =>
            {


                GenerateRenderBuffers();

                isTaskCompleted = true;
                //     GenerateMesh(verticesOpq, verticesNS, verticesWT);
                //  Debug.WriteLine(verticesOpqArray.Length);
           //     Debug.WriteLine("rebuild chunk thread:" + Thread.CurrentThread.ManagedThreadId);
                isReadyToRender = true;
            }));
           
                    

              



            //  sw.Stop();
            //   Debug.WriteLine(sw.ElapsedMilliseconds);







            //      t.RunSynchronously();



        }
        public bool disposed { get; set; }


        public void Dispose()
        {
            // Debug.WriteLine("dispose");
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        ~Chunk()
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
                this.lightPoints = null;
                //   this.semaphore=null;
            }
            disposed = true;






        }
    }
    }
