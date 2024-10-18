using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Rendering.Particle;

namespace monogameMinecraftShared.Asset
{
    public partial class BlockResourcesManager
    {
        public Texture2D particleAtlas;
        public Texture2D particleAtlasNormal;
        public Texture2D particleAtlasMER;

        public void LoadDefaultParticleResources(ContentManager cm, GraphicsDevice device)
        {
            try
            {
                Texture2D atlasTmp = cm.Load<Texture2D>("terrain");
                Color[] tmpColor = new Color[atlasTmp.Width * atlasTmp.Height];
                atlasTmp.GetData(tmpColor);
                particleAtlas = new Texture2D(device, atlasTmp.Width, atlasTmp.Height, false, SurfaceFormat.Color);
                particleAtlas.SetData(tmpColor);
                //     atlasTmp.Dispose();
            }
            catch
            {
                particleAtlas = null;
            }

            try
            {
                Texture2D atlasNormalTmp = cm.Load<Texture2D>("terrainnormal");
                Color[] tmpColor = new Color[atlasNormalTmp.Width * atlasNormalTmp.Height];
                atlasNormalTmp.GetData(tmpColor);
                particleAtlasNormal = new Texture2D(device, atlasNormalTmp.Width, atlasNormalTmp.Height, false,
                    SurfaceFormat.Color);
                particleAtlasNormal.SetData(tmpColor);
                //     atlasNormalTmp.Dispose();
            }
            catch
            {
                particleAtlasNormal = null;
            }

            try
            {
                // atlasMER =cmTemp.Load<Texture2D>("terrainmer");

                Texture2D atlasMERTmp = cm.Load<Texture2D>("terrainmer");
                Color[] tmpColor = new Color[atlasMERTmp.Width * atlasMERTmp.Height];
                atlasMERTmp.GetData(tmpColor);
                particleAtlasMER = new Texture2D(device, atlasMERTmp.Width, atlasMERTmp.Height, false, SurfaceFormat.Color);
                particleAtlasMER.SetData(tmpColor);
                //       atlasMERTmp.Dispose();
            }
            catch
            {
                particleAtlasMER = null;
            }
           // pr.SetTexture(particleAtlas,particleAtlasNormal,particleAtlasMER);
        }

        public void LoadParticleResources(ContentManager cm, GraphicsDevice device)
        {
            try
            {
                Texture2D atlasTmp = cm.Load<Texture2D>("terrain");
                Color[] tmpColor = new Color[atlasTmp.Width * atlasTmp.Height];
                atlasTmp.GetData(tmpColor);
                particleAtlas = new Texture2D(device, atlasTmp.Width, atlasTmp.Height, false, SurfaceFormat.Color);
                particleAtlas.SetData(tmpColor);
                atlasTmp.Dispose();
            }
            catch
            {
                particleAtlas = null;
            }

            try
            {
                Texture2D atlasNormalTmp = cm.Load<Texture2D>("terrainnormal");
                Color[] tmpColor = new Color[atlasNormalTmp.Width * atlasNormalTmp.Height];
                atlasNormalTmp.GetData(tmpColor);
                particleAtlasNormal = new Texture2D(device, atlasNormalTmp.Width, atlasNormalTmp.Height, false,
                    SurfaceFormat.Color);
                particleAtlasNormal.SetData(tmpColor);
                atlasNormalTmp.Dispose();
            }
            catch
            {
                particleAtlasNormal = null;
            }

            try
            {
                // atlasMER =cmTemp.Load<Texture2D>("terrainmer");

                Texture2D atlasMERTmp = cm.Load<Texture2D>("terrainmer");
                Color[] tmpColor = new Color[atlasMERTmp.Width * atlasMERTmp.Height];
                atlasMERTmp.GetData(tmpColor);
                particleAtlasMER = new Texture2D(device, atlasMERTmp.Width, atlasMERTmp.Height, false, SurfaceFormat.Color);
                particleAtlasMER.SetData(tmpColor);
                atlasMERTmp.Dispose();
            }
            catch
            {
                particleAtlasMER = null;
            }
           /* if (particleAtlas != null && particleAtlasNormal != null && particleAtlasMER != null)
            {
                pr.SetTexture(particleAtlasNormal, particleAtlas, particleAtlasMER);
            }*/

        }
    }
}
