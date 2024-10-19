using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftShared.Core;
using Microsoft.Xna.Framework.Input.Touch;
using monogameMinecraftShared.Asset;

namespace monogameMinecraftShared.UI
{


    public class InputField : UIElement
    {
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        
        public static bool CapsLockStatus
        {
            get
            {
                byte[] bs = new byte[256];
                GetKeyboardState(bs);
                return bs[0x14] == 1;
            }
        }
        public Rectangle inputFieldRect;

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
        public Texture2D selectedTexture;
        public SpriteFont font;
        public GameWindow window;
        public Vector2 initalWidthHeight;

        public bool numbersOnly = false;
        //  public bool keepsAspectRatio = false;
        public string text { get; set; }
        public string optionalTag { get; set; }
        public bool leftAligned=false;
        public float leftAlignedOffset = 0;
        public int pixelOffset = 0;
        public bool isSelected = false;
        public int maxAllowedCharacters;
        public bool useEnterActions=false;
        public Action<InputField> onTextChangedAction;
        public Action<InputField> onEnterPressedAction;
        public InputField(UIStateManager state, Vector2 position, float width, float height, Texture2D tex, Texture2D texSelected, SpriteFont font, SpriteBatch sb, GameWindow window, Action<InputField> action, string text, float textScale, int maxAllowedCharacters, bool numbersOnly,bool leftAligned=false,float leftAlignedOffset = 0,bool useEnterActions=false)
        {
            element00Pos = position;
            element10Pos = new Vector2(position.X + width, position.Y);
            element11Pos = new Vector2(position.X + width, position.Y + height);
            element01Pos = new Vector2(position.X, position.Y + height);
            Debug.WriteLine(element00Pos + " " + element10Pos + " " + element11Pos + " " + element01Pos);
            texture = tex;
            selectedTexture = texSelected;


            this.font = font;
            this.numbersOnly = numbersOnly;
            spriteBatch = sb;
            this.window = window;
            onTextChangedAction = action;
            this.text = text;
            this.textScale = textScale;
            this.maxAllowedCharacters = maxAllowedCharacters;
            this.leftAligned= leftAligned;
            this.leftAlignedOffset = leftAlignedOffset;
            this.useEnterActions = useEnterActions;
            OnResize(state);



        }
        public void Draw(UIStateManager state)
        {
            DrawString(state,null);
        }

        public void DrawString(UIStateManager state,string text1)
        {
            //  this.text = text;
            this.text = text == null ? " " : text;
            // ButtonRect.Center;
            if (texture != null && selectedTexture != null)
            {
                spriteBatch.Draw(isSelected == true ? selectedTexture : texture, inputFieldRect, Color.White);
            }

            textHeight = (element01Pos - element00Pos).Y;

            textPixelPos = new Vector2Int(inputFieldRect.Center.X, inputFieldRect.Center.Y);

            Vector2 textSize;
            if (font == null)
            {
                textSize = new Vector2(0f, 0f);
            }
            else
            {
                textSize = font.MeasureString(text) / 2f;
            }

            float textSizeScaling=1f;


           /* if (leftAligned == true)
            {
               
               //     textSizeScaling = (MathF.Min(inputFieldRect.Height, inputFieldRect.Width))  / ((float)MathF.Min(textSize.X, textSize.Y) * 2f) * textScale;
              
              //      textSizeScaling = inputFieldRect.Width / ((float)textSize.X * 2f) * textScale;
               
            }
            else
            { 
                if (state.ScreenRect.Height < state.ScreenRect.Width)
                {
                    textSizeScaling = inputFieldRect.Height / ((float)textSize.Y * 2f) * textScale;
                }
                else
                {
                    textSizeScaling = inputFieldRect.Width / ((float)textSize.X * 2f) * textScale;
                }
                textSizeScaling = (MathF.Min(inputFieldRect.Height, inputFieldRect.Width)) / ((float)MathF.Min(textSize.X, textSize.Y) * 2f) * textScale;
            }*/
            float textScalingVertical  = inputFieldRect.Height / ((float)textSize.Y * 2f) * textScale;

            float textScalingHorizontal  = inputFieldRect.Width / ((float)textSize.X * 2f) * textScale;
            textSizeScaling = MathF.Min(textScalingHorizontal, textScalingVertical);

            textSize *= textSizeScaling;

            //   Debug.WriteLine(textSize/2f);
            // textSize.Y = 0;
            // spriteBatch.DrawString(font, text, new Vector2(textPixelPos.x,textPixelPos.y), Color.White);
            if (font != null)
            {
                if (leftAligned == true)
                {
                    spriteBatch.DrawString(font, text, new Vector2(inputFieldRect.X+ pixelOffset, textPixelPos.y - textSize.Y), Color.White, 0f, new Vector2(0f, 0f), textSizeScaling, SpriteEffects.None, 1);
                }
                else
                {
                    spriteBatch.DrawString(font, text, new Vector2(textPixelPos.x - textSize.X, textPixelPos.y - textSize.Y), Color.White, 0f, new Vector2(0f, 0f), textSizeScaling, SpriteEffects.None, 1);
                }
              
            }


        }

        public int TryParseInt()
        {
            int value;
            bool sucess = int.TryParse(text, out value);

            if (!sucess)
            {
                return int.MinValue;
            }
            return value;
        }
        public void OnResize(UIStateManager state)
        {
            GetScreenSpaceRect(state);
        }
        MouseState mouseState;
        MouseState lastMouseState;
        KeyboardState keyboardState;
        private KeyboardState lastKeyboardState;
        public bool isHovered
        {
            get
            {
                //     Debug.WriteLine(ButtonRect.X+" "+ ButtonRect.Y + " "+ ButtonRect.Width + " "+ ButtonRect.Height);
                //     Debug.WriteLine(UIElement.ScreenRect.X + " " + UIElement.ScreenRect.Y + " " + UIElement.ScreenRect.Width + " " + UIElement.ScreenRect.Height);
                bool isTouchHovered = false;

                foreach (var touch in UITouchscreenInputHelper.allTouches)
                {
                    if (this.inputFieldRect.Contains(touch.Position + UITouchscreenInputHelper.screenRectOffset))
                    {
                        isTouchHovered = true;
                        break;
                    }
                }

                if (inputFieldRect.Contains(new Vector2(mouseState.X, mouseState.Y)))
                {
                    return true;

                }
                else
                {

                    return isTouchHovered;
                }
            }

        }
        public InputField() { }
        public void Update(UIStateManager state)
        {
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            bool isTouched = false;
            foreach (var tc in UITouchscreenInputHelper.allTouches)
            {

                if (tc.State == TouchLocationState.Released)
                {
                    isTouched = true;
                    //   Debug.WriteLine("touched");
                }
            }
            if ((mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)||isTouched==true)
            {
                //   Debug.WriteLine("pressed");

                if (isHovered)
                {
                    isSelected = true;
                    if (UIResourcesManager.instance.uiSounds.ContainsKey("uiclick"))
                    {
                        UIResourcesManager.instance.uiSounds["uiclick"].Play(1f, 0.5f, 0f);
                    }

                    UITouchscreenInputHelper.androidCurEditingElement = this;
                    UITouchscreenInputHelper.androidIsInputPanelOpened= true;
                }
                else
                {
                    isSelected = false;
                }


            }

            lastMouseState = mouseState;
            if (isSelected)
            {
                if (keyboardState.GetPressedKeyCount() > 0)
                {
                    foreach (var key in keyboardState.GetPressedKeys())
                    {
                        //   Debug.WriteLine(key);
                        if (key == Keys.Enter)
                        {
                            if (lastKeyboardState.IsKeyDown(Keys.Enter) == false && text.Length > 0)
                            {
                                if (onEnterPressedAction != null)
                                {
                                    onEnterPressedAction(this);
                                }
                                break;
                            }
                        }
                        if (key == Keys.Back)
                        {
                            if (lastKeyboardState.IsKeyDown(Keys.Back) == false && text.Length > 0)
                            {
                                text = text.Remove(text.Length - 1, 1);
                                break;
                            }
                        }
                   
                        if (lastKeyboardState.IsKeyDown(key) == false)
                        {
                            if (text.Length < maxAllowedCharacters)
                            {
                                if (key >= (Keys)48 && key <= (Keys)57)
                                {
                                    if (key == Keys.D1&&(keyboardState.IsKeyDown(Keys.LeftShift)|| keyboardState.IsKeyDown(Keys.RightShift)))
                                    {
                                        text += "!";
                                    }
                                    else
                                    {
                                        string keyString = key.ToString();
                                        keyString = keyString.Remove(0, 1);
                                        text += keyString;
                                    }
                                   
                                }

                                if (key == Keys.Subtract || key == Keys.OemMinus)
                                {
                                    string keyString = "-";

                                    text += keyString;
                                }
                                if (key == Keys.OemPeriod)
                                {
                                    string keyString = ".";

                                    text += keyString;
                                }

                                if (key == Keys.OemComma)
                                {
                                    string keyString = ",";

                                    text += keyString;
                                }
                                if (numbersOnly == false)
                                {
                                    if (key >= (Keys)65 && key <= (Keys)90)
                                    {
                                        string keyString = key.ToString();
                                        if (CapsLockStatus == false)
                                        {
                                            keyString = keyString.ToLower();



                                        }
                                        text += keyString;

                                    }

                                    if (key == Keys.Space)
                                    {
                                        text += " ";
                                    }
                                }
                            }

                        }

                        if (onTextChangedAction != null)
                        {
                            onTextChangedAction(this);
                        }
                    }
                }
            }
            lastKeyboardState = keyboardState;

        }
        public void GetScreenSpaceRect(UIStateManager state)
        {
            Debug.WriteLine(element00Pos + " " + element01Pos + " " + element10Pos + " " + element11Pos);

            Vector2 transformedP00 = new Vector2(element00Pos.X * state.ScreenRect.Width, element00Pos.Y * state.ScreenRect.Height);
            float width = (element10Pos - element00Pos).X * state.ScreenRect.Width;
            float height = (element01Pos - element00Pos).Y * state.ScreenRect.Height;
            /* if (keepsAspectRatio)
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
             }*/
            inputFieldRect = new Rectangle((int)transformedP00.X, (int)transformedP00.Y, (int)width, (int)height);
            this.pixelOffset = (int)((float)inputFieldRect.Width * (float)leftAlignedOffset);
            Debug.WriteLine(inputFieldRect.X + " " + inputFieldRect.Y + " " + inputFieldRect.Width + " " + inputFieldRect.Height);

            //      this.textPixelPos = new Vector2Int((int)(textPos.X * UIElement.ScreenRect.Width), (int)(textPos.Y * UIElement.ScreenRect.Height));
            Debug.WriteLine(textPixelPos.x + " " + textPixelPos.y);
        }

        public void Initialize()
        {

        }
    }
}
