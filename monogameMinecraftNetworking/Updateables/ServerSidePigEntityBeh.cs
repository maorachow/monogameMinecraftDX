using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.World;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;

namespace monogameMinecraftNetworking.Updateables
{
    public class ServerSidePigEntityBeh : ServerSideEntityBeh
    {
        public ServerSidePigEntityBeh(Vector3 position, float rotationX, float rotationY, float rotationZ, string entityID, float entityHealth, bool isEntityHurt, int worldID, IMultiplayerServer server) : base(position, rotationX, rotationY, rotationZ, 1, entityID, entityHealth, isEntityHurt, worldID, server)
        {
            this.position = position;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;
            typeID = 1;
            this.entityID = entityID;
            this.entityHealth = entityHealth;
            this.isEntityHurt = isEntityHurt;
            this.server = server;
            isEntityDying = false;
            this.curWorldID = worldID;
            entitySize = new Vector3(0.8f, 0.8f, 0.8f);
            InitBounds();
            ServerSideEntityManager.worldEntities.Add(this);

        }

        public Vector3 lastPos;

        public Vector3 headRot;
        public Vector3 bodyRot;

        public bool hasReachedCurStep = false;
        public bool isPathfindingNeeded = false;
        public bool hasReachedFinalStep = false;
        public float timeSpentToNextStep = 0f;

        public float timeInUnloadedChunks = 0f;
        public Vector3 secondaryTargetPos;
        public override EntityData ToEntityData()
        {
            EntityData tmpData = new EntityData(typeID, position.X, position.Y, position.Z, bodyRot.X, bodyRot.Y, bodyRot.Z, entityID, entityHealth, curWorldID, isEntityHurt, isEntityDying, Float3Data.ToBytes(new Float3Data(headRot.X, headRot.Y, headRot.Z)));
            return tmpData;
        }
        public override void SaveSingleEntity()
        {

            EntityData tmpData = new EntityData(typeID, position.X, position.Y, position.Z, bodyRot.X, bodyRot.Y, bodyRot.Z, entityID, entityHealth, curWorldID, isEntityHurt, isEntityDying, Float3Data.ToBytes(new Float3Data(headRot.X, headRot.Y, headRot.Z)));

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
        public Vector3 FindClosestPlayerPos(IMultiplayerServer server)
        {

            Vector3 returnVal = new Vector3(float.MaxValue);
            foreach (var client in server.remoteClients)
            {
                if (client.isUserDataLoaded)
                {
                    if (client.curUserData.curWorldID == curWorldID)
                    {
                        Vector3 newPos = new Vector3(client.curUserData.posX, client.curUserData.posY,
                            client.curUserData.posZ);
                        if ((newPos - position).Length() < (returnVal - position).Length())
                        {
                            returnVal = newPos;
                        }
                    }
                }
            }

            if (returnVal.X > float.MaxValue * 0.9f || returnVal.Y > float.MaxValue * 0.9f ||
                returnVal.Z > float.MaxValue * 0.9f)
            {
                return position;
            }
            return returnVal;
        }
        Random rand= new Random();
        public Vector3 FindRandomLandingPos()
        {
            Vector2 landingPos = new Vector2((rand.NextSingle() * 2 - 1) * 20f+position.X, (rand.NextSingle() * 2 - 1) * 20f + position.Z);
            Vector3 returnVal = new Vector3(landingPos.X,ServerSideChunkHelper.GetChunkLandingPointColliding(landingPos.X, landingPos.Y,curWorldID)+0.5f,landingPos.Y);
            Debug.WriteLine(ServerSideChunkHelper.GetBlock(returnVal,curWorldID));

            if (returnVal.X > float.MaxValue * 0.9f || returnVal.Y > float.MaxValue * 0.9f ||
                returnVal.Z > float.MaxValue * 0.9f)
            {
                return position;
            }
            return returnVal;
        }
        public override void OnFixedUpdate(float deltaTime)
        {
            if (rand.NextSingle() >= 1 - deltaTime * 0.1f)
            {
                NetworkingUtility.EnqueueTodoList(server.serverTodoLists,(null,new MessageProtocol((byte)MessageCommandType.EntitySoundBroadcast,MessagePackSerializer.Serialize(new EntitySoundBroadcastData(position.X,position.Y,position.Z,"1say")))));
            }
                if (isPathfindingNeeded == true)
            {
                //  Debug.WriteLine("try find path");
                //      bool isNewPathValid=false;
                //      ServerSideEntityManager.pathfindingManager.GetThreeDimensionalMapPath(ServerSideChunkHelper.Vec3ToBlockPos(position+new Vector3(0f,0.1f,0f)), ServerSideChunkHelper.Vec3ToBlockPos(game.gamePlayer.position+new Vector3(0,-0.5f,0f)),
                //          out isNewPathValid,ref entityPath);
              //  Debug.WriteLine(ServerSideChunkHelper.Vec3ToBlockPos(FindRandomLandingPos()));
                ServerSideEntityManager.pathfindingManager.GetThreeDimensionalMapPathAsync(ServerSideChunkHelper.Vec3ToBlockPos(position + new Vector3(0f, 0.1f, 0f)), ServerSideChunkHelper.Vec3ToBlockPos(FindRandomLandingPos()), this);
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


                if (entityDyingTime >= 1f && isEntityDying)
                {
                    RemoveCurrentEntity();
                    ServerSideEntityManager.worldEntities.Remove(this);

                }
                return;
            }

            entityLifetime += deltaTime;
            if (!isPathValid)
            {
                isPathfindingNeeded = true;
                targetPos = position;
                
                secondaryTargetPos = FindClosestPlayerPos(server)+new Vector3(0f,1f,0f);
                if ((secondaryTargetPos - position).Length() > 10f)
                {
                    secondaryTargetPos = targetPos + new Vector3(0f, 1f, 0f);
                }
            }
            else
            {
                targetPos = entityPath.steps[entityPath.curStep];
                secondaryTargetPos = FindClosestPlayerPos(server) + new Vector3(0f, 1f, 0f);
                if ((secondaryTargetPos - position).Length() > 10f)
                {
                    secondaryTargetPos = targetPos + new Vector3(0f, 1f, 0f);
                }
            }

            entityMotionVec = Vector3.Lerp(entityMotionVec, Vector3.Zero, 3f * deltaTime);

            curSpeed = MathHelper.Lerp(curSpeed, (new Vector2(position.X, position.Z) - new Vector2(lastPos.X, lastPos.Z)).Length() / deltaTime, 5f * deltaTime);
            //        Debug.WriteLine(curSpeed);
            lastPos = position;
            Vector3Int intPos = Vector3Int.FloorToIntVec3(position);

            curChunk = ServerSideChunkHelper.GetChunk(ServerSideChunkHelper.Vec3ToChunkPos(position), curWorldID);
            if (curChunk == null)
            {
                timeInUnloadedChunks += deltaTime;
                if (timeInUnloadedChunks > 30f)
                {
                    RemoveCurrentEntity();
                    ServerSideEntityManager.worldEntities.Remove(this);
                }
            }
            else
            {
                timeInUnloadedChunks = 0f;
            }

            if (curChunk != null)
            {


                isNeededUpdateBlock = true;

            }

            /*      if (curChunk != null)
                  {
                      lastChunkIsReadyToRender = curChunk.isReadyToRender;
                  }

                  */



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


            if (Vector3.Distance(position, targetPos) < 1.1f || BlockCollidingBoundingBoxHelper.BoundingBoxIntersectsPoint(bounds, targetPos))
            {

                hasReachedCurStep = true;


            }
            else
            {



                hasReachedCurStep = false;
                timeSpentToNextStep += deltaTime;

            }
            Vector3 movePos = new Vector3(targetPos.X - position.X, 0, targetPos.Z - position.Z);
            float movePosY = movePos.Y;
            if (movePos.X == 0 && movePos.Y == 0 && movePos.Z == 0)
            {
                movePos = new Vector3(0.00f, 0.000f, 0.001f);
            }

            Vector3 lookDir = new Vector3(secondaryTargetPos.X - position.X, secondaryTargetPos.Y - position.Y - 1f,
                secondaryTargetPos.Z - position.Z);
            lookDir.Normalize();
            Vector3 movePosN = Vector3.Normalize(movePos) * 5f * deltaTime;

            entityVec = movePosN;
            //              Debug.WriteLine(movePos);
            if (isGround != false || !(entityGravity < 0f))
            {
                Vector3 entityRot = LookRotation(lookDir);
                Vector3 entityPrimaryRot=LookRotation(movePos);


                //     Debug.WriteLine(headRot.Y);

                bodyRot.Y = entityPrimaryRot.Y;
                Vector3 forwardDir = Vector3.Normalize(movePosN) ;
                
                if (Vector3.Dot(forwardDir, lookDir) > -0.4f)
                {
                    headRot.X = entityRot.X;
                    headRot.Y = entityRot.Y - bodyRot.Y;
                    headRot.Z = entityRot.Z;
                }
                else
                {
                    headRot.X = 0f;
                    headRot.Y = 0f;
                    headRot.Z =0f;
                }
              
                //       headRot.Y = MathHelper.Clamp(headRot.Y, -90f, 90f);



            }
            else
            {
            }


        //    Quaternion headQuat = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(headRot.Y), 0, 0);
          //  bodyQuat = Quaternion.Lerp(bodyQuat, headQuat, 10f * deltaTime);

      //      Debug.WriteLine("is pathfinding needed:"+isPathfindingNeeded);
            if (hasReachedCurStep && isPathValid)
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
                    hasReachedFinalStep = true;
                }
            }

            if (timeSpentToNextStep >= 3f)
            {
                timeSpentToNextStep = 0f;
                isPathValid = false;
                isPathfindingNeeded = true;
            }
            if ((!isPathValid || hasReachedFinalStep == true) /*&& Vector3.Distance(position, FindClosestPlayerPos(server)) > 2f*/)
            {
                isPathfindingNeeded = true;
            }
            if (curChunk == null)
            {
                isPathfindingNeeded = false;
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
                entityHurtCD -= 1f * deltaTime;
                isEntityHurt = true;
            }
            else
            {
                isEntityHurt = false;
            }


            Vector3 movePos1 = Vector3.Normalize(new Vector3(targetPos.X - position.X, 0, targetPos.Z - position.Z));

            if (Vector3.Distance(position, targetPos) > 0.6f)
            {

                if (isGround && curSpeed <= 0.1f/* && Vec3Magnitude(movePos1) > 2f*/)
                {

                    entityGravity = 5f;
                }








            }
            if (entityMotionVec.Length() < 2f)
            {
                EntityMove(entityVec.X, 0, entityVec.Z);
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

        }
    }
}
