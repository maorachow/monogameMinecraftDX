using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraft;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace monogameMinecraftDX
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
        public Vector2 ToVector2() { return new Vector2(x, y); }


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
    public class BlockResourcesManager
    {

        public static Dictionary<int, List<Vector2>> blockInfo;
        public static Dictionary<int, SoundEffect> blockSoundInfo;

        public static Texture2D atlas;
        public static Texture2D atlasNormal;
        public static Texture2D atlasMER;


        public static void WriteDefaultBlockInfo(string path)
        {

            Dictionary<int, List<Vector2Data>> blockInfoData = new Dictionary<int, List<Vector2Data>>();
            foreach (var item in Chunk.blockInfo)
            {
                blockInfoData.Add(item.Key, Vector2Data.FromVector2List(item.Value));
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
        //0None 1Stone 2Grass 3Dirt 4Side grass block 5Bedrock 6WoodX 7WoodY 8WoodZ 9Leaves 10Diamond Ore 11Sand 14Sea Lantern
        //100Water 101Grass
        //102torch
        //200Leaves
        //0-99solid blocks
        //100-199no hitbox blocks
        //200-299hitbox nonsolid blocks
        public static void WriteDefaultBlockSoundInfo(string path)
        {

            Dictionary<int, string> blockInfoData = new Dictionary<int, string>{

                {1,"stonedig" },
               {2,"grassdig" },
                   {3,"dirtdig" },
                   {4,"grassdig" },
                   {5,"stonedig" },
                   {6,"wooddig" },
                   {7,"wooddig" },
                   {8,"wooddig" },
                   {9,"grassdig" },
                   {10,"stonedig" },
                {11,"dirtdig" },
                {12,"stonedig" },
                {13,"stonedig" },
                {14,"stonedig" },
                 {100,"waterdig" },
                {101,"grassdig" },
                {102,"wooddig" }

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
                atlasNormal = new Texture2D(device, atlasNormalTmp.Width, atlasNormalTmp.Height, false, SurfaceFormat.Color);
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
            Dictionary<int, string> blockSoundInfoData = new Dictionary<int, string>{

                {1,"stonedig" },
               {2,"grassdig" },
                   {3,"dirtdig" },
                   {4,"grassdig" },
                   {5,"stonedig" },
                   {6,"wooddig" },
                   {7,"wooddig" },
                   {8,"wooddig" },
                   {9,"grassdig" },
                   {10,"stonedig" },
                {11,"dirtdig" },
                {12,"stonedig" },
                {13,"stonedig" },
                {14,"stonedig" },
                 {100,"waterdig" },
                {101,"grassdig" },
                {102,"wooddig" }

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
        public static void LoadResources(string path, ContentManager cm, GraphicsDevice device, ChunkRenderer cr)
        {
            string blockInfoDataString;
            string blockSoundInfoDataString;
            try
            {
                blockInfoDataString = File.ReadAllText(path + "/blockinfodata.json");
                blockSoundInfoDataString = File.ReadAllText(path + "/blocksoundinfodata.json");
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

            blockInfo = new Dictionary<int, List<Vector2>>();
            Dictionary<int, List<Vector2Data>> blockInfoData = JsonSerializer.Deserialize<Dictionary<int, List<Vector2Data>>>(blockInfoDataString);
            Dictionary<int, string> blockSoundInfoData = JsonSerializer.Deserialize<Dictionary<int, string>>(blockSoundInfoDataString);
            blockSoundInfo = new Dictionary<int, SoundEffect>();
            foreach (var item in blockInfoData)
            {
                blockInfo.Add(item.Key, Vector2Data.ToVector2List(item.Value));
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
                atlasNormal = new Texture2D(device, atlasNormalTmp.Width, atlasNormalTmp.Height, false, SurfaceFormat.Color);
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
            Chunk.blockSoundInfo = blockSoundInfo;
            Chunk.blockInfo = blockInfo;
            //   cmTemp.Dispose();
            cr.SetTexture(atlasNormal, null, atlas, atlasMER);
            ChunkHelper.RebuildAllChunks();
        }
    }
}
