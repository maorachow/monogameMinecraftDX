using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftDX.Animations;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.Pathfinding;
using monogameMinecraftDX.Physics;
using monogameMinecraftDX.Rendering;
using monogameMinecraftDX.Updateables;
using monogameMinecraftDX.World;

namespace monogameMinecraftDX
{
    public class ZombieEntityBeh:EntityBeh
    {
       
        public ZombieEntityBeh(Vector3 position, float rotationX, float rotationY, float rotationZ, string entityID, float entityHealth, bool isEntityHurt, MinecraftGame game): base(position, rotationX, rotationY, rotationZ, 0, entityID, entityHealth, isEntityHurt, game)
        {
            this.position = position;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            this.typeID = 0;
            this.entityID = entityID;
            this.entityHealth = entityHealth;
            this.isEntityHurt = isEntityHurt;
            this.game = game;
            isEntityDying = false;
            animationBlend = new AnimationBlend(new AnimationState[] { new AnimationState(EntityManager.zombieAnim, EntityRenderer.zombieModel), new AnimationState(EntityManager.entityDieAnim, EntityRenderer.zombieModel) }, EntityRenderer.zombieModel);
            entitySize = new Vector3(0.6f, 1.8f, 0.6f);
            InitBounds();
            EntityManager.worldEntities.Add(this);

        }

        public Vector3 lastPos;


       
       
        public bool hasReachedCurStep = false;
        public bool isPathfindingNeeded = false;
        public bool hasReachedFinalStep = false;
        public float timeSpentToNextStep = 0f;

        public float timeInUnloadedChunks = 0f;
        public override void OnFixedUpdate(float deltaTime)
        {
            if (isPathfindingNeeded == true)
            {
              //  Debug.WriteLine("try find path");
          //      bool isNewPathValid=false;
         //      EntityManager.pathfindingManager.GetThreeDimensionalMapPath(ChunkHelper.Vec3ToBlockPos(position+new Vector3(0f,0.1f,0f)), ChunkHelper.Vec3ToBlockPos(game.gamePlayer.position+new Vector3(0,-0.5f,0f)),
           //          out isNewPathValid,ref entityPath);
                EntityManager.pathfindingManager.GetThreeDimensionalMapPathAsync(ChunkHelper.Vec3ToBlockPos(position + new Vector3(0f, 0.1f, 0f)), ChunkHelper.Vec3ToBlockPos(game.gamePlayer.position + new Vector3(0, -0.5f, 0f)),this);
       /*         if (entityPath == null)
                {
                    isNewPathValid = false;
                }
                isPathValid = isNewPathValid;*/
                isPathfindingNeeded = false;
            }
        }
        public override void OnUpdate(float deltaTime)
        {
            if (isEntityDying == true)
            {
                entityDyingTime += deltaTime;
                isEntityHurt = true;
                animationBlend.Update(deltaTime, 0f, 1f);

                if (entityDyingTime >= 1f && isEntityDying)
                {
                    RemoveCurrentEntity();
                    EntityManager.worldEntities.Remove(this);

                }
                return;
            }
            animationBlend.Update(deltaTime, MathHelper.Clamp(curSpeed / 3f, 0f, 1f), 0f);
            entityLifetime += deltaTime;
            if (!isPathValid)
            {
                targetPos = game.gamePlayer.position;
            }
            else
            {
                targetPos = entityPath.steps[entityPath.curStep];
            }
       
            entityMotionVec = Vector3.Lerp(entityMotionVec, Vector3.Zero, 3f * deltaTime);

            curSpeed = MathHelper.Lerp(curSpeed, (new Vector2(position.X, position.Z) - new Vector2(lastPos.X, lastPos.Z)).Length() / deltaTime, 5f * deltaTime);
            //        Debug.WriteLine(curSpeed);
            lastPos = position;
            Vector3Int intPos = Vector3Int.FloorToIntVec3(position);

            curChunk = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(position));
            if (curChunk == null)
            {
                timeInUnloadedChunks += deltaTime;
                if (timeInUnloadedChunks > 30f)
                {
                    RemoveCurrentEntity();
                    EntityManager.worldEntities.Remove(this);
                }
            }
            else
            {
                timeInUnloadedChunks=0f;
            }

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


            if (Vector3.Distance(position, targetPos) < 0.6f||BlockCollidingBoundingBoxHelper.BoundingBoxIntersectsPoint(bounds,targetPos))
            {

                hasReachedCurStep = true;
            }
            else
            {

                Vector3 movePos = new Vector3(targetPos.X - position.X, 0, targetPos.Z - position.Z);
                float movePosY = movePos.Y;
                if (movePos.X == 0 && movePos.Y == 0 && movePos.Z == 0)
                {
                    movePos = new Vector3(0.00f, 0.001f, 0.00f);
                }

                Vector3 lookPos = new Vector3(targetPos.X - position.X, targetPos.Y - position.Y - 1f,
                    targetPos.Z - position.Z);
                lookPos.Normalize();
                Vector3 movePosN = Vector3.Normalize(movePos) * 5f * deltaTime;

                entityVec = movePosN;
                //              Debug.WriteLine(movePos);
                if (isGround != false || !(entityGravity < 0f))
                {
                    Vector3 entityRot = LookRotation(lookPos);
                    rotationX = entityRot.X;
                    rotationY = entityRot.Y;
                    rotationZ = entityRot.Z;
                }
                else
                {
                }


                Quaternion headQuat = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), 0, 0);
                bodyQuat = Quaternion.Lerp(bodyQuat, headQuat, 10f * deltaTime);

                hasReachedCurStep = false;
                timeSpentToNextStep += deltaTime;

            }

            if (hasReachedCurStep&&isPathValid)
            {
                if (entityPath.curStep < entityPath.steps.Count - 1)
                {

                    entityPath.curStep++;
                //    Debug.WriteLine("current step index:"+entityPath.curStep);
                    timeSpentToNextStep = 0f;
                    hasReachedFinalStep = false;
                }
                else
                {
                    hasReachedFinalStep=true;
                   
             
                }
            }

            if (timeSpentToNextStep >= 3f)
            {
                timeSpentToNextStep = 0f;
                isPathValid = false;
                isPathfindingNeeded = true;
            }
            if ((!isPathValid||hasReachedFinalStep==true)&& Vector3.Distance(position, game.gamePlayer.position)>2f)
            {
                isPathfindingNeeded = true;
            }
            if(curChunk==null)
            {
                isPathfindingNeeded=false;
            }
           
        /*    if (isPathValid == false&&(game.gamePlayer.position-position).Length()<=2*Chunk.chunkWidth)
            {
                isPathfindingNeeded = true;
            }*/

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


            Vector3 movePos1 =Vector3.Normalize(new Vector3(targetPos.X - position.X, 0, targetPos.Z - position.Z)) ;

            if (Vector3.Distance(position, targetPos) > 0.6f)
            {

                if (isGround && curSpeed <= 0.1f/* && Vec3Magnitude(movePos1) > 2f*/)
                {

                    entityGravity = 5f;
                }


                if (entityMotionVec.Length() < 2f)
                {
                    EntityMove(entityVec.X, 0, entityVec.Z);
                }





            }
            entityVec.Y = entityGravity *deltaTime;




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
         
        }
    }
}
