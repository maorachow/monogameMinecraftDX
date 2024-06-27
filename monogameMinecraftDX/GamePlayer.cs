using MessagePack;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using monogameMinecraftDX.World;
using monogameMinecraftDX.Physics;
using monogameMinecraftDX.Core;
using Microsoft.Xna.Framework.Graphics;
namespace monogameMinecraftDX
{
    public class GamePlayer:IMovableCollider
    {
        public Camera cam;
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
        public Chunk curChunk;
        public int playerInWorldID;
        public GraphicsDevice graphicsDevice;
        public static int ReadPlayerData(GamePlayer player, Game game, bool ExludePlayerInWorldIDData = false)
        {

            //   gameWorldDataPath = WorldManager.gameWorldDataPath;

            if (!Directory.Exists(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData");

            }
            if (!Directory.Exists(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData"))
            {
                Directory.CreateDirectory(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData");
            }

            if (!File.Exists(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/player.json"))
            {
                FileStream fs = File.Create(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/player.json");
                fs.Close();
            }

            byte[] playerDataBytes = File.ReadAllBytes(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData/player.json");
            /*  List<ChunkData> tmpList = new List<ChunkData>();
              foreach (string s in worldData)
              {
                  ChunkData tmp = JsonConvert.DeserializeObject<ChunkData>(s);
                  tmpList.Add(tmp);
              }
              foreach (ChunkData w in tmpList)
              {
                  chunkDataReadFromDisk.Add(new Vector2Int(w.chunkPos.x, w.chunkPos.y), w);
              }*/
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
            //    isJsonReadFromDisk = true;
            return playerInWorldID;
        }
        void SetBoundPosition(Vector3 pos)
        {
            bounds = new BoundingBox(pos - new Vector3(playerWidth / 2f, playerHeight / 2f, playerWidth / 2f), pos + new Vector3(playerWidth / 2f, playerHeight / 2f, playerWidth / 2f));
            
        }
        public static void SavePlayerData(GamePlayer player, bool savePlayerInWorldID = true)
        {

            //   gameWorldDataPath = WorldManager.gameWorldDataPath;

            if (!Directory.Exists(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData");

            }
            if (!Directory.Exists(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData"))
            {
                Directory.CreateDirectory(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData");
            }

            if (!File.Exists(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/player.json"))
            {
                FileStream fs = File.Create(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData" + "/GameData/player.json");
                fs.Close();
            }

            byte[] playerDataBytes = MessagePackSerializer.Serialize(new PlayerData(player.position.X, player.position.Y, player.position.Z, player.inventoryData, savePlayerInWorldID == true ? VoxelWorld.currentWorld.worldID : player.playerInWorldID));
            Debug.WriteLine(playerDataBytes.Length);
            File.WriteAllBytes(ChunkHelper.gameWorldDataPath + "unityMinecraftServerData/GameData/player.json", playerDataBytes);
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
              }*/


            //    isJsonReadFromDisk = true;
        }
        public static float playerWidth = 0.6f;
        public static float playerHeight = 1.8f;
        public Vector3 GetBoundingBoxCenter(BoundingBox box)
        {
            return (box.Max + box.Min) / 2f;
        }
        public GamePlayer(Vector3 min, Vector3 max, Game game)
        {
            bounds = new BoundingBox(min, max);
            position = GetBoundingBoxCenter(bounds);
            cam = new Camera(position, new Vector3(0.0f, 0f, 1.0f), new Vector3(1.0f, 0f, 0.0f), Vector3.UnitY, game);
            GetBlocksAround(bounds);
        }
        public List< BoundingBox> blocksAround = new List< BoundingBox>();
        public List<BoundingBox> GetBlocksAround(BoundingBox aabb)
        {

            int minX = ChunkHelper.FloorFloat(aabb.Min.X - 0.1f);
            int minY = ChunkHelper.FloorFloat(aabb.Min.Y - 0.1f);
            int minZ = ChunkHelper.FloorFloat(aabb.Min.Z - 0.1f);
            int maxX = ChunkHelper.CeilFloat(aabb.Max.X + 0.1f);
            int maxY = ChunkHelper.CeilFloat(aabb.Max.Y + 0.1f);
            int maxZ = ChunkHelper.CeilFloat(aabb.Max.Z + 0.1f);

            this.blocksAround = new List< BoundingBox>();

            for (int z = minZ - 1; z <= maxZ + 1; z++)
            {
                for (int x = minX - 1; x <= maxX + 1; x++)
                {
                    for (int y = minY - 1; y <= maxY + 1; y++)
                    {
                        BlockData blockID = ChunkHelper.GetBlockData(new Vector3(x, y, z));
                       
                            this.blocksAround.Add(BlockBoundingBoxUtility.GetBoundingBox(x,y,z,blockID));
                        
                    }
                }
            }


              return this.blocksAround;


        }
        public bool TryHitEntity()
        {
            Microsoft.Xna.Framework.Ray ray = new Microsoft.Xna.Framework.Ray(cam.position, cam.front);
            float rayHitDis = 10000f;
            int finalIndex = -1;
            for (int i = 0; i < EntityBeh.worldEntities.Count; i++)
            {
                EntityBeh entity = EntityBeh.worldEntities[i];
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
                EntityBeh.HurtEntity(EntityBeh.worldEntities[finalIndex].entityID, 4f, cam.position);
                return true;
            }
            return false;
        }
        public bool BreakBlock()
        {
            monogameMinecraftDX.Physics. Ray ray = new monogameMinecraftDX.Physics.Ray(cam.position, cam.front);
            Vector3Int blockPoint =new Vector3Int(-1,-1,-1);
            BlockFaces blockFaces = BlockFaces.PositiveY;
           VoxelCast.Cast(ray,3,out blockPoint, out blockFaces,this, graphicsDevice);
           

            ChunkHelper.BreakBlock(blockPoint);
            GetBlocksAround(bounds);
        
                return true;
           
        }
        public void PlaceBlock()
        {
            if (inventoryData[currentSelectedHotbar] == 0)
            {
                return;
            }
           Physics. Ray ray = new Physics.Ray(cam.position, cam.front);
            Vector3Int blockPoint = new Vector3Int(-1,-1,-1);
           
            BlockFaces blockFaces = BlockFaces.PositiveY;
            VoxelCast.Cast(ray, 3, out blockPoint, out blockFaces,this, graphicsDevice);
            if(blockPoint.y < 0)
            {
                return ;
            }
            Vector3 setBlockPoint = new Vector3(blockPoint.x+0.5f, blockPoint.y + 0.5f, blockPoint.z + 0.5f);
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
                    setBlockPoint.Y +=  -0.6f;
                    break;
                case BlockFaces.NegativeZ:
                    setBlockPoint.Z += -0.6f;
                    break;
            }
            
            Vector3Int setBlockPointInt = ChunkHelper.Vec3ToBlockPos(setBlockPoint);
            switch (Chunk.blockInfosNew[inventoryData[currentSelectedHotbar]].shape)
            {
                case BlockShape.Solid:

                    ChunkHelper.SetBlockWithUpdate(setBlockPoint, inventoryData[currentSelectedHotbar]);
                    break;
                    case BlockShape.Torch:


                    /*  if ((setBlockPointInt - blockPointInt).x < -0.5f)
                      {
                          ChunkHelper.SetBlockWithUpdate(setBlockPoint,new BlockData( inventoryData[currentSelectedHotbar],1));
                          GetBlocksAround(bounds);
                          return;
                      }
                      if ((setBlockPointInt - blockPointInt).x >0.5f)
                      {
                          ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 2));
                          GetBlocksAround(bounds);
                          return;
                      }
                      if ((setBlockPointInt - blockPointInt).z < -0.5f)
                      {
                          ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 3));
                          GetBlocksAround(bounds);
                          return;
                      }
                      if ((setBlockPointInt - blockPointInt).z >0.5f)
                      {
                          ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 4));
                          GetBlocksAround(bounds);
                          return;
                      }*/
                    switch (blockFaces)
                    {
                        case BlockFaces.PositiveX:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 2));
                            GetBlocksAround(bounds);
                            return;
                         
                        case BlockFaces.PositiveY:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;


                        case BlockFaces.PositiveZ:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 4));
                            GetBlocksAround(bounds);
                            return;
                            
                        case BlockFaces.NegativeX:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 1));
                            GetBlocksAround(bounds);
                            return;
                          
                        case BlockFaces.NegativeY:
                            return;
                            
                        case BlockFaces.NegativeZ:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 3));
                            GetBlocksAround(bounds);
                            return;
                         
                    }
                    break;
                case BlockShape.Slabs:
                    switch (blockFaces)
                    {
                        case BlockFaces.PositiveX:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.PositiveY:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;


                        case BlockFaces.PositiveZ:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.NegativeX:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.NegativeY:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 1));
                            GetBlocksAround(bounds);
                            return;

                        case BlockFaces.NegativeZ:
                            ChunkHelper.SetBlockWithUpdate(setBlockPoint, new BlockData(inventoryData[currentSelectedHotbar], 0));
                            GetBlocksAround(bounds);
                            return;

                    }
                    break;
                default:
                    ChunkHelper.SetBlockWithUpdate(setBlockPoint, inventoryData[currentSelectedHotbar]);
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
                curGravity += deltaTime * (-9.8f);
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
             if (ChunkHelper.CheckIsPosInChunkBorder(position, curChunk) || !ChunkHelper.CheckIsPosInChunk(position, curChunk))
            {
                isChunkNeededUpdate = true;
                curChunk = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(position));
            }
      //  isChunkNeededUpdate = true;
        }

        float playerTeleportingCD = 0f;
        public void PlayerTryTeleportToEnderWorld(MinecraftGame game, float deltaTime)
        {
            if (playerTeleportingCD >= 4f)
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





        }
        public void UpdatePlayer(MinecraftGame game, float deltaTime)
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
                finalY=finalMoveVec.Y;
            }
            else
            {
                finalY=curGravity* deltaTime;
            }
            if (finalMoveVec.X != 0.0f)
                Move(new Vector3(((cam.horizontalRight * finalMoveVec.X).X), 0f, (cam.horizontalRight * finalMoveVec.X).Z), false);


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
        public void PlayerBlockOnFootChanged(MinecraftGame game, float deltaTime)
        {
            //   Debug.WriteLine(blockOnFootID);
            if (blockOnFootID == 13)
            {

                PlayerTryTeleportToEnderWorld(game, deltaTime);
            }
        }
        public void GetBlockOnFoot(MinecraftGame game, float deltaTime)
        {
            blockOnFootID = ChunkHelper.GetBlock(new Vector3((bounds.Min.X + bounds.Max.X) / 2f, bounds.Min.Y - 0.1f, (bounds.Min.Z + bounds.Max.Z) / 2f));
            PlayerBlockOnFootChanged(game, deltaTime);
            prevBlockOnFootID = blockOnFootID;
        }
        public void ProcessPlayerInputs(Vector3 dir, float deltaTime, KeyboardState kState, MouseState mState, MouseState prevMouseState)
        {

            playerCurIntPos = new Vector3Int((int)position.X, (int)position.Y, (int)position.Z);
            if (playerCurIntPos != playerLastIntPos)
            {
                GetBlocksAround(bounds);
            }
            playerLastIntPos = playerCurIntPos;

            finalMoveVec = deltaTime * moveVelocity * new Vector3(dir.X, dir.Y, dir.Z);
            //    Debug.WriteLine(finalMoveVec);
            if (kState.IsKeyDown(Keys.F) && jumpCD <= 0f)
            {
                isPlayerFlying = !isPlayerFlying;
                jumpCD = 1f;
            }
            if (kState.IsKeyDown(Keys.LeftControl))
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
          
            if (mState.LeftButton == ButtonState.Pressed)
            {
                isLeftMouseButtonDown = true;
            }
            else
            {
                isLeftMouseButtonDown = false;
            }
            if (mState.RightButton == ButtonState.Pressed)
            {
                isRightMouseButtonDown = true;
            }
            else
            {
                isRightMouseButtonDown = false;
            }
            if (mState.ScrollWheelValue - prevMouseState.ScrollWheelValue != 0f)
            {
                currentSelectedHotbar += (int)((mState.ScrollWheelValue - prevMouseState.ScrollWheelValue) / 120f);
                currentSelectedHotbar = MathHelper.Clamp(currentSelectedHotbar, 0, 8);
                //      Debug.WriteLine(mState.ScrollWheelValue - prevMouseState.ScrollWheelValue);
            }
        }
        public float breakBlockCD = 0f;
    }

    public class Camera
    {


        public Camera(Vector3 position, Vector3 front, Vector3 right, Vector3 up, Game game)
        {
            this.position = position;
            this.front = front;
            this.right = right;

            aspectRatio = game.GraphicsDevice.DisplayMode.AspectRatio;
            //   this.worldUp = up;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), game.GraphicsDevice.DisplayMode.AspectRatio, 0.1f, 1000f);
        }

        public Vector3 position;
        public Vector3 front;
        public Vector3 right;
        public Vector3 up;
        public Vector3 horizontalFront;
        public Vector3 horizontalRight;
        public float aspectRatio;
        public static Vector3 worldUp = new Vector3(0f, 1f, 0f);
        public Matrix viewMatrix { get { return Matrix.CreateLookAt(position, position + front, up); } set { value = Matrix.CreateLookAt(position, position + front, up); } }
        public Matrix viewMatrixOrigin { get { return Matrix.CreateLookAt(new Vector3(0, 0, 0), front, up); } set { value = Matrix.CreateLookAt(new Vector3(0, 0, 0), front, up); } }
        public Matrix viewMatrixHorizontal { get { return Matrix.CreateLookAt(position, position + horizontalFront, worldUp); } set { value = Matrix.CreateLookAt(position, position + horizontalFront, worldUp); } }
        //public Matrix viewMatrix;
        public Matrix projectionMatrix;
        public float Yaw;
        public float Pitch;
        public float MovementSpeed;
        public float MouseSensitivity = 0.3f;
        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {

            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch += yoffset;

            // make sure that when pitch is out of bounds, screen doesn't get flipped
            if (constrainPitch)
            {

                if (Pitch > 89.0f)
                    Pitch = 89.0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            // update Front, Right and Up Vectors using the updated Euler angles
            updateCameraVectors();
        }
        public void updateCameraVectors()
        {
            // calculate the new Front vector
            Vector3 tmpfront;
            tmpfront.X = MathF.Cos(MathHelper.ToRadians(Yaw)) * MathF.Cos(MathHelper.ToRadians(Pitch));
            tmpfront.Y = MathF.Sin(MathHelper.ToRadians(Pitch));
            tmpfront.Z = MathF.Sin(MathHelper.ToRadians(Yaw)) * MathF.Cos(MathHelper.ToRadians(Pitch));
            //   Quaternion q = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(Yaw), MathHelper.ToRadians(Pitch), MathHelper.ToRadians(0f));
            // Matrix rotMat=Matrix.CreateFromQuaternion(q);



            //   var lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotMat);
            horizontalFront = new Vector3(tmpfront.X, 0, tmpfront.Z);
            horizontalFront.Normalize();

            front = tmpfront;
            front.Normalize();
            // also re-calculate the Right and Up vector
            Vector3 tmpright = (Vector3.Cross(front, worldUp));
            tmpright.Normalize();
            // normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            Vector3 tmpup = (Vector3.Cross(right, front));
            horizontalRight = Vector3.Cross(horizontalFront, worldUp);
            tmpup.Normalize();
            right = tmpright;
            up = tmpup;
            viewMatrix = Matrix.CreateLookAt(position, position + front, up);
            viewMatrixHorizontal = Matrix.CreateLookAt(position, position + horizontalFront, worldUp);
        }
        /*     public Matrix GetViewMatrix()
             {
                 viewMatrix = Matrix.CreateLookAt(position, position + front, up);
                 Debug.WriteLine("getmat");
                 return viewMatrix;
             }*/
    }
}
