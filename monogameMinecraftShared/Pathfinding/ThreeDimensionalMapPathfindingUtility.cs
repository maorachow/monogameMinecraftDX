using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Physics;
using monogameMinecraftShared.World;

namespace monogameMinecraftShared.Pathfinding
{
    public static class ThreeDimensionalMapPathfindingUtility
    {
        public static int GetHeuristicPriority(Vector3Int a, Vector3Int b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }

        public static int GetHeuristicPriorityGroundWeighted(Vector3Int a, Vector3Int b, bool isOnGround)
        {
            return isOnGround ? (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2 : Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }

        public static WalkablePath FindPathByBlockData(BlockData[,,] map, Vector3Int mapSpaceStart, Vector3Int mapSpaceTarget)
        {
            //      Debug.WriteLine("start:"+mapSpaceStart.ToString());


            //     Debug.WriteLine("target:"+mapSpaceTarget.ToString());
            PriorityQueue<Vector3Int, int> frontier = new PriorityQueue<Vector3Int, int>();
            Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

            bool[,,] reached = new bool[map.GetLength(0), map.GetLength(1), map.GetLength(2)];
            frontier.Enqueue(mapSpaceStart, 0);
            reached[mapSpaceStart.x, mapSpaceStart.y, mapSpaceStart.z] = true;
            int pathfindingSteps = 0;
            while (frontier.Count > 0)
            {
                Vector3Int current = frontier.Dequeue();
                if (current.x == mapSpaceTarget.x && current.y == mapSpaceTarget.y && current.z == mapSpaceTarget.z)
                {
                    break;

                }

                for (int i = 0; i < 6; i++)
                {
                    Vector3Int next = new Vector3Int();
                    switch (i)
                    {
                        case 0://left
                            next = current + new Vector3Int(-1, 0, 0);
                            break;
                        case 1://right
                            next = current + new Vector3Int(1, 0, 0);
                            break;
                        case 2://back
                            next = current + new Vector3Int(0, 0, -1);
                            break;
                        case 3://front
                            next = current + new Vector3Int(0, 0, 1);
                            break;


                        case 4://bottom
                            next = current + new Vector3Int(0, -1, 0);
                            break;
                        case 5://up
                            next = current + new Vector3Int(0, 1, 0);
                            break;
                    }

                    bool isNextValid = !(next.x < 0 || next.x > map.GetLength(0) - 1 || next.y < 0 || next.y > map.GetLength(1) - 1 || next.z < 0 || next.z > map.GetLength(2) - 1);
                    if (isNextValid == true && reached[next.x, next.y, next.z] == true)
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true && cameFrom.ContainsKey(next))
                    {
                        Debug.WriteLine("contains next");
                        isNextValid = false;
                    }
                    if (isNextValid == true && BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, next.y, next.z]))
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true && !BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, next.y, next.z]) && !BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, next.y - 2, next.z]))
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true)
                    {
                        frontier.Enqueue(next, GetHeuristicPriority(next, mapSpaceTarget));
                        cameFrom[next] = current;
                        reached[next.x, next.y, next.z] = true;
                    }
                }

                pathfindingSteps++;
            }
            //    Debug.WriteLine("pathfinding steps:" + pathfindingSteps);
            List<Vector3> finalPath = new List<Vector3>();
            Vector3Int goalPos = mapSpaceTarget;
            foreach (var el in cameFrom)
            {
                //   Debug.WriteLine(el.Key.ToString());
            }
            while (goalPos != mapSpaceStart)
            {
                if (!cameFrom.ContainsKey(goalPos))
                {
                    //              Debug.WriteLine("not containing goalPos");
                    break;
                }
                finalPath.Add(new Vector3(goalPos.x + 0.5f, goalPos.y + 0.5f, goalPos.z + 0.5f));
                goalPos = cameFrom[goalPos];
            }
            finalPath.Add(new Vector3(mapSpaceStart.x + 0.5f, mapSpaceStart.y + 0.5f, mapSpaceStart.z + 0.5f));
            finalPath.Reverse();
            return new WalkablePath(finalPath);
        }



        public static WalkablePath FindWorldSpacePathByBlockData(BlockData[,,] map, Vector3Int origin, Vector3Int worldSpaceStart, Vector3Int worldSpaceTarget, out bool isPathValid)
        {


            if (map.GetLength(0) <= 0 || map.GetLength(1) <= 0 || map.GetLength(2) <= 0)
            {
                isPathValid = false;
                return null;
            }
            //     Debug.WriteLine("worldStart:" + worldSpaceStart.ToString());


            //   Debug.WriteLine("worldTarget:" + worldSpaceTarget.ToString());
            Vector3Int mapSpaceStart = new Vector3Int(Math.Clamp(worldSpaceStart.x - origin.x, 0, map.GetLength(0) - 1), Math.Clamp(worldSpaceStart.y - origin.y, 0, map.GetLength(1) - 1), Math.Clamp(worldSpaceStart.z - origin.z, 0, map.GetLength(2) - 1));
            Vector3Int mapSpaceTarget = new Vector3Int(Math.Clamp(worldSpaceTarget.x - origin.x, 0, map.GetLength(0) - 1), Math.Clamp(worldSpaceTarget.y - origin.y, 0, map.GetLength(1) - 1), Math.Clamp(worldSpaceTarget.z - origin.z, 0, map.GetLength(2) - 1));

            //     Debug.WriteLine("start:" + mapSpaceStart.ToString());


            //   Debug.WriteLine("target:" + mapSpaceTarget.ToString());



            PriorityQueue<Vector3Int, int> frontier = new PriorityQueue<Vector3Int, int>();
            Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

            bool[,,] reached = new bool[map.GetLength(0), map.GetLength(1), map.GetLength(2)];
            frontier.Enqueue(mapSpaceStart, 0);
            reached[mapSpaceStart.x, mapSpaceStart.y, mapSpaceStart.z] = true;
            int pathfindingSteps = 0;
            while (frontier.Count > 0)
            {
                Vector3Int current = frontier.Dequeue();
                if (current.x == mapSpaceTarget.x && current.y == mapSpaceTarget.y && current.z == mapSpaceTarget.z)
                {
                    break;

                }

                for (int i = 0; i < 6; i++)
                {
                    Vector3Int next = new Vector3Int();
                    switch (i)
                    {
                        case 0://left
                            next = current + new Vector3Int(-1, 0, 0);
                            break;
                        case 1://right
                            next = current + new Vector3Int(1, 0, 0);
                            break;
                        case 2://back
                            next = current + new Vector3Int(0, 0, -1);
                            break;
                        case 3://front
                            next = current + new Vector3Int(0, 0, 1);
                            break;


                        case 4://bottom
                            next = current + new Vector3Int(0, -1, 0);
                            break;
                        case 5://up
                            next = current + new Vector3Int(0, 1, 0);
                            break;
                        default:

                            Debug.WriteLine("invalid direction");
                            break;
                    }

                    bool isNextValid = !(next.x < 0 || next.x > map.GetLength(0) - 1 || next.y < 0 || next.y > map.GetLength(1) - 1 || next.z < 0 || next.z > map.GetLength(2) - 1);
                    if (isNextValid == true && reached[next.x, next.y, next.z] == true)
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true && cameFrom.ContainsKey(next))
                    {

                        isNextValid = false;
                    }
                    if (isNextValid == true && BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, next.y, next.z]))
                    {
                        isNextValid = false;
                    }

                    if (isNextValid == true && !BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, next.y, next.z]) && !BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, Math.Clamp(next.y - 2, 0, map.GetLength(1) - 1), next.z]))
                    {
                        isNextValid = false;
                    }
                    /*   if (isNextValid == true && !BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, next.y, next.z]) && BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, Math.Clamp(next.y +1, 0, map.GetLength(1) - 1), next.z]))
                       {
                           isNextValid = false;
                       }*/
                    if (isNextValid == true)
                    {
                        frontier.Enqueue(next, GetHeuristicPriorityGroundWeighted(next, mapSpaceTarget, BlockBoundingBoxUtility.IsBlockWithBoundingBox(map[next.x, next.y - 1, next.z])));
                        cameFrom[next] = current;
                        reached[next.x, next.y, next.z] = true;
                    }
                }

                pathfindingSteps++;
            }
            //      Debug.WriteLine("pathfinding steps:" + pathfindingSteps);
            List<Vector3> finalPath = new List<Vector3>();
            Vector3Int goalPos = mapSpaceTarget;
            bool hasFoundTarget = true;

            while (goalPos != mapSpaceStart)
            {
                if (!cameFrom.ContainsKey(goalPos))
                {
                    hasFoundTarget = false;
                    //              Debug.WriteLine("not containing goalPos");
                    break;
                }
                finalPath.Add(new Vector3(goalPos.x + 0.5f, goalPos.y, goalPos.z + 0.5f) + (Vector3)origin);
                goalPos = cameFrom[goalPos];
            }
            finalPath.Add(new Vector3(mapSpaceStart.x + 0.5f, mapSpaceStart.y, mapSpaceStart.z + 0.5f) + (Vector3)origin);
            finalPath.Reverse();
            isPathValid = hasFoundTarget;
            return new WalkablePath(finalPath);
        }
    }
}
