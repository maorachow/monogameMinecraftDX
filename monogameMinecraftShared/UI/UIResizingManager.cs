using monogameMinecraftShared.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace monogameMinecraftShared.UI
{
    public class UIResizingManager
    {
        private UIStateManager stateManager;

        public UIResizingManager(UIStateManager state)
        {
            this.stateManager= state;
        }
        public void Resize(MinecraftGameBase game)
        {
            stateManager.curState.OnResize(this.stateManager);
          /*  if (game is MinecraftGameBase)
            {
                MinecraftGameBase game1= (MinecraftGameBase)game;
                UIElement.ScreenRect = game.Window.ClientBounds;
                Debug.WriteLine("screenRect:" + UIElement.ScreenRect);
                foreach (UIElement element in UIElement.menuUIs)
                {
                    element.OnResize(stateManager);
                }
                foreach (UIElement element1 in UIElement.settingsUIsPage1)
                {
                    element1.OnResize(stateManager);
                }
                foreach (UIElement element1 in UIElement.settingsUIsPage2)
                {
                    element1.OnResize(stateManager);
                }
                foreach (UIElement element1 in UIElement.pauseMenuUIs)
                {
                    element1.OnResize(stateManager);
                }
                foreach (UIElement element1 in UIElement.inventoryUIs)
                {
                    element1.OnResize(stateManager);
                }
                foreach (UIElement element1 in UIElement.structureOperationsSavingUIs)
                {
                    element1.OnResize(stateManager);
                }

                foreach (UIElement element1 in UIElement.inGameUIs)
                {
                    element1.OnResize(stateManager);
                }
                foreach (UIElement element1 in UIElement.structureOperationsPlacingUIs)
                {
                    element1.OnResize(stateManager);
                }
                switch (game1.status)
                {
                    case GameStatus.Started:
                        foreach (UIElement element1 in UIElement.pauseMenuUIs)
                        {
                            element1.OnResize(stateManager);
                        }
                        foreach (UIElement element1 in UIElement.inventoryUIs)
                        {
                            element1.OnResize(stateManager);
                        }

                        break;
                }
            }*/
      
        }
    }
}
