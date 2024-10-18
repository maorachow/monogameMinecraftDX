using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;

namespace monogameMinecraftShared.UI
{
    public partial interface UIElement
    {
       
        public void GetScreenSpaceRect(UIStateManager state);
        public void Draw(UIStateManager state);
        public void DrawString(UIStateManager state,string text);
        public void Update(UIStateManager state);
        public void Initialize();
        public void OnResize(UIStateManager state);
        public string text { get; set; }

        public string optionalTag { get; set; }
        [Obsolete]
        
        public static Dictionary<string, Texture2D> UITextures = new Dictionary<string, Texture2D>();

        [Obsolete]
        public static Rectangle ScreenRect = new Rectangle(0, 0, 800, 480);

        [Obsolete]
        public static Rectangle ScreenRectInital = new Rectangle(0, 0, 800, 480);

        [Obsolete]
        public static List<UIElement> menuUIs = new List<UIElement>();

        [Obsolete]
        public static List<UIElement> settingsUIsPage1 = new List<UIElement>();

        [Obsolete]
        public static List<UIElement> mobileInGameTOuchUIs = new List<UIElement>();

        [Obsolete]
        public static List<UIElement> settingsUIsPage2 = new List<UIElement>();

        [Obsolete]
        public static int settingsUIsPageID;

        [Obsolete]
        public static List<UIElement> inGameUIs = new List<UIElement>();

        [Obsolete]
        public static List<UIElement> pauseMenuUIs = new List<UIElement>();

        [Obsolete]
        public static List<UIElement> inventoryUIs = new List<UIElement>();

        [Obsolete]
        public static List<UIElement> structureOperationsSavingUIs = new List<UIElement>();

        [Obsolete]
        public static List<UIElement> structureOperationsPlacingUIs = new List<UIElement>();

        [Obsolete]
        public static int structureOperationsUIsPageID;

        [Obsolete]
        public static Dictionary<string, SoundEffect> uiSounds = new Dictionary<string, SoundEffect>();

        [Obsolete]
        public static List<UIElement> chatMessagesUIs = new List<UIElement>();  

    }
}
