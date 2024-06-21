//using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace monogameMinecraft
{

    /*
     0,0               1,0
     
     
     
     

    0,1                1,1
     */
    public class UIButton : UIElement
    {
        public Rectangle ButtonRect;
        public Action<UIButton> ButtonAction;
        public Action<UIButton> ButtonUpdateAction;
        public Vector2Int textPixelPos;
        public Vector2 textPos;
        public Vector2 textWH;
        public float textHeight;
        public float textScale;
        public Vector2 element00Pos;
        public Vector2 element01Pos;
        public Vector2 element11Pos;
        public Vector2 element10Pos;
        // public string text="123";
        SpriteBatch spriteBatch;
        public Texture2D texture;
        public SpriteFont font;
        public GameWindow window;
        public Vector2 initalWidthHeight;
        public bool keepsAspectRatio=false;
        public string text { get; set; }
        public bool isClickable = true;
 
        public UIButton(Vector2 position, float width, float height, Texture2D tex, Vector2 tPos, SpriteFont font, SpriteBatch sb, GameWindow window, Action<UIButton> action, string text, Action<UIButton> buttonUpdateAction, float textScale,bool keepsAspectRatio=false,bool isClickable=true)
        {
            element00Pos = position;
            element10Pos = new Vector2(position.X + width, position.Y);
            element11Pos = new Vector2(position.X + width, position.Y + height);
            element01Pos = new Vector2(position.X, position.Y + height);
            Debug.WriteLine(element00Pos + " " + element10Pos + " " + element11Pos + " " + element01Pos);
            this.texture = tex;
            this.textPos = tPos;
            this.font = font;

            this.spriteBatch = sb;
            this.window = window;
            this.ButtonAction = action;
            this.text = text;
            this.textScale = textScale;
            OnResize();
            ButtonUpdateAction = buttonUpdateAction;
            this.keepsAspectRatio = keepsAspectRatio;
            if (this.keepsAspectRatio)
            {
                initalWidthHeight=new Vector2(width, height);

            }
            this.isClickable = isClickable;
        }
        public void Draw()
        {
            DrawString(null);
        }

        public void DrawString(string text)
        {
            this.text = text;
            text = text == null ? " " : text;
            // ButtonRect.Center;
            if(texture!= null)
            {
                spriteBatch.Draw(texture, ButtonRect, Color.White);
            }
           
            textHeight = (element01Pos - element00Pos).Y;

            this.textPixelPos = new Vector2Int(ButtonRect.Center.X, ButtonRect.Center.Y);
          
            Vector2 textSize;
            if (font == null)
            {
                textSize = new Vector2(0f, 0f);
            }
            else
            {
                textSize = font.MeasureString(text) / 2f;
            }
            float textSizeScaling = ((float)UIElement.ScreenRect.Height / (float)UIElement.ScreenRectInital.Height) * 2f * textScale;
            textSize *= textSizeScaling;

            //   Debug.WriteLine(textSize/2f);
            // textSize.Y = 0;
            // spriteBatch.DrawString(font, text, new Vector2(textPixelPos.x,textPixelPos.y), Color.White);
            if(font!=null) {   spriteBatch.DrawString(font, text, new Vector2(textPixelPos.x - textSize.X, textPixelPos.y - textSize.Y), Color.White, 0f, new Vector2(0f, 0f), textSizeScaling, SpriteEffects.None, 1);
            }

          
        }

        public void OnResize()
        {
            this.GetScreenSpaceRect();
        }
        MouseState mouseState;
        MouseState lastMouseState;
        public bool isHovered
        {
            get
            {
                //     Debug.WriteLine(ButtonRect.X+" "+ ButtonRect.Y + " "+ ButtonRect.Width + " "+ ButtonRect.Height);
                //     Debug.WriteLine(UIElement.ScreenRect.X + " " + UIElement.ScreenRect.Y + " " + UIElement.ScreenRect.Width + " " + UIElement.ScreenRect.Height);
                if (this.ButtonRect.Contains(new Vector2(mouseState.X, mouseState.Y)))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }

        }
        public UIButton() { }
        public void Update()
        {
            mouseState = Mouse.GetState();
            if (isHovered && mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released&&isClickable==true)
            {
                //   Debug.WriteLine("pressed");


                if (UIElement.uiSounds.ContainsKey("uiclick"))
                {
                    UIElement.uiSounds["uiclick"].Play(1f, 0.5f, 0f);
                }
                ButtonAction(this);
            }

            lastMouseState = mouseState;
        }
        public void GetScreenSpaceRect()
        {
            Debug.WriteLine(element00Pos + " " + element01Pos + " " + element10Pos + " " + element11Pos);
        
            Vector2 transformedP00 = new Vector2((element00Pos.X * UIElement.ScreenRect.Width), (element00Pos.Y * UIElement.ScreenRect.Height));
            float width = (element10Pos - element00Pos).X * UIElement.ScreenRect.Width;
            float height = (element01Pos - element00Pos).Y * UIElement.ScreenRect.Height;
            if (keepsAspectRatio)
            {
                if(width > height)
                {
                   float width1 = height * (initalWidthHeight.Y / initalWidthHeight.X);
                    float originOffsetW = MathF.Abs(width1 - width) / 2f;
                    width = width1;
                    transformedP00.X += originOffsetW;
                }
                else
                {
                    float height1 = width * (initalWidthHeight.X / initalWidthHeight.Y);
                    float originOffsetH = MathF.Abs(height1 - height) / 2f;
                    height = height1;
                    transformedP00.Y += originOffsetH;
                }
              
          //    
             
                //        element10Pos = new Vector2(element00Pos.X + initalWidthHeight.X, element00Pos.Y);
                //       element11Pos = new Vector2(element00Pos.X + initalWidthHeight.X, element00Pos.Y + initalWidthHeight.Y+(initalWidthHeight.X / initalWidthHeight.Y));
                //        element01Pos = new Vector2(element00Pos.X, element00Pos.Y  +initalWidthHeight.Y*(initalWidthHeight.X/ initalWidthHeight.Y));
            }
            this.ButtonRect = new Rectangle((int)transformedP00.X, (int)transformedP00.Y, (int)width, (int)height);
            Debug.WriteLine(ButtonRect.X + " " + ButtonRect.Y + " " + ButtonRect.Width + " " + ButtonRect.Height);

            //      this.textPixelPos = new Vector2Int((int)(textPos.X * UIElement.ScreenRect.Width), (int)(textPos.Y * UIElement.ScreenRect.Height));
            Debug.WriteLine(this.textPixelPos.x + " " + this.textPixelPos.y);
        }

        public void Initialize()
        {
            if (ButtonUpdateAction != null) { ButtonUpdateAction(this); }

        }
    }
}
