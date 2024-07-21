using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.Rendering;
using monogameMinecraftDX.Updateables;
using monogameMinecraftDX.Utility;
using monogameMinecraftDX.World;
using ThreadState = System.Diagnostics.ThreadState;

namespace monogameMinecraftDX.Pathfinding
{
    public class PathfindingManager
    {
        public WalkablePath curDebuggingPath;
        public bool drawDebugLines = true;
        public PathfindingManager()
        {
            entityPathfindingQueue = new ConcurrentDictionary<EntityBeh, (Vector3Int pathStart, Vector3Int pathTarget)>();
        }

        public void Initialize()
        {
            isThreadQuitting = false;
            findEntityPathThread = new Thread(FindEntityPathsThread);
            findEntityPathThread.IsBackground = true;
            findEntityPathThread.Start();
        }
        public WalkablePath GetFlatTilemapPath(Vector3Int start, Vector3Int target,out bool isPathValid)
        {
            int distance = Math.Max(Math.Abs(start.x - target.x), Math.Abs(start.z - target.z));
            if (distance > 1 * Chunk.chunkWidth)
            {
                isPathValid = false;
                return null;
            }

            Vector3Int heightMapOrigin = new Vector3Int(Math.Min(start.x, target.x), 0, Math.Min(start.z, target.z))-new Vector3Int(8,0,8);
            Vector3Int heightMapMax = new Vector3Int(Math.Max(start.x, target.x), 0, Math.Max(start.z, target.z)) + new Vector3Int(8, 0, 8);
            int[,] heightMap = ChunkHelper.GetAreaHeightMap(heightMapOrigin, heightMapMax.x - heightMapOrigin.x,
                heightMapMax.z - heightMapOrigin.z);
            WalkablePath path= FlatTilemapPathfindingUtility.FindWorldSpacePathByChunkHeightMap(heightMap, heightMapOrigin, start, target,
               out var isFoundPathValid);
            isPathValid = isFoundPathValid;
            return path;
        }

        public WalkablePath GetThreeDimensionalMapPath(Vector3Int start, Vector3Int target, out bool isPathValid )
        {
            int distance = Math.Max(Math.Abs(start.x - target.x),Math.Max(Math.Abs(start.z - target.z), Math.Abs(start.y - target.y)));
            if (distance > 1 * Chunk.chunkWidth)
            {
                isPathValid = false;
                return null;
            }

            Vector3Int mapOrigin = new Vector3Int(Math.Min(start.x, target.x), Math.Min(start.y, target.y), Math.Min(start.z, target.z)) - new Vector3Int(8,8, 8);
            Vector3Int mapMax = new Vector3Int(Math.Max(start.x, target.x), Math.Max(start.y, target.y), Math.Max(start.z, target.z)) + new Vector3Int(8, 8,8);
           BlockData[,,] map = ChunkHelper.GetBlocks(mapOrigin, mapMax.x - mapOrigin.x,
                mapMax.y - mapOrigin.y, mapMax.z - mapOrigin.z);
            WalkablePath path = ThreeDimensionalMapPathfindingUtility.FindWorldSpacePathByBlockData(map, mapOrigin, start, target,
                out var isFoundPathValid);
            isPathValid = isFoundPathValid;
        //    VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureOrigin = mapOrigin;
        //    VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureSize =mapMax- mapOrigin;
            return path;
        }

        public ConcurrentDictionary<EntityBeh,(Vector3Int pathStart, Vector3Int pathTarget)> entityPathfindingQueue;
        public bool isThreadQuitting=false;

        public void QuitThread()
        {
            isThreadQuitting = true;
            findEntityPathThread?.Join();
        }

        public Thread findEntityPathThread;
         
        public void FindEntityPathsThread()
        {
            while (true)
            {
                if (isThreadQuitting == true)
                {
                    return;
                }
                 
                if (entityPathfindingQueue.Count > 0)
                {
                    
                   KeyValuePair< EntityBeh,( Vector3Int pathStart, Vector3Int pathTarget)> item= entityPathfindingQueue.First();
                    bool isPathValid = false;
                    GetThreeDimensionalMapPath(item.Value. pathStart, item.Value.pathTarget, out isPathValid, ref item.Key.entityPath);
                    item.Key.isPathValid= isPathValid;
                    entityPathfindingQueue.Remove(item.Key,out _);
                }
            }
        }

        public void GetThreeDimensionalMapPathAsync(Vector3Int start, Vector3Int target, EntityBeh entity)
        {
            entityPathfindingQueue.TryAdd(entity,(start, target));
        }
        public void GetThreeDimensionalMapPath(Vector3Int start, Vector3Int target, out bool isPathValid, ref WalkablePath result)
        {
        //    Debug.WriteLine("thread id:"+Thread.CurrentThread.ManagedThreadId);
            int distance = Math.Max(Math.Abs(start.x - target.x), Math.Max(Math.Abs(start.z - target.z), Math.Abs(start.y - target.y)));
            if (distance > 1 * Chunk.chunkWidth)
            {
                isPathValid = false;
                return;
            }

            Vector3Int mapOrigin = new Vector3Int(Math.Min(start.x, target.x), Math.Min(start.y, target.y), Math.Min(start.z, target.z)) - new Vector3Int(8, 8, 8);
            Vector3Int mapMax = new Vector3Int(Math.Max(start.x, target.x), Math.Max(start.y, target.y), Math.Max(start.z, target.z)) + new Vector3Int(8, 8, 8);
            BlockData[,,] map = ChunkHelper.GetBlocks(mapOrigin, mapMax.x - mapOrigin.x,
                mapMax.y - mapOrigin.y, mapMax.z - mapOrigin.z);
           
           
           
                result = ThreeDimensionalMapPathfindingUtility.FindWorldSpacePathByBlockData(map, mapOrigin, start,
                    target,
                    out var isFoundPathValid1);
                isPathValid = isFoundPathValid1;
            
          
           
            //    VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureOrigin = mapOrigin;
            //    VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureSize =mapMax- mapOrigin;
            return;
        }
        public void DrawDebuggingPath(Vector3 origin,GamePlayer player,RenderPipelineManager rpm)
        {
            if (!drawDebugLines)
            {
                return;
            }
        //    rpm.walkablePathVisualizationRenderer.VisualizeWalkablePath(curDebuggingPath, player.cam.viewMatrix, player.cam.projectionMatrix, origin);
            foreach (var entity in EntityManager.worldEntities)
            {
                if ((ZombieEntityBeh)entity != null)
                {
                    rpm.walkablePathVisualizationRenderer.VisualizeWalkablePath(((ZombieEntityBeh)entity).entityPath, player.cam.viewMatrix, player.cam.projectionMatrix, new Vector3(0,0,0));
                }
             
               
            }
            
        }
    }
}
