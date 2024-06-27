using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.World;

namespace monogameMinecraftDX.Asset
{
    public static class StructureManager
    {
        public static Dictionary<string, StructureData> allStructureDatas = new Dictionary<string, StructureData>();

        public static void SaveStructureData(Vector3Int saveOrigin, Vector3Int saveSize,string savePath)
        {
            BlockData[,,] blockData = ChunkHelper.GetBlocks(saveOrigin, saveSize.x, saveSize.y, saveSize.z);
            StructureData data= new StructureData(blockData);
            FileStream fs;
            if (File.Exists(savePath))
            {
                fs = new FileStream(savePath, FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            }

            fs.Close();
            byte[] dataBytes = MessagePackSerializer.Serialize(data);
            File.WriteAllBytes(savePath, dataBytes);
        }
        public static void SaveGeneratingStructureData(StructureGeneratingParam genParams,Vector3Int saveOrigin, Vector3Int saveSize, string savePath)
        {
            BlockData[,,] blockData = ChunkHelper.GetBlocks(saveOrigin, saveSize.x, saveSize.y, saveSize.z);
         
            FileStream fs;
            if (File.Exists(savePath))
            {
                fs = new FileStream(savePath, FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            }

            GeneratingStructureData genData = new GeneratingStructureData(blockData, genParams);
            fs.Close();
            byte[] dataBytes = MessagePackSerializer.Serialize(genData);
            File.WriteAllBytes(savePath, dataBytes);
        }
        public static BlockData[,,] LoadStructure(string savePath)
        {
            byte[] dataBytes;
            try
            {
                dataBytes = File.ReadAllBytes(savePath);
            }
            catch(Exception e) 
            {
                Debug.WriteLine(e);
                return null;
            }

            if (dataBytes.Length <= 0)
            {
                return null;
            }

            StructureData data = MessagePackSerializer.Deserialize<StructureData>(dataBytes);
            return data.blockDatas;
        }

        public static GeneratingStructureData? LoadGeneratingStructure(string savePath)
        {
            byte[] dataBytes;
            try
            {
                dataBytes = File.ReadAllBytes(savePath);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

            if (dataBytes.Length <= 0)
            {
                return null;
            }

            GeneratingStructureData data = MessagePackSerializer.Deserialize<GeneratingStructureData>(dataBytes);
            return data;
        }
    }
}
