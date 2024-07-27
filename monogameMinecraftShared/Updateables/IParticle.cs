using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Physics;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.World;
 
 

namespace monogameMinecraftShared.Updateables
{
    public interface IParticle
    {
        public void Update(float deltaTime);
        public Vector3 position { get; set; }
        public bool isAlive { get; set; }


    }

    public struct TexturedGravityParticle : IParticle
    {
        public void Update(float deltaTime)
        {
            lifeTime -= deltaTime;
            if (lifeTime < 0f)
            {
                isAlive = false;
            }

            if (isAlive == false)
            {
                return;
            }

            Move(motionVector.X * deltaTime, (motionVector.Y + gravity) * deltaTime, motionVector.Z * deltaTime);
            if (!isGround)
            {
                gravity += -9.8f * deltaTime;
            }
            else
            {
                gravity = 0f;
                motionVector.Y = 0f;
                motionVector *= 1f - deltaTime * friction;
            }
            //     Debug.WriteLine("particle:" + position+"lifetime:"+lifeTime);
        }

        // public List<BoundingBox> blocksAround;

        public List<BoundingBox> GetBlocksAround(BoundingBox aabb)
        {

            int minX = ChunkHelper.FloorFloat(aabb.Min.X - 0.1f);
            int minY = ChunkHelper.FloorFloat(aabb.Min.Y - 0.1f);
            int minZ = ChunkHelper.FloorFloat(aabb.Min.Z - 0.1f);
            int maxX = ChunkHelper.CeilFloat(aabb.Max.X + 0.1f);
            int maxY = ChunkHelper.CeilFloat(aabb.Max.Y + 0.1f);
            int maxZ = ChunkHelper.FloorFloat(aabb.Max.Z + 0.1f);

            List<BoundingBox> blocksAround = new List<BoundingBox>();

            for (int z = minZ - 1; z <= maxZ + 1; z++)
            {
                for (int x = minX - 1; x <= maxX + 1; x++)
                {
                    for (int y = minY - 1; y <= maxY + 1; y++)
                    {


                        blocksAround.Add(ParticleManager.instance.GetOrFetchBoundingBox(new Vector3Int(x, y, z)));

                    }
                }
            }


            return blocksAround;


        }

        public BoundingBox bounds;
        public bool isGround;
        public void Move(float dx, float dy, float dz)
        {


            float movX = dx;
            float movY = dy;
            float movZ = dz;


            List<BoundingBox> allBounds = GetBlocksAround(bounds);




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
        public TexturedGravityParticle(Vector3 position, float size, Vector2 uvCorner, Vector2 uvWidth, float lifeTime, Vector3 initalMotionVector, float friction)
        {
            this.position = position;
            this.size = size;
            this.uvCorner = uvCorner;
            this.uvWidth = uvWidth;
            this.lifeTime = lifeTime;
            bounds = new BoundingBox(position - new Vector3(size / 2f), position + new Vector3(size / 2f));
            isAlive = true;
            motionVector = initalMotionVector;
            isGround = false;
            this.friction = friction;
            gravity = 0f;
        }
        public void GetInstancingElement(GamePlayer player, out VertexMatrix4x4UVScale element)
        {
            Matrix transMat = Matrix.CreateScale(size) *
                              Matrix.CreateBillboard(position, position - player.cam.front, player.cam.up, player.cam.front);
            element = new VertexMatrix4x4UVScale(new Vector4(transMat.M11, transMat.M12, transMat.M13, transMat.M14),
                    new Vector4(transMat.M21, transMat.M22, transMat.M23, transMat.M24),
            new Vector4(transMat.M31, transMat.M32, transMat.M33, transMat.M34),
            new Vector4(transMat.M41, transMat.M42, transMat.M43, transMat.M44), new Vector4(uvCorner.X, uvCorner.Y, uvWidth.X, uvWidth.Y), size);
        }
        public Vector3 position { get; set; }
        public Vector3 motionVector;
        public float friction;
        public float gravity;
        public bool isAlive { get; set; }
        public float size;
        public Vector2 uvCorner;
        public Vector2 uvWidth;
        public float lifeTime;
    }

}
