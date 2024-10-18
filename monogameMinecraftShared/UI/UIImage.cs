using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameMinecraftShared.UI
{
    public class UIImage : UIElement
    {
        public Rectangle imageRect;


        public Vector2 element00Pos;
        public Vector2 element01Pos;
        public Vector2 element11Pos;
        public Vector2 element10Pos;
        public string text { get; set; }
        public string optionalTag { get; set; }
        SpriteBatch spriteBatch;
        public Texture2D texture;
 
        public UIImage(UIStateManager state, Vector2 position, float width, float height, Texture2D tex, SpriteBatch sb)
        {
            element00Pos = position;
            element10Pos = new Vector2(position.X + width, position.Y);
            element11Pos = new Vector2(position.X + width, position.Y + height);
            element01Pos = new Vector2(position.X, position.Y + height);
            texture = tex;


            spriteBatch = sb;
            OnResize(state);
        }
        public void OnResize(UIStateManager state)
        {
            GetScreenSpaceRect(state);
        }
        public void DrawString(UIStateManager state, string text)
        {
            //   this.text = text;
            //     text = text == null ? " " : text;

            spriteBatch.Draw(texture, imageRect, Color.White);
        }
        public void Update(UIStateManager state) { }
        public void Draw(UIStateManager state)
        {
            DrawString(state,null);
        }
        public void GetScreenSpaceRect(UIStateManager state)
        {
            Vector2 transformedP00 = new Vector2(element00Pos.X * state.ScreenRect.Width, element00Pos.Y * state.ScreenRect.Height);
            float width = (element10Pos - element00Pos).X * state.ScreenRect.Width;
            float height = (element01Pos - element00Pos).Y * state.ScreenRect.Height;
            imageRect = new Rectangle((int)transformedP00.X, (int)transformedP00.Y, (int)width, (int)height);


        }

        public void Initialize()
        {

        }
    }
}
