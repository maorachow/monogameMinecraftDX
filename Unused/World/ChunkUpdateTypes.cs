using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.Updateables;

namespace monogameMinecraftDX.World
{
    public interface IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }
        public void Update();
    }

    public struct PlacingBlockOperation: IChunkUpdateOperation
    {
        public WorldUpdater worldUpdater;
        public Vector3Int position { get; set; }
        public BlockData placingBlockData;

        public PlacingBlockOperation(Vector3Int position, WorldUpdater worldUpdater, BlockData placingBlockData)
        {
            this.position = position;
            this.worldUpdater = worldUpdater;
            this.placingBlockData=placingBlockData;
        }

        public void Update()
        {
            BlockShape? shapeThis =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0)));
           
            BlockShape? shapeRight =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0)));
            BlockShape? shapeLeft =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0)));
            BlockShape? shapeFront =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1)));
            BlockShape? shapeBack =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1)));

            ChunkHelper.SetBlockWithoutUpdateWithSaving(position, placingBlockData);
            if (shapeLeft != null && (shapeLeft.Value == BlockShape.Fence))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(-1, 0, 0), this.worldUpdater, new Vector3Int(1, 0, 0), 1));
            }
            if (shapeRight != null && (shapeRight.Value == BlockShape.Fence))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(1, 0, 0), this.worldUpdater, new Vector3Int(-1, 0, 0), 1));
            }
            if (shapeFront != null && (shapeFront.Value == BlockShape.Fence))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, 1), this.worldUpdater, new Vector3Int(0, 0, -1), 1));
            }
            if (shapeBack != null && (shapeBack.Value == BlockShape.Fence))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, -1), this.worldUpdater, new Vector3Int(0, 0, 1), 1));
            }
        }
    }
    public struct FenceUpdatingOperation: IChunkUpdateOperation
    {
        public WorldUpdater worldUpdater;
        public Vector3Int position { get; set; }
        public Vector3Int updateFromPoint;
        public int stackDepth = 0;
        public FenceUpdatingOperation(Vector3Int position, WorldUpdater worldUpdater, Vector3Int updateFromPoint, int stackDepth)
        {
            this.position = position;
            this.worldUpdater = worldUpdater;
            this.updateFromPoint = updateFromPoint;
            this.stackDepth = stackDepth;
        }
        public void Update()
        {
            BlockShape? shapeThis =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0)));
            if (shapeThis is not BlockShape.Fence)
            {
                return;
            }
            BlockShape? shapeRight =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0)));
            BlockShape? shapeLeft =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0)));
            BlockShape? shapeFront =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1)));
            BlockShape? shapeBack =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1)));
            bool[] shapes = new[] { false, false, false, false, false, false, false, false };
            if (shapeLeft != null && (shapeLeft.Value == BlockShape.Fence || shapeLeft.Value == BlockShape.Solid))
            {
                shapes[7] = true;
            }
            else
            {
                shapes[7] = false;
            }

            if (shapeRight != null && (shapeRight.Value == BlockShape.Fence || shapeRight.Value == BlockShape.Solid))
            {
                shapes[6] = true;
            }
            else
            {
                shapes[6] = false;
            }

            if (shapeBack != null && (shapeBack.Value == BlockShape.Fence || shapeBack.Value == BlockShape.Solid))
            {
                shapes[5] = true;
            }
            else
            {
                shapes[5] = false;
            }

            if (shapeFront != null && (shapeFront.Value == BlockShape.Fence || shapeFront.Value == BlockShape.Solid))
            {
                shapes[4] = true;
            }
            else
            {
                shapes[4] = false;
            }
            //     Debug.WriteLine("from::"+updateFromPoint);
            ChunkHelper.SetBlockOptionalDataWithoutUpdate(position,MathUtility.GetByte(shapes));

            if (stackDepth >= 2)
            {
                return;
            }
            if (shapeLeft != null && (shapeLeft.Value == BlockShape.Fence)&&updateFromPoint.x!=-1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(-1, 0, 0),this.worldUpdater, new Vector3Int(1, 0, 0), stackDepth+1));
            }
            if (shapeRight != null && (shapeRight.Value == BlockShape.Fence) && updateFromPoint.x != 1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(1, 0, 0), this.worldUpdater, new Vector3Int(-1, 0, 0), stackDepth + 1));
            }
            if (shapeFront != null && (shapeFront.Value == BlockShape.Fence) && updateFromPoint.z != 1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, 1), this.worldUpdater, new Vector3Int(0, 0, -1), stackDepth + 1));
            }
            if (shapeBack != null && (shapeBack.Value == BlockShape.Fence) && updateFromPoint.z != -1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, -1), this.worldUpdater, new Vector3Int(0, 0, 1), stackDepth + 1));
            }
        }
    }

    public struct BreakBlockOperation : IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }
        public WorldUpdater worldUpdater;
        public BlockData prevBlockData;
        public BreakBlockOperation(Vector3Int position, WorldUpdater worldUpdater,BlockData prevBlockData)
        {
            this.position= position;
            this.worldUpdater= worldUpdater;  
            this.prevBlockData= prevBlockData;
        }
        public void Update()
        {
            Vector3Int tempPos = position;
            var key = prevBlockData;
            if (Chunk.blockInfosNew.ContainsKey(key.blockID))
            {
                worldUpdater.onUpdatedOneShot += () =>
                {
                    ParticleEmittingHelper.EmitParticleWithParamCustomUV(
                        new Vector3(tempPos.x + 0.5f, tempPos.y + 0.5f, tempPos.z + 0.5f),
                        ParticleEmittingHelper.allParticles["blockbreaking"],
                        new Vector4(Chunk.blockInfosNew[key.blockID].uvCorners[0].X,
                            Chunk.blockInfosNew[key.blockID].uvCorners[0].Y,
                            Chunk.blockInfosNew[key.blockID].uvSizes[0].X / 4.0f,
                            Chunk.blockInfosNew[key.blockID].uvSizes[0].Y / 4.0f),new Vector2(Chunk.blockInfosNew[key.blockID].uvSizes[0].X * 0.75f, Chunk.blockInfosNew[key.blockID].uvSizes[0].Y * 0.75f));
                };
            }
        
            BlockShape? shapeRight =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0)));
            BlockShape? shapeLeft =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0)));
            BlockShape? shapeFront =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1)));
            BlockShape? shapeBack =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1)));

            BlockShape? shapeThis =
                ChunkHelper.GetBlockShape(prevBlockData);

            if (shapeThis is BlockShape.Door)
            {
                Debug.WriteLine("break door");
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new DoorBreakingOperation(position,prevBlockData));
            }
            if (shapeLeft != null && (shapeLeft.Value == BlockShape.Water))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(-1, 0, 0), this.worldUpdater, new Vector3Int(1, 0, 0), 1));
            }
            if (shapeRight != null && (shapeRight.Value == BlockShape.Water))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(1, 0, 0), this.worldUpdater, new Vector3Int(-1, 0, 0), 1));
            }
            if (shapeFront != null && (shapeFront.Value == BlockShape.Water))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, 0, 1), this.worldUpdater, new Vector3Int(0, 0, 1), 1));
            }
            if (shapeBack != null && (shapeBack.Value == BlockShape.Water))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, 0, -1), this.worldUpdater, new Vector3Int(0, 0, -1), 1));
            }

            if (shapeLeft != null && (shapeLeft.Value == BlockShape.Fence))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(-1, 0, 0), this.worldUpdater, new Vector3Int(1, 0, 0), 1));
            }
            if (shapeRight != null && (shapeRight.Value == BlockShape.Fence))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(1, 0, 0), this.worldUpdater, new Vector3Int(-1, 0, 0), 1));
            }
            if (shapeFront != null && (shapeFront.Value == BlockShape.Fence))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, 1), this.worldUpdater, new Vector3Int(0, 0, -1), 1));
            }
            if (shapeBack != null && (shapeBack.Value == BlockShape.Fence))
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, -1), this.worldUpdater, new Vector3Int(0, 0, 1),1));
            }



        }
     
    }

    public struct WaterFloodOperation : IChunkUpdateOperation
    {

        public WorldUpdater worldUpdater;
        public Vector3Int position { get ; set; }

        public Vector3Int updateFromPoint;
        public int stackDepth = 0;
        public WaterFloodOperation(Vector3Int position, WorldUpdater worldUpdater, Vector3Int updateFromPoint, int stackDepth)
        {
            this.position = position;
            this.worldUpdater = worldUpdater;
            this.updateFromPoint = updateFromPoint;
            this.stackDepth = stackDepth;
        }
        public void Update()
        {
            BlockShape? shapeThis =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0)));
            BlockData dataThis = ChunkHelper.GetBlockData(position);
         //   Debug.WriteLine("data this:"+dataThis.blockID);
            if (shapeThis is not BlockShape.Water)
            {
                return;
            }
            BlockShape? shapeRight =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0)));
            BlockShape? shapeLeft =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0)));
            BlockShape? shapeFront =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1)));
            BlockShape? shapeBack =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1)));

         
            BlockShape? shapeBottom =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0)));

            if (shapeLeft == null || (shapeLeft.Value !=BlockShape.Solid&&shapeLeft.Value!=BlockShape.Water && shapeLeft.Value != BlockShape.Fence && shapeLeft.Value != BlockShape.Slabs))
            {
                Debug.WriteLine("left");
                ChunkHelper.SetBlockWithoutUpdate(position+new Vector3Int(-1,0,0), dataThis.blockID);
            }
           

            if (shapeRight == null || (shapeRight.Value != BlockShape.Solid && shapeRight.Value != BlockShape.Water && shapeRight.Value != BlockShape.Fence && shapeRight.Value != BlockShape.Slabs))
            {
                Debug.WriteLine("right");
                ChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(1, 0, 0), dataThis.blockID);
            }
            

            if (shapeBack == null || (shapeBack.Value != BlockShape.Solid && shapeBack.Value != BlockShape.Water && shapeBack.Value != BlockShape.Fence && shapeBack.Value != BlockShape.Slabs))
            {
                Debug.WriteLine("back");
                ChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, 0, -1), dataThis.blockID);
            }
            if (shapeFront == null || (shapeFront.Value != BlockShape.Solid && shapeFront.Value != BlockShape.Water && shapeFront.Value != BlockShape.Fence && shapeFront.Value != BlockShape.Slabs))
            {
                Debug.WriteLine("front");
                ChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, 0, 1), dataThis.blockID);
            }
            if (shapeBottom == null || (shapeBottom.Value != BlockShape.Solid && shapeBottom.Value != BlockShape.Water && shapeBottom.Value != BlockShape.Fence && shapeBottom.Value != BlockShape.Slabs))
            {
                Debug.WriteLine("bottom");
                ChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, -1, 0), dataThis.blockID);
            }
            shapeRight =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0)));
            shapeLeft =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0)));
          shapeFront =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1)));
          shapeBack =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1)));

          shapeBottom =
              ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0)));



          if (stackDepth >= 2)
          {
              return;
          }
       /*     if (shapeLeft != null && (shapeLeft.Value == BlockShape.Water) && updateFromPoint.x != -1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(-1, 0, 0), this.worldUpdater, new Vector3Int(1, 0, 0), stackDepth+1));
            }
            if (shapeRight != null && (shapeRight.Value == BlockShape.Water) && updateFromPoint.x != 1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(1, 0, 0), this.worldUpdater, new Vector3Int(-1, 0, 0), stackDepth + 1));
            }
            if (shapeFront != null && (shapeFront.Value == BlockShape.Water) && updateFromPoint.z != 1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, 0, 1), this.worldUpdater, new Vector3Int(0, 0, -1), stackDepth + 1));
            }
            if (shapeBack != null && (shapeBack.Value == BlockShape.Water) && updateFromPoint.z !=- 1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, 0, -1), this.worldUpdater, new Vector3Int(0, 0, 1), stackDepth + 1));
            }
            if (shapeBottom != null && (shapeBottom.Value == BlockShape.Water) && updateFromPoint.y != -1)
            {
                worldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, -1, 0), this.worldUpdater, new Vector3Int(0, 1, 0), stackDepth + 1));
            }*/
        }
    }

    public struct DoorInteractingOperation : IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }

        public DoorInteractingOperation(Vector3Int position)
        {
            this.position= position;
        }
        public void Update()
        {

            BlockShape? shapeThis =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0)));
     //    Debug.WriteLine(shapeThis);
            if (shapeThis is not BlockShape.Door)
            {
                return;
            }

            BlockShape? shapeDown =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0)));
            BlockShape? shapeUp =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0)));

            if (shapeThis is BlockShape.Door)
            {
                BlockData data = ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0));
                bool[] dataBinary = MathUtility.GetBooleanArray(data.optionalDataValue);
                dataBinary[4] = !dataBinary[4];
                ChunkHelper.SetBlockOptionalDataWithoutUpdate(position + new Vector3Int(0, 0, 0), MathUtility.GetByte(dataBinary));
           //     Debug.WriteLine(dataBinary);
            }
            if (shapeDown is BlockShape.Door)
            {
                BlockData data=ChunkHelper.GetBlockData(position + new Vector3Int(0,-1,0));
                bool[] dataBinary = MathUtility.GetBooleanArray(data.optionalDataValue);
                dataBinary[4] = !dataBinary[4];
                ChunkHelper.SetBlockOptionalDataWithoutUpdate(position + new Vector3Int(0, -1, 0),MathUtility.GetByte(dataBinary));
      //          Debug.WriteLine(dataBinary);
            }

            if (shapeUp is BlockShape.Door)
            {
                BlockData data = ChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0));
                bool[] dataBinary = MathUtility.GetBooleanArray(data.optionalDataValue);
                dataBinary[4] = !dataBinary[4];
                ChunkHelper.SetBlockOptionalDataWithoutUpdate(position + new Vector3Int(0, 1, 0), MathUtility.GetByte(dataBinary));
       //         Debug.WriteLine(dataBinary);
            }
          
        }
    }

    public struct DoorUpperPartPlacingOperation : IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }

        public DoorUpperPartPlacingOperation(Vector3Int position)
        {
            this.position = position;
        }
        public void Update()
        {

            BlockShape? shapeThis =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position));
            Debug.WriteLine(shapeThis);
            if (shapeThis is not BlockShape.Door)
            {
                return;
            }

            
            BlockShape? shapeUp =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0)));

      

            if (shapeUp is not BlockShape.Door)
            {
                BlockData data = ChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0));
                bool[] dataBinary = MathUtility.GetBooleanArray(data.optionalDataValue);
                dataBinary[5] = true;
                ChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, 1, 0), data.blockID);
                ChunkHelper.SetBlockOptionalDataWithoutUpdate(position + new Vector3Int(0, 1, 0), MathUtility.GetByte(dataBinary));
             //   Debug.WriteLine(dataBinary[7]);
            }

        }
    }


    public struct DoorBreakingOperation : IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }
        public BlockData prevBlockData;

        public DoorBreakingOperation(Vector3Int position,BlockData prevBlockData)
        {
            this.position = position;
            this.prevBlockData = prevBlockData;
        }
        public void Update()
        {

            BlockShape? shapeThis =
                ChunkHelper.GetBlockShape(prevBlockData);
            Debug.WriteLine(shapeThis);
            if (shapeThis is not BlockShape.Door)
            {
                return;
            }

            BlockData dataThis = prevBlockData;

            bool[] dataBinary = MathUtility.GetBooleanArray(dataThis.optionalDataValue);
            BlockShape? shapeUp =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0)));
            BlockShape? shapeDown =
                ChunkHelper.GetBlockShape(ChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0)));


            //break door bottom
            if (dataBinary[5] == false)
            {
                if (shapeUp is BlockShape.Door)
                {

                    ChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, 1, 0), (short)0);

                    //   Debug.WriteLine(dataBinary[7]);
                }
            }
            else
            {
                
                    if (shapeDown is BlockShape.Door)
                    {

                        ChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, -1, 0), (short)0);

                        //   Debug.WriteLine(dataBinary[7]);
                    }
                
            }

          

        }
    }
}
