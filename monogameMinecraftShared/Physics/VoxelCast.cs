using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
 
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.World;

namespace monogameMinecraftShared.Physics
{
    public static partial class VoxelCast
    {

    //    public static VisualizationLine line = new VisualizationLine(null, new Vector3(0, 0, 0), new Vector3(0, 1, 0), 1, new Color(1, 1, 1));
      //  public static List<BoundingBox> blocksTmp = new List<BoundingBox>();
        public static void Cast(Ray ray, int radius, out Vector3Int result, out BlockFaces resultFaces, GamePlayer player)
        {
            //     blocksTmp.Clear();

            float minDist = 10000f;
            resultFaces = BlockFaces.PositiveY;
            result = new Vector3Int(-1, -1, -1);
            for (int i = -radius + 1; i < radius; i++)
            {
                for (int j = -radius + 1; j < radius; j++)
                {
                    for (int k = -radius + 1; k < radius; k++)
                    {
                        Vector3Int blockPos = ChunkHelper.Vec3ToBlockPos(new Vector3(ray.origin.X + i, ray.origin.Y + j, ray.origin.Z + k));
                        BlockData data = ChunkHelper.GetBlockData(blockPos);
                        BoundingBox blockBounds;

                        blockBounds = BlockBoundingBoxUtility.GetBoundingBoxSelectable(blockPos.x, blockPos.y, blockPos.z, data);


                        //     blocksTmp.Add(blockBounds);
                        BlockFaces face1 = BlockFaces.PositiveY;
                        float? resultDis = ray.Intersects(blockBounds, out face1);

                        //  Debug.WriteLine(resultDis != null);
                        if (resultDis != null)
                        {
                            if (minDist > resultDis.Value)
                            {
                                result = blockPos;
                                resultFaces = face1;
                                minDist = resultDis.Value;
                            }

                        }
                    }
                }
            }

            //   Debug.WriteLine(result);
        }


      
    }
}
