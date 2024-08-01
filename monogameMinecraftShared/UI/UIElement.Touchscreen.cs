using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;

namespace monogameMinecraftShared.UI
{
    public partial interface UIElement
    {

        public static TouchCollection allTouches = new TouchCollection();
        public static Vector2 screenRectOffset=new Vector2();
        public static void UpdateTouches()
        {
            allTouches = TouchPanel.GetState();
        /*    if (allTouches.Count > 0)
            {
                Debug.WriteLine(allTouches[0].Position);
            }*/
        }

        public static bool CheckIsPointColliding(ref List<UIElement> uiElements, Vector2 screenPosition)
        {
            foreach (var element in uiElements)
            {
                if (element is UIButton buttonElement)
                {
                    if (buttonElement.ButtonRect.Contains(screenPosition+ screenRectOffset))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static UIElement androidCurEditingElement = null;
        public static bool androidIsInputPanelOpened=false;
    }
}
