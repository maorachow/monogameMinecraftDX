using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using monogameMinecraftDX.World;
using monogameMinecraftDX.Rendering;
using monogameMinecraftDX.UI;
// ReSharper disable All

namespace monogameMinecraftDX
{
    namespace Asset
    {
        public struct Vector2Data
        {
            [JsonInclude]
            public float x;

            [JsonInclude]
            public float y;

            public Vector2Data(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public Vector2 ToVector2()
            {
                return new Vector2(x, y);
            }


            public static List<Vector2Data> FromVector2List(List<Vector2> list)
            {
                List<Vector2Data> ret = new List<Vector2Data>();
                for (int i = 0; i < list.Count; i++)
                {
                    ret.Add(new Vector2Data(list[i].X, list[i].Y));
                }

                return ret;
            }

            public static List<Vector2> ToVector2List(List<Vector2Data> list)
            {
                List<Vector2> ret = new List<Vector2>();
                for (int i = 0; i < list.Count; i++)
                {
                    ret.Add(new Vector2(list[i].x, list[i].y));
                }

                return ret;
            }
        }

        public struct BlockInfoJsonData
        {
            [JsonInclude]
            public List<Vector2Data> uvCorners;

            [JsonInclude]
            public List<Vector2Data> uvSizes;

            [JsonInclude]
            public BlockShape shape;

            public BlockInfoJsonData(List<Vector2Data> uvCorners, List<Vector2Data> uvSizes, BlockShape bs)
            {
                this.shape = bs;
                this.uvCorners = uvCorners;
                this.uvSizes = uvSizes;
            }

            public static BlockInfoJsonData FromBlockInfo(BlockInfo info)
            {
                BlockInfoJsonData ret = new BlockInfoJsonData(Vector2Data.FromVector2List(info.uvCorners),
                    Vector2Data.FromVector2List(info.uvSizes), info.shape);
                return ret;
            }

            public static BlockInfo ToBlockInfo(BlockInfoJsonData info)
            {
                BlockInfo ret = new BlockInfo(Vector2Data.ToVector2List(info.uvCorners),
                    Vector2Data.ToVector2List(info.uvSizes), info.shape);
                return ret;
            }
        }

        public class BlockResourcesManager
        {
            public static Dictionary<int, BlockInfo> blockInfo;
            public static Dictionary<int, SoundEffect> blockSoundInfo;

            public static Texture2D atlas;
            public static Texture2D atlasNormal;
            public static Texture2D atlasMER;


            public static void WriteDefaultBlockInfo(string path)
            {
                Dictionary<int, BlockInfoJsonData> blockInfoData = new Dictionary<int, BlockInfoJsonData>();
                foreach (var item in Chunk.blockInfosNew)
                {
                    blockInfoData.Add(item.Key, BlockInfoJsonData.FromBlockInfo(item.Value));
                }

                string blockInfoDataString = JsonSerializer.Serialize(blockInfoData);

                FileStream fs;
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                }

                fs.Close();
                File.WriteAllText(path, blockInfoDataString);


                Debug.WriteLine(blockInfoDataString);
            }

            //0None 1Stone 2Grass 3Dirt 4Side grass block 5Bedrock 6WoodX 7WoodY 8WoodZ 9Leaves 10Diamond Ore 11Sand 12End Stone 13End Portal 14Sea Lantern 15Iron Block 16Cobblestone 17Wood Planks
            //100Water 101Grass
            //102torch
            //200Leaves
            //0-99solid blocks
            //100-199no hitbox blocks
            //200-299hitbox nonsolid blocks
            public static void WriteDefaultBlockSoundInfo(string path)
            {
                Dictionary<int, string> blockInfoData = new Dictionary<int, string>
                {
                    { 1, "stonedig" },
                    { 2, "grassdig" },
                    { 3, "dirtdig" },
                    { 4, "grassdig" },
                    { 5, "stonedig" },
                    { 6, "wooddig" },
                    { 7, "wooddig" },
                    { 8, "wooddig" },
                    { 9, "grassdig" },
                    { 10, "stonedig" },
                    { 11, "dirtdig" },
                    { 12, "stonedig" },
                    { 13, "stonedig" },
                    { 14, "stonedig" },
                    { 15, "stonedig" },
                    { 16, "stonedig" },
                    { 17, "wooddig" },
                    { 100, "waterdig" },
                    { 101, "grassdig" },
                    { 102, "wooddig" }
                };
                string blockInfoDataString = JsonSerializer.Serialize(blockInfoData);

                FileStream fs;
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                }

                fs.Close();
                File.WriteAllText(path, blockInfoDataString);
            }

            public static void WriteDefaultBlockSpritesInfo(string path)
            {
                Dictionary<int, string> blockInfoData = new Dictionary<int, string>
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
                    { 100, "blocksprites/water" },
                    { 101, "blocksprites/grass" },
                    { 102, "blocksprites/torch_on" }
                };
                string blockInfoDataString = JsonSerializer.Serialize(blockInfoData);

                FileStream fs;
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                }

                fs.Close();
                File.WriteAllText(path, blockInfoDataString);
            }

            public static ContentManager contentManager;

            public static void LoadDefaultResources(ContentManager cm, GraphicsDevice device, ChunkRenderer cr)
            {
                blockSoundInfo = new Dictionary<int, SoundEffect>();
                try
                {
                    Texture2D atlasTmp = cm.Load<Texture2D>("terrainnomipmap");
                    Color[] tmpColor = new Color[atlasTmp.Width * atlasTmp.Height];
                    atlasTmp.GetData(tmpColor);
                    atlas = new Texture2D(device, atlasTmp.Width, atlasTmp.Height, false, SurfaceFormat.Color);
                    atlas.SetData(tmpColor);
                    //     atlasTmp.Dispose();
                }
                catch
                {
                    atlas = null;
                }

                try
                {
                    Texture2D atlasNormalTmp = cm.Load<Texture2D>("terrainnormal");
                    Color[] tmpColor = new Color[atlasNormalTmp.Width * atlasNormalTmp.Height];
                    atlasNormalTmp.GetData(tmpColor);
                    atlasNormal = new Texture2D(device, atlasNormalTmp.Width, atlasNormalTmp.Height, false,
                        SurfaceFormat.Color);
                    atlasNormal.SetData(tmpColor);
                    //     atlasNormalTmp.Dispose();
                }
                catch
                {
                    atlasNormal = null;
                }

                try
                {
                    // atlasMER =cmTemp.Load<Texture2D>("terrainmer");

                    Texture2D atlasMERTmp = cm.Load<Texture2D>("terrainmer");
                    Color[] tmpColor = new Color[atlasMERTmp.Width * atlasMERTmp.Height];
                    atlasMERTmp.GetData(tmpColor);
                    atlasMER = new Texture2D(device, atlasMERTmp.Width, atlasMERTmp.Height, false, SurfaceFormat.Color);
                    atlasMER.SetData(tmpColor);
                    //       atlasMERTmp.Dispose();
                }
                catch
                {
                    atlasMER = null;
                }

                Dictionary<int, string> blockSoundInfoData = new Dictionary<int, string>
                {
                    { 1, "stonedig" },
                    { 2, "grassdig" },
                    { 3, "dirtdig" },
                    { 4, "grassdig" },
                    { 5, "stonedig" },
                    { 6, "wooddig" },
                    { 7, "wooddig" },
                    { 8, "wooddig" },
                    { 9, "grassdig" },
                    { 10, "stonedig" },
                    { 11, "dirtdig" },
                    { 12, "stonedig" },
                    { 13, "stonedig" },
                    { 14, "stonedig" },
                    { 15, "stonedig" },
                    { 16, "stonedig" },
                    { 17, "wooddig" },
                    { 100, "waterdig" },
                    { 101, "grassdig" },
                    { 102, "wooddig" }
                };

                foreach (var item in blockSoundInfoData)
                {
                    try
                    {
                        SoundEffect se = cm.Load<SoundEffect>("sounds/" + item.Value);

                        //    se.Play(1, 0, 0);


                        blockSoundInfo.Add(item.Key, se);
                    }
                    catch
                    {
                        blockSoundInfo.Add(item.Key, null);
                    }
                }

                Chunk.blockSoundInfo = blockSoundInfo;
                cr.SetTexture(atlasNormal, null, atlas, atlasMER);
            }

            public static void LoadDefaultUIResources(ContentManager content, MinecraftGame game)
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
                    { 100, "blocksprites/water" },
                    { 101, "blocksprites/grass" },
                    { 102, "blocksprites/torch_on" }
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

                UIUtility.InitInventoryUI(game, UIUtility.sf);
            }

            public static void LoadResources(string path, ContentManager cm, GraphicsDevice device, ChunkRenderer cr,
                MinecraftGame game)
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

                if (contentManager != null)
                {
                    contentManager.Dispose();
                }

                contentManager = new ContentManager(cm.ServiceProvider, path + "/");

                blockInfo = new Dictionary<int, BlockInfo>();
                //    Dictionary<int, List<Vector2Data>> blockInfoData = JsonSerializer.Deserialize<Dictionary<int, List<Vector2Data>>>(blockInfoDataString);
                Dictionary<int, BlockInfoJsonData> blockInfoDataNew =
                    JsonSerializer.Deserialize<Dictionary<int, BlockInfoJsonData>>(blockInfoDataString);
                Dictionary<int, string> blockSoundInfoData =
                    JsonSerializer.Deserialize<Dictionary<int, string>>(blockSoundInfoDataString);
                Dictionary<int, string> blockSpriteInfoData =
                    JsonSerializer.Deserialize<Dictionary<int, string>>(blockSpriteInfoDataString);
                blockSoundInfo = new Dictionary<int, SoundEffect>();

                foreach (var info in blockInfoDataNew)
                {
                    blockInfo.Add(info.Key, BlockInfoJsonData.ToBlockInfo(info.Value));
                }

                try
                {
                    Texture2D atlasTmp = contentManager.Load<Texture2D>("terrain");
                    Color[] tmpColor = new Color[atlasTmp.Width * atlasTmp.Height];
                    atlasTmp.GetData(tmpColor);
                    atlas = new Texture2D(device, atlasTmp.Width, atlasTmp.Height, false, SurfaceFormat.Color);
                    atlas.SetData(tmpColor);
                    atlasTmp.Dispose();
                }
                catch
                {
                    atlas = null;
                }

                try
                {
                    Texture2D atlasNormalTmp = contentManager.Load<Texture2D>("terrainnormal");
                    Color[] tmpColor = new Color[atlasNormalTmp.Width * atlasNormalTmp.Height];
                    atlasNormalTmp.GetData(tmpColor);
                    atlasNormal = new Texture2D(device, atlasNormalTmp.Width, atlasNormalTmp.Height, false,
                        SurfaceFormat.Color);
                    atlasNormal.SetData(tmpColor);
                    atlasNormalTmp.Dispose();
                }
                catch
                {
                    atlasNormal = null;
                }

                try
                {
                    // atlasMER =cmTemp.Load<Texture2D>("terrainmer");

                    Texture2D atlasMERTmp = contentManager.Load<Texture2D>("terrainmer");
                    Color[] tmpColor = new Color[atlasMERTmp.Width * atlasMERTmp.Height];
                    atlasMERTmp.GetData(tmpColor);
                    atlasMER = new Texture2D(device, atlasMERTmp.Width, atlasMERTmp.Height, false, SurfaceFormat.Color);
                    atlasMER.SetData(tmpColor);
                    atlasMERTmp.Dispose();
                }
                catch
                {
                    atlasMER = null;
                }


                foreach (var item in blockSoundInfoData)
                {
                    try
                    {
                        SoundEffect se = contentManager.Load<SoundEffect>(item.Value);

                        //    se.Play(1, 0, 0);


                        blockSoundInfo.Add(item.Key, se);
                    }
                    catch
                    {
                        blockSoundInfo.Add(item.Key, null);
                    }
                }

                foreach (var item in blockSpriteInfoData)
                {
                    try
                    {
                        Texture2D sprite = contentManager.Load<Texture2D>(item.Value);

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

                Chunk.blockSoundInfo = blockSoundInfo;
                Chunk.blockInfosNew = blockInfo;
                UIUtility.InitInventoryUI(game, UIUtility.sf);
                //   cmTemp.Dispose();
                cr.SetTexture(atlasNormal, null, atlas, atlasMER);
                ChunkHelper.RebuildAllChunks();
            }
        }
    }
}