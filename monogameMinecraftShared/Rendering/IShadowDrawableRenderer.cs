using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameMinecraftShared.Rendering
{
    public interface IShadowDrawableRenderer
    {
        public void DrawShadow(Matrix shadowMat,Effect shadowMapShader);
    }
}
