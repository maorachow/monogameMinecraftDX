using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.WIC;

namespace monogameMinecraftDX.World
{
    public class StructureOperationsManager
    {
        public static void InvertBlockDataX(ref BlockData[,,] input)
        {
            int beginX = 0;
            int endX = input.GetLength(0)-1;
            while (beginX < endX)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    for (int z = 0; z < input.GetLength(2); z++)
                    {
                        BlockData tmp1= input[beginX, y, z];
                        BlockData tmp2 = input[endX, y, z];
                        input[beginX, y, z] = tmp2;
                        input[endX, y, z] = tmp1;
                       
                    }
                }
                beginX++;
                endX--;
            }
        }
        public static void InvertBlockDataY(ref BlockData[,,] input)
        {
            int beginY = 0;
            int endY = input.GetLength(1) - 1;
            while (beginY < endY)
            {
                for (int x = 0; x < input.GetLength(0); x++)
                {
                    for (int z = 0; z < input.GetLength(2); z++)
                    {
                        BlockData tmp1 = input[x, beginY, z];
                        BlockData tmp2 = input[x, endY, z];
                        input[x, beginY, z] = tmp2;
                        input[x, endY, z] = tmp1;
                      
                    }
                }
                beginY++;
                endY--;
            }
        }


        public static void InvertBlockDataZ(ref BlockData[,,] input)
        {
            int beginZ = 0;
            int endZ = input.GetLength(1) - 1;
            while (beginZ < endZ)
            {
                for (int x = 0; x < input.GetLength(0); x++)
                {
                    for (int y = 0; y < input.GetLength(1); y++)
                    {
                        BlockData tmp1 = input[x, y, beginZ];
                        BlockData tmp2 = input[x, y, endZ];
                        input[x, y, beginZ] = tmp2;
                        input[x, y, endZ] = tmp1;
                      
                    }
                }
                beginZ++;
                endZ--;
            }
        }

        public Dictionary<string,StructureData> allStructureDatas;

        public VoxelWorld curWorld;

        public Vector3Int curSaveStructureOrigin;
        public Vector3Int curSaveStructureSize;

        public StructureData? curPlacingStructureData;
        public Vector3Int curPlacingStructureOrigin;

        public string tmpSavingStructureName="structurename";

        public string tmpPlacingStructureName = "structurename";
        public Vector3Int curPlacingStructureSize
        {
            get
            {
                if (curPlacingStructureData!= null)
                {
                    return new Vector3Int(curPlacingStructureData.Value.blockDatas.GetLength(0),
                        curPlacingStructureData.Value.blockDatas.GetLength(1),
                        curPlacingStructureData.Value.blockDatas.GetLength(2));
                }
                else
                {
                    return new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

                }
            }
        }

        public bool isShowingStructureSavingBounds = false;
        public bool isShowingStructurePlacingBounds = false;
        public void DrawStructureSavingBounds(GamePlayer player)
        {
            if (isShowingStructureSavingBounds == true)
            {
                if (curSaveStructureOrigin.x > int.MinValue && curSaveStructureOrigin.y > int.MinValue &&
                    curSaveStructureOrigin.z > int.MinValue && curSaveStructureSize.x > int.MinValue &&
                    curSaveStructureSize.y > int.MinValue && curSaveStructureSize.z > int.MinValue)
                {
                    BoundingBoxVisualizationUtility.VisualizeBoundingBox(new BoundingBox(new Vector3(curSaveStructureOrigin.x, curSaveStructureOrigin.y, curSaveStructureOrigin.z), new Vector3(curSaveStructureOrigin.x + curSaveStructureSize.x, curSaveStructureOrigin.y + curSaveStructureSize.y, curSaveStructureOrigin.z + curSaveStructureSize.z)), player.cam.viewMatrix, player.cam.projectionMatrix);
                }
                
            }
       
        }

        public void DrawStructurePlacingBounds(GamePlayer player)
        {
            if (isShowingStructurePlacingBounds == true)
            {
                if (curPlacingStructureOrigin.x > int.MinValue && curPlacingStructureOrigin.y > int.MinValue &&
                    curPlacingStructureOrigin.z > int.MinValue && curPlacingStructureSize.x > int.MinValue &&
                    curPlacingStructureSize.y > int.MinValue && curPlacingStructureSize.z > int.MinValue)
                {
                    BoundingBoxVisualizationUtility.VisualizeBoundingBox(
                        new BoundingBox(
                            new Vector3(curPlacingStructureOrigin.x, curPlacingStructureOrigin.y, curPlacingStructureOrigin.z),
                            new Vector3(curPlacingStructureOrigin.x + curPlacingStructureSize.x,
                                curPlacingStructureOrigin.y + curPlacingStructureSize.y,
                                curPlacingStructureOrigin.z + curPlacingStructureSize.z)), player.cam.viewMatrix,
                        player.cam.projectionMatrix);
                }

            }

        }
        public void ReadStructureDatas()
        {
            allStructureDatas=new Dictionary<string, StructureData>();

            if (!Directory.Exists(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData");

            }
            if (!Directory.Exists(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData/GameData"))
            {
                Directory.CreateDirectory(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData/GameData");
            }

            if (!File.Exists(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/" + Path.GetFileNameWithoutExtension(curWorld.curWorldSaveName) + "/savedstructures"))
            {
              Directory.CreateDirectory(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData" +"/GameData/" + Path.GetFileNameWithoutExtension(curWorld.curWorldSaveName) + "/savedstructures");
                
            }

            string path = VoxelWorld.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/" + Path.GetFileNameWithoutExtension(curWorld.curWorldSaveName) + "/savedstructures";
            DirectoryInfo d = new DirectoryInfo(path);

            FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is FileInfo)     
                {
                    Debug.WriteLine(fsinfo.FullName);
                    
                    string ext =Path.GetExtension(fsinfo.FullName);
                    string fileName=Path.GetFileNameWithoutExtension(fsinfo.FullName);
                    if (ext == ".bin")
                    {
                        byte[] structureData = File.ReadAllBytes(fsinfo.FullName);
                       
                         
                        if (structureData.Length > 0)
                        {
                            try
                            {
                                StructureData data = MessagePackSerializer.Deserialize<StructureData>(structureData);
                                allStructureDatas.TryAdd(fileName, data);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                            }
                          
                        }
                    }
                     

                }
               
            }
 


        }

        public bool isStructurePlacingInvertX = false;
        public bool isStructurePlacingInvertY = false;
        public bool isStructurePlacingInvertZ = false;
        public void PlaceSavedStructureWithTmpParams()
        {
            if (curPlacingStructureOrigin.x > int.MinValue && curPlacingStructureOrigin.y > int.MinValue &&
                curPlacingStructureOrigin.z > int.MinValue && curPlacingStructureSize.x > int.MinValue &&
                curPlacingStructureSize.y > int.MinValue && curPlacingStructureSize.z > int.MinValue)
            {

                PlaceStructure(curPlacingStructureOrigin,tmpPlacingStructureName,isStructurePlacingInvertX,isStructurePlacingInvertY,isStructurePlacingInvertZ);
            }
        }
        public void PlaceStructure(Vector3Int origin,string structureName,bool inverseX=false,bool inverseY=false,bool inverseZ=false)
        {
            if (!allStructureDatas.ContainsKey(structureName))
            {
                Debug.WriteLine("structure not found");
                return;
            }
            StructureData data = new StructureData((BlockData[,,])allStructureDatas[structureName].blockDatas.Clone());
            if (inverseX)
            {
                InvertBlockDataX(ref data.blockDatas);
            }
            if (inverseY)
            {
                InvertBlockDataY(ref data.blockDatas);
            }
            if (inverseZ)
            {
                InvertBlockDataZ(ref data.blockDatas);
            }
            ChunkHelper.FillBlocks(data.blockDatas,origin,BlockFillMode.Default,true,true);
        }


        public void AddOrReplaceStructure(string name, StructureData data)
        {
            allStructureDatas.TryAdd(name,data);
            if (allStructureDatas.ContainsKey(name))
            {
                allStructureDatas[name] = data;
            }
        }

        public void SaveNewStructure(string name)
        {

            if (curSaveStructureOrigin.x > int.MinValue && curSaveStructureOrigin.y > int.MinValue &&
                curSaveStructureOrigin.z > int.MinValue && curSaveStructureSize.x > int.MinValue &&
                curSaveStructureSize.y > int.MinValue && curSaveStructureSize.z > int.MinValue)
            {
                if (curSaveStructureSize.x <128 &&
                    curSaveStructureSize.y <128 && curSaveStructureSize.z<128&& curSaveStructureSize.x > 0 &&
                    curSaveStructureSize.y >0 && curSaveStructureSize.z>0)
                {
                    BlockData[,,] blockDatas = ChunkHelper.GetBlocks(curSaveStructureOrigin, curSaveStructureSize.x,
                        curSaveStructureSize.y, curSaveStructureSize.z);
                    StructureData data = new StructureData(blockDatas);
                    AddOrReplaceStructure(name, data);
                    return;
                }
            }
              Debug.WriteLine("invalid structure saving params");
        }
        public void SaveAllStructures()
        {
          
            if (Directory.Exists(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData/GameData/" +Path.GetFileNameWithoutExtension(curWorld.curWorldSaveName)+ "/savedstructures"))
            {
             
            }
            else
            {
                Directory.CreateDirectory(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/" + Path.GetFileNameWithoutExtension(curWorld.curWorldSaveName) + "/savedstructures");
            }
       
            foreach (var data in allStructureDatas)
            {
                byte[] dataBytes = MessagePackSerializer.Serialize(data.Value);
                File.WriteAllBytes(VoxelWorld.gameWorldDataPath + "unityMinecraftServerData/GameData/" + Path.GetFileNameWithoutExtension(curWorld.curWorldSaveName) + "/savedstructures/"+data.Key+".bin", dataBytes);
            }


        }
        public StructureOperationsManager(VoxelWorld world)
        {
            this.curWorld = world;
        }
    }
}
