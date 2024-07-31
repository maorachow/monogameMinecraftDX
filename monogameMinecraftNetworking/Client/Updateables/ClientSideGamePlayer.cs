using MessagePack;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Physics;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.World;
using monogameMinecraftShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftNetworking.Data;

namespace monogameMinecraftNetworking.Client.Updateables
{
    public class ClientSideGamePlayer : IMovableCollider, IGamePlayer
    {
        public Camera cam { get; set; }
        public BoundingBox bounds { get; set; }
        public Vector3 position { get; set; }
        public float moveVelocity = 5f;
        float fastPlayerSpeed = 20f;
        float slowPlayerSpeed = 5f;
        public bool isLanded = false;
        public int currentSelectedHotbar = 0;
        public short[] inventoryData = new short[9];
        public static bool isPlayerDataSaved = false;
        public bool isChunkNeededUpdate = false;
        public ClientSideChunk curChunk;
        public int playerInWorldID;
        public GraphicsDevice graphicsDevice;

        
        public string playerName;
        public UserData ToUserData()
        {
            return new UserData(position.X, position.Y, position.Z, cam.Pitch, cam.Yaw, 0, playerName, false,
                ClientSideVoxelWorld.singleInstance.worldID);
        }
      /*  public static int ReadPlayerData(GamePlayer player, Game game, bool ExludePlayerInWorldIDData = false)
        {

            //   gameWorldDataPath = WorldManager.gameWorldDataPath;

            if (!Directory.Exists(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData");

            }
            if (!Directory.Exists(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData"))
            {
                Directory.CreateDirectory(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData");
            }

            if (!File.Exists(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/player.json"))
            {
                FileStream fs = File.Create(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/player.json");
                fs.Close();
            }

            byte[] playerDataBytes = File.ReadAllBytes(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData/player.json");
            /*  List<ChunkData> tmpList = new List<ChunkData>();
              foreach (string s in worldData)
              {
                  ChunkData tmp = JsonConvert.DeserializeObject<ChunkData>(s);
                  tmpList.Add(tmp);
              }
              foreach (ChunkData w in tmpList)
              {
                  chunkDataReadFromDisk.Add(new Vector2Int(w.chunkPos.x, w.chunkPos.y), w);
              }
            int playerInWorldID = 0;
            if (playerDataBytes.Length > 0)
            {
                PlayerData playerData = MessagePackSerializer.Deserialize<PlayerData>(playerDataBytes);
                //  Debug.WriteLine(playerData.posX);
                player.SetBoundPosition(new Vector3(playerData.posX, playerData.posY, playerData.posZ) + new Vector3(0f, 0.3f, 0f));
                player.inventoryData = (short[])playerData.inventoryData.Clone();

                player.GetBlocksAround(player.bounds);
                player.playerInWorldID = playerData.playerInWorldID;
                if (!ExludePlayerInWorldIDData)
                {
                    playerInWorldID = playerData.playerInWorldID;
                }

            }
            player.inventoryData[0] = 1;
            player.inventoryData[1] = 7;
            player.inventoryData[2] = 102;
            player.inventoryData[3] = 14;
            player.inventoryData[4] = 13;
            player.inventoryData[5] = 103;
            player.inventoryData[6] = 104;
            //    isJsonReadFromDisk = true;
            return playerInWorldID;
        }*/
        void SetBoundPosition(Vector3 pos)
        {
            bounds = new BoundingBox(pos - new Vector3(playerWidth / 2f, playerHeight / 2f, playerWidth / 2f), pos + new Vector3(playerWidth / 2f, playerHeight / 2f, playerWidth / 2f));

        }
  /*      public static void SavePlayerData(GamePlayer player, bool savePlayerInWorldID = true)
        {

            //   gameWorldDataPath = WorldManager.gameWorldDataPath;

            if (!Directory.Exists(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData");

            }
            if (!Directory.Exists(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData"))
            {
                Directory.CreateDirectory(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData");
            }

            if (!File.Exists(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/player.json"))
            {
                FileStream fs = File.Create(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/player.json");
                fs.Close();
            }

            byte[] playerDataBytes = MessagePackSerializer.Serialize(new PlayerData(player.position.X, player.position.Y, player.position.Z, player.inventoryData, savePlayerInWorldID == true ? VoxelWorld.currentWorld.worldID : player.playerInWorldID));
            Debug.WriteLine(playerDataBytes.Length);
            File.WriteAllBytes(ClientSideChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData/player.json", playerDataBytes);
            isPlayerDataSaved = true;
            /*  List<ChunkData> tmpList = new List<ChunkData>();
              foreach (string s in worldData)
              {
                  ChunkData tmp = JsonConvert.DeserializeObject<ChunkData>(s);
                  tmpList.Add(tmp);
              }
              foreach (ChunkData w in tmpList)
              {
                  chunkDataReadFromDisk.Add(new Vector2Int(w.chunkPos.x, w.chunkPos.y), w);
              }


            //    isJsonReadFromDisk = true;
        }*/
        public static float playerWidth = 0.6f;
        public static float playerHeight = 1.8f;
        public Vector3 GetBoundingBoxCenter(BoundingBox box)
        {
            return (box.Max + box.Min) / 2f;
        }
        public ClientSideGamePlayer(Vector3 min, Vector3 max, Game game,string name)
        {
            this.playerName=name;
            bounds = new BoundingBox(min, max);
            position = GetBoundingBoxCenter(bounds);
            cam = new Camera(position, new Vector3(0.0f, 0f, 1.0f), new Vector3(1.0f, 0f, 0.0f), Vector3.UnitY, game);
            GetBlocksAround(bounds);
        }
        public List<BoundingBox> blocksAround = new List<BoundingBox>();
        public List<BoundingBox> GetBlocksAround(BoundingBox aabb)
        {

            int minX = ClientSideChunkHelper.FloorFloat(aabb.Min.X - 0.1f);
            int minY = ClientSideChunkHelper.FloorFloat(aabb.Min.Y - 0.1f);
            int minZ = ClientSideChunkHelper.FloorFloat(aabb.Min.Z - 0.1f);
            int maxX = ClientSideChunkHelper.CeilFloat(aabb.Max.X + 0.1f);
            int maxY = ClientSideChunkHelper.CeilFloat(aabb.Max.Y + 0.1f);
            int maxZ = ClientSideChunkHelper.CeilFloat(aabb.Max.Z + 0.1f);

            blocksAround = new List<BoundingBox>();

            for (int z = minZ - 1; z <= maxZ + 1; z++)
            {
                for (int x = minX - 1; x <= maxX + 1; x++)
                {
                    for (int y = minY - 1; y <= maxY + 1; y++)
                    {
                        BlockData blockID = ClientSideChunkHelper.GetBlockData(new Vector3(x, y, z));

                        blocksAround.Add(BlockBoundingBoxUtility.GetBoundingBox(x, y, z, blockID));

                    }
                }
            }


            return blocksAround;


        }
        public bool TryHitEntity()
        {
            Microsoft.Xna.Framework.Ray ray = new Microsoft.Xna.Framework.Ray(cam.position, cam.front);
            float rayHitDis = 10000f;
            int finalIndex = -1;
            for (int i = 0; i < EntityManager.worldEntities.Count; i++)
            {
                EntityBeh entity = EntityManager.worldEntities[i];
                if (ray.Intersects(entity.bounds) <= 4f)
                {
                    if (rayHitDis > (float)ray.Intersects(entity.bounds))
                    {
                        finalIndex = i;
                    }
                    rayHitDis = MathF.Min(rayHitDis, (float)ray.Intersects(entity.bounds));

                    //     EntityBeh.HurtEntity(entity.entityID, 4f, cam.position);

                    //  return true;
                }
            }
            if (finalIndex >= 0)
            {
                EntityManager.HurtEntity(EntityManager.worldEntities[finalIndex].entityID, 4f, cam.position);
                return true;
            }
            return false;
        }
        Random rand = new Random();
        public bool BreakBlock()
        {
            monogameMinecraftShared. Physics.Ray ray = new monogameMinecraftShared.Physics.Ray(cam.position, cam.front);
            Vector3Int blockPoint = new Vector3Int(-1, -1, -1);
            BlockFaces blockFaces = BlockFaces.PositiveY;
            VoxelCast.CastClientSide(ray, 3, out blockPoint, out blockFaces, this);

            /*    for (int i = 0; i < 30; i++)
                {
                    ParticleManager.instance.SpawnNewParticleTexturedGravity(new Vector3(blockPoint.x + 0.5f, blockPoint.y + 0.5f, blockPoint.z + 0.5f), 0.2f, new Vector2(0f, 0f), new Vector2(1f, 1f), 3f, new Vector3(rand.NextSingle()*4f-2f, rand.NextSingle() * 4f - 2f, rand.NextSingle() * 4f - 2f), 3f);
                 }*/


            //     VoxelWorld.currentWorld.worldUpdater.queuedChunkUpdatePoints.Enqueue(new BreakBlockOperation(blockPoint, VoxelWorld.currentWorld.worldUpdater,ClientSideChunkHelper.GetBlockData(blockPoint)));
            //  ClientSideChunkHelper.BreakBlock(blockPoint);
            ClientSideChunkHelper.SendBreakBlockOperation(blockPoint, ClientSideVoxelWorld.singleInstance.worldID, ClientSideVoxelWorld.gameInstance.networkingClient.socket);
            Debug.WriteLine("client break block");
            GetBlocksAround(bounds);

            return true;

        }
        public void PlaceBlock()
        {

            if (inventoryData[currentSelectedHotbar] == 0)
            {
                return;
            }
            monogameMinecraftShared.Physics.Ray ray = new monogameMinecraftShared.Physics.Ray(cam.position, cam.front);
            Vector3Int blockPoint = new Vector3Int(-1, -1, -1);

            BlockFaces blockFaces = BlockFaces.PositiveY;
            VoxelCast.CastClientSide(ray, 3, out blockPoint, out blockFaces, this);
            if (blockPoint.y < 0)
            {
                return;
            }
            Vector3 setBlockPoint = new Vector3(blockPoint.x + 0.5f, blockPoint.y + 0.5f, blockPoint.z + 0.5f);

            Vector3 castBlockPoint = new Vector3(blockPoint.x, blockPoint.y, blockPoint.z);

            // ParticleManager.instance.SpawnNewParticle(setBlockPoint,1f,new Vector2(0f,0f),new Vector2(1f,1f),1f,new Vector3(1f,1f,1f),1f);

            switch (blockFaces)
            {
                case BlockFaces.PositiveX:
                    setBlockPoint.X += 0.6f;

                    break;
                case BlockFaces.PositiveY:
                    setBlockPoint.Y += 0.6f;

                    break;
                case BlockFaces.PositiveZ:
                    setBlockPoint.Z += 0.6f;

                    break;
                case BlockFaces.NegativeX:
                    setBlockPoint.X += -0.6f;

                    break;
                case BlockFaces.NegativeY:
                    setBlockPoint.Y += -0.6f;

                    break;
                case BlockFaces.NegativeZ:
                    setBlockPoint.Z += -0.6f;

                    break;
            }
            //interactable blocks

            if (ClientSideChunkHelper.GetBlockShape(ClientSideChunkHelper.GetBlockData(castBlockPoint)) is BlockShape.Door)
            {
              //  VoxelWorld.currentWorld.worldUpdater.queuedChunkUpdatePoints.Enqueue(new DoorInteractingOperation((Vector3Int)castBlockPoint));
                return;
            }



            Vector3Int setBlockPointInt = ClientSideChunkHelper.Vec3ToBlockPos(setBlockPoint);
            switch (Chunk.blockInfosNew[inventoryData[currentSelectedHotbar]].shape)
            {
                case BlockShape.Solid:

                    // ClientSideChunkHelper.SetBlockWithUpdate(setBlockPoint, inventoryData[currentSelectedHotbar]);
                //    ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, inventoryData[currentSelectedHotbar]);
                    break;
                case BlockShape.Torch:


                    /*  if ((setBlockPointInt - blockPointInt).x < -0.5f)
                      {
                          ClientSideChunkHelper.SetBlockWithUpdate(setBlockPoint,new BlockData( inventoryData[currentSelectedHotbar],1));
                          GetBlocksAround(bounds);
                          return;
                      }
                      if ((setBlockPointInt - blockPointInt).x >0.5f)
                      {
                          ClientSideChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 2));
                          GetBlocksAround(bounds);
                          return;
                      }
                      if ((setBlockPointInt - blockPointInt).z < -0.5f)
                      {
                          ClientSideChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 3));
                          GetBlocksAround(bounds);
                          return;
                      }
                      if ((setBlockPointInt - blockPointInt).z >0.5f)
                      {
                          ClientSideChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 4));
                          GetBlocksAround(bounds);
                          return;
                      }*/
                    switch (blockFaces)
                    {
                        case BlockFaces.PositiveX:
                     //       ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 2));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.PositiveY:
                       //     ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;


                        case BlockFaces.PositiveZ:
                     //       ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 4));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.NegativeX:
                      //      ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 1));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.NegativeY:
                            return;

                        case BlockFaces.NegativeZ:
                     //       ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 3));
                            GetBlocksAround(bounds);
                            return;

                    }
                    break;
                case BlockShape.Slabs:
                    switch (blockFaces)
                    {
                        case BlockFaces.PositiveX:
                      //      ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.PositiveY:
                        //    ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;


                        case BlockFaces.PositiveZ:
                      //      ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.NegativeX:
                     //       ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.NegativeY:
                        //    ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 1));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.NegativeZ:
                         //   ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;

                    }
                    break;
                case BlockShape.Fence:
                    //      bool[] data = new bool[8] { false, false, false, false, true, true, true, true };
                //    ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt,
               //          new BlockData(inventoryData[currentSelectedHotbar], 0));
                  //  VoxelWorld.currentWorld.worldUpdater.queuedChunkUpdatePoints.Enqueue(new FenceUpdatingOperation(setBlockPointInt, VoxelWorld.currentWorld.worldUpdater, new Vector3Int(0, 0, 0), 0));
                    GetBlocksAround(bounds);
                    break;


                case BlockShape.Door:

                    bool[] dataBools = new[] { false, false, false, false, false, false, false, false };

                    switch (blockFaces)
                    {
                        case BlockFaces.PositiveX:
                            return;

                        case BlockFaces.PositiveY:
                            if (MathF.Abs(ray.direction.X) > MathF.Abs(ray.direction.Z))
                            {
                                if (ray.direction.X < 0)
                                {
                                    dataBools[6] = false;
                                    dataBools[7] = true;
                                }
                                else
                                {
                                    dataBools[6] = false;
                                    dataBools[7] = false;
                                }
                            }
                            else
                            {
                                if (ray.direction.Z < 0)
                                {
                                    dataBools[6] = true;
                                    dataBools[7] = true;
                                }
                                else
                                {
                                    dataBools[6] = true;
                                    dataBools[7] = false;
                                }
                            }
                            break;


                        case BlockFaces.PositiveZ:
                            return;

                        case BlockFaces.NegativeX:
                            return;

                        case BlockFaces.NegativeY:
                            return;

                        case BlockFaces.NegativeZ:
                            return;

                    }

                    byte optionalDataVal = MathUtility.GetByte(dataBools);
                    if (ClientSideChunkHelper.GetBlock(setBlockPoint + new Vector3(0, 1, 0)) == 0)
                    {
                     //   ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, new BlockData(inventoryData[currentSelectedHotbar], optionalDataVal));
                    //    VoxelWorld.currentWorld.worldUpdater.queuedChunkUpdatePoints.Enqueue(new DoorUpperPartPlacingOperation(setBlockPointInt));
                    }


                    break;
                default:
                  //  ClientSideChunkHelper.SendPlaceBlockOperation(setBlockPointInt, inventoryData[currentSelectedHotbar]);
                    break;
            }


            GetBlocksAround(bounds);
        }
        public void Move(Vector3 moveVec, bool isClipable)
        {

            //  this.ySize *= 0.4;
            float dx = moveVec.X;
            float dy = moveVec.Y;
            float dz = moveVec.Z;
            float movX = dx;
            float movY = dy;
            float movZ = dz;
            if (isClipable)
            {
                bounds = BlockCollidingBoundingBoxHelper.offset(bounds, 0, dy, 0);
                bounds = BlockCollidingBoundingBoxHelper.offset(bounds, dx, 0, 0);
                bounds = BlockCollidingBoundingBoxHelper.offset(bounds, 0, 0, dz);
                position = GetBoundingBoxCenter(bounds);
                cam.position = position + new Vector3(0f, 0.6f, 0f);

                return;
            }
            if (blocksAround.Count == 0)
            {
                bounds = BlockCollidingBoundingBoxHelper.offset(bounds, 0, dy, 0);
                bounds = BlockCollidingBoundingBoxHelper.offset(bounds, dx, 0, 0);
                bounds = BlockCollidingBoundingBoxHelper.offset(bounds, 0, 0, dz);
                position = GetBoundingBoxCenter(bounds);
                cam.position = position + new Vector3(0f, 0.6f, 0f);

                return;
            }





            foreach (var bb in blocksAround)
            {
                dy = BlockCollidingBoundingBoxHelper.calculateYOffset(bb, bounds, dy);
            }

            bounds = BlockCollidingBoundingBoxHelper.offset(bounds, 0, dy, 0);

            if (movY != dy && movY < 0)
            {
                isLanded = true;
                //     curGravity = 0f;
            }
            else
            {
                isLanded = false;
            }

            //      bool fallingFlag = (this.onGround || (dy != movY && movY < 0));

            foreach (var bb in blocksAround)
            {
                dx = BlockCollidingBoundingBoxHelper.calculateXOffset(bb, bounds, dx);
            }

            bounds = BlockCollidingBoundingBoxHelper.offset(bounds, dx, 0, 0);

            foreach (var bb in blocksAround)
            {
                dz = BlockCollidingBoundingBoxHelper.calculateZOffset(bb, bounds, dz);
            }
            //    Debug.WriteLine(dx + " " + movX + " " + dy + " " + movY + " " + dz + " " + movZ) ;
            bounds = BlockCollidingBoundingBoxHelper.offset(bounds, 0, 0, dz);
            position = GetBoundingBoxCenter(bounds);
            cam.position = position + new Vector3(0f, 0.6f, 0f);

        }
        public void MoveToPosition(Vector3 pos)
        {
            bounds = new BoundingBox(pos - new Vector3(0.3f, 0.9f, 0.3f), pos + new Vector3(0.3f, 0.9f, 0.3f));
            position = GetBoundingBoxCenter(bounds);
            cam.position = position + new Vector3(0f, 0.6f, 0f);
        }
        public Vector3Int playerCurIntPos;
        public Vector3Int playerLastIntPos;
        public float curGravity;
        public void ApplyGravity(float deltaTime)
        {

            if (isPlayerFlying)
            {
                return;
            }
            if (isLanded == true)
            {
                curGravity = 0f;
            }
            else
            {
                curGravity += deltaTime * -9.8f;
                curGravity = MathHelper.Clamp(curGravity, -25f, 25f);
            }
        }
        public void Jump()
        {


            if (isLanded == true)
            {



                curGravity = 5f;
            }


        }
        bool isPlayerFlying = false;

        void UpdatePlayerChunk()
        {
            //    Debug.WriteLine(ChunkManager.CheckIsPosInChunkBorder(position, curChunk));
            if (ClientSideChunkHelper.CheckIsPosInChunkBorder(position, curChunk) || !ClientSideChunkHelper.CheckIsPosInChunk(position, curChunk))
            {
                isChunkNeededUpdate = true;
                curChunk = ClientSideChunkHelper.GetChunk(ClientSideChunkHelper.Vec3ToChunkPos(position));
            }
            //  isChunkNeededUpdate = true;
        }

        float playerTeleportingCD = 0f;
        public void PlayerTryTeleportToEnderWorld(ClientGameBase game, float deltaTime)
        {
      /*      if (playerTeleportingCD >= 4f)
            {
                switch (VoxelWorld.currentWorld.worldID)
                {
                    case 0:
                        VoxelWorld.voxelWorlds[1].actionOnSwitchedWorld = () =>
                        {
                            Debug.WriteLine("action teleport to world 1");
                            MoveToPosition(new Vector3(0f, 150f, 0f));
                        };
                        VoxelWorld.SwitchToWorld(1, game);
                        break;
                    case 1:
                        VoxelWorld.voxelWorlds[0].actionOnSwitchedWorld = () =>
                        {
                            Debug.WriteLine("action teleport to world 0");
                            MoveToPosition(new Vector3(0f, 150f, 0f));
                        };
                        VoxelWorld.SwitchToWorld(0, game);
                        break;
                    default:
                        break;
                }
                playerTeleportingCD = 0f;
            }
            else
            {
                playerTeleportingCD += deltaTime;
            }
      */




        }
        public void UpdatePlayer(ClientGameBase game, float deltaTime)
        {
            isChunkNeededUpdate = true;
            UpdatePlayerChunk();
            GetBlockOnFoot(game, deltaTime);
            UpdatePlayerMovement(deltaTime);
            ApplyGravity(deltaTime);

        }
        public float jumpCD = 0f;
        public bool isJumping = false;
        public Vector3 finalMoveVec;
        public bool isLeftMouseButtonDown = false;
        public bool isRightMouseButtonDown = false;
        public void UpdatePlayerMovement(float deltaTime)
        {

            if (breakBlockCD > 0f)
            {
                breakBlockCD -= deltaTime;
            }
            if (jumpCD >= 0f)
            {
                jumpCD -= deltaTime;
            }
            if (isJumping == true)
            {
                Jump();
                isJumping = false;
            }
            float finalY = 0f;
            if (isPlayerFlying == true)
            {
                finalY = finalMoveVec.Y;
            }
            else
            {
                finalY = curGravity * deltaTime;
            }
            if (finalMoveVec.X != 0.0f)
                Move(new Vector3((cam.horizontalRight * finalMoveVec.X).X, 0f, (cam.horizontalRight * finalMoveVec.X).Z), false);


            if (finalMoveVec.Z != 0.0f)
                Move(new Vector3((cam.horizontalFront * finalMoveVec.Z).X, 0f, (cam.horizontalFront * finalMoveVec.Z).Z), false);


            Move(new Vector3(0f, finalY, 0f), false);
            if (breakBlockCD <= 0f && isLeftMouseButtonDown == true)
            {
                bool isEntityHit = TryHitEntity();
                if (!isEntityHit)
                {
                    BreakBlock();
                }
                breakBlockCD = 0.15f;
            }
            if (breakBlockCD <= 0f && isRightMouseButtonDown == true)
            {
                PlaceBlock();
                breakBlockCD = 0.15f;
            }

        }
        public short prevBlockOnFootID = 0;
        public short blockOnFootID = 0;
        public void PlayerBlockOnFootChanged(ClientGameBase game, float deltaTime)
        {
            //   Debug.WriteLine(blockOnFootID);
            if (blockOnFootID == 13)
            {

                PlayerTryTeleportToEnderWorld(game, deltaTime);
            }
        }
        public void GetBlockOnFoot(ClientGameBase game, float deltaTime)
        {
            blockOnFootID = ClientSideChunkHelper.GetBlock(new Vector3((bounds.Min.X + bounds.Max.X) / 2f, bounds.Min.Y - 0.1f, (bounds.Min.Z + bounds.Max.Z) / 2f));
            PlayerBlockOnFootChanged(game, deltaTime);
            prevBlockOnFootID = blockOnFootID;
        }
        public void ProcessPlayerInputs(Vector3 dir, float deltaTime, KeyboardState kState, MouseState mState, MouseState prevMouseState, bool isFlyingPressed, bool isSpeedUpPressed, bool isLMBPressed, bool isRMBPressed, float scrollDelta)
        {

            playerCurIntPos = new Vector3Int((int)position.X, (int)position.Y, (int)position.Z);
            if (playerCurIntPos != playerLastIntPos)
            {
                GetBlocksAround(bounds);
            }
            playerLastIntPos = playerCurIntPos;

            finalMoveVec = deltaTime * moveVelocity * new Vector3(dir.X, dir.Y, dir.Z);
            //    Debug.WriteLine(finalMoveVec);
            if (isFlyingPressed && jumpCD <= 0f)
            {
                isPlayerFlying = !isPlayerFlying;
                jumpCD = 1f;
            }
            if (isSpeedUpPressed)
            {
                moveVelocity = fastPlayerSpeed;
            }
            else
            {
                moveVelocity = slowPlayerSpeed;
            }

            if (dir.Y > 0f)
            {
                //  Debug.WriteLine("dir up");
                if (jumpCD <= 0f)
                {
                    //        Debug.WriteLine("jump");
                    //Jump();
                    isJumping = true;
                    //  jumpCD = 0.01f;
                }

            }

            if (isLMBPressed)
            {
                isLeftMouseButtonDown = true;
            }
            else
            {
                isLeftMouseButtonDown = false;
            }
            if (isRMBPressed)
            {
                isRightMouseButtonDown = true;
            }
            else
            {
                isRightMouseButtonDown = false;
            }

            currentSelectedHotbar += (int)(scrollDelta / 120f);
            currentSelectedHotbar = MathHelper.Clamp(currentSelectedHotbar, 0, 8);
            //      Debug.WriteLine(mState.ScrollWheelValue - prevMouseState.ScrollWheelValue);

        }
        public float breakBlockCD = 0f;
    }
}
