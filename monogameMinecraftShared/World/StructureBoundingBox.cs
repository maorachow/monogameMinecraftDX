using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;

namespace monogameMinecraftShared.World
{
    public struct StructureBoundingBox
    {

        public Vector3Int min;
        public Vector3Int max;

        public bool Intersects(BoundingBox box)
        {
            return box.Intersects(new BoundingBox((Vector3)min, (Vector3)max));

        }
    }
}
