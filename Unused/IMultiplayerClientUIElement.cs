using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;

namespace monogameMinecraftShared.UI
{
    public partial interface IMultiplayerClientUIElement
    {
        public static Dictionary<string, Texture2D> UITextures = new Dictionary<string, Texture2D>();
        public static Rectangle ScreenRect = new Rectangle(0, 0, 800, 480);
        public static Rectangle ScreenRectInital = new Rectangle(0, 0, 800, 480);
        public void GetScreenSpaceRect();
        public void Draw();
        public void DrawString(string text);
        public void Update();
        public void Initialize();
        public void OnResize();
        public string text { get; set; }
        public static List<IMultiplayerClientUIElement> menuUIs = new List<IMultiplayerClientUIElement>();
 
        public static List<IMultiplayerClientUIElement> inGameUIs = new List<IMultiplayerClientUIElement>();
        public static List<IMultiplayerClientUIElement> pauseMenuUIs = new List<IMultiplayerClientUIElement>();
        public static List<IMultiplayerClientUIElement> inventoryUIs = new List<IMultiplayerClientUIElement>();
    /*    public static List<UIElement> structureOperationsSavingUIs = new List<UIElement>();
        public static List<UIElement> structureOperationsPlacingUIs = new List<UIElement>();
        public static int structureOperationsUIsPageID;*/
        public static Dictionary<string, SoundEffect> uiSounds = new Dictionary<string, SoundEffect>();

    }
}
