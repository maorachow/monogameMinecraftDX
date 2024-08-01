using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using monogameMinecraftNetworking.Client;

namespace monogameMinecraftShared.UI
{
    public class InGameUI : IMultiplayerClientUIElement
    {
        public string playerCrosshair = "+";
        public SpriteFont font;
        public SpriteBatch spriteBatch;
        public ClientGameBase game;
        public GameWindow window;
        public Texture2D hotbarTex;
        public Texture2D selectedHotbarTex;
        public Vector2[] hotbarItemNodes;
        public float hotbarItemWidth;
        //   public static List<UIElement> UIElements = new List<UIElement>();
        public string text { get; set; }
        public InGameUI(SpriteFont sf, GameWindow gw, SpriteBatch sb,ClientGameBase game, Texture2D hotbarTex, Texture2D selectedHotbar)
        {
            font = sf;
            window = gw;
            spriteBatch = sb;
            this.game = game;
            this.hotbarTex = hotbarTex;
            selectedHotbarTex = selectedHotbar;
            //    this.player = player;
        }
        public void Drawposition()
        {
            if (game.gamePlayer != null)
            {
                spriteBatch.DrawString(font, new StringBuilder("Position:" + (int)game.gamePlayer.position.X + " " + (int)game.gamePlayer.position.Y + " " + (int)game.gamePlayer.position.Z), new Vector2(0, 0), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 1);
            }

        }
        public void Draw()
        {
            //   Drawposition();
            DrawString(null);
        }
        void DrawCrosshair()
        {
            Vector2 textSize = font.MeasureString(playerCrosshair) / 2f;
            float textSizeScaling = IMultiplayerClientUIElement.ScreenRect.Height / (float)IMultiplayerClientUIElement.ScreenRectInital.Height * 2f;
            textSize *= textSizeScaling;

            spriteBatch.DrawString(font, playerCrosshair, new Vector2(IMultiplayerClientUIElement.ScreenRect.Width / 2 - textSize.X, IMultiplayerClientUIElement.ScreenRect.Height / 2 - textSize.Y), Color.White, 0f, new Vector2(0f, 0f), textSizeScaling, SpriteEffects.None, 1);
        }
        public void DrawString(string text)
        {
            // this. Draw();
            DrawCrosshair();
            Drawposition();
            DrawHotbar();
        }

        public void DrawHotbar()
        {
            float hotbaraspectratio = hotbarTex.Width / (float)hotbarTex.Height;

            float textureSizeScaling = IMultiplayerClientUIElement.ScreenRect.Height / (float)IMultiplayerClientUIElement.ScreenRectInital.Height * 2f;
            int hotbarWidth = (int)(hotbarTex.Width * textureSizeScaling);
            hotbarItemWidth = hotbarWidth / 9f;
            int hotbarHeight = (int)(hotbarTex.Height * textureSizeScaling);
            Rectangle hotbarRect = new Rectangle(IMultiplayerClientUIElement.ScreenRect.Width / 2 - hotbarWidth / 2, IMultiplayerClientUIElement.ScreenRect.Height - hotbarHeight, hotbarWidth, hotbarHeight);
            spriteBatch.Draw(hotbarTex, hotbarRect, Color.White);
            for (int i = 0; i < game.gamePlayer.inventoryData.Length; i++)
            {
                DrawBlockSpriteAtPoint(game.gamePlayer.inventoryData[i], new Vector2(IMultiplayerClientUIElement.ScreenRect.Width / 2 - hotbarWidth / 2 + i * hotbarItemWidth, IMultiplayerClientUIElement.ScreenRect.Height - hotbarHeight));
            }
            DrawSelectedHotbar(hotbarRect);
        }
        void DrawBlockSpriteAtPoint(short blockID, Vector2 position)
        {
            if (blockID == 0) { return; }

            spriteBatch.Draw(IMultiplayerClientUIElement.UITextures.ContainsKey("blocktexture" + blockID) && IMultiplayerClientUIElement.UITextures["blocktexture" + blockID] != null ? IMultiplayerClientUIElement.UITextures["blocktexture" + blockID] : IMultiplayerClientUIElement.UITextures["blocktexture-1"], new Rectangle((int)position.X, (int)position.Y, (int)hotbarItemWidth, (int)hotbarItemWidth), Color.White);
        }
        void DrawSelectedHotbar(Rectangle hotbarRect)
        {
            float textureSizeScaling = IMultiplayerClientUIElement.ScreenRect.Height / (float)IMultiplayerClientUIElement.ScreenRectInital.Height * 2f;
            int selectedHotbarWidth = (int)(selectedHotbarTex.Width * textureSizeScaling);
            int selectedHotbarHeight = (int)(selectedHotbarTex.Height * textureSizeScaling);
            Rectangle selectedHotbarRect = new Rectangle(hotbarRect.X - (int)(3 * textureSizeScaling) + game.gamePlayer.currentSelectedHotbar * (int)hotbarItemWidth, hotbarRect.Y - (int)(3 * textureSizeScaling), (int)hotbarItemWidth + (int)(6 * textureSizeScaling), (int)hotbarItemWidth + (int)(6 * textureSizeScaling));
            spriteBatch.Draw(selectedHotbarTex, selectedHotbarRect, Color.White);
        }
        public void GetScreenSpaceRect()
        {

        }

        public void OnResize()
        {

        }

        public void Update()
        {

        }

        public void Initialize()
        {

        }
    }
}
