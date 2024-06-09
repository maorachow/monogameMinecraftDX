using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace monogameMinecraftDX
{
    public class GlobalMaterialParamsManager
    {
        public static GlobalMaterialParamsManager instance ;
        public float metallic = 0f;
        public float roughness = 0f;
        public GlobalMaterialParamsManager() { instance = this; }
        public void Update(GameTime gameTime)
        {
            /*      if (Keyboard.GetState().IsKeyDown(Keys.M))
                  {
                      metallic += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.3f;
                  }
                  if (Keyboard.GetState().IsKeyDown(Keys.R))
                  {
                      roughness += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.3f;
                  }
                  if(metallic>1f)
                  {
                      metallic = 0f;
                  }
                  if (roughness > 1f)
                  {
                      roughness = 0f;
                  }*/
            metallic = 0.1f;
            roughness = 0.1f;
        //    Debug.WriteLine("metallic: "+metallic.ToString()+ "roughness: " + roughness.ToString());
        }
    }
}
