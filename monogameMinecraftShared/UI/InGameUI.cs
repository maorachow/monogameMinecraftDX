using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Updateables;

namespace monogameMinecraftShared.UI
{
    public class InGameUI : UIElement
    {
        public string playerCrosshair = "+";
        public SpriteFont font;
        public SpriteBatch spriteBatch;
        public GamePlayerReference gamePlayer;

        public GameWindow window;
        public Texture2D hotbarTex;
        public Texture2D selectedHotbarTex;
        public Vector2[] hotbarItemNodes;
        public float hotbarItemWidth;

        
        //   public static List<UIElement> UIElements = new List<UIElement>();
        string UIElement.text { get; set; }
        public string optionalTag { get; set; }

        public InGameUI(UIStateManager state, SpriteFont sf, GameWindow gw, SpriteBatch sb, GamePlayerReference gamePlayer, Texture2D hotbarTex, Texture2D selectedHotbar)
        {
            font = sf;
            window = gw;
            spriteBatch = sb;
            this.gamePlayer = gamePlayer;
            this.hotbarTex = hotbarTex;
            selectedHotbarTex = selectedHotbar;
            //    this.player = player;
        }
        public void Drawposition(UIStateManager state)
        {
            if (gamePlayer != null)
            {
                spriteBatch.DrawString(font, new StringBuilder("Position:" + (int)gamePlayer.gamePlayer.position.X + " " + (int)gamePlayer.gamePlayer.position.Y + " " + (int)gamePlayer.gamePlayer.position.Z), new Vector2(0, 0), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 1);
            }

        }
        public void Draw(UIStateManager state)
        {
            //   Drawposition();
            DrawString(state,null);
        }
        void DrawCrosshair(UIStateManager state)
        {
            Vector2 textSize = font.MeasureString(playerCrosshair) / 2f;
            float textSizeScaling = state.ScreenRect.Height / (float)state.ScreenRectInital.Height * 2f;
            textSize *= textSizeScaling;

            spriteBatch.DrawString(font, playerCrosshair, new Vector2(state.ScreenRect.Width / 2 - textSize.X, state.ScreenRect.Height / 2 - textSize.Y), Color.White, 0f, new Vector2(0f, 0f), textSizeScaling, SpriteEffects.None, 1);
        }
        public void DrawString(UIStateManager state,string text)
        {
            // this. Draw();
            DrawCrosshair(state);
            Drawposition(state);
            DrawHotbar(state);
        }

        public void DrawHotbar(UIStateManager state)
        {
            float hotbaraspectratio = hotbarTex.Width / (float)hotbarTex.Height;

            float textureSizeScaling = state.ScreenRect.Height / (float)state.ScreenRectInital.Height * 2f;
            int hotbarWidth = (int)(hotbarTex.Width * textureSizeScaling);
            hotbarItemWidth = hotbarWidth / 9f;
            int hotbarHeight = (int)(hotbarTex.Height * textureSizeScaling);
            Rectangle hotbarRect = new Rectangle(state.ScreenRect.Width / 2 - hotbarWidth / 2, state.ScreenRect.Height - hotbarHeight, hotbarWidth, hotbarHeight);
            spriteBatch.Draw(hotbarTex, hotbarRect, Color.White);
            for (int i = 0; i < gamePlayer.gamePlayer.inventoryData.Length; i++)
            {
                DrawBlockSpriteAtPoint(state, gamePlayer.gamePlayer.inventoryData[i], new Vector2(state.ScreenRect.Width / 2 - hotbarWidth / 2 + i * hotbarItemWidth, state.ScreenRect.Height - hotbarHeight));
            }
            DrawSelectedHotbar(state,hotbarRect);
        }
        void DrawBlockSpriteAtPoint(UIStateManager state, short blockID, Vector2 position)
        {
            if (blockID == 0) { return; }

            spriteBatch.Draw(UIResourcesManager.instance.UITextures.ContainsKey("blocktexture" + blockID) && UIResourcesManager.instance.UITextures["blocktexture" + blockID] != null ? UIResourcesManager.instance.UITextures["blocktexture" + blockID] : UIResourcesManager.instance.UITextures["blocktexture-1"], new Rectangle((int)position.X, (int)position.Y, (int)hotbarItemWidth, (int)hotbarItemWidth), Color.White);
        }
        void DrawSelectedHotbar(UIStateManager state,Rectangle hotbarRect)
        {
            float textureSizeScaling = state.ScreenRect.Height / (float)state.ScreenRectInital.Height * 2f;
            int selectedHotbarWidth = (int)(selectedHotbarTex.Width * textureSizeScaling);
            int selectedHotbarHeight = (int)(selectedHotbarTex.Height * textureSizeScaling);
            Rectangle selectedHotbarRect = new Rectangle(hotbarRect.X - (int)(3 * textureSizeScaling) + gamePlayer.gamePlayer.currentSelectedHotbar * (int)hotbarItemWidth, hotbarRect.Y - (int)(3 * textureSizeScaling), (int)hotbarItemWidth + (int)(6 * textureSizeScaling), (int)hotbarItemWidth + (int)(6 * textureSizeScaling));
            spriteBatch.Draw(selectedHotbarTex, selectedHotbarRect, Color.White);
        }
        public void GetScreenSpaceRect(UIStateManager state)
        {

        }

        public void OnResize(UIStateManager state)
        {

        }

        public void Update(UIStateManager state)
        {

        }

        public void Initialize()
        {

        }
    }
}
