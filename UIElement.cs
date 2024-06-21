﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace monogameMinecraft
{
    public interface UIElement
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
        public static List<UIElement> menuUIs = new List<UIElement>();
        public static List<UIElement> settingsUIsPage1 = new List<UIElement>();
        public static List<UIElement> settingsUIsPage2 = new List<UIElement>();
        public static int settingsUIsPageID;
        public static List<UIElement> inGameUIs = new List<UIElement>();
        public static List<UIElement> pauseMenuUIs = new List<UIElement>();
        public static List<UIElement> inventoryUIs = new List<UIElement>();
        public static Dictionary<string, SoundEffect> uiSounds = new Dictionary<string, SoundEffect>();
    }
}
