//using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using monogameMinecraftShared.Core;
using Microsoft.Xna.Framework.Input.Touch;
using monogameMinecraftShared.Asset;

namespace monogameMinecraftShared.UI
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
        public Texture2D texturePressed;

        public bool isConstantPressable = false;
        public SpriteFont font;
        public GameWindow window;
        public Vector2 initalWidthHeight;
        public bool keepsAspectRatio = false;
        public string text { get; set; }
        public string optionalTag { get; set; }
        public bool isClickable = true;
        public UIPanel optionalBasePanel;
         
        public bool isConstantPressed=false;
        public UIButton(UIStateManager state, Vector2 position, float width, float height, Texture2D tex, Vector2 tPos, SpriteFont font, SpriteBatch sb, GameWindow window, Action<UIButton> action, string text, Action<UIButton> buttonUpdateAction, float textScale, bool keepsAspectRatio = false, bool isClickable = true, UIPanel optionalBasePanel = null, bool isConstantPressable = false, Texture2D texturePressed = null)
        {
            element00Pos = position;
            element10Pos = new Vector2(position.X + width, position.Y);
            element11Pos = new Vector2(position.X + width, position.Y + height);
            element01Pos = new Vector2(position.X, position.Y + height);
            Debug.WriteLine(element00Pos + " " + element10Pos + " " + element11Pos + " " + element01Pos);
            texture = tex;
            textPos = tPos;
            this.font = font;

            spriteBatch = sb;
            this.window = window;
            ButtonAction = action;
            this.text = text;
            this.textScale = textScale;
            OnResize(state);
            ButtonUpdateAction = buttonUpdateAction;
            this.keepsAspectRatio = keepsAspectRatio;
            if (this.keepsAspectRatio)
            {
                initalWidthHeight = new Vector2(width, height);

            }
            this.isClickable = isClickable;
            this.optionalBasePanel = optionalBasePanel;
            if (optionalBasePanel != null)
            {
                optionalBasePanel.OnResize(state);
                OnResize(state);
            }

            this.isConstantPressable = isConstantPressable;
            this.texturePressed = texturePressed;
        }
        public void Draw(UIStateManager state)
        {
            DrawString(state,null);
        }

        public void DrawString(UIStateManager state, string text1)
        {
          //  this.text = text;
            text = text == null ? " " : text;
            // ButtonRect.Center;
            if (texture != null)
            {
                if (texturePressed == null)
                {
                    spriteBatch.Draw(texture, ButtonRect, Color.White);
                }
                else
                {
                    if (isConstantPressed == true)
                    {
                        spriteBatch.Draw(texturePressed, ButtonRect, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(texture, ButtonRect, Color.White);
                    }
                }
                
            }

            textHeight = (element01Pos - element00Pos).Y;

            textPixelPos = new Vector2Int(ButtonRect.Center.X, ButtonRect.Center.Y);

            Vector2 textSize;
            if (font == null)
            {
                textSize = new Vector2(0f, 0f);
            }
            else
            {
                textSize = font.MeasureString(text) / 2f;
            }
            float textSizeScaling = state.ScreenRect.Height / (float)state.ScreenRectInital.Height * 2f * textScale;

            float horizontalTextSizeScaling = (ButtonRect.Width / (float)(textSize.X*2));
            textSize *= MathF.Min(textSizeScaling, horizontalTextSizeScaling) ;

            //   Debug.WriteLine(textSize/2f);
            // textSize.Y = 0;
            // spriteBatch.DrawString(font, text, new Vector2(textPixelPos.x,textPixelPos.y), Color.White);
            if (font != null)
            {
                spriteBatch.DrawString(font, text, new Vector2(textPixelPos.x - textSize.X, textPixelPos.y - textSize.Y), Color.White, 0f, new Vector2(0f, 0f), MathF.Min(horizontalTextSizeScaling,textSizeScaling), SpriteEffects.None, 1);
            }


        }

        public void OnResize(UIStateManager state)
        {
            GetScreenSpaceRect(state);
        }
        MouseState mouseState;
        MouseState lastMouseState;
        public bool isHovered
        {
            get
            {
                
                //     Debug.WriteLine(ButtonRect.X+" "+ ButtonRect.Y + " "+ ButtonRect.Width + " "+ ButtonRect.Height);
                //     Debug.WriteLine(UIElement.ScreenRect.X + " " + UIElement.ScreenRect.Y + " " + UIElement.ScreenRect.Width + " " + UIElement.ScreenRect.Height);
                bool isTouchHovered = false;
                foreach (var touch in UITouchscreenInputHelper.allTouches)
                {
                    if (touch.Position.X<10|| touch.Position.Y < 10)
                    {
             //           isTouchHovered = true;
                      
                    }
             //       Debug.WriteLine(touch.Position);
                    if (this.ButtonRect.Contains(touch.Position+ UITouchscreenInputHelper.screenRectOffset))
                    {
               //         Debug.WriteLine(UITouchscreenInputHelper.screenRectOffset);
                        isTouchHovered = true;
                        break;
                    }
                }

                if (ButtonRect.Contains(new Vector2(mouseState.X, mouseState.Y)))
                {
                    return true;

                }
                else
                {

                    return isTouchHovered;
                }
            }

        }
        public UIButton() { }
        public void Update(UIStateManager state)
        {
            mouseState = Mouse.GetState();

            bool isTouched = false;

            if (!isConstantPressable)
            {
                foreach (var tc in UITouchscreenInputHelper.allTouches)
                {

                    if (tc.State == TouchLocationState.Released && isHovered)
                    {
                        isTouched = true;
                        //   Debug.WriteLine("touched");
                    }
                }
                if (((isHovered && mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released) || isTouched == true) && isClickable == true)
                {
                    //   Debug.WriteLine("pressed");


                    if (UIResourcesManager.instance.uiSounds.ContainsKey("uiclick"))
                    {
                        UIResourcesManager.instance.uiSounds["uiclick"].Play(1f, 0.5f, 0f);
                    }
                    ButtonAction(this);
                }
            }
            else
            {
                foreach (var tc in UITouchscreenInputHelper.allTouches)
                {

                    if ((tc.State == TouchLocationState.Pressed|| tc.State == TouchLocationState.Moved) && isHovered)
                    {
                        isTouched = true;
                        //   Debug.WriteLine("touched");
                    }
                }


                if (((isHovered && mouseState.LeftButton == ButtonState.Pressed) || isTouched == true) && isClickable == true)
                {
                    //   Debug.WriteLine("pressed");

                    isConstantPressed = true;
                    ButtonAction(this);
                    
                }
                else
                {
                    isConstantPressed = false;
                }
            }
        

            lastMouseState = mouseState;
        }
        public void GetScreenSpaceRect(UIStateManager state)
        {
          //  Debug.WriteLine(element00Pos + " " + element01Pos + " " + element10Pos + " " + element11Pos);
            Rectangle alignedRect;
            bool originAligned = false;
            if (optionalBasePanel == null)
            {
                alignedRect = state.ScreenRect;
                originAligned= true;
            }
            else
            {
                optionalBasePanel.OnResize(state);
                alignedRect = optionalBasePanel.screenSpaceRect;
                originAligned = false;  
            }
            
            Vector2 transformedP00 = new Vector2(element00Pos.X * alignedRect.Width+alignedRect.X, element00Pos.Y * alignedRect.Height + alignedRect.Y);
            if (originAligned == true)
            {
                transformedP00 = new Vector2(element00Pos.X * alignedRect.Width, element00Pos.Y * alignedRect.Height);
            }
            float width = (element10Pos - element00Pos).X * alignedRect.Width;
            float height = (element01Pos - element00Pos).Y * alignedRect.Height;
            if (keepsAspectRatio)
            {
                if (width > height)
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

           
                ButtonRect = new Rectangle((int)transformedP00.X, (int)transformedP00.Y, (int)width, (int)height);
           
           
          //  Debug.WriteLine(ButtonRect.X + " " + ButtonRect.Y + " " + ButtonRect.Width + " " + ButtonRect.Height);

            //      this.textPixelPos = new Vector2Int((int)(textPos.X * UIElement.ScreenRect.Width), (int)(textPos.Y * UIElement.ScreenRect.Height));
      //      Debug.WriteLine(textPixelPos.x + " " + textPixelPos.y);
        }

        public void Initialize()
        {
            if (ButtonUpdateAction != null) { ButtonUpdateAction(this); }

        }
    }
}
