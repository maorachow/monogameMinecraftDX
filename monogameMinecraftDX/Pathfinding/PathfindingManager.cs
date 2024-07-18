using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.Rendering;
using monogameMinecraftDX.Updateables;
using monogameMinecraftDX.Utility;
using monogameMinecraftDX.World;

namespace monogameMinecraftDX.Pathfinding
{
    public class PathfindingManager
    {
        public WalkablePath curDebuggingPath;
        public bool drawDebugLines = true;
        public PathfindingManager()
        {
             
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
        public void DrawDebuggingPath(Vector3 origin,GamePlayer player,RenderPipelineManager rpm)
        {
            if (!drawDebugLines)
            {
                return;
            }
            foreach (var entity in EntityManager.worldEntities)
            {
                if ((ZombieEntityBeh)entity != null)
                {
                    rpm.walkablePathVisualizationRenderer.VisualizeWalkablePath(((ZombieEntityBeh)entity).entityPath, player.cam.viewMatrix, player.cam.projectionMatrix, origin);
                }
             
               
            }
            
        }
    }
}
