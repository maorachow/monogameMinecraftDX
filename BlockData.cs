using monogameMinecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Reflection.Metadata.Ecma335;
using MessagePack;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace monogameMinecraftDX
{
    [MessagePackObject]
    public struct BlockData
    {
        [Key(0)]
        public short blockID;
        [Key(1)]
        public byte optionalDataValue;
        public BlockData(short blockID)
        {
            this.blockID = blockID;
            this.optionalDataValue = 0;
        }
        public BlockData(short blockID,byte optionalDataValue)
        {
            this.blockID = blockID;
            this.optionalDataValue = optionalDataValue;
        }
        public static explicit operator BlockData(short data)=>new BlockData(data);
        public static implicit operator BlockData(int data) => new BlockData((short)data);
        public static implicit operator short(BlockData data) => data.blockID;

    }

    public enum BlockShape
    {
        Solid=0,
        CrossModel=1,
        Torch=2,
        Slabs=3,
        Stairs=4,
        Water=5

    }

     
    public struct BlockInfo
    {

        public List<Vector2> uvCorners;
        public List<Vector2> uvSizes;
        
        public BlockShape shape;
        public BlockInfo(List<Vector2> uvCorners, List<Vector2> uvSizes, BlockShape shape)
        {
            this.uvCorners = uvCorners;
            this.uvSizes = uvSizes;
            this.shape = shape;
        }
        public BlockInfo(List<Vector2> uvCorners, List<Vector2> uvSizes)
        {
            this.uvCorners = uvCorners;
            this.uvSizes = uvSizes;
            this.shape = BlockShape.Solid;
        }
    }
   
    public static class BlockMeshBuildingHelper
    {
        
        public static void BuildSingleBlock( Chunk curChunk,int x,int y,int z,BlockData blockData,ref List<VertexPositionNormalTangentTexture> OpqVerts, ref List<VertexPositionNormalTangentTexture> NSVerts, ref List<VertexPositionNormalTangentTexture> WTVerts, ref List<ushort> OpqIndices, ref List<ushort> NSIndices, ref List<ushort> WTIndices)
        {
            if(blockData.blockID == 0||!Chunk.blockInfosNew.ContainsKey(blockData.blockID))
            {
                return;
            }
            short typeid=blockData.blockID;
            switch (Chunk.blockInfosNew[blockData.blockID].shape)
            {
                case BlockShape.Solid:
                    if(Chunk.blockInfosNew[blockData.blockID].uvCorners.Count<6|| Chunk.blockInfosNew[blockData.blockID].uvSizes.Count < 6)
                    {
                        return;
                    }
                    if (curChunk.CheckNeedBuildFace(x - 1, y, z, false))
                        BuildFaceComplex( new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, OpqVerts, OpqIndices);
                    //Right
                    if (curChunk.CheckNeedBuildFace(x + 1, y, z, false))
                        BuildFaceComplex(new Vector3(x + 1, y, z), new Vector3(0, 1, 0), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, OpqVerts, OpqIndices);

                    //Bottom
                    if (curChunk.CheckNeedBuildFace(x, y - 1, z, false))
                        BuildFaceComplex( new Vector3(x, y, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, OpqVerts, OpqIndices);
                    //Top
                    if (curChunk.CheckNeedBuildFace(x, y + 1, z, false))
                        BuildFaceComplex( new Vector3(x, y + 1, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, OpqVerts, OpqIndices);

                    //Back
                    if (curChunk.CheckNeedBuildFace(x, y, z - 1, false))
                        BuildFaceComplex( new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, OpqVerts, OpqIndices);
                    //Front
                    if (curChunk.CheckNeedBuildFace(x, y, z + 1, false))
                        BuildFaceComplex( new Vector3(x, y, z + 1), new Vector3(0, 1, 0), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, OpqVerts, OpqIndices);
                    break;
                case BlockShape.Water:

                    if (Chunk.blockInfosNew[blockData.blockID].uvCorners.Count < 6 || Chunk.blockInfosNew[blockData.blockID].uvSizes.Count < 6)
                    {
                        return;
                    }

                    //water
                    //left
                    if (curChunk.CheckNeedBuildFace(x - 1, y, z, true) && curChunk.GetChunkBlockType(x - 1, y, z) != 100)
                    {
                        if (curChunk.GetChunkBlockType(x, y + 1, z) != 100)
                        {
                            BuildFaceComplex( new Vector3(x, y, z), new Vector3(0f, 0.8f, 0f), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, WTVerts, WTIndices);




                        }
                        else
                        {
                            BuildFaceComplex( new Vector3(x, y, z), new Vector3(0f, 1f, 0f), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, WTVerts, WTIndices);






                        }

                    }

                    //Right
                    if (curChunk.CheckNeedBuildFace(x + 1, y, z, true) && curChunk.GetChunkBlockType(x + 1, y, z) != 100)
                    {
                        if (curChunk.GetChunkBlockType(x, y + 1, z) != 100)
                        {
                            BuildFaceComplex( new Vector3(x + 1, y, z), new Vector3(0f, 0.8f, 0f), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, WTVerts,  WTIndices);



                        }
                        else
                        {
                            BuildFaceComplex( new Vector3(x + 1, y, z), new Vector3(0f, 1f, 0f), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, WTVerts, WTIndices);



                        }

                    }



                    //Bottom
                    if (curChunk.CheckNeedBuildFace(x, y - 1, z, true) && curChunk.GetChunkBlockType(x, y - 1, z) != 100)
                    {
                        BuildFaceComplex( new Vector3(x, y, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, WTVerts, WTIndices);




                    }

                    //Top
                    if (curChunk.CheckNeedBuildFace(x, y + 1, z, true) && curChunk.GetChunkBlockType(x, y + 1, z) != 100)
                    {
                        BuildFaceComplex( new Vector3(x, y + 0.8f, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, WTVerts, WTIndices);




                    }




                    //Back
                    if (curChunk.CheckNeedBuildFace(x, y, z - 1, true) && curChunk.GetChunkBlockType(x, y, z - 1) != 100)
                    {
                        if (curChunk.GetChunkBlockType(x, y + 1, z) != 100)
                        {
                            BuildFaceComplex( new Vector3(x, y, z), new Vector3(0f, 0.8f, 0f), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, WTVerts, WTIndices);




                        }
                        else
                        {
                            BuildFaceComplex( new Vector3(x, y, z), new Vector3(0f, 1f, 0f), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, WTVerts, WTIndices);







                        }

                    }


                    //Front
                    if (curChunk.CheckNeedBuildFace(x, y, z + 1, true) && curChunk.GetChunkBlockType(x, y, z + 1) != 100)
                    {
                        if (curChunk.GetChunkBlockType(x, y + 1, z) != 100)
                        {
                            BuildFaceComplex( new Vector3(x, y, z + 1), new Vector3(0f, 0.8f, 0f), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, WTVerts, WTIndices);


                        }
                        else
                        {
                            BuildFaceComplex( new Vector3(x, y, z + 1), new Vector3(0f, 1f, 0f), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, WTVerts, WTIndices);

                        }

                    }
                    break;
                case BlockShape.Torch:
                    //datavalues: 0ground 1left 2right 3front 4back
                    if (Chunk.blockInfosNew[blockData.blockID].uvCorners.Count < 6 || Chunk.blockInfosNew[blockData.blockID].uvSizes.Count < 6)
                    {
                        return;
                    }
                    switch (blockData.optionalDataValue)
                    {
                        case 0:
                            Matrix transformMat = Matrix.CreateRotationX(MathHelper.ToRadians(0f));
                            
                            BuildFaceComplex(new Vector3(x, y, z),transformMat,new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, NSVerts, NSIndices);
                            //Right

                            BuildFaceComplex(new Vector3(x, y, z),transformMat, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0.125f, 0f, 0f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, NSVerts, NSIndices);

                            //Bottom

                            BuildFaceComplex(new Vector3(x, y, z),transformMat, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, NSVerts, NSIndices);
                            //Top

                            BuildFaceComplex(new Vector3(x, y, z),transformMat, new Vector3(x, y, z) + new Vector3(0.4375f, 0.625f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, NSVerts, NSIndices);

                            //Back

                            BuildFaceComplex(new Vector3(x, y, z),transformMat, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, NSVerts, NSIndices);
                            //Front

                            BuildFaceComplex(new Vector3(x, y, z),transformMat, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0f, 0f, 0.125f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, NSVerts, NSIndices);

                            break;
                        case 1:

                            Matrix transformMat1 = Matrix.CreateRotationZ(MathHelper.ToRadians(25f)) * Matrix.CreateTranslation(new Vector3(0.3f, 0.1f, 0f));

                            BuildFaceComplex(new Vector3(x, y, z), transformMat1, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, NSVerts, NSIndices);
                            //Right

                            BuildFaceComplex(new Vector3(x, y, z), transformMat1, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0.125f, 0f, 0f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, NSVerts, NSIndices);

                            //Bottom

                            BuildFaceComplex(new Vector3(x, y, z), transformMat1, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, NSVerts, NSIndices);
                            //Top

                            BuildFaceComplex(new Vector3(x, y, z), transformMat1, new Vector3(x, y, z) + new Vector3(0.4375f, 0.625f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, NSVerts, NSIndices);

                            //Back

                            BuildFaceComplex(new Vector3(x, y, z), transformMat1, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, NSVerts, NSIndices);
                            //Front

                            BuildFaceComplex(new Vector3(x, y, z), transformMat1, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0f, 0f, 0.125f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, NSVerts, NSIndices);

                            break;
                        case 2:

                            Matrix transformMat2 = Matrix.CreateRotationZ(MathHelper.ToRadians(-25f)) * Matrix.CreateTranslation(new Vector3(-0.3f, 0.1f,0f ));

                            BuildFaceComplex(new Vector3(x, y, z), transformMat2, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, NSVerts, NSIndices);
                            //Right

                            BuildFaceComplex(new Vector3(x, y, z), transformMat2, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0.125f, 0f, 0f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, NSVerts, NSIndices);

                            //Bottom

                            BuildFaceComplex(new Vector3(x, y, z), transformMat2, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, NSVerts, NSIndices);
                            //Top

                            BuildFaceComplex(new Vector3(x, y, z), transformMat2, new Vector3(x, y, z) + new Vector3(0.4375f, 0.625f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, NSVerts, NSIndices);

                            //Back

                            BuildFaceComplex(new Vector3(x, y, z), transformMat2, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, NSVerts, NSIndices);
                            //Front

                            BuildFaceComplex(new Vector3(x, y, z), transformMat2, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0f, 0f, 0.125f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, NSVerts, NSIndices);

                            break;
                        case 3:

                            Matrix transformMat3 = Matrix.CreateRotationX(MathHelper.ToRadians(-25f)) * Matrix.CreateTranslation(new Vector3( 0f, 0.1f,0.3f));

                            BuildFaceComplex(new Vector3(x, y, z), transformMat3, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, NSVerts, NSIndices);
                            //Right

                            BuildFaceComplex(new Vector3(x, y, z), transformMat3, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0.125f, 0f, 0f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, NSVerts, NSIndices);

                            //Bottom

                            BuildFaceComplex(new Vector3(x, y, z), transformMat3, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, NSVerts, NSIndices);
                            //Top

                            BuildFaceComplex(new Vector3(x, y, z), transformMat3, new Vector3(x, y, z) + new Vector3(0.4375f, 0.625f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, NSVerts, NSIndices);

                            //Back

                            BuildFaceComplex(new Vector3(x, y, z), transformMat3, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, NSVerts, NSIndices);
                            //Front

                            BuildFaceComplex(new Vector3(x, y, z), transformMat3, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0f, 0f, 0.125f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, NSVerts, NSIndices);

                            break;
                        case 4:

                            Matrix transformMat4 = Matrix.CreateRotationX(MathHelper.ToRadians(25f))*Matrix.CreateTranslation(new Vector3(0f,0.1f,-0.3f)) ;

                            BuildFaceComplex(new Vector3(x, y, z), transformMat4, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, NSVerts, NSIndices);
                            //Right

                            BuildFaceComplex(new Vector3(x, y, z), transformMat4, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0.125f, 0f, 0f), new Vector3(0f, 0.625f, 0f), new Vector3(0f, 0f, 0.125f), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, NSVerts, NSIndices);

                            //Bottom

                            BuildFaceComplex(new Vector3(x, y, z), transformMat4, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, NSVerts, NSIndices);
                            //Top

                            BuildFaceComplex(new Vector3(x, y, z), transformMat4, new Vector3(x, y, z) + new Vector3(0.4375f, 0.625f, 0.4375f), new Vector3(0f, 0f, 0.125f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, NSVerts, NSIndices);

                            //Back

                            BuildFaceComplex(new Vector3(x, y, z), transformMat4, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, NSVerts, NSIndices);
                            //Front

                            BuildFaceComplex(new Vector3(x, y, z), transformMat4, new Vector3(x, y, z) + new Vector3(0.4375f, 0f, 0.4375f) + new Vector3(0f, 0f, 0.125f), new Vector3(0f, 0.625f, 0f), new Vector3(0.125f, 0f, 0f), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, NSVerts, NSIndices);

                            break;
                    }
                  

                 //   lightPoints.Add(new Vector3(x, y, z) + new Vector3(0.5f, 0.725f, 0.5f) + new Vector3(chunkPos.x, 0, chunkPos.y));
                    break;

                case BlockShape.CrossModel:
                    if (Chunk.blockInfosNew[blockData.blockID].uvCorners.Count < 1 || Chunk.blockInfosNew[blockData.blockID].uvSizes.Count < 1)
                    {
                        return;
                    }
                    Vector3 randomCrossModelOffset = new Vector3(0f, 0f, 0f);
                    BuildFaceComplex( new Vector3(x, y, z) + randomCrossModelOffset, new Vector3(0f, 1f, 0f) + randomCrossModelOffset, new Vector3(1f, 0f, 1f) + randomCrossModelOffset, Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, NSVerts, NSIndices);
                    BuildFaceComplex( new Vector3(x, y, z) + randomCrossModelOffset, new Vector3(0f, 1f, 0f) + randomCrossModelOffset, new Vector3(1f, 0f, 1f) + randomCrossModelOffset, Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], true, NSVerts, NSIndices);






                    BuildFaceComplex( new Vector3(x, y, z + 1f) + randomCrossModelOffset, new Vector3(0f, 1f, 0f) + randomCrossModelOffset, new Vector3(1f, 0f, -1f) + randomCrossModelOffset, Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, NSVerts, NSIndices);
                    BuildFaceComplex( new Vector3(x, y, z + 1f) + randomCrossModelOffset, new Vector3(0f, 1f, 0f) + randomCrossModelOffset, new Vector3(1f, 0f, -1f) + randomCrossModelOffset, Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], true, NSVerts, NSIndices);

                    break;

                case BlockShape.Slabs:
                    if (Chunk.blockInfosNew[blockData.blockID].uvCorners.Count < 6 || Chunk.blockInfosNew[blockData.blockID].uvSizes.Count < 6)
                    {
                        return;
                    }
                    switch (blockData.optionalDataValue)
                    {
                        case 0:
                          
                                BuildFaceComplex(new Vector3(x, y, z), new Vector3(0, 0.5f, 0), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, OpqVerts, OpqIndices);
                            //Right
                             
                                BuildFaceComplex(new Vector3(x + 1, y, z), new Vector3(0, 0.5f, 0), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, OpqVerts, OpqIndices);

                            //Bottom
                            if (curChunk.CheckNeedBuildFace(x, y - 1, z, false))
                                BuildFaceComplex(new Vector3(x, y, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, OpqVerts, OpqIndices);
                            //Top
                            
                                BuildFaceComplex(new Vector3(x, y + 0.5f, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, OpqVerts, OpqIndices);

                            //Back
                           
                                BuildFaceComplex(new Vector3(x, y, z), new Vector3(0, 0.5f, 0), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, OpqVerts, OpqIndices);
                            //Front
                           
                                BuildFaceComplex(new Vector3(x, y, z + 1), new Vector3(0, 0.5f, 0), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, OpqVerts, OpqIndices);
                            break;

                            case 1:

                            BuildFaceComplex(new Vector3(x, y+0.5f, z), new Vector3(0, 0.5f, 0), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, OpqVerts, OpqIndices);
                            //Right

                            BuildFaceComplex(new Vector3(x + 1, y + 0.5f, z), new Vector3(0, 0.5f, 0), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, OpqVerts, OpqIndices);

                            //Bottom
                          
                                BuildFaceComplex(new Vector3(x, y + 0.5f, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, OpqVerts, OpqIndices);
                            //Top
                            if (curChunk.CheckNeedBuildFace(x, y + 1, z, false))
                                BuildFaceComplex(new Vector3(x, y + 1f, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, OpqVerts, OpqIndices);

                            //Back

                            BuildFaceComplex(new Vector3(x, y + 0.5f, z), new Vector3(0, 0.5f, 0), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, OpqVerts, OpqIndices);
                            //Front

                            BuildFaceComplex(new Vector3(x, y + 0.5f, z + 1), new Vector3(0, 0.5f, 0), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, OpqVerts, OpqIndices);
                            break;
                            case 2:
                            if (curChunk.CheckNeedBuildFace(x - 1, y, z, false))
                                BuildFaceComplex(new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[0], Chunk.blockInfosNew[blockData.blockID].uvSizes[0], false, OpqVerts, OpqIndices);
                            //Right
                            if (curChunk.CheckNeedBuildFace(x + 1, y, z, false))
                                BuildFaceComplex(new Vector3(x + 1, y, z), new Vector3(0, 1, 0), new Vector3(0, 0, 1), Chunk.blockInfosNew[blockData.blockID].uvCorners[1], Chunk.blockInfosNew[blockData.blockID].uvSizes[1], true, OpqVerts, OpqIndices);

                            //Bottom
                            if (curChunk.CheckNeedBuildFace(x, y - 1, z, false))
                                BuildFaceComplex(new Vector3(x, y, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[2], Chunk.blockInfosNew[blockData.blockID].uvSizes[2], false, OpqVerts, OpqIndices);
                            //Top
                            if (curChunk.CheckNeedBuildFace(x, y + 1, z, false))
                                BuildFaceComplex(new Vector3(x, y + 1, z), new Vector3(0, 0, 1), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[3], Chunk.blockInfosNew[blockData.blockID].uvSizes[3], true, OpqVerts, OpqIndices);

                            //Back
                            if (curChunk.CheckNeedBuildFace(x, y, z - 1, false))
                                BuildFaceComplex(new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[4], Chunk.blockInfosNew[blockData.blockID].uvSizes[4], true, OpqVerts, OpqIndices);
                            //Front
                            if (curChunk.CheckNeedBuildFace(x, y, z + 1, false))
                                BuildFaceComplex(new Vector3(x, y, z + 1), new Vector3(0, 1, 0), new Vector3(1, 0, 0), Chunk.blockInfosNew[blockData.blockID].uvCorners[5], Chunk.blockInfosNew[blockData.blockID].uvSizes[5], false, OpqVerts, OpqIndices);
                            break;
                    }
                   
                    break;

            }

        }

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
            if (Chunk.blockInfo.ContainsKey(typeid))
            {
                uvCorner = Chunk.blockInfo[typeid][side];
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
        static void BuildFaceComplex(Vector3 origin,Matrix transformMat,Vector3 corner, Vector3 up, Vector3 right, Vector2 uvCorner, Vector2 uvWidth, bool reversed, List<VertexPositionNormalTangentTexture> verts, List<ushort> indices)
        {
            VertexPositionNormalTangentTexture vert00 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert01 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert11 = new VertexPositionNormalTangentTexture();
            VertexPositionNormalTangentTexture vert10 = new VertexPositionNormalTangentTexture();
            short index = (short)verts.Count;
            Vector3 corner1 = corner -( origin+new Vector3(0.5f,0.5f,0.5f));
            vert00.Position = Vector3.Transform(corner1, transformMat)+origin + new Vector3(0.5f, 0.5f, 0.5f);
            vert01.Position =Vector3.Transform(corner1 + up,transformMat) + origin + new Vector3(0.5f, 0.5f, 0.5f);
            vert11.Position = Vector3.Transform(corner1 + up+right, transformMat) + origin + new Vector3(0.5f, 0.5f, 0.5f);
            vert10.Position = Vector3.Transform(corner1 + right, transformMat) + origin + new Vector3(0.5f, 0.5f, 0.5f);
    //        Debug.WriteLine(vert00.Position + " " + vert01.Position+" "+vert11.Position+" "+vert10.Position);
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


                Vector3 normal = -Vector3.Cross(Vector3.TransformNormal(up,transformMat), Vector3.TransformNormal(right,transformMat));
                Vector3 tangent = Vector3.TransformNormal(-right,transformMat);
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

                Vector3 normal = Vector3.Cross(Vector3.TransformNormal(up, transformMat), Vector3.TransformNormal(right, transformMat));
                Vector3 tangent = Vector3.TransformNormal(right,transformMat);

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
        static void BuildFaceComplex(Vector3 corner, Vector3 up, Vector3 right, Vector2 uvCorner, Vector2 uvWidth, bool reversed, List<VertexPositionNormalTangentTexture> verts, List<ushort> indices)
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
    }
}
