using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftDX.Rendering;

namespace monogameMinecraftDX.Utility
{
    public class TerrainMipmapGenerator : FullScreenQuadRenderer
    {
        public static TerrainMipmapGenerator instance;
        public GraphicsDevice device;
        public Effect textureCopyEffect;
        //   public Texture2D sourceTex;
        //   public Texture2D mipmapSourceTex;
        public RenderTarget2D terrainMip1;
        public RenderTarget2D terrainMip2;
        public RenderTarget2D terrainMip3;
        public RenderTarget2D terrainMip4;
        public RenderTarget2D terrainMip5;
        public RenderTarget2D terrainMip6;
        public RenderTarget2D terrainMip7;
        public RenderTarget2D terrainMip8;
        public RenderTarget2D terrainMip9;
        public RenderTarget2D terrainMip10;




        public TerrainMipmapGenerator(GraphicsDevice device, Effect textureCopyEffect)
        {
            instance = this;
            this.device = device;
            this.textureCopyEffect = textureCopyEffect;
            InitializeVertices();
            InitializeQuadBuffers(device);

        }
        public Texture2D GenerateMipmap(in Texture2D sourceTex, bool isNormalMap = false)
        {
            //   Texture2D sourceTex1 = sourceTex;
            Texture2D mipmapSourceTex = new Texture2D(device, sourceTex.Width, sourceTex.Height, true, sourceTex.Format);
            terrainMip1 = new RenderTarget2D(device, sourceTex.Width / 2, sourceTex.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip2 = new RenderTarget2D(device, terrainMip1.Width / 2, terrainMip1.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip3 = new RenderTarget2D(device, terrainMip2.Width / 2, terrainMip2.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip4 = new RenderTarget2D(device, terrainMip3.Width / 2, terrainMip3.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip5 = new RenderTarget2D(device, terrainMip4.Width / 2, terrainMip4.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip6 = new RenderTarget2D(device, terrainMip5.Width / 2, terrainMip5.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip7 = new RenderTarget2D(device, terrainMip6.Width / 2, terrainMip6.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip8 = new RenderTarget2D(device, terrainMip7.Width / 2, terrainMip7.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip9 = new RenderTarget2D(device, terrainMip8.Width / 2, terrainMip8.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            terrainMip10 = new RenderTarget2D(device, terrainMip9.Width / 2, terrainMip9.Height / 2, true, sourceTex.Format, DepthFormat.Depth16);
            textureCopyEffect.Parameters["TextureCopy"].SetValue(sourceTex);
            textureCopyEffect.Parameters["pixelSize"].SetValue(new Vector2(1f / sourceTex.Width, 1f / sourceTex.Height));
            RenderQuad(device, terrainMip1, textureCopyEffect);

            textureCopyEffect.Parameters["TextureCopy"].SetValue(terrainMip1);
            textureCopyEffect.Parameters["pixelSize"].SetValue(new Vector2(1f / terrainMip1.Width, 1f / terrainMip1.Height));
            RenderQuad(device, terrainMip2, textureCopyEffect);

            textureCopyEffect.Parameters["TextureCopy"].SetValue(terrainMip2);
            textureCopyEffect.Parameters["pixelSize"].SetValue(new Vector2(1f / terrainMip2.Width, 1f / terrainMip2.Height));
            RenderQuad(device, terrainMip3, textureCopyEffect);

            textureCopyEffect.Parameters["TextureCopy"].SetValue(terrainMip3);
            textureCopyEffect.Parameters["pixelSize"].SetValue(new Vector2(1f / terrainMip3.Width, 1f / terrainMip3.Height));
            RenderQuad(device, terrainMip4, textureCopyEffect);

            textureCopyEffect.Parameters["TextureCopy"].SetValue(terrainMip4);
            textureCopyEffect.Parameters["pixelSize"].SetValue(new Vector2(1f / terrainMip4.Width, 1f / terrainMip4.Height));
            RenderQuad(device, terrainMip5, textureCopyEffect);

            textureCopyEffect.Parameters["TextureCopy"].SetValue(terrainMip5);
            textureCopyEffect.Parameters["pixelSize"].SetValue(new Vector2(1f / terrainMip5.Width, 1f / terrainMip5.Height));
            RenderQuad(device, terrainMip6, textureCopyEffect);
            if (isNormalMap)
            {
                RenderQuadPureColor(device, terrainMip7, new Color(0.5f, 0.5f, 1f, 1f));


                RenderQuadPureColor(device, terrainMip8, new Color(0.5f, 0.5f, 1f, 1f));
                RenderQuadPureColor(device, terrainMip9, new Color(0.5f, 0.5f, 1f, 1f));
                RenderQuadPureColor(device, terrainMip10, new Color(0.5f, 0.5f, 1f, 1f));
            }
            else
            {
                RenderQuadPureColor(device, terrainMip7, new Color(0.231f, 0.278f, 0.522f, 1f));


                RenderQuadPureColor(device, terrainMip8, new Color(0.231f, 0.278f, 0.522f, 1f));
                RenderQuadPureColor(device, terrainMip9, new Color(0.231f, 0.278f, 0.522f, 1f));
                RenderQuadPureColor(device, terrainMip10, new Color(0.231f, 0.278f, 0.522f, 1f));
            }

            //    RenderQuad(device, terrainMip7, textureCopyEffect);


            //  RenderQuad(device, terrainMip8, textureCopyEffect);
            //   RenderQuadPureColor(device, terrainMip10, new Color(0.266f, 0.534f, 0.281f, 1f));
            Color[] atlasMip0 = new Color[sourceTex.Width * sourceTex.Height];

            Color[] atlasMip1 = new Color[terrainMip1.Width * terrainMip1.Height];
            Color[] atlasMip2 = new Color[terrainMip2.Width * terrainMip2.Height];
            Color[] atlasMip3 = new Color[terrainMip3.Width * terrainMip3.Height];
            Color[] atlasMip4 = new Color[terrainMip4.Width * terrainMip4.Height];
            Color[] atlasMip5 = new Color[terrainMip5.Width * terrainMip5.Height];
            Color[] atlasMip6 = new Color[terrainMip6.Width * terrainMip6.Height];
            Color[] atlasMip7 = new Color[terrainMip7.Width * terrainMip7.Height];
            Color[] atlasMip8 = new Color[terrainMip8.Width * terrainMip8.Height];
            Color[] atlasMip9 = new Color[terrainMip9.Width * terrainMip9.Height];
            Color[] atlasMip10 = new Color[terrainMip10.Width * terrainMip10.Height];

            sourceTex.GetData(0, null, atlasMip0, 0, sourceTex.Width * sourceTex.Height);

            terrainMip1.GetData(0, null, atlasMip1, 0, atlasMip1.Length);
            terrainMip2.GetData(0, null, atlasMip2, 0, atlasMip2.Length);
            terrainMip3.GetData(0, null, atlasMip3, 0, atlasMip3.Length);
            terrainMip4.GetData(0, null, atlasMip4, 0, atlasMip4.Length);
            terrainMip5.GetData(0, null, atlasMip5, 0, atlasMip5.Length);
            terrainMip6.GetData(0, null, atlasMip6, 0, atlasMip6.Length);
            terrainMip7.GetData(0, null, atlasMip7, 0, atlasMip7.Length);
            terrainMip8.GetData(0, null, atlasMip8, 0, atlasMip8.Length);
            terrainMip9.GetData(0, null, atlasMip9, 0, atlasMip9.Length);
            terrainMip10.GetData(0, null, atlasMip10, 0, atlasMip10.Length);

            mipmapSourceTex.SetData(0, 0, null, atlasMip0, 0, mipmapSourceTex.Width * mipmapSourceTex.Height);
            mipmapSourceTex.SetData(1, 0, null, atlasMip1, 0, mipmapSourceTex.Width / 2 * mipmapSourceTex.Height / 2);
            mipmapSourceTex.SetData(2, 0, null, atlasMip2, 0, mipmapSourceTex.Width / 4 * mipmapSourceTex.Height / 4);
            mipmapSourceTex.SetData(3, 0, null, atlasMip3, 0, mipmapSourceTex.Width / 8 * mipmapSourceTex.Height / 8);
            mipmapSourceTex.SetData(4, 0, null, atlasMip4, 0, mipmapSourceTex.Width / 16 * mipmapSourceTex.Height / 16);
            mipmapSourceTex.SetData(5, 0, null, atlasMip5, 0, mipmapSourceTex.Width / 32 * mipmapSourceTex.Height / 32);
            mipmapSourceTex.SetData(6, 0, null, atlasMip6, 0, mipmapSourceTex.Width / 64 * mipmapSourceTex.Height / 64);
            mipmapSourceTex.SetData(7, 0, null, atlasMip7, 0, mipmapSourceTex.Width / 128 * mipmapSourceTex.Height / 128);
            mipmapSourceTex.SetData(8, 0, null, atlasMip8, 0, mipmapSourceTex.Width / 256 * mipmapSourceTex.Height / 256);
            mipmapSourceTex.SetData(9, 0, null, atlasMip9, 0, mipmapSourceTex.Width / 512 * mipmapSourceTex.Height / 512);
            mipmapSourceTex.SetData(10, 0, null, atlasMip10, 0, mipmapSourceTex.Width / 1024 * mipmapSourceTex.Height / 1024);
            return mipmapSourceTex;
        }
    }
}
