using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Data;
using monogameMinecraftShared.Animations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using monogameMinecraftNetworking.Client.Rendering;
using monogameMinecraftShared.Asset;

namespace monogameMinecraftNetworking.Client.Updateables
{
    public class ClientSidePlayersCacheObject
    {
        public UserData data;
        public AnimationBlend animState;

        public ClientSidePlayersCacheObject(UserData data, AnimationBlend animState)
        {
            this.animState=animState;
            this.data=data;
        }
    }
    public class ClientSidePlayersManager
    {
        public List<ClientSidePlayersCacheObject> allUsersCache;
        public List<UserData> latestAllUserDatas;
        public List<UserData> previousAllUserDatas;
        public IMultiplayerClient client;
        public object allUsersCacheLock=new object();

        public float previousTimeSinceLastUpdate = 0.05f;
        public float timeSinceLastUpdate = 0f;
        public ClientSidePlayersManager(IMultiplayerClient client)
        {
            this.allUsersCache = new List<ClientSidePlayersCacheObject>();
            this.latestAllUserDatas =  new List<UserData>();
            this.previousAllUserDatas = new List<UserData>();
            this.client = client;
            client.allUsersUpdatedAction += Update;
            client.prevAllUsersUpdatedAction += PrevUpdate;
        }

        public static void LoadPlayerResources(ContentManager cm)
        {
            EntityResourcesManager.instance.TryAddCustomEntityModels(cm,
                new CustomModelLoadingItem("player", "playermodel", "steve")
                );
            EntityResourcesManager.instance.TryLoadCustomEntityAnims(new Tuple<string, Animation>("playerAnim", new Animation(new List<AnimationStep> {

                    new AnimationStep(new Dictionary<string, AnimationTransformation> {

                        { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leftLeg",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leftArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) }
                    }, 0.5f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {
                        { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 75f, 0f),  new Vector3(1f, 1f, 1f)) },
                        { "leftLeg", new AnimationTransformation(new Vector3(0f,0.0f, 0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leftArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) }
                    }, 0.5f)

                }, true))
            , new Tuple<string, Animation>("playerAttackAnim", new Animation(new List<AnimationStep>
                {






                    new AnimationStep(new Dictionary<string, AnimationTransformation> {


                        { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.05f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {


                        { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(-20f,30f , 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.05f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {


                        { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(-40f,110f , 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.05f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {


                        { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3( 50f,80f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.05f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {


                        { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3( 60f,30f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.05f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {


                        { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.05f),

                }, true))
            );
        }
        public void Update()
        {

            lock (allUsersCacheLock)
            {
                //     Debug.WriteLine("update user");
                latestAllUserDatas = client.allUserDatas;
                foreach (var item1 in latestAllUserDatas)
                {
                    if (allUsersCache.FindIndex((item) => { return item.data.userName == item1.userName; }) == -1)
                    {
                        allUsersCache.Add(new ClientSidePlayersCacheObject(item1,
                            new SingleTexturedAnimatedModel(
                                new AnimationState[]
                                {
                                    new AnimationState( EntityResourcesManager.instance.loadedEntityAnims["playerAnim"],
                                        EntityResourcesManager.instance.loadedEntityModels["player"].model),
                                    new AnimationState( EntityResourcesManager.instance.loadedEntityAnims["playerAttackAnim"],
                                        EntityResourcesManager.instance.loadedEntityModels["player"].model)
                                }, EntityResourcesManager.instance.loadedEntityModels["player"].model, EntityResourcesManager.instance.loadedEntityModels["player"].texture)));
                    }
                }

                for (int i = 0; i < allUsersCache.Count; i++)
                {
                    ClientSidePlayersCacheObject item2 = allUsersCache[i];
                    if (latestAllUserDatas.FindIndex((item) => { return item.userName == item2.data.userName; }) == -1)
                    {
                        allUsersCache.RemoveAt(i);
                        i--;
                    }
                }

                previousTimeSinceLastUpdate = timeSinceLastUpdate;
                if (previousTimeSinceLastUpdate <= 0f)
                {
                    previousTimeSinceLastUpdate = 0.05f;
                }
                timeSinceLastUpdate = 0f;
            }

        }

        public void PrevUpdate()
        {
            if (client.allUserDatas != null)
            {
                previousAllUserDatas = client.allUserDatas;
            }
        }
        public void FrameUpdate(float deltaTime)
        {
            timeSinceLastUpdate += deltaTime;
            if (previousAllUserDatas == null)
            {
                Debug.WriteLine("prev user datas null");
                return;
            }
            lock (allUsersCacheLock)
            {
                foreach (var item1 in latestAllUserDatas)
                {
                    int idx = allUsersCache.FindIndex((item) => { return item.data.userName == item1.userName; });
                    int idxInPreviousDatas = previousAllUserDatas.FindIndex((item) => { return item.userName == item1.userName; });
                    if (idx != -1&&idxInPreviousDatas!=-1)
                    {

                        Vector3 curPos = new Vector3(item1.posX, item1.posY,
                            item1.posZ);
                        Vector3 prevPos = new Vector3(previousAllUserDatas[idxInPreviousDatas].posX, previousAllUserDatas[idxInPreviousDatas].posY,
                            previousAllUserDatas[idxInPreviousDatas].posZ);
                        Vector3 curRot = new Vector3(item1.rotX, item1.rotY,
                            item1.rotZ);
                        Vector3 prevRot = new Vector3(previousAllUserDatas[idxInPreviousDatas].rotX, previousAllUserDatas[idxInPreviousDatas].rotY,
                            previousAllUserDatas[idxInPreviousDatas].rotZ);

                        Vector3 lerpPos = Vector3.Lerp(prevPos, curPos,timeSinceLastUpdate/previousTimeSinceLastUpdate);
                        Vector3 lerpRot = Vector3.Lerp(prevRot, curRot, timeSinceLastUpdate / previousTimeSinceLastUpdate);
                        float moveLength =
                            (new Vector3(allUsersCache[idx].data.posX, 0, allUsersCache[idx].data.posZ) - new Vector3(lerpPos.X, 0, lerpPos.Z)).Length();
                        allUsersCache[idx].data.posX = lerpPos.X;
                        allUsersCache[idx].data.posY = lerpPos.Y;
                        allUsersCache[idx].data.posZ = lerpPos.Z;
                        allUsersCache[idx].data.rotX = lerpRot.X;
                        allUsersCache[idx].data.rotY = lerpRot.Y;
                        allUsersCache[idx].data.rotZ = lerpRot.Z;
                        allUsersCache[idx].data.curWorldID = item1.curWorldID;
                        allUsersCache[idx].data.isAttacking = item1.isAttacking;
                        allUsersCache[idx].data.userName = item1.userName;
                        // Debug.WriteLine("speed:"+ (moveLength / deltaTime));

                        if ((moveLength / deltaTime) / 5f < 0.1f)
                        {
                            float lerpValue =
                                MathHelper.Lerp(
                                    allUsersCache[idx].animState.animationStates[0].elapsedTimeInStep, 0.25f,
                                    deltaTime * 10f);
                            float animationUpdateDelta = lerpValue - allUsersCache[idx].animState
                                .animationStates[0].elapsedTimeInStep;
                            allUsersCache[idx].animState.Update(animationUpdateDelta, 1,0);
                        }
                        else
                        {
                        //    Debug.WriteLine("player speed:" + (moveLength / deltaTime) / 5f);
                            allUsersCache[idx].animState.Update(deltaTime, (moveLength / deltaTime) / 5f,0f);
                        }

                        if (allUsersCache[idx].data.isAttacking == true)
                        {
                            allUsersCache[idx].animState.Update(deltaTime, 0f,1f);
                        }
                        else
                        {
                            float lerpValue =
                                MathHelper.Lerp(
                                    allUsersCache[idx].animState.animationStates[1].totalElapsedTime, 0.001f,
                                    deltaTime * 10f);
                            float animationUpdateDelta1 = lerpValue - allUsersCache[idx].animState
                                .animationStates[1].totalElapsedTime;
                            allUsersCache[idx].animState.Update(animationUpdateDelta1, 0f,1f);
                        }
                      
                    }
                    else if(idx != -1 && idxInPreviousDatas == -1)

                    {
                        Vector3 curPos = new Vector3(item1.posX, item1.posY,
                            item1.posZ);
                        Vector3 curRot = new Vector3(item1.rotX, item1.rotY,
                            item1.rotZ);
                        allUsersCache[idx].data.posX = curPos.X;
                        allUsersCache[idx].data.posY = curPos.Y;
                        allUsersCache[idx].data.posZ = curPos.Z;
                        allUsersCache[idx].data.rotX = curRot.X;
                        allUsersCache[idx].data.rotY = curRot.Y;
                        allUsersCache[idx].data.rotZ = curRot.Z;
                        allUsersCache[idx].data.curWorldID = item1.curWorldID;
                        allUsersCache[idx].data.isAttacking = item1.isAttacking;
                        allUsersCache[idx].data.userName = item1.userName;
                        allUsersCache[idx].animState.Update(deltaTime,0.1f);
                    }
                }
            }

        }

    }
}
