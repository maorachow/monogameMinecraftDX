using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace monogameMinecraftDX.Utility
{
    public class RandomTextureGenerator
    {
        public static RandomTextureGenerator instance;
        public RandomTextureGenerator() { instance = this; }
        public Texture2D randomTex;
        Random random = new Random();
        public void GenerateTexture(int width, int height, GraphicsDevice gd)
        {
            Color[] noiseValues = new Color[width * height];
            for (int i = 0; i < width * height; i++)
            {
                Vector3 noise = new Vector3(
                random.NextSingle(),
                random.NextSingle(),
                random.NextSingle());
                noiseValues[i] = new Color(noise);
            }
            randomTex = new Texture2D(gd, width, height, false, SurfaceFormat.Color);
            randomTex.SetData(noiseValues);
        }


    }

}