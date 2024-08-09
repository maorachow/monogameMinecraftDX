using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameMinecraftShared.UI
{
    public class TextListUI:UIElement
    {
        public Vector2 element00Pos;
        public Vector2 element01Pos;
        public Vector2 element11Pos;
        public Vector2 element10Pos;
        public UIPanel optionalBasePanel;
        public bool keepsAspectRatio;
        public Rectangle textListRect;
        public Vector2 initalWidthHeight;
        public List<string> texts;

        public Texture2D texture;
        public SpriteFont font;
        public SpriteBatch spriteBatch;
        public int maxAllowedLines;
        public float textScale;
        public TextListUI(Vector2 position, float width, float height, Texture2D tex, SpriteFont font, SpriteBatch sb, float textScale,int maxAllowedLines,UIPanel optionalBasePanel=null)
        {
            element00Pos = position;
            element10Pos = new Vector2(position.X + width, position.Y);
            element11Pos = new Vector2(position.X + width, position.Y + height);
            element01Pos = new Vector2(position.X, position.Y + height);
            Debug.WriteLine(element00Pos + " " + element10Pos + " " + element11Pos + " " + element01Pos);
            texture = tex;
            
            this.font = font;

            spriteBatch = sb;
            
            this.text = text;
           
            OnResize();
            this.textScale= textScale;
            this.maxAllowedLines=maxAllowedLines;
            if (optionalBasePanel != null)
            {
                this.optionalBasePanel= optionalBasePanel;
            }
            this.keepsAspectRatio = keepsAspectRatio;
            if (this.keepsAspectRatio)
            {
                initalWidthHeight = new Vector2(width, height);

            }
           
            this.optionalBasePanel = optionalBasePanel;
            if (optionalBasePanel != null)
            {
                optionalBasePanel.OnResize();
                OnResize();
            }

            texts = new List<string>();


        }
        public object textAppendLock=new object();

        public void AppendText(string text)
        {
            lock (textAppendLock)
            {
                texts.Insert(0, text);
            }
         
        }
        public void GetScreenSpaceRect()
        {
            Rectangle alignedRect;
            bool originAligned = false;
            if (optionalBasePanel == null)
            {
                alignedRect = UIElement.ScreenRect;
                originAligned = true;
            }
            else
            {
                optionalBasePanel.OnResize();
                alignedRect = optionalBasePanel.screenSpaceRect;
                originAligned = false;
            }

            Vector2 transformedP00 = new Vector2(element00Pos.X * alignedRect.Width + alignedRect.X, element00Pos.Y * alignedRect.Height + alignedRect.Y);
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


            textListRect = new Rectangle((int)transformedP00.X, (int)transformedP00.Y, (int)width, (int)height);

        }

        public void Draw()
        {
            spriteBatch.Draw(texture,textListRect,Color.White);
            float curPixel = textListRect.Height - ((float)textListRect.Height / maxAllowedLines);
            lock (textAppendLock)
            {
                for (int i=0;i<texts.Count;i++)
                {
                    if (i >=maxAllowedLines)
                    {
                        continue;
                    }

                
                    string text1= texts[i];
                    Vector2 textSize;
                    if (font == null)
                    {
                        textSize = new Vector2(0f, 0f);
                    }
                    else
                    {
                        try
                        {
                            textSize = font.MeasureString(text1);
                        }
                        catch
                        {
                            textSize = new Vector2(0f, 0f);
                        }

                    }
                    float textSizeScaling = (textListRect.Height / (float)maxAllowedLines) / textSize.Y;
                    float horizontalTextSizeScaling = (textListRect.Width / (float)textSize.X);
                    textSize *= textSizeScaling;
                    spriteBatch.DrawString(font,text1,new Vector2(textListRect.X,textListRect.Y+curPixel),Color.White,0f,new Vector2(0,0), MathF.Min(textSizeScaling,horizontalTextSizeScaling), SpriteEffects.None,1);
                    curPixel -= ((float)textListRect.Height / maxAllowedLines);
                }
            }
            
           

        }

        public void DrawString(string text)
        {
          Draw();
        }

        public void Update()
        {
           
        }

        public void Initialize()
        {
           
        }

        public void OnResize()
        {
           GetScreenSpaceRect();
        }

        public string text { get; set; }
    }
}
