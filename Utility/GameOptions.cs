using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using monogameMinecraftDX.UI;

namespace monogameMinecraftDX.Utility
{
    public class GameOptions
    {
        public static string path = AppDomain.CurrentDomain.BaseDirectory;
        public static int renderDistance = 512;
        public static bool renderShadow = false;
        public static bool renderFarShadow = false;
        public static bool renderSSAO = false;
        public static bool renderSSR = false;
        public static bool renderSSID = false;
        public static bool renderLightShaft = false;
        public static bool renderContactShadow = false;
        public static bool showGraphicsDebug = false;
        public static bool renderMotionBlur = false;
        public static void ReadOptionsJson()
        {
            if (!Directory.Exists(path + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(path + "unityMinecraftServerData");

            }


            if (!File.Exists(path + "unityMinecraftServerData" + "/options.json"))
            {
                FileStream fs = File.Create(path + "unityMinecraftServerData" + "/options.json");
                fs.Close();
            }

            string data = File.ReadAllText(path + "unityMinecraftServerData/options.json");
            if (data.Length > 0)
            {
                try
                {
                    GameOptionsData dataOptions = JsonSerializer.Deserialize<GameOptionsData>(data);
                    renderDistance = dataOptions.renderDistance;
                    renderShadow = dataOptions.renderShadow;
                    renderFarShadow = dataOptions.renderFarShadow;
                    renderSSAO = dataOptions.renderSSAO;
                    renderLightShaft = dataOptions.renderLightShaft;
                    renderSSR = dataOptions.renderSSR;
                    renderSSID = dataOptions.renderSSID;
                    renderContactShadow = dataOptions.renderContactShadow;
                    showGraphicsDebug = dataOptions.showGraphicsDebug;
                    renderMotionBlur = dataOptions.renderMotionBlur;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

            }

        }

        public static void SaveOptions(object obj)
        {

            FileStream fs;
            if (File.Exists(path + "unityMinecraftServerData/options.json"))
            {
                fs = new FileStream(path + "unityMinecraftServerData/options.json", FileMode.Truncate, FileAccess.Write);//Truncate模式打开文件可以清空。
            }
            else
            {
                fs = new FileStream(path + "unityMinecraftServerData/options.json", FileMode.Create, FileAccess.Write);
            }
            fs.Close();



            GameOptionsData data = new GameOptionsData(renderDistance, renderShadow, renderFarShadow, renderSSAO, renderLightShaft, renderSSR, renderSSID, renderContactShadow, showGraphicsDebug, renderMotionBlur);
            string dataSerialized = JsonSerializer.Serialize(data);
            File.WriteAllText(path + "unityMinecraftServerData/options.json", dataSerialized);

        }
        public static void ChangeRenderDistance(UIButton obj)
        {
            obj.text = "Render Distance : " + renderDistance;
            renderDistance += 32;
            if (renderDistance >= 544)
            {
                renderDistance = 64;
            }
            obj.text = "Render Distance : " + renderDistance;
        }

        public static void UpdateRenderDistanceUIText(UIButton obj)
        {
            obj.text = "Render Distance : " + renderDistance;

        }
        public static void ChangeRenderShadow(UIButton obj)
        {
            obj.text = "Render Shadow : " + renderShadow.ToString();
            renderShadow = !renderShadow;
            obj.text = "Render Shadow : " + renderShadow.ToString();
        }
        public static void UpdateRenderShadowUIText(UIButton obj)
        {
            obj.text = "Render Shadow : " + renderShadow.ToString();

        }
        public static void ChangeRenderFarShadow(UIButton obj)
        {

            obj.text = "Render Far Shadow : " + renderFarShadow.ToString();
            renderFarShadow = !renderFarShadow;
            obj.text = "Render Far Shadow : " + renderFarShadow.ToString();
        }

        public static void UpdateRenderFarShadowUIText(UIButton obj)
        {

            obj.text = "Render Far Shadow : " + renderFarShadow.ToString();

        }
        public static void ChangeRenderSSAO(UIButton obj)
        {

            obj.text = "Render SSAO : " + renderSSAO.ToString();
            renderSSAO = !renderSSAO;
            obj.text = "Render SSAO : " + renderSSAO.ToString();
        }
        public static void UpdateRenderSSAOUIText(UIButton obj)
        {

            obj.text = "Render SSAO : " + renderSSAO.ToString();

        }
        public static void ChangeRenderSSR(UIButton obj)
        {

            obj.text = "Render SSR : " + renderSSR.ToString();
            renderSSR = !renderSSR;
            obj.text = "Render SSR : " + renderSSR.ToString();
        }
        public static void UpdateRenderSSRUIText(UIButton obj)
        {

            obj.text = "Render SSR : " + renderSSR.ToString();

        }


        public static void ChangeRenderContactShadow(UIButton obj)
        {

            obj.text = "Render Contact Shadow : " + renderContactShadow.ToString();
            renderContactShadow = !renderContactShadow;
            obj.text = "Render Contact Shadow : " + renderContactShadow.ToString();
        }
        public static void UpdateRenderContactShadowUIText(UIButton obj)
        {

            obj.text = "Render Contact Shadow : " + renderContactShadow.ToString();

        }

        public static void ChangeRenderSSID(UIButton obj)
        {

            obj.text = "Render SSID : " + renderSSID.ToString();
            renderSSID = !renderSSID;
            obj.text = "Render SSID : " + renderSSID.ToString();
        }

        public static void UpdateRenderSSIDUIText(UIButton obj)
        {

            obj.text = "Render SSID : " + renderSSID.ToString();

        }
        public static void ChangeRenderLightShaft(UIButton obj)
        {

            obj.text = "Render Light Shaft : " + renderLightShaft.ToString();
            renderLightShaft = !renderLightShaft;
            obj.text = "Render Light Shaft : " + renderLightShaft.ToString();
        }

        public static void UpdateRenderLightShaftUIText(UIButton obj)
        {

            obj.text = "Render Light Shaft : " + renderLightShaft.ToString();

        }


        public static void ChangeShowGraphicsDebug(UIButton obj)
        {

            obj.text = "Show Graphics Debug : " + showGraphicsDebug.ToString();
            showGraphicsDebug = !showGraphicsDebug;
            obj.text = "Show Graphics Debug : " + showGraphicsDebug.ToString();
        }

        public static void UpdateShowGraphicsDebugUIText(UIButton obj)
        {

            obj.text = "Show Graphics Debug : " + showGraphicsDebug.ToString();

        }



        public static void ChangeRenderMotionBlur(UIButton obj)
        {

            obj.text = "Render Motion Blur : " + renderMotionBlur.ToString();
            renderMotionBlur = !renderMotionBlur;
            obj.text = "Render Motion Blur : " + renderMotionBlur.ToString();
        }

        public static void UpdateRenderMotionBlurUIText(UIButton obj)
        {

            obj.text = "Render Motion Blur : " + renderMotionBlur.ToString();

        }
    }

    public class GameOptionsData
    {
        [JsonInclude]
        public int renderDistance;
        [JsonInclude]
        public bool renderShadow;
        [JsonInclude]
        public bool renderFarShadow;
        [JsonInclude]
        public bool renderSSAO;
        [JsonInclude]
        public bool renderLightShaft;
        [JsonInclude]
        public bool renderSSR;
        [JsonInclude]
        public bool renderSSID;
        [JsonInclude]
        public bool renderContactShadow;
        [JsonInclude]
        public bool showGraphicsDebug;
        [JsonInclude]
        public bool renderMotionBlur;
        public GameOptionsData(int renderDistance, bool renderShadow, bool renderFarShadow, bool renderSSAO, bool renderLightShaft, bool renderSSR, bool renderSSID, bool renderContactShadow, bool showGraphicsDebug, bool renderMotionBlur)
        {
            this.renderDistance = renderDistance;
            this.renderShadow = renderShadow;
            this.renderFarShadow = renderFarShadow;
            this.renderSSAO = renderSSAO;
            this.renderLightShaft = renderLightShaft;
            this.renderSSR = renderSSR;
            this.renderSSID = renderSSID;
            this.renderContactShadow = renderContactShadow;
            this.showGraphicsDebug = showGraphicsDebug;
            this.renderMotionBlur = renderMotionBlur;
        }
    }
}
