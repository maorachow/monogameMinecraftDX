using monogameMinecraftShared.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.World;
using monogameMinecraftShared.Animations;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Pathfinding;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.World;
using monogameMinecraftShared;
using EntityData = monogameMinecraftNetworking.Data.EntityData;
namespace monogameMinecraftNetworking.Updateables
{
    public class ServerSideEntityBeh : IMovableCollider
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
      
        public Vector3 targetPos;
        public Vector3Int lastIntPos;
        public bool isNeededUpdateBlock;
        public float entityGravity;
        public float entityLifetime;
        public float curSpeed;
        public bool isEntityDying = false;
        public float entityDyingTime = 0f;
        // public AnimationState animationState;
 

        public WalkablePath entityPath;
        public bool isPathValid;
        public int curWorldID;
        public IMultiplayerServer server;
        public ServerSideEntityBeh(Vector3 position, float rotationX, float rotationY, float rotationZ, int typeID, string entityID, float entityHealth, bool isEntityHurt,int curWorldID ,IMultiplayerServer server)
        {
            this.position = position;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.typeID = typeID;
            this.entityID = entityID;
            this.entityHealth = entityHealth;
            this.isEntityHurt = isEntityHurt;
           this.server=server;
            isEntityDying = false;
            /*       switch (typeID)
                   {
                       case 0:
                           animationBlend = new AnimationBlend(new AnimationState[] { new AnimationState(ServerSideEntityManager.zombieAnim, EntityRenderer.zombieModel), new AnimationState(entityDieAnim, EntityRenderer.zombieModel) }, EntityRenderer.zombieModel);
                           break;
                       default:
                           break;
                   }*/
        }

        public EntityData ToEntityData()
        {
            EntityData tmpData = new EntityData(typeID, position.X, position.Y, position.Z, rotationX, rotationY, rotationZ, entityID, entityHealth, curWorldID,isEntityHurt, isEntityDying);
            return tmpData;
        }
        public void SaveSingleEntity()
        {

            EntityData tmpData = new EntityData(typeID, position.X, position.Y, position.Z, rotationX, rotationY, rotationZ, entityID, entityHealth, curWorldID, isEntityHurt,isEntityDying);

            foreach (EntityData ed in ServerSideEntityManager.entityDataReadFromDisk)
            {
                if (ed.entityID == entityID)
                {

                    ServerSideEntityManager.entityDataReadFromDisk.Remove(ed);
                    break;
                }
            }

            ServerSideEntityManager.entityDataReadFromDisk.Add(tmpData);
        }

        public void RemoveCurrentEntity()
        {
            foreach (EntityData ed in ServerSideEntityManager.entityDataReadFromDisk)
            {
                if (ed.entityID == entityID)
                {

                    ServerSideEntityManager.entityDataReadFromDisk.Remove(ed);
                    break;
                }
            }
        }


        public void InitBounds()
        {
            bounds = new BoundingBox(position - entitySize / 2f, position + entitySize / 2f);
        }
        public bool CheckIsGround()
        {
            Vector3 pos = new Vector3((bounds.Min.X + bounds.Max.X) / 2f, bounds.Min.Y - 0.1f, (bounds.Min.Z + bounds.Max.Z) / 2f);

            int blockID = ServerSideChunkHelper.GetBlock(pos,curWorldID);

            if (blockID > 0 && blockID < 100)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public ServerSideChunk curChunk;
        public bool lastChunkIsReadyToRender;
        public float Vec3Magnitude(Vector3 pos)
        {
            return (float)Math.Sqrt(pos.X * pos.X + pos.Y * pos.Y + pos.Z * pos.Z);
        }

        public virtual void OnFixedUpdate(float deltaTime)
        {

        }
        public virtual void OnUpdate(float deltaTime)
        {

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
            eulerAngles.Y = (float)Math.Atan2(fromDir.X, fromDir.Z) * 360f / (MathF.PI * 2f);
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
            foreach (var entity in ServerSideEntityManager.worldEntities)
            {
                if (entity != this)
                {
                    if (MathF.Abs(entity.position.X - position.X) < 10f && MathF.Abs(entity.position.Y - position.Y) < 10f && MathF.Abs(entity.position.Z - position.Z) < 10f)
                    {
                        entitiyBoundsAround.Add(new BoundingBox(entity.bounds.Min, entity.bounds.Max));
                    }
                }
            }
        }
        public List<BoundingBox> GetBlocksAround(BoundingBox aabb)
        {

            int minX = ServerSideChunkHelper.FloorFloat(aabb.Min.X - 0.1f);
            int minY = ServerSideChunkHelper.FloorFloat(aabb.Min.Y - 0.1f);
            int minZ = ServerSideChunkHelper.FloorFloat(aabb.Min.Z - 0.1f);
            int maxX = ServerSideChunkHelper.CeilFloat(aabb.Max.X + 0.1f);
            int maxY = ServerSideChunkHelper.CeilFloat(aabb.Max.Y + 0.1f);
            int maxZ = ServerSideChunkHelper.FloorFloat(aabb.Max.Z + 0.1f);

            blocksAround = new List<BoundingBox>();

            for (int z = minZ - 1; z <= maxZ + 1; z++)
            {
                for (int x = minX - 1; x <= maxX + 1; x++)
                {
                    for (int y = minY - 1; y <= maxY + 1; y++)
                    {
                        BlockData blockID = ServerSideChunkHelper.GetBlockData(new Vector3(x, y, z),curWorldID);

                        blocksAround.Add(BlockBoundingBoxUtility.GetBoundingBox(x, y, z, blockID));

                    }
                }
            }


            return blocksAround;


        }
    }
}
