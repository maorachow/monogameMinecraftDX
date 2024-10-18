using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.VisualBasic;

namespace monogameMinecraftShared.UI
{
    // ReSharper disable all InconsistentNaming


    public enum UIStateTypes
    {
        Menu,
        Settings,
        InGame,
        InGameInventoryOpened,
        InGamePaused,
        StructureOperations,
        InGameChatMessages
    }
    public class UIStateManager
    {
        public static Dictionary<UIStateTypes, IUIState> allStates = new Dictionary<UIStateTypes, IUIState>
        {
            {
                UIStateTypes.Menu,new UIStateMenu()
            },
            {
                UIStateTypes.Settings,new UIStateSettings()
            },
            {
                UIStateTypes.InGame,new UIStateInGame()
            },
            {
                UIStateTypes.InGameInventoryOpened,new UIStateInGameInventoryOpened()
            },
            {
                UIStateTypes.InGamePaused,new UIStateInGamePaused()
            },
            {
                UIStateTypes.StructureOperations,new UIStateStructureOperations()
            },
            {
                UIStateTypes.InGameChatMessages,new UIStateInGameChatMessages()
            },

        };

        private MinecraftGameBase game;
        private UIConstructionManagerBase uiConstructionManager;
        public Rectangle ScreenRect = new Rectangle(0, 0, 800, 480);
        public Rectangle ScreenRectInital = new Rectangle(0, 0, 800, 480);
        public List<UIElement> menuUIs = new List<UIElement>();
        public List<UIElement> settingsUIsPage1 = new List<UIElement>();

        public List<UIElement> mobileInGameTOuchUIs = new List<UIElement>();
        public List<UIElement> settingsUIsPage2 = new List<UIElement>();
        public int settingsUIsPageID;
        public List<UIElement> inGameUIs = new List<UIElement>();
        public List<UIElement> pauseMenuUIs = new List<UIElement>();
        public List<UIElement> inventoryUIs = new List<UIElement>();
        public List<UIElement> structureOperationsSavingUIs = new List<UIElement>();
        public List<UIElement> structureOperationsPlacingUIs = new List<UIElement>();
        public int structureOperationsUIsPageID;
        
        public List<UIElement> chatMessagesUIs = new List<UIElement>();
        public bool isValid=false;
        public IUIState curState;

        public void SwitchToState(IUIState toState)
        {
            curState=toState;
            curState.OnAttachedToManager(this);
        }

        public void Draw()
        {
            if (!isValid)
            {
                return;
            }
            curState.Draw(this);
        }

        public void Update(float deltaTime)
        {
            if (!isValid)
            {
                return;
            }
            curState.Update(deltaTime,this);
        }

        public UIStateManager(MinecraftGameBase game)
        {
            this.game= game;
            
                uiConstructionManager = new UIConstructionManager(this, game);
          
          
        }

        public void SetUIConstructionManager(UIConstructionManagerBase customUIConstructionManager)
        {
            if (isValid == true)
            {
                Debug.WriteLine("cannot set construction manager after UI constructed");
                return;
            }
            this.uiConstructionManager= customUIConstructionManager;
        }

        public void Initialize()
        {
            uiConstructionManager.ConstructAll();
            isValid = true;
        }

    }


    public interface IUIState
    {
        public void OnAttachedToManager(UIStateManager state);
        public void Draw(UIStateManager state);
        public void Update(float deltaTime, UIStateManager state);

        public void OnResize(UIStateManager  state);
    }

    public class UIStateMenu: IUIState
    {
        public void OnAttachedToManager(UIStateManager state)
        {
            foreach (var el in state.menuUIs)
            {
                el.Initialize();
            }
            OnResize(state);
        }
        public void Draw(UIStateManager state)
        {
            foreach (var el in state.menuUIs)
            {
                el.DrawString(state,el.text);
            }
        }

        public void Update(float deltaTime, UIStateManager state)
        {
            foreach (var el in state.menuUIs)
            {
                el.Update(state);
            }
        }

        public void OnResize(UIStateManager state)
        {
            foreach (var el in state.menuUIs)
            {
                el.OnResize(state);
            }
        }
    }
    public class UIStateSettings : IUIState
    {
        public void OnAttachedToManager(UIStateManager state)
        {
            foreach (var el in state.settingsUIsPage1)
            {
                el.Initialize();
            }
            foreach (var el in state.settingsUIsPage2)
            {
                el.Initialize();
            }
            OnResize(state);
        }
        public void Draw(UIStateManager state)
        {
            switch (state.settingsUIsPageID)
            {
                case 0:
                    foreach (var el in state.settingsUIsPage1)
                    {
                        el.DrawString(state, el.text);
                    }
                    break;
                    case 1:
                    foreach (var el in state.settingsUIsPage2)
                    {
                        el.DrawString(state, el.text);
                    }
                    break;
            }
           
        }

        public void Update(float deltaTime, UIStateManager state)
        {
            switch (state.settingsUIsPageID)
            {
                case 0:
                    foreach (var el in state.settingsUIsPage1)
                    {
                        el.Update(state);
                    }
                    break;
                case 1:
                    foreach (var el in state.settingsUIsPage2)
                    {
                        el.Update(state);
                    }
                    break;
            }
        }

        public void OnResize(UIStateManager state)
        {
            foreach (var el in state.settingsUIsPage1)
            {
                el.OnResize(state);
            }
            foreach (var el in state.settingsUIsPage2)
            {
                el.OnResize(state);
            }
        }
    }





    public class UIStateInGame : IUIState
    {
        public void OnAttachedToManager(UIStateManager state)
        {
            OnResize(state);
        }
        public void Draw(UIStateManager state)
        {
           
                    foreach (var el in state.inGameUIs)
                    {
                        el.DrawString(state, el.text);
                    }
          

        }

        public void Update(float deltaTime, UIStateManager state)
        {
            
                    foreach (var el in state.inGameUIs)
                    {
                        el.Update(state);
                    }
              
        }

        public void OnResize(UIStateManager state)
        {
            foreach (var el in state.inGameUIs)
            {
                el.OnResize(state);
            }
          
        }
    }



    public class UIStateInGameInventoryOpened : IUIState
    {
        public void OnAttachedToManager(UIStateManager state)
        {
            OnResize(state);
        }
        public void Draw(UIStateManager state)
        {

            foreach (var el in state.inGameUIs)
            {
                el.DrawString(state, el.text);
            }

            foreach (var el in state.inventoryUIs)
            {
                el.DrawString(state, el.text);
            }
        }

        public void Update(float deltaTime, UIStateManager state)
        {

            foreach (var el in state.inventoryUIs)
            {
                el.Update(state);
            }

        }

        public void OnResize(UIStateManager state)
        {
            foreach (var el in state.inGameUIs)
            {
                el.OnResize(state);
            }
            foreach (var el in state.inventoryUIs)
            {
                el.OnResize(state);
            }

        }
    }



    public class UIStateInGamePaused : IUIState
    {
        public void OnAttachedToManager(UIStateManager state)
        {
            OnResize(state);
        }
        public void Draw(UIStateManager state)
        {

            foreach (var el in state.inGameUIs)
            {
                el.DrawString(state, el.text);
            }

            foreach (var el in state.pauseMenuUIs)
            {
                el.DrawString(state, el.text);
            }
        }

        public void Update(float deltaTime, UIStateManager state)
        {

            foreach (var el in state.pauseMenuUIs)
            {
                el.Update(state);
            }

        }

        public void OnResize(UIStateManager state)
        {
            foreach (var el in state.inGameUIs)
            {
                el.OnResize(state);
            }
            foreach (var el in state.pauseMenuUIs)
            {
                el.OnResize(state);
            }

        }
    }


    public class UIStateStructureOperations : IUIState
    {
        public void OnAttachedToManager(UIStateManager state)
        {
            OnResize(state);
        }
        public void Draw(UIStateManager state)
        {

            switch (state.structureOperationsUIsPageID)
            {
                case 0:
                    foreach (var el in state.structureOperationsSavingUIs)
                    {
                        el.DrawString(state,el.text);
                    }

                    break;
                case 1:
                    foreach (var el in state.structureOperationsPlacingUIs)
                    {
                        el.DrawString(state, el.text);
                    }

                    break;
                default:
                    foreach (var el in state.structureOperationsSavingUIs)
                    {
                        el.DrawString(state, el.text);
                    }

                    break;
            }

        }

        public void Update(float deltaTime, UIStateManager state)
        {

            switch (state.structureOperationsUIsPageID)
            {
                case 0:
                    foreach (var el in state.structureOperationsSavingUIs)
                    {
                        el.Update(state);
                    }

                    break;
                case 1:
                    foreach (var el in state.structureOperationsPlacingUIs)
                    {
                        el.Update(state);
                    }

                    break;
                default:
                    foreach (var el in state.structureOperationsSavingUIs)
                    {
                        el.Update(state);
                    }

                    break;
            }

        }

        public void OnResize(UIStateManager state)
        {
            foreach (var el in state.structureOperationsPlacingUIs)
            {
                el.OnResize(state);
            }
            foreach (var el in state.structureOperationsSavingUIs)
            {
                el.OnResize(state);
            }

        }
    }


    public class UIStateInGameChatMessages : IUIState
    {
        public void OnAttachedToManager(UIStateManager state)
        {
            OnResize(state);
        }
        public void Draw(UIStateManager state)
        {

            foreach (var el in state.inGameUIs)
            {
                el.DrawString(state, el.text);
            }

            foreach (var el in state.chatMessagesUIs)
            {
                el.DrawString(state, el.text);
            }
        }

        public void Update(float deltaTime, UIStateManager state)
        {

            foreach (var el in state.chatMessagesUIs)
            {
                el.Update(state);
            }

        }

        public void OnResize(UIStateManager state)
        {
            foreach (var el in state.inGameUIs)
            {
                el.OnResize(state);
            }
            foreach (var el in state.chatMessagesUIs)
            {
                el.OnResize(state);
            }

        }
    }
}
