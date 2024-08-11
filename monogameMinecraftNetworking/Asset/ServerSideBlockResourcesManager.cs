using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Asset
{
    public class ServerSideBlockResourcesManager
    {
        public static Dictionary<int,BlockInfo> blockInfos=new Dictionary<int,BlockInfo>();

        public static void LoadBlockInfo(string path)
        {

            if (!Directory.Exists(path))
            {
                Console.WriteLine("block info laoding failed: directory does not exist");
                return;
            }
            string blockInfoDataString;
          
            try
            {
                blockInfoDataString = File.ReadAllText(path + "/blockinfodata.json");
       
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return;
            }

            try
            {
                Dictionary<int, BlockInfoJsonData> blockInfoDatas =
                    JsonSerializer.Deserialize<Dictionary<int, BlockInfoJsonData>>(blockInfoDataString);
                blockInfos=new Dictionary<int,BlockInfo>();
                foreach (var item in blockInfoDatas)
                {
                    blockInfos.Add(item.Key,BlockInfoJsonData.ToBlockInfo(item.Value));
                }
                Chunk.blockInfosNew= blockInfos;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return;
                
            }


        }
    }
}
