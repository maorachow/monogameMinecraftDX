using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.World;
namespace monogameMinecraftShared.Physics
{

    public static class BlockBoundingBoxUtility
    {

        public static bool IsBlockWithBoundingBox(BlockShape shape)
        {
            switch (shape)
            {
                case BlockShape.Fence:
                    return true;
                case BlockShape.Door:
                    return true;
                case BlockShape.Solid:
                    return true;
                case BlockShape.SolidTransparent:
                    return true;
                case BlockShape.Stairs:
                    return true;
                case BlockShape.WallAttachment:
                    return false;
                default:
                    return false;
            }
        }
        public static bool IsBlockWithBoundingBox(BlockData data)
        {
            if (data.blockID == 0)
            {
                return false;
            }
            if (!Chunk.blockInfosNew.ContainsKey(data.blockID))
            {
                return false;
            }
            BlockShape shape = Chunk.blockInfosNew[data.blockID].shape;
            switch (shape)
            {
                case BlockShape.Fence:
                    return true;
                case BlockShape.Door:
                    return true;
                case BlockShape.Solid:
                    return true;
                case BlockShape.SolidTransparent:
                    return true;
                case BlockShape.Stairs:
                    return true;
                case BlockShape.WallAttachment:
                    return false;
                default:
                    return false;
            }
        }
        public static BoundingBox GetBoundingBox(int x, int y, int z, BlockData blockData)
        {
            if (blockData.blockID == 0)
            {
                return new BoundingBox();
            }
            BlockShape shape = Chunk.blockInfosNew[blockData].shape;
            if (shape == BlockShape.Solid)
            {
                return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));

            }
            if (shape == BlockShape.SolidTransparent)
            {
                return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));

            }
            if (shape == BlockShape.Slabs)
            {
                switch (blockData.optionalDataValue)
                {
                    case 0:
                        return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 0.5f, z + 1));
                    case 1:
                        return new BoundingBox(new Vector3(x, y + 0.5f, z), new Vector3(x + 1, y + 1f, z + 1));
                    case 2:
                        return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));
                }
            }

            if (shape == BlockShape.Fence)
            {
                bool[] fenceDatabools = MathUtility.GetBooleanArray(blockData.optionalDataValue);
                Vector3 boxMinPoint = new Vector3(x + 0.375f, y, z + 0.375f);
                Vector3 boxMaxPoint = new Vector3(x + 0.625f, y + 1.5f, z + 0.625f);
                bool isLeftBuilt = fenceDatabools[7];
                bool isRightBuilt = fenceDatabools[6];
                bool isBackBuilt = fenceDatabools[5];
                bool isFrontBuilt = fenceDatabools[4];
                if (isLeftBuilt)
                {
                    boxMinPoint.X = x + 0f;
                }

                if (isRightBuilt)
                {
                    boxMaxPoint.X = x + 1f;
                }

                if (isBackBuilt)
                {
                    boxMinPoint.Z = z + 0f;
                }

                if (isFrontBuilt)
                {
                    boxMaxPoint.Z = z + 1f;
                }
                return new BoundingBox(boxMinPoint, boxMaxPoint);

            }

            if (shape == BlockShape.Door)
            {
                bool[] doorDataBools = MathUtility.GetBooleanArray(blockData.optionalDataValue);


                byte doorFaceID = 0;
                Vector3 boxMin = new Vector3(x, y, z);
                Vector3 boxMax = new Vector3(x + 1, y + 1, z + 1);
                if (doorDataBools[6] == false)
                {
                    if (doorDataBools[7] == false)
                    {
                        doorFaceID = 0;
                    }
                    else
                    {
                        doorFaceID = 1;
                    }
                }
                else
                {
                    if (doorDataBools[7] == false)
                    {
                        doorFaceID = 2;
                    }
                    else
                    {
                        doorFaceID = 3;
                    }
                }

                bool isOpen = doorDataBools[4];

                switch (doorFaceID)
                {
                    case 0:
                        if (!isOpen)
                        {
                            boxMin = new Vector3(x, y, z);
                            boxMax = new Vector3(x + 0.1875f, y + 1, z + 1);
                        }
                        else
                        {
                            boxMin = new Vector3(x, y, z + 1 - 0.1875f);
                            boxMax = new Vector3(x + 1, y + 1, z + 1);
                        }

                        break;
                    case 1:
                        if (!isOpen)
                        {
                            boxMin = new Vector3(x + 1 - 0.1875f, y, z);
                            boxMax = new Vector3(x + 1, y + 1, z + 1f);
                        }
                        else
                        {
                            boxMin = new Vector3(x, y, z);
                            boxMax = new Vector3(x + 1, y + 1, z + 0.1875f);
                        }

                        break;
                    case 2:
                        if (!isOpen)
                        {
                            boxMin = new Vector3(x, y, z);
                            boxMax = new Vector3(x + 1, y + 1, z + 0.1875f);

                        }
                        else
                        {
                            boxMin = new Vector3(x, y, z);
                            boxMax = new Vector3(x + 0.1875f, y + 1, z + 1);
                        }

                        break;
                    case 3:
                        if (!isOpen)
                        {
                            boxMin = new Vector3(x, y, z + 1 - 0.1875f);
                            boxMax = new Vector3(x + 1, y + 1, z + 1);
                        }
                        else
                        {
                            boxMin = new Vector3(x + 1 - 0.1875f, y, z);
                            boxMax = new Vector3(x + 1, y + 1, z + 1f);
                        }

                        break;
                }



                return new BoundingBox(boxMin, boxMax);

            }

            if (shape == BlockShape.WallAttachment)
            {
                return new BoundingBox();
            }

            return new BoundingBox();

        }

        public static BoundingBox GetBoundingBoxSelectable(int x, int y, int z, BlockData blockData)
        {
            if (blockData.blockID == 0)
            {
                return new BoundingBox();
            }
            BlockShape shape = Chunk.blockInfosNew[blockData].shape;
            if (shape == BlockShape.Solid)
            {
                return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));

            }
            if (shape == BlockShape.SolidTransparent)
            {
                return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));

            }
            if (shape == BlockShape.Slabs)
            {
                switch (blockData.optionalDataValue)
                {
                    case 0:
                        return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 0.5f, z + 1));
                    case 1:
                        return new BoundingBox(new Vector3(x, y + 0.5f, z), new Vector3(x + 1, y + 1f, z + 1));
                    case 2:
                        return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));
                }
            }
            if (shape == BlockShape.Torch)
            {

                return new BoundingBox(new Vector3(x + 0.25f, y, z + 0.25f), new Vector3(x + 0.75f, y + 0.75f, z + +0.75f));


            }
            if (shape == BlockShape.CrossModel)
            {

                return new BoundingBox(new Vector3(x + 0.25f, y, z + 0.25f), new Vector3(x + 0.75f, y + 0.75f, z + +0.75f));


            }
            if (shape == BlockShape.Water)
            {
                return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1));
            }

            if (shape == BlockShape.Fence)
            {
                return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1f, y + 1f, z + 1f));
            }
            if (shape == BlockShape.Door)
            {
                return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1f, y + 1f, z + 1f));
            }
            if (shape == BlockShape.WallAttachment)
            {
                return new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1f, y + 1f, z + 1f));
            }
            return new BoundingBox();

        }
    }
}
