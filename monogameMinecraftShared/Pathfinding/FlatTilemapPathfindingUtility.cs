using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;

namespace monogameMinecraftShared.Pathfinding
{
    public static class FlatTilemapPathfindingUtility
    {
        public static int GetHeuristicPriority(Vector2Int a, Vector2Int b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }
        public static WalkablePath FindPathByChunkHeightMap(int[,] heightMap, Vector2Int mapSpaceStart, Vector2Int mapSpaceTarget)
        {
            int curHeight = heightMap[mapSpaceStart.x, mapSpaceStart.y];
            PriorityQueue<Vector2Int, int> frontier = new PriorityQueue<Vector2Int, int>();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            bool[,] reached = new bool[heightMap.GetLength(0), heightMap.GetLength(1)];
            frontier.Enqueue(mapSpaceStart, 0);
            reached[mapSpaceStart.x, mapSpaceStart.y] = true;
            int pathfindingSteps = 0;
            while (frontier.Count > 0)
            {
                Vector2Int current = frontier.Dequeue();
                if (current.x == mapSpaceTarget.x && current.y == mapSpaceTarget.y)
                {
                    break;

                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2Int next = new Vector2Int();
                    switch (i)
                    {
                        case 0://left
                            next = current + new Vector2Int(-1, 0);
                            break;
                        case 1://right
                            next = current + new Vector2Int(1, 0);
                            break;
                        case 2://back
                            next = current + new Vector2Int(0, -1);
                            break;
                        case 3://front
                            next = current + new Vector2Int(0, 1);
                            break;
                    }

                    bool isNextValid = !(next.x < 0 || next.x > heightMap.GetLength(0) - 1 || next.y < 0 || next.y > heightMap.GetLength(1) - 1);
                    if (isNextValid == true && reached[next.x, next.y] == true)
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true && cameFrom.ContainsKey(next))
                    {
                        isNextValid = false;
                    }
                    curHeight = heightMap[current.x, current.y];
                    if (isNextValid == true && curHeight < heightMap[next.x, next.y] && Math.Abs(curHeight - heightMap[next.x, next.y]) > 1)
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true)
                    {
                        frontier.Enqueue(next, GetHeuristicPriority(next, mapSpaceTarget));
                        cameFrom[next] = current;
                    }
                }

                pathfindingSteps++;
            }
            Debug.WriteLine("pathfinding steps:" + pathfindingSteps);
            List<Vector3> finalPath = new List<Vector3>();
            Vector2Int goalPos = mapSpaceTarget;
            while (goalPos != mapSpaceStart)
            {
                if (!cameFrom.ContainsKey(goalPos))
                {
                    break;
                }
                finalPath.Add(new Vector3(goalPos.x + 0.5f, heightMap[goalPos.x, goalPos.y] + 0.5f, goalPos.y + 0.5f));
                goalPos = cameFrom[goalPos];
            }
            finalPath.Add(new Vector3(mapSpaceStart.x + 0.5f, heightMap[mapSpaceStart.x, mapSpaceStart.y] + 0.5f, mapSpaceStart.y + 0.5f));
            finalPath.Reverse();
            return new WalkablePath(finalPath);
        }

        static Random rand = new Random();
        public static WalkablePath FindWorldSpacePathByChunkHeightMap(int[,] heightMap, Vector3Int origin, Vector3Int worldSpaceStart, Vector3Int worldSpaceTarget, out bool isPathValid)
        {

            if (heightMap.GetLength(0) <= 0 || heightMap.GetLength(1) <= 0)
            {
                isPathValid = false;
                return null;
            }
            Vector2Int mapSpaceStart = new Vector2Int(Math.Clamp(worldSpaceStart.x - origin.x, 0, heightMap.GetLength(0) - 1), Math.Clamp(worldSpaceStart.z - origin.z, 0, heightMap.GetLength(1) - 1));
            Vector2Int mapSpaceTarget = new Vector2Int(Math.Clamp(worldSpaceTarget.x - origin.x, 0, heightMap.GetLength(0) - 1), Math.Clamp(worldSpaceTarget.z - origin.z, 0, heightMap.GetLength(1) - 1));


            int curHeight = heightMap[mapSpaceStart.x, mapSpaceStart.y];
            PriorityQueue<Vector2Int, int> frontier = new PriorityQueue<Vector2Int, int>();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            bool[,] reached = new bool[heightMap.GetLength(0), heightMap.GetLength(1)];
            frontier.Enqueue(mapSpaceStart, 0);
            reached[mapSpaceStart.x, mapSpaceStart.y] = true;
            int pathfindingSteps = 0;
            while (frontier.Count > 0)
            {
                Vector2Int current = frontier.Dequeue();
                if (current.x == mapSpaceTarget.x && current.y == mapSpaceTarget.y)
                {
                    break;

                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2Int next = new Vector2Int();
                    switch (i)
                    {
                        case 0://left
                            next = current + new Vector2Int(-1, 0);
                            break;
                        case 1://right
                            next = current + new Vector2Int(1, 0);
                            break;
                        case 2://back
                            next = current + new Vector2Int(0, -1);
                            break;
                        case 3://front
                            next = current + new Vector2Int(0, 1);
                            break;
                        default:
                            Debug.WriteLine("invalid direction");
                            break;
                    }

                    bool isNextValid = !(next.x < 0 || next.x > heightMap.GetLength(0) - 1 || next.y < 0 || next.y > heightMap.GetLength(1) - 1);
                    if (isNextValid == true && reached[next.x, next.y] == true)
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true && cameFrom.ContainsKey(next))
                    {
                        isNextValid = false;
                    }
                    curHeight = heightMap[current.x, current.y];
                    if (isNextValid == true && curHeight < heightMap[next.x, next.y] && Math.Abs(curHeight - heightMap[next.x, next.y]) > 1)
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true)
                    {
                        frontier.Enqueue(next, GetHeuristicPriority(next, mapSpaceTarget));
                        reached[next.x, next.y] = true;
                        cameFrom[next] = current;
                    }
                }

                pathfindingSteps++;
            }
            //    Debug.WriteLine("pathfinding steps:" + pathfindingSteps);
            List<Vector3> finalPath = new List<Vector3>();
            Vector2Int goalPos = mapSpaceTarget;
            bool hasFoundTarget = true;
            while (goalPos != mapSpaceStart)
            {
                if (!cameFrom.ContainsKey(goalPos))
                {
                    hasFoundTarget = false;
                    break;
                }
                finalPath.Add(new Vector3(goalPos.x + 0.5f, heightMap[goalPos.x, goalPos.y], goalPos.y + 0.5f) + (Vector3)origin);
                goalPos = cameFrom[goalPos];
            }
            finalPath.Add(new Vector3(mapSpaceStart.x + 0.5f, heightMap[mapSpaceStart.x, mapSpaceStart.y], mapSpaceStart.y + 0.5f) + (Vector3)origin);
            finalPath.Reverse();

            isPathValid = hasFoundTarget;
            return new WalkablePath(finalPath);
        }
    }
}
