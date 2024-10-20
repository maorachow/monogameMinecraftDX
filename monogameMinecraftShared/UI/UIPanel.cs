﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameMinecraftShared.UI
{
    public class UIPanel:UIElement
    {


        public Rectangle screenSpaceRect;

        public Vector2 element00Pos;
        public Vector2 element01Pos;
        public Vector2 element11Pos;
        public Vector2 element10Pos;

        public Vector2 initalWidthHeight;
        public bool keepsAspectRatio = false;


        public UIPanel(UIStateManager state, Vector2 position, float width, float height, bool keepsAspectRatio = false)
        {
            element00Pos = position;
            element10Pos = new Vector2(position.X + width, position.Y);
            element11Pos = new Vector2(position.X + width, position.Y + height);
            element01Pos = new Vector2(position.X, position.Y + height);
            Debug.WriteLine(element00Pos + " " + element10Pos + " " + element11Pos + " " + element01Pos);
          
            this.text = text;
         
            OnResize(state);
         
            this.keepsAspectRatio = keepsAspectRatio;
            if (this.keepsAspectRatio)
            {
                initalWidthHeight = new Vector2(width, height);

            }
        
        }
        public void GetScreenSpaceRect(UIStateManager state)
        {
            Debug.WriteLine("Panel:"+element00Pos + " " + element01Pos + " " + element10Pos + " " + element11Pos);

            Vector2 transformedP00 = new Vector2(element00Pos.X * state.ScreenRect.Width, element00Pos.Y * state.ScreenRect.Height);
            float width = (element10Pos - element00Pos).X * state.ScreenRect.Width;
            float height = (element01Pos - element00Pos).Y * state.ScreenRect.Height;
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
            screenSpaceRect = new Rectangle((int)transformedP00.X, (int)transformedP00.Y, (int)width, (int)height);
            Debug.WriteLine("Panel:"+screenSpaceRect.X + " " + screenSpaceRect.Y + " " + screenSpaceRect.Width + " " + screenSpaceRect.Height);
 
        }

        public void Draw(UIStateManager state)
        {
           
        }

        public void DrawString(UIStateManager state,string text)
        {
             
        }

        public void Update(UIStateManager state)
        {
            
        }

        public void Initialize()
        {
           
        }

        public void OnResize(UIStateManager state)
        {
            GetScreenSpaceRect( state);
        }

        public string text { get; set; }
        public string optionalTag { get; set; }
    }
}
