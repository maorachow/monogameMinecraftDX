using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftShared.UI
{
    public partial interface UIElement
    {

        public static TouchCollection allTouches = new TouchCollection();

        public static void UpdateTouches()
        {
            allTouches = TouchPanel.GetState();
        }
    }
}
