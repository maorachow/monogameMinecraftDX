using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace monogameMinecraftShared.Physics
{
    public interface IMovableCollider
    {

        public List<BoundingBox> GetBlocksAround(BoundingBox aabb);
        public BoundingBox bounds { get; set; }
        public Vector3 position { get; set; }
    }
}
