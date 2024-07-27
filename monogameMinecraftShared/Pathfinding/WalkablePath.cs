using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace monogameMinecraftShared.Pathfinding
{
    public class WalkablePath
    {
        public List<Vector3> steps;
        public int curStep = 0;

        public WalkablePath(List<Vector3> steps)
        {
            this.steps = steps;
        }
    }
}
