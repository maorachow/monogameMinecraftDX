using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.World;
namespace monogameMinecraftDX.Physics
{

    public static class BlockBoundingBoxUtility
    {
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
                Vector3 boxMinPoint=new Vector3(x+ 0.375f, y, z + 0.375f);
                Vector3 boxMaxPoint = new Vector3(x + 0.625f, y+1.5f, z + 0.625f);
                bool isLeftBuilt = fenceDatabools[7];
                bool isRightBuilt = fenceDatabools[6];
                bool isBackBuilt = fenceDatabools[5];
                bool isFrontBuilt = fenceDatabools[4];
                if (isLeftBuilt)
                {
                    boxMinPoint.X = x+ 0f;
                }

                if (isRightBuilt)
                {
                    boxMaxPoint.X = x+ 1f;
                }

                if (isBackBuilt)
                {
                    boxMinPoint.Z =z+ 0f;
                }

                if (isFrontBuilt)
                {
                    boxMaxPoint.Z=z+ 1f;
                }
                return new BoundingBox(boxMinPoint, boxMaxPoint);

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
              
                        return new BoundingBox(new Vector3(x+0.25f, y, z + 0.25f), new Vector3(x +0.75f, y + 0.75f, z + +0.75f));
               
            
            }
            if (shape == BlockShape.CrossModel)
            {

                return new BoundingBox(new Vector3(x + 0.25f, y, z + 0.25f), new Vector3(x + 0.75f, y + 0.75f, z + +0.75f));


            }
            if(shape== BlockShape.Water)
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
            return new BoundingBox();

        }
    }
}
