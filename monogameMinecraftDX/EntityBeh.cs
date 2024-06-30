using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using monogameMinecraftDX.World;
using System;
using System.Collections.Generic;
using monogameMinecraftDX.Animations;
using monogameMinecraftDX.Physics;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.Rendering;
using monogameMinecraftDX.Utility;
namespace monogameMinecraftDX
{
    public class EntityBeh:IMovableCollider
    {
      
        public Vector3 position { get; set; }
        public float rotationX;
        public float rotationY;
        public float bodyRotationY;
        public Quaternion bodyQuat;
        public float rotationZ;
        public int typeID;
        public string entityID;
        public BoundingBox bounds { get; set; }
        public List<BoundingBox> blocksAround;
        public static float gravity = -9.8f;
        public Vector3 entityVec;
        public Vector3 entitySize;
        public bool isGround = false;
        public float entityHealth;
        public bool isEntityHurt;
        public float entityHurtCD;
        public Vector3 entityMotionVec;
        public MinecraftGame game;
        public Vector3 targetPos;
        public Vector3Int lastIntPos;
        public bool isNeededUpdateBlock;
        public float entityGravity;
        public float entityLifetime;
        public float curSpeed;
        public bool isEntityDying = false;
        public float entityDyingTime = 0f;
        // public AnimationState animationState;
        public AnimationBlend animationBlend;

        public EntityBeh(Vector3 position, float rotationX, float rotationY, float rotationZ, int typeID, string entityID, float entityHealth, bool isEntityHurt, MinecraftGame game)
        {
            this.position = position;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.typeID = typeID;
            this.entityID = entityID;
            this.entityHealth = entityHealth;
            this.isEntityHurt = isEntityHurt;
            this.game = game;
            isEntityDying = false;
     /*       switch (typeID)
            {
                case 0:
                    animationBlend = new AnimationBlend(new AnimationState[] { new AnimationState(EntityManager.zombieAnim, EntityRenderer.zombieModel), new AnimationState(entityDieAnim, EntityRenderer.zombieModel) }, EntityRenderer.zombieModel);
                    break;
                default:
                    break;
            }*/
        }
      
        public void SaveSingleEntity()
        {

            EntityData tmpData = new EntityData(this.typeID, position.X, position.Y + entitySize.Y / 2f, position.Z, this.rotationX, this.rotationY, this.rotationZ, this.entityID, this.entityHealth, VoxelWorld.currentWorld.worldID);

            foreach (EntityData ed in EntityManager.entityDataReadFromDisk)
            {
                if (ed.entityID == this.entityID)
                {

                    EntityManager.entityDataReadFromDisk.Remove(ed);
                    break;
                }
            }

            EntityManager.entityDataReadFromDisk.Add(tmpData);
        }
       
      

        public void InitBounds()
        {
            bounds = new BoundingBox(position - entitySize / 2f, position + entitySize / 2f);
        }
        public bool CheckIsGround()
        {
            Vector3 pos = new Vector3((bounds.Min.X + bounds.Max.X) / 2f, bounds.Min.Y - 0.1f, (bounds.Min.Z + bounds.Max.Z) / 2f);

            int blockID = ChunkHelper.GetBlock(pos);

            if (blockID > 0 && blockID < 100)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
       
        public Chunk curChunk;
        public bool lastChunkIsReadyToRender;
        public float Vec3Magnitude(Vector3 pos)
        {
            return (float)Math.Sqrt(pos.X * pos.X + pos.Y * pos.Y + pos.Z * pos.Z);
        }
        public virtual void OnUpdate(float deltaTime)
        {


          /*  switch (typeID)
            {
                case 0:
                    if (isEntityDying == true)
                    {
                        entityDyingTime += deltaTime;
                        isEntityHurt = true;
                        animationBlend.Update(deltaTime, 0f, 1f);

                        if (entityDyingTime >= 1f && isEntityDying)
                        {
                            worldEntities.Remove(this);

                        }
                        return;
                    }
                    animationBlend.Update(deltaTime, MathHelper.Clamp(curSpeed / 3f, 0f, 1f), 0f);
                    entityLifetime += deltaTime;
                    targetPos = game.gamePlayer.position;
                    entityMotionVec = Vector3.Lerp(entityMotionVec, Vector3.Zero, 3f * deltaTime);

                    curSpeed = MathHelper.Lerp(curSpeed, (new Vector2(position.X, position.Z) - new Vector2(lastPos.X, lastPos.Z)).Length() / deltaTime, 5f * deltaTime);
                    //        Debug.WriteLine(curSpeed);
                    lastPos = position;
                    Vector3Int intPos = Vector3Int.FloorToIntVec3(position);

                    curChunk = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(position));


                    if (curChunk != null)
                    {
                        if (lastChunkIsReadyToRender != curChunk.isReadyToRender && (lastChunkIsReadyToRender == false && curChunk.isReadyToRender == true))
                        {
                            //  Debug.WriteLine("update");
                            isNeededUpdateBlock = true;
                            //      GetBlocksAround(bounds);
                        }
                    }

                    if (curChunk != null)
                    {
                        lastChunkIsReadyToRender = curChunk.isReadyToRender;
                    }





                    if (lastIntPos != intPos)
                    {
                        isNeededUpdateBlock = true;
                    }
                    lastIntPos = intPos;
                    if (isNeededUpdateBlock)
                    {
                        GetBlocksAround(bounds);
                        isNeededUpdateBlock = false;
                    }
                    GetEntitiesAround();


                    if (Vector3.Distance(position, targetPos) > 1f)
                    {
                        Vector3 movePos = new Vector3(targetPos.X - position.X, 0, targetPos.Z - position.Z);
                        if (movePos.X == 0 && movePos.Y == 0 && movePos.Z == 0)
                        {
                            movePos = new Vector3(0.001f, 0.001f, 0.001f);
                        }
                        Vector3 lookPos = new Vector3(targetPos.X - position.X, targetPos.Y - position.Y - 1f, targetPos.Z - position.Z);
                        Vector3 movePosN = Vector3.Normalize(movePos) * 5f * deltaTime;
                        entityVec = movePosN;
                        //              Debug.WriteLine(movePos);
                        Vector3 entityRot = LookRotation(lookPos);
                        rotationX = entityRot.X; rotationY = entityRot.Y; rotationZ = entityRot.Z;
                        Quaternion headQuat = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), 0, 0);
                        bodyQuat = Quaternion.Lerp(bodyQuat, headQuat, 10f * deltaTime);




                    }




                    //  Debug.WriteLine(curSpeed);

                    //     }

                    //   EntityMove(entityVec.X, entityVec.Y, entityVec.Z);

                    //     Debug.WriteLine(position.X + " " + position.Y + " " + position.Z);
                    if (entityHealth <= 0f)
                    {
                        isEntityDying = true;
                    }

                    if (entityHurtCD >= 0f)
                    {
                        entityHurtCD -= (1f * deltaTime);
                        isEntityHurt = true;
                    }
                    else
                    {
                        isEntityHurt = false;
                    }


                    Vector3 movePos1 = new Vector3(targetPos.X - position.X, 0, targetPos.Z - position.Z);

                    if (Vector3.Distance(position, targetPos) > 2f)
                    {

                        if (isGround && curSpeed <= 0.1f && Vec3Magnitude(movePos1) > 2f)
                        {

                            entityGravity = 5f;
                        }


                        if (entityMotionVec.Length() < 2f)
                        {
                            EntityMove(entityVec.X, 0, entityVec.Z);
                        }





                    }
                    entityVec.Y = entityGravity * deltaTime;




                    Vector3 entityMotionVecN = entityMotionVec * deltaTime;


                    EntityMove(entityMotionVecN.X, entityMotionVecN.Y, entityMotionVecN.Z);
                    EntityMove(0, entityVec.Y, 0);
                    if (isGround)
                    {

                        entityGravity = 0f;
                    }
                    else
                    {

                        entityGravity += -9.8f * deltaTime;
                    }

                    break;
            }*/

        }

        public void OnDraw()
        {

        }
     
        protected Vector3 ToEulerAngles(Quaternion q)
        {
            Vector3 angles;

            // roll (x-axis rotation)
            float sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            float cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.Z = MathF.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            float sinp = 2 * (q.W * q.Y - q.Z * q.X);

            angles.X = MathF.Asin(sinp);

            // yaw (z-axis rotation)
            float siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            float cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Y = MathF.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        public Vector3 LookRotation(Vector3 fromDir)
        {
            Vector3 eulerAngles = new Vector3();

            //AngleX = arc cos(sqrt((x^2 + z^2)/(x^2+y^2+z^2)))
            eulerAngles.X = (float)Math.Acos(Math.Sqrt((fromDir.X * fromDir.X + fromDir.Z * fromDir.Z) / (fromDir.X * fromDir.X + fromDir.Y * fromDir.Y + fromDir.Z * fromDir.Z))) * 360f / (MathF.PI * 2f);
            if (fromDir.Y > 0) eulerAngles.X = 360f - eulerAngles.X;

            //AngleY = arc tan(x/z)
            eulerAngles.Y = (float)Math.Atan2((float)fromDir.X, (float)fromDir.Z) * 360f / (MathF.PI * 2f);
            if (eulerAngles.Y < 0) eulerAngles.Y += 180f;
            if (fromDir.X < 0) eulerAngles.Y += 180f;
            //AngleZ = 0
            eulerAngles.Z = 0f;
            return eulerAngles;
        }


        protected void EntityMove(float dx, float dy, float dz)
        {


            float movX = dx;
            float movY = dy;
            float movZ = dz;


            List<BoundingBox> allBounds = new List<BoundingBox>();
            allBounds.AddRange(entitiyBoundsAround);
            allBounds.AddRange(blocksAround);


            foreach (var bb in allBounds)
            {
                dy = BlockCollidingBoundingBoxHelper.calculateYOffset(bb, bounds, dy);
            }

            bounds = BlockCollidingBoundingBoxHelper.offset(bounds, 0, dy, 0);

            if (movY != dy && movY < 0)
            {
                isGround = true;
                //     curGravity = 0f;
            }
            else
            {
                isGround = false;
            }


            foreach (var bb in allBounds)
            {
                dx = BlockCollidingBoundingBoxHelper.calculateXOffset(bb, bounds, dx);
            }

            bounds = BlockCollidingBoundingBoxHelper.offset(bounds, dx, 0, 0);

            foreach (var bb in allBounds)
            {
                dz = BlockCollidingBoundingBoxHelper.calculateZOffset(bb, bounds, dz);
            }

            bounds = BlockCollidingBoundingBoxHelper.offset(bounds, 0, 0, dz);
            position = new Vector3((bounds.Min.X + bounds.Max.X) / 2f, bounds.Min.Y, (bounds.Min.Z + bounds.Max.Z) / 2f);
        }
        public List<BoundingBox> entitiyBoundsAround;
        public void GetEntitiesAround()
        {
            entitiyBoundsAround = new List<BoundingBox>();
            foreach (var entity in EntityManager.worldEntities)
            {
                if (entity != this)
                {
                    if (MathF.Abs(entity.position.X - position.X) < 10f && MathF.Abs(entity.position.Y - position.Y) < 10f && MathF.Abs(entity.position.Z - position.Z) < 10f)
                    {
                        this.entitiyBoundsAround.Add(new BoundingBox(entity.bounds.Min, entity.bounds.Max));
                    }
                }
            }
        }
        public List<BoundingBox> GetBlocksAround(BoundingBox aabb)
        {

            int minX = ChunkHelper.FloorFloat(aabb.Min.X - 0.1f);
            int minY = ChunkHelper.FloorFloat(aabb.Min.Y - 0.1f);
            int minZ = ChunkHelper.FloorFloat(aabb.Min.Z - 0.1f);
            int maxX = ChunkHelper.CeilFloat(aabb.Max.X + 0.1f);
            int maxY = ChunkHelper.CeilFloat(aabb.Max.Y + 0.1f);
            int maxZ = ChunkHelper.FloorFloat(aabb.Max.Z + 0.1f);

            this.blocksAround = new List<BoundingBox>();

            for (int z = minZ - 1; z <= maxZ + 1; z++)
            {
                for (int x = minX - 1; x <= maxX + 1; x++)
                {
                    for (int y = minY - 1; y <= maxY + 1; y++)
                    {
                        BlockData blockID = ChunkHelper.GetBlockData(new Vector3(x, y, z));
                       
                            this.blocksAround.Add(BlockBoundingBoxUtility.GetBoundingBox(x, y, z, blockID));
                       
                    }
                }
            }


            return this.blocksAround;


        }
    }

}
