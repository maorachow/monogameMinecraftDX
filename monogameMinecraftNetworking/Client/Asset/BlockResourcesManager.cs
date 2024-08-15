using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftNetworking.Client.UI;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Rendering.Particle;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Client.Asset
{
    public class BlockResourcesManager
    {
        public static void LoadDefaultUIResources(ContentManager content, Game game)
        {
            Dictionary<int, string> blockSpriteInfoData = new Dictionary<int, string>
            {
                { 1, "blocksprites/stone" },
                { 2, "blocksprites/grass_side_carried" },
                { 3, "blocksprites/dirt" },
                { 4, "blocksprites/grass_side_carried" },
                { 5, "blocksprites/bedrock" },
                { 6, "blocksprites/log_oak" },
                { 7, "blocksprites/log_oak" },
                { 8, "blocksprites/log_oak" },
                { 9, "blocksprites/leaves_oak_carried" },
                { 10, "blocksprites/diamond_ore" },
                { 11, "blocksprites/sand" },
                { 12, "blocksprites/end_stone" },
                { 13, "blocksprites/endframe_top" },
                { 14, "blocksprites/sea_lantern" },
                { 15, "blocksprites/iron_block" },
                { 16, "blocksprites/cobblestone" },
                { 17, "blocksprites/planks_oak" },
                { 18, "blocksprites/wool_colored_red" },
                { 19, "blocksprites/wool_colored_green" },
                { 20, "blocksprites/wool_colored_blue" },
                { 21, "blocksprites/planks_oak" },
                { 22, "blocksprites/wool_colored_white" },
                { 23, "blocksprites/wool_colored_black" },
                { 100, "blocksprites/water" },
                { 101, "blocksprites/grass" },
                { 102, "blocksprites/torch_on" },
                { 103, "blocksprites/fences" },
                { 104, "blocksprites/woodendoor" },
                { 105, "blocksprites/ladder" },
                { 106, "blocksprites/glass_green" },
                { 107, "blocksprites/glass_blue" },
                { 108, "blocksprites/glass_black" },
                { 109, "blocksprites/glass" },
                { 110, "blocksprites/glass_white" },
                { 111, "blocksprites/glass_red" },

            };
            foreach (var item in blockSpriteInfoData)
            {
                try
                {
                    Texture2D sprite = content.Load<Texture2D>(item.Value);

                    //    se.Play(1, 0, 0);
                    if (!UIElement.UITextures.ContainsKey("blocktexture" + item.Key))
                    {
                        UIElement.UITextures.Add("blocktexture" + item.Key, sprite);
                    }
                    else
                    {
                        UIElement.UITextures["blocktexture" + item.Key] = sprite;
                    }
                }
                catch
                {
                    UIElement.UITextures["blocktexture" + item.Key] = null;
                }
            }

            MultiplayerClientUIUtility.InitInventoryUI(game, MultiplayerClientUIUtility.sf);
        }

        public static void LoadResources(string path, ContentManager cm, GraphicsDevice device, ChunkRenderer cr, ParticleRenderer pr,
               Game game)
        {
            string blockInfoDataString;
            string blockSoundInfoDataString;
            string blockSpriteInfoDataString;
            try
            {
                blockInfoDataString = File.ReadAllText(path + "/blockinfodata.json");
                blockSoundInfoDataString = File.ReadAllText(path + "/blocksoundinfodata.json");

                blockSpriteInfoDataString = File.ReadAllText(path + "/blockspriteinfodata.json");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return;
            }

            if (monogameMinecraftShared.Asset.BlockResourcesManager. contentManager != null)
            {
                monogameMinecraftShared.Asset.BlockResourcesManager.contentManager.Dispose();
            }

            monogameMinecraftShared.Asset.BlockResourcesManager.contentManager = new ContentManager(cm.ServiceProvider, path + "/");

            monogameMinecraftShared.Asset.BlockResourcesManager.blockInfo = new Dictionary<int, BlockInfo>();
            //    Dictionary<int, List<Vector2Data>> blockInfoData = JsonSerializer.Deserialize<Dictionary<int, List<Vector2Data>>>(blockInfoDataString);
            Dictionary<int, BlockInfoJsonData> blockInfoDataNew =
                JsonSerializer.Deserialize<Dictionary<int, BlockInfoJsonData>>(blockInfoDataString);
            Dictionary<int, string> blockSoundInfoData =
                JsonSerializer.Deserialize<Dictionary<int, string>>(blockSoundInfoDataString);
            Dictionary<int, string> blockSpriteInfoData =
                JsonSerializer.Deserialize<Dictionary<int, string>>(blockSpriteInfoDataString);
            monogameMinecraftShared.Asset.BlockResourcesManager.blockSoundInfo = new Dictionary<int, SoundEffect>();

            foreach (var info in blockInfoDataNew)
            {
                monogameMinecraftShared.Asset.BlockResourcesManager.blockInfo.Add(info.Key, BlockInfoJsonData.ToBlockInfo(info.Value));
            }

            try
            {
                Texture2D atlasTmp = monogameMinecraftShared.Asset.BlockResourcesManager.contentManager.Load<Texture2D>("terrain");
                Color[] tmpColor = new Color[atlasTmp.Width * atlasTmp.Height];
                atlasTmp.GetData(tmpColor);
                monogameMinecraftShared.Asset.BlockResourcesManager.atlas = new Texture2D(device, atlasTmp.Width, atlasTmp.Height, false, SurfaceFormat.Color);
                monogameMinecraftShared.Asset.BlockResourcesManager.atlas.SetData(tmpColor);
                atlasTmp.Dispose();
            }
            catch
            {
                monogameMinecraftShared.Asset.BlockResourcesManager.atlas = null;
            }

            try
            {
                Texture2D atlasNormalTmp = monogameMinecraftShared.Asset.BlockResourcesManager.contentManager.Load<Texture2D>("terrainnormal");
                Color[] tmpColor = new Color[atlasNormalTmp.Width * atlasNormalTmp.Height];
                atlasNormalTmp.GetData(tmpColor);
                monogameMinecraftShared.Asset.BlockResourcesManager.atlasNormal = new Texture2D(device, atlasNormalTmp.Width, atlasNormalTmp.Height, false,
                    SurfaceFormat.Color);
                monogameMinecraftShared.Asset.BlockResourcesManager.atlasNormal.SetData(tmpColor);
                atlasNormalTmp.Dispose();
            }
            catch
            {
                monogameMinecraftShared.Asset.BlockResourcesManager.atlasNormal = null;
            }

            try
            {
                // atlasMER =cmTemp.Load<Texture2D>("terrainmer");

                Texture2D atlasMERTmp = monogameMinecraftShared.Asset.BlockResourcesManager.contentManager.Load<Texture2D>("terrainmer");
                Color[] tmpColor = new Color[atlasMERTmp.Width * atlasMERTmp.Height];
                atlasMERTmp.GetData(tmpColor);
                monogameMinecraftShared.Asset.BlockResourcesManager.atlasMER = new Texture2D(device, atlasMERTmp.Width, atlasMERTmp.Height, false, SurfaceFormat.Color);
                monogameMinecraftShared.Asset.BlockResourcesManager.atlasMER.SetData(tmpColor);
                atlasMERTmp.Dispose();
            }
            catch
            {
                monogameMinecraftShared.Asset.BlockResourcesManager.atlasMER = null;
            }


            foreach (var item in blockSoundInfoData)
            {
                try
                {
                    SoundEffect se = monogameMinecraftShared.Asset.BlockResourcesManager.contentManager.Load<SoundEffect>(item.Value);

                    //    se.Play(1, 0, 0);


                    monogameMinecraftShared.Asset.BlockResourcesManager.blockSoundInfo.Add(item.Key, se);
                }
                catch
                {
                    monogameMinecraftShared.Asset.BlockResourcesManager.blockSoundInfo.Add(item.Key, null);
                }
            }

            foreach (var item in blockSpriteInfoData)
            {
                try
                {
                    Texture2D sprite = monogameMinecraftShared.Asset.BlockResourcesManager.contentManager.Load<Texture2D>(item.Value);

                    //    se.Play(1, 0, 0);
                    if (!UIElement.UITextures.ContainsKey("blocktexture" + item.Key))
                    {
                        UIElement.UITextures.Add("blocktexture" + item.Key, sprite);
                    }
                    else
                    {
                        UIElement.UITextures["blocktexture" + item.Key] = sprite;
                    }
                }
                catch
                {
                    UIElement.UITextures["blocktexture" + item.Key] = null;
                }
            }

            Chunk.blockSoundInfo = monogameMinecraftShared.Asset.BlockResourcesManager.blockSoundInfo;
            Chunk.blockInfosNew = monogameMinecraftShared.Asset.BlockResourcesManager.blockInfo;
            MultiplayerClientUIUtility.InitInventoryUI(game, MultiplayerClientUIUtility.sf);
            //   cmTemp.Dispose();
            if (monogameMinecraftShared.Asset.BlockResourcesManager.atlasNormal != null && monogameMinecraftShared.Asset.BlockResourcesManager.atlas != null && monogameMinecraftShared.Asset.BlockResourcesManager.atlasMER != null)
            {
                cr.SetTexture(monogameMinecraftShared.Asset.BlockResourcesManager.atlasNormal, null, monogameMinecraftShared.Asset.BlockResourcesManager.atlas, monogameMinecraftShared.Asset.BlockResourcesManager.atlasMER);
            }

           // ClientSideChunkHelper.RebuildAllChunks();

            monogameMinecraftShared.Asset.BlockResourcesManager.LoadParticleResources(monogameMinecraftShared.Asset.BlockResourcesManager.contentManager, device, pr);
        }
    }
}
