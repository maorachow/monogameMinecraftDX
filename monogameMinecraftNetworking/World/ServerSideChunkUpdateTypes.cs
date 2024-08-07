using monogameMinecraftShared.Core;
using monogameMinecraftShared.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Data;

namespace monogameMinecraftNetworking.World
{
    public interface IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }

        public int worldID { get; set; }
        public void Update();


        public static IChunkUpdateOperation? ParseFromData(ChunkUpdateData data,ServerSideWorldUpdater updater)
        {
            switch ((ChunkUpdateDataTypes)data.dataType)
            {
                case ChunkUpdateDataTypes.BreakingBlockUpdate:

                    return new BreakBlockOperation(new Vector3Int(data.posX, data.posY, data.posZ), updater,
                        data.optionalData2, data.worldID);

                case ChunkUpdateDataTypes.PlaceBlockUpdate:

                    return new PlacingBlockOperation(new Vector3Int(data.posX, data.posY, data.posZ), updater,
                        data.optionalData1, data.worldID);
                case ChunkUpdateDataTypes.DoorInteractingUpdate:
                    return new DoorInteractingOperation(new Vector3Int(data.posX, data.posY, data.posZ),
                         data.worldID,updater);

                case ChunkUpdateDataTypes.FenceUpdatingUpdate:
                    return new FenceUpdatingOperation(new Vector3Int(data.posX, data.posY, data.posZ), updater, new Vector3Int(data.posX, data.posY, data.posZ),0,
                        data.worldID);
                case ChunkUpdateDataTypes.DoorUpperPartPlacingUpdate:
                    return new DoorUpperPartPlacingOperation(new Vector3Int(data.posX, data.posY, data.posZ),
                        data.worldID);
                default:
                    return null;
            }
        }
    }

    public struct PlacingBlockOperation : IChunkUpdateOperation
    {
        public ServerSideWorldUpdater ServerSideWorldUpdater;
        public Vector3Int position { get; set; }
        public BlockData placingBlockData;

        public int worldID { get; set; }

        public PlacingBlockOperation(Vector3Int position, ServerSideWorldUpdater ServerSideWorldUpdater, BlockData placingBlockData,int worldID)
        {
            this.position = position;
            this.ServerSideWorldUpdater = ServerSideWorldUpdater;
            this.placingBlockData = placingBlockData;
            this.worldID = worldID;
        }

        public void Update()
        {
            BlockShape? shapeThis =
               ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0),worldID));

            BlockShape? shapeRight =
               ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0), worldID));
            BlockShape? shapeLeft =
               ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0), worldID));
            BlockShape? shapeFront =
               ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1), worldID));
            BlockShape? shapeBack =
               ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1), worldID));

            ServerSideChunkHelper.SetBlockWithoutUpdateWithSaving(position, placingBlockData, worldID);
            lock (ServerSideWorldUpdater.chunksNeededRebuildListLock)
            {
                ServerSideWorldUpdater.soundDatasToSend.Add(new BlockSoundBroadcastData(position.x+0.5f,position.y+0.5f,position.z+0.5f,placingBlockData.blockID));
            }
            if (shapeLeft != null && shapeLeft.Value == BlockShape.Fence)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(-1, 0, 0), ServerSideWorldUpdater, new Vector3Int(1, 0, 0), 1, this.worldID));
            }
            if (shapeRight != null && shapeRight.Value == BlockShape.Fence)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(1, 0, 0), ServerSideWorldUpdater, new Vector3Int(-1, 0, 0), 1, this.worldID));
            }
            if (shapeFront != null && shapeFront.Value == BlockShape.Fence)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, 1), ServerSideWorldUpdater, new Vector3Int(0, 0, -1), 1, this.worldID));
            }
            if (shapeBack != null && shapeBack.Value == BlockShape.Fence)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, -1), ServerSideWorldUpdater, new Vector3Int(0, 0, 1), 1, this.worldID));
            }


        }
    }
    public struct FenceUpdatingOperation : IChunkUpdateOperation
    {
        public ServerSideWorldUpdater ServerSideWorldUpdater;
        public Vector3Int position { get; set; }
        public Vector3Int updateFromPoint;
        public int stackDepth = 0;
        public int worldID { get; set; }
        public FenceUpdatingOperation(Vector3Int position, ServerSideWorldUpdater ServerSideWorldUpdater, Vector3Int updateFromPoint, int stackDepth,int worldID)
        {
            this.position = position;
            this.ServerSideWorldUpdater = ServerSideWorldUpdater;
            this.updateFromPoint = updateFromPoint;
            this.stackDepth = stackDepth;
            this.worldID = worldID;
        }
        public void Update()
        {
            BlockShape? shapeThis =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0), worldID));
            if (shapeThis is not BlockShape.Fence)
            {
                return;
            }
            BlockShape? shapeRight =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0), worldID));
            BlockShape? shapeLeft =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0), worldID));
            BlockShape? shapeFront =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1), worldID));
            BlockShape? shapeBack =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1), worldID));
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
            ServerSideChunkHelper.SetBlockOptionalDataWithoutUpdate(position, MathUtility.GetByte(shapes), worldID);

            if (stackDepth >= 2)
            {
                return;
            }
            if (shapeLeft != null && shapeLeft.Value == BlockShape.Fence && updateFromPoint.x != -1)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(-1, 0, 0), ServerSideWorldUpdater, new Vector3Int(1, 0, 0), stackDepth + 1, worldID));
            }
            if (shapeRight != null && shapeRight.Value == BlockShape.Fence && updateFromPoint.x != 1)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(1, 0, 0), ServerSideWorldUpdater, new Vector3Int(-1, 0, 0), stackDepth + 1, worldID));
            }
            if (shapeFront != null && shapeFront.Value == BlockShape.Fence && updateFromPoint.z != 1)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, 1), ServerSideWorldUpdater, new Vector3Int(0, 0, -1), stackDepth + 1, worldID));
            }
            if (shapeBack != null && shapeBack.Value == BlockShape.Fence && updateFromPoint.z != -1)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, -1), ServerSideWorldUpdater, new Vector3Int(0, 0, 1), stackDepth + 1, worldID));
            }
        }
    }

    public struct BreakBlockOperation : IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }
        public ServerSideWorldUpdater ServerSideWorldUpdater;
        public BlockData prevBlockData;
        public int worldID { get; set; }
        public BreakBlockOperation(Vector3Int position, ServerSideWorldUpdater ServerSideWorldUpdater, BlockData prevBlockData,int worldID)
        {
            this.position = position;
            this.ServerSideWorldUpdater = ServerSideWorldUpdater;
            this.prevBlockData = prevBlockData;
            this.worldID = worldID;
        }
        public void Update()
        {
            Debug.WriteLine("server break block");
            Vector3Int tempPos = position;
            var key = prevBlockData;
            ServerSideChunkHelper.SetBlockWithoutUpdateWithSaving(position,0,worldID);

            lock (ServerSideWorldUpdater.chunksNeededRebuildListLock)
            {
                ServerSideWorldUpdater.soundDatasToSend.Add(new BlockSoundBroadcastData(position.x + 0.5f, position.y + 0.5f, position.z + 0.5f, prevBlockData.blockID));
                ServerSideWorldUpdater.particleDatasToSend.Add(new BlockParticleEffectBroadcastData(position.x + 0.5f, position.y + 0.5f, position.z + 0.5f, prevBlockData.blockID));
            }
            //send break block particle command to client
            BlockShape? shapeRight =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0), worldID));
            BlockShape? shapeLeft =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0), worldID));
            BlockShape? shapeFront =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1), worldID));
            BlockShape? shapeBack =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1), worldID));

            BlockShape? shapeThis =
                ServerSideChunkHelper.GetBlockShape(prevBlockData);

            BlockShape? shapeUp =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0), worldID));

            if (shapeUp is BlockShape.CrossModel)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new BreakBlockOperation(position + new Vector3Int(0, 1, 0),ServerSideWorldUpdater, ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0), worldID),worldID));
            }
            if (shapeThis is BlockShape.Door)
            {
                Debug.WriteLine("break door");
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new DoorBreakingOperation(position, prevBlockData, worldID));
            }
            if (shapeLeft != null && shapeLeft.Value == BlockShape.Water)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(-1, 0, 0), ServerSideWorldUpdater, new Vector3Int(1, 0, 0), 1, worldID));
            }
            if (shapeRight != null && shapeRight.Value == BlockShape.Water)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(1, 0, 0), ServerSideWorldUpdater, new Vector3Int(-1, 0, 0), 1, worldID));
            }
            if (shapeFront != null && shapeFront.Value == BlockShape.Water)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, 0, 1), ServerSideWorldUpdater, new Vector3Int(0, 0, 1), 1, worldID));
            }
            if (shapeBack != null && shapeBack.Value == BlockShape.Water)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, 0, -1), ServerSideWorldUpdater, new Vector3Int(0, 0, -1), 1, worldID));
            }

            if (shapeLeft != null && shapeLeft.Value == BlockShape.Fence)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(-1, 0, 0), ServerSideWorldUpdater, new Vector3Int(1, 0, 0), 1, worldID));
            }
            if (shapeRight != null && shapeRight.Value == BlockShape.Fence)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(1, 0, 0), ServerSideWorldUpdater, new Vector3Int(-1, 0, 0), 1, worldID));
            }
            if (shapeFront != null && shapeFront.Value == BlockShape.Fence)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, 1), ServerSideWorldUpdater, new Vector3Int(0, 0, -1), 1, worldID));
            }
            if (shapeBack != null && shapeBack.Value == BlockShape.Fence)
            {
                ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(position + new Vector3Int(0, 0, -1), ServerSideWorldUpdater, new Vector3Int(0, 0, 1), 1, worldID));
            }



        }

    }

    public struct WaterFloodOperation : IChunkUpdateOperation
    {

        public ServerSideWorldUpdater ServerSideWorldUpdater;
        public Vector3Int position { get; set; }

        public Vector3Int updateFromPoint;
        public int stackDepth = 0;
        public int worldID { get; set; }
        public WaterFloodOperation(Vector3Int position, ServerSideWorldUpdater ServerSideWorldUpdater, Vector3Int updateFromPoint, int stackDepth,int worldID)
        {
            this.position = position;
            this.ServerSideWorldUpdater = ServerSideWorldUpdater;
            this.updateFromPoint = updateFromPoint;
            this.stackDepth = stackDepth;
            this.worldID = worldID;
        }
        public void Update()
        {
            BlockShape? shapeThis =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0), worldID));
            BlockData dataThis = ServerSideChunkHelper.GetBlockData(position, worldID);
            //   Debug.WriteLine("data this:"+dataThis.blockID);
            if (shapeThis is not BlockShape.Water)
            {
                return;
            }
            BlockShape? shapeRight =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0), worldID));
            BlockShape? shapeLeft =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0), worldID));
            BlockShape? shapeFront =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1), worldID));
            BlockShape? shapeBack =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1), worldID));


            BlockShape? shapeBottom =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0), worldID));

            if (shapeLeft == null || shapeLeft.Value != BlockShape.Solid && shapeLeft.Value != BlockShape.Water && shapeLeft.Value != BlockShape.Fence && shapeLeft.Value != BlockShape.Slabs)
            {
                Debug.WriteLine("left");
                ServerSideChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(-1, 0, 0), dataThis.blockID, worldID);
            }


            if (shapeRight == null || shapeRight.Value != BlockShape.Solid && shapeRight.Value != BlockShape.Water && shapeRight.Value != BlockShape.Fence && shapeRight.Value != BlockShape.Slabs)
            {
                Debug.WriteLine("right");
                ServerSideChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(1, 0, 0), dataThis.blockID, worldID);
            }


            if (shapeBack == null || shapeBack.Value != BlockShape.Solid && shapeBack.Value != BlockShape.Water && shapeBack.Value != BlockShape.Fence && shapeBack.Value != BlockShape.Slabs)
            {
                Debug.WriteLine("back");
                ServerSideChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, 0, -1), dataThis.blockID, worldID);
            }
            if (shapeFront == null || shapeFront.Value != BlockShape.Solid && shapeFront.Value != BlockShape.Water && shapeFront.Value != BlockShape.Fence && shapeFront.Value != BlockShape.Slabs)
            {
                Debug.WriteLine("front");
                ServerSideChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, 0, 1), dataThis.blockID, worldID);
            }
            if (shapeBottom == null || shapeBottom.Value != BlockShape.Solid && shapeBottom.Value != BlockShape.Water && shapeBottom.Value != BlockShape.Fence && shapeBottom.Value != BlockShape.Slabs)
            {
                Debug.WriteLine("bottom");
                ServerSideChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, -1, 0), dataThis.blockID, worldID);
            }
            shapeRight =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(1, 0, 0), worldID));
            shapeLeft =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(-1, 0, 0), worldID));
            shapeFront =
                  ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 1), worldID));
            shapeBack =
                  ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, -1), worldID));

            shapeBottom =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0), worldID));



            if (stackDepth >= 2)
            {
                return;
            }
            /*     if (shapeLeft != null && (shapeLeft.Value == BlockShape.Water) && updateFromPoint.x != -1)
                 {
                     ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(-1, 0, 0), this.ServerSideWorldUpdater, new Vector3Int(1, 0, 0), stackDepth+1));
                 }
                 if (shapeRight != null && (shapeRight.Value == BlockShape.Water) && updateFromPoint.x != 1)
                 {
                     ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(1, 0, 0), this.ServerSideWorldUpdater, new Vector3Int(-1, 0, 0), stackDepth + 1));
                 }
                 if (shapeFront != null && (shapeFront.Value == BlockShape.Water) && updateFromPoint.z != 1)
                 {
                     ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, 0, 1), this.ServerSideWorldUpdater, new Vector3Int(0, 0, -1), stackDepth + 1));
                 }
                 if (shapeBack != null && (shapeBack.Value == BlockShape.Water) && updateFromPoint.z !=- 1)
                 {
                     ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, 0, -1), this.ServerSideWorldUpdater, new Vector3Int(0, 0, 1), stackDepth + 1));
                 }
                 if (shapeBottom != null && (shapeBottom.Value == BlockShape.Water) && updateFromPoint.y != -1)
                 {
                     ServerSideWorldUpdater.queuedChunkUpdatePoints.Enqueue(new WaterFloodOperation(position + new Vector3Int(0, -1, 0), this.ServerSideWorldUpdater, new Vector3Int(0, 1, 0), stackDepth + 1));
                 }*/
        }
    }

    public struct DoorInteractingOperation : IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }
        public int worldID { get; set; }
        public ServerSideWorldUpdater serverSideWorldUpdater;
        public DoorInteractingOperation(Vector3Int position,int worldID,ServerSideWorldUpdater worldUpdater)
        {
            this.position = position;
            this.worldID = worldID;
            this.serverSideWorldUpdater = worldUpdater;
        }
        public void Update()
        {

            BlockShape? shapeThis =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0), worldID));
            //    Debug.WriteLine(shapeThis);
            if (shapeThis is not BlockShape.Door)
            {
                return;
            }

            BlockShape? shapeDown =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0), worldID));
            BlockShape? shapeUp =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0), worldID));

            if (shapeThis is BlockShape.Door)
            {
                BlockData data = ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0), worldID);
                bool[] dataBinary = MathUtility.GetBooleanArray(data.optionalDataValue);
                dataBinary[4] = !dataBinary[4];
                ServerSideChunkHelper.SetBlockOptionalDataWithoutUpdate(position + new Vector3Int(0, 0, 0), MathUtility.GetByte(dataBinary),worldID);
                //     Debug.WriteLine(dataBinary);
            }
            if (shapeDown is BlockShape.Door)
            {
                BlockData data = ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0), worldID);
                bool[] dataBinary = MathUtility.GetBooleanArray(data.optionalDataValue);
                dataBinary[4] = !dataBinary[4];
                ServerSideChunkHelper.SetBlockOptionalDataWithoutUpdate(position + new Vector3Int(0, -1, 0), MathUtility.GetByte(dataBinary), worldID);
                //          Debug.WriteLine(dataBinary);
            }

            if (shapeUp is BlockShape.Door)
            {
                BlockData data = ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0), worldID);
                bool[] dataBinary = MathUtility.GetBooleanArray(data.optionalDataValue);
                dataBinary[4] = !dataBinary[4];
                ServerSideChunkHelper.SetBlockOptionalDataWithoutUpdate(position + new Vector3Int(0, 1, 0), MathUtility.GetByte(dataBinary), worldID);
                //         Debug.WriteLine(dataBinary);
            }
            lock (serverSideWorldUpdater.chunksNeededRebuildListLock)
            {
                serverSideWorldUpdater.soundDatasToSend.Add(new BlockSoundBroadcastData(position.x + 0.5f, position.y + 0.5f, position.z + 0.5f, ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0), worldID).blockID));
            }
        }
    }

    public struct DoorUpperPartPlacingOperation : IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }

        public int worldID { get; set; }
        public DoorUpperPartPlacingOperation(Vector3Int position,int worldID)
        {
            this.position = position;
            this.worldID = worldID;
        }
        public void Update()
        {

            BlockShape? shapeThis =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position,worldID));
            Debug.WriteLine(shapeThis);
            if (shapeThis is not BlockShape.Door)
            {
                return;
            }


            BlockShape? shapeUp =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0), worldID));



            if (shapeUp is not BlockShape.Door)
            {
                BlockData data = ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 0, 0), worldID);
                bool[] dataBinary = MathUtility.GetBooleanArray(data.optionalDataValue);
                dataBinary[5] = true;
                ServerSideChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, 1, 0), data.blockID, worldID);
                ServerSideChunkHelper.SetBlockOptionalDataWithoutUpdate(position + new Vector3Int(0, 1, 0), MathUtility.GetByte(dataBinary), worldID);
                //   Debug.WriteLine(dataBinary[7]);
            }

        }
    }


    public struct DoorBreakingOperation : IChunkUpdateOperation
    {
        public Vector3Int position { get; set; }
        public BlockData prevBlockData;

        public int worldID { get; set; }
        public DoorBreakingOperation(Vector3Int position, BlockData prevBlockData,int worldID)
        {
            this.position = position;
            this.prevBlockData = prevBlockData;
            this.worldID = worldID;
        }
        public void Update()
        {

            BlockShape? shapeThis =
                ServerSideChunkHelper.GetBlockShape(prevBlockData);
            Debug.WriteLine(shapeThis);
            if (shapeThis is not BlockShape.Door)
            {
                return;
            }

            BlockData dataThis = prevBlockData;

            bool[] dataBinary = MathUtility.GetBooleanArray(dataThis.optionalDataValue);
            BlockShape? shapeUp =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, 1, 0), worldID));
            BlockShape? shapeDown =
                ServerSideChunkHelper.GetBlockShape(ServerSideChunkHelper.GetBlockData(position + new Vector3Int(0, -1, 0), worldID));


            //break door bottom
            if (dataBinary[5] == false)
            {
                if (shapeUp is BlockShape.Door)
                {

                    ServerSideChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, 1, 0), (short)0, worldID);

                    //   Debug.WriteLine(dataBinary[7]);
                }
            }
            else
            {

                if (shapeDown is BlockShape.Door)
                {

                    ServerSideChunkHelper.SetBlockWithoutUpdate(position + new Vector3Int(0, -1, 0), (short)0, worldID);

                    //   Debug.WriteLine(dataBinary[7]);
                }

            }



        }
    }
}
