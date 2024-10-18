using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Client.UI
{
    public class MultiplayerClientUIConstructionManager:UIConstructionManagerBase
    {
        public MultiplayerClientUIConstructionManager(UIStateManager uiStateManager, ClientGameBase game):base(uiStateManager)
        {

            
            this.game = game;
        }

        public override void ConstructAll()
        {
            ConstructStartMenu();
            ConstructSettings();
            ConstructInGame();
            ConstructPauseMenu();
       //     ConstructStructureOperations();
            ConstructInventory();
            ConstructChatMessages();
        }

        public void ConstructStartMenu()
        {
            ClientGameBase gameClient=game as ClientGameBase;
            UIButton connectResultButton = new UIButton(uiStateManager, new Vector2(0.3f, 0.85f), 0.4f, 0.1f, UIResourcesManager.instance.UITextures["menubackgroundtransparent"], new Vector2(0.4f, 0.55f), UIResourcesManager.instance.sf, game._spriteBatch, game.Window, null, "Connection Result : ", null, 0.5f, false, false, null, false);
            connectResultButton.optionalTag = "connectResultButton";
            uiStateManager.menuUIs = new List<UIElement> {

                new UIImage(uiStateManager,new Vector2(0f,0f),1f,1f,UIResourcesManager.instance.UITextures["menubackground"],game._spriteBatch),


                new UIButton(uiStateManager,new Vector2(0.3f, 0.1f), 0.4f, 0.1f, UIResourcesManager.instance.UITextures["menubackgroundtransparent"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, null,"Enter IP Address, Port And Player Name",null,0.6f,false,false),
                new InputField(uiStateManager,new Vector2(0.3f, 0.25f), 0.4f, 0.1f, UIResourcesManager.instance.UITextures["inputfield"],UIResourcesManager.instance.UITextures["inputfieldhighlighted"],UIResourcesManager.instance.sf,game._spriteBatch,game.Window, (field =>gameClient.inputIPAddress= field.text) ,"",1,16,false),

                new InputField(uiStateManager,new Vector2(0.3f, 0.4f), 0.4f, 0.1f,UIResourcesManager.instance.UITextures["inputfield"],UIResourcesManager.instance.UITextures["inputfieldhighlighted"],UIResourcesManager.instance.sf,game._spriteBatch,game.Window, (field =>gameClient.inputPort=field.TryParseInt() ) ,"",1,10,false),
                new InputField(uiStateManager,new Vector2(0.3f, 0.55f), 0.4f, 0.1f,UIResourcesManager.instance.UITextures["inputfield"],UIResourcesManager.instance.UITextures["inputfieldhighlighted"],UIResourcesManager.instance.sf,game._spriteBatch,game.Window,  (field =>gameClient.inputUserName=field.text ) ,"",1,10,false),
                new UIButton(uiStateManager,new Vector2(0.3f, 0.7f), 0.2f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, gameClient.InitGameplay ,"Start Game",null,1),
                new UIButton(uiStateManager,new Vector2(0.5f, 0.7f), 0.2f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, gameClient.GoToSettings ,"Settings",null,1),
                connectResultButton
                //         testElement,
                //     new UIButton(new Vector2(0.3f, 0.6f), 0.4f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, game.GoToSettings ,"Game Settings",null,1)

            };

        }

        public void ConstructSettings()
        {
            if (game.gamePlatformType == GamePlatformType.HighDefDX)
            {
                uiStateManager.settingsUIsPage1 = new List<UIElement> {

                new UIImage(uiStateManager,new Vector2(0f,0f),1f,1f,UIResourcesManager.instance.UITextures["menubackground"],game._spriteBatch),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, null ,"Render Distance : 128",null,1,false,false),
                 new UIButton(uiStateManager,new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderShadow ,"Render Shadow : "+GameOptions.renderShadow,GameOptions.UpdateRenderShadowUIText,1),
                  new UIButton(uiStateManager,new Vector2(0.25f, 0.4f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderFarShadow ,"Render Far Shadow : "+GameOptions.renderFarShadow,GameOptions.UpdateRenderFarShadowUIText,1),
                  new UIButton(uiStateManager,new Vector2(0.25f, 0.55f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,GameOptions.ChangeRenderSSAO ,"Render SSAO: "+GameOptions.renderSSAO,GameOptions.UpdateRenderSSAOUIText ,1),
                  new UIButton(uiStateManager,new Vector2(0.25f, 0.7f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,GameOptions.ChangeRenderLightShaft ,"Render Light Shaft :"+GameOptions.renderLightShaft,GameOptions.UpdateRenderLightShaftUIText,1 ),
                  new UIButton(uiStateManager,new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu",null ,1),
                  new UIButton(uiStateManager,new Vector2(0.8f, 0.4f), 0.1f, 0.2f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,(obj)=>{uiStateManager.settingsUIsPageID+=1; } ,"Next Page",null ,0.5f),
            };


                uiStateManager.settingsUIsPage2 = new List<UIElement> {

                new UIImage(uiStateManager,new Vector2(0f,0f),1f,1f,UIResourcesManager.instance.UITextures["menubackground"],game._spriteBatch),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderSSR ,"Render SSR : "+GameOptions.renderSSR,GameOptions.UpdateRenderSSRUIText,1),
                 new UIButton(uiStateManager,new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderSSID ,"Render SSID : "+GameOptions.renderSSID,GameOptions.UpdateRenderSSIDUIText,1),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.4f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderContactShadow ,"Render Contact Shadow : "+GameOptions.renderContactShadow,GameOptions.UpdateRenderContactShadowUIText,1),
                 new UIButton(uiStateManager,new Vector2(0.25f, 0.55f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeShowGraphicsDebug ,"Show Graphics Debug : "+GameOptions.showGraphicsDebug,GameOptions.UpdateShowGraphicsDebugUIText,1),
                     new UIButton(uiStateManager,new Vector2(0.25f, 0.7f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderMotionBlur ,"Render Motion Blur : "+GameOptions.renderMotionBlur,GameOptions.UpdateRenderMotionBlurUIText,1),
             //     new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu" ),
                  new UIButton(uiStateManager,new Vector2(0.1f, 0.4f), 0.1f, 0.2f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,(obj)=>{uiStateManager.settingsUIsPageID-=1; } ,"Previous Page" ,null,0.5f),
            };
            }
            else if (game.gamePlatformType == GamePlatformType.LowDefGL)
            {
                uiStateManager.settingsUIsPage1 = new List<UIElement> {

                new UIImage(uiStateManager,new Vector2(0f,0f),1f,1f,UIResourcesManager.instance.UITextures["menubackground"],game._spriteBatch),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, null ,"Render Distance : 128",null,1,false,false),


                new UIButton(uiStateManager,new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,GameOptions.ChangeRenderSSAO ,"Render SSAO: "+GameOptions.renderSSAO,GameOptions.UpdateRenderSSAOUIText ,1),

                new UIButton(uiStateManager,new Vector2(0.25f, 0.4f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeShowGraphicsDebug ,"Show Graphics Debug : "+GameOptions.showGraphicsDebug,GameOptions.UpdateShowGraphicsDebugUIText,1),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu",null ,1),
                  new UIButton(uiStateManager,new Vector2(0.8f, 0.4f), 0.1f, 0.2f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,(obj)=>{uiStateManager.settingsUIsPageID+=1; } ,"Next Page",null ,0.5f),
            };


                uiStateManager.settingsUIsPage2 = new List<UIElement> {
                    new UIImage(uiStateManager,new Vector2(0f,0f),1f,1f,UIResourcesManager.instance.UITextures["menubackground"],game._spriteBatch),
             //     new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu" ),
                  new UIButton(uiStateManager,new Vector2(0.1f, 0.4f), 0.1f, 0.2f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,(obj)=>{uiStateManager.settingsUIsPageID-=1; } ,"Previous Page" ,null,0.5f),
            };
            }
            else if (game.gamePlatformType == GamePlatformType.VeryLowDefMobile)
            {

                uiStateManager.settingsUIsPage1 = new List<UIElement> {

                    new UIImage(uiStateManager,new Vector2(0f,0f),1f,1f,UIResourcesManager.instance.UITextures["menubackground"],game._spriteBatch),
                    new UIButton(uiStateManager,new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, null ,"Render Distance : 128",null,1,false,false),




                    new UIButton(uiStateManager,new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, GameOptions.ChangeShowGraphicsDebug ,"Show Graphics Debug : "+GameOptions.showGraphicsDebug,GameOptions.UpdateShowGraphicsDebugUIText,1),
                    new UIButton(uiStateManager,new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu",null ,1),

                };
            }
        }

        public void ConstructInGame()
        {

            UIPanel leftDownPanel = new UIPanel(uiStateManager, new Vector2(0.05f, 0.6f), 0.3f, 0.3f, true);

            UIPanel rightDownPanel = new UIPanel(uiStateManager, new Vector2(0.65f, 0.6f), 0.3f, 0.3f, true);

            UIPanel rightUpPanel = new UIPanel(uiStateManager, new Vector2(0.65f, 0.05f), 0.3f, 0.3f, true);

            UIPanel leftUpPanel = new UIPanel(uiStateManager, new Vector2(0.05f, 0.05f), 0.3f, 0.3f, true);

            UIPanel middleDownPanel = new UIPanel(uiStateManager, new Vector2(0.2f, 0.85f), 0.6f, 0.15f, false);
            TextListUI chatMessageListElement = new TextListUI(uiStateManager, new Vector2(0f, 0.1f), 0.4f, 0.2f,
                UIResourcesManager.instance.UITextures["menubackgroundtransparent"], UIResourcesManager.instance.sf, game._spriteBatch, 1f, 8);
            chatMessageListElement.optionalTag = "chatMessageList";
            if (game.gamePlatformType == GamePlatformType.VeryLowDefMobile)
            {
                uiStateManager.inGameUIs = new List<UIElement>
                {
                    new InGameUI(uiStateManager,UIResourcesManager.instance.sf,game.Window,game._spriteBatch, game.gamePlayerR,UIResourcesManager.instance.UITextures["hotbartexture"],UIResourcesManager.instance.UITextures["selectedhotbar"]),
                    chatMessageListElement,
                    leftDownPanel,
                    new UIButton(uiStateManager,new Vector2(0.333f, 0.0f), 0.333f, 0.333f, UIResourcesManager.instance.UITextures["mobiletouchup"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,   (ub) => { PlayerInputManager.mobilemotionVec.Z = 1f;} ,"",null,1,false,true,leftDownPanel,true,UIResourcesManager.instance.UITextures["mobiletouchuppressed"]),


                    new UIButton(uiStateManager,new Vector2(0.333f, 0.666f), 0.333f, 0.333f, UIResourcesManager.instance.UITextures["mobiletouchdown"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,   (ub) => {PlayerInputManager.mobilemotionVec.Z = -1f;} ,"",null,1,false,true,leftDownPanel,true,UIResourcesManager.instance.UITextures["mobiletouchdownpressed"]),

                    new UIButton(uiStateManager,new Vector2(0.666f, 0.333f), 0.333f, 0.333f, UIResourcesManager.instance.UITextures["mobiletouchright"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,   (ub) => { PlayerInputManager.mobilemotionVec.X = 1f;} ,"",null,1,false,true,leftDownPanel,true,UIResourcesManager.instance.UITextures["mobiletouchrightpressed"]),

                    new UIButton(uiStateManager,new Vector2(0f, 0.333f), 0.333f, 0.333f, UIResourcesManager.instance.UITextures["mobiletouchleft"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,   (ub) => { PlayerInputManager.mobilemotionVec.X = -1f;} ,"",null,1,false,true,leftDownPanel,true,UIResourcesManager.instance.UITextures["mobiletouchleftpressed"]),



                    rightDownPanel,

                    rightUpPanel,
                    leftUpPanel,
                    middleDownPanel,
                    new UIButton(uiStateManager,new Vector2(0.25f, 0.25f), 0.5f, 0.5f, UIResourcesManager.instance.UITextures["mobiletouchjump"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobilemotionVec.Y = 1f;} ,"",null,1,false,true,rightDownPanel,true,UIResourcesManager.instance.UITextures["mobiletouchjumppressed"]),



                    new UIButton(uiStateManager,new Vector2(0.25f, 0.0f), 0.5f, 0.5f, UIResourcesManager.instance.UITextures["mobiletouchattack"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileIsLMBPressed = true;} ,"",null,1,false,true,leftUpPanel,true,UIResourcesManager.instance.UITextures["mobiletouchattackpressed"]),


                    new UIButton(uiStateManager,new Vector2(0.25f, 0.5f), 0.5f, 0.5f, UIResourcesManager.instance.UITextures["mobiletouchsprint"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileIsSprintingPressed = true;} ,"",null,1,false,true,leftUpPanel,true,UIResourcesManager.instance.UITextures["mobiletouchsprintpressed"]),

                    new UIButton(uiStateManager,new Vector2(0.25f, 0.0f), 0.5f, 0.5f, UIResourcesManager.instance.UITextures["mobiletouchinteract"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileIsRMBPressed =true;} ,"",null,1,false,true,rightUpPanel,true,UIResourcesManager.instance.UITextures["mobiletouchinteractpressed"]),

                    new UIButton(uiStateManager,new Vector2(0.25f, 0.5f), 0.5f, 0.5f, UIResourcesManager.instance.UITextures["mobiletouchfly"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileIsFlyingPressed =true;} ,"",null,1,false,true,rightUpPanel,false),
                    new UIButton(uiStateManager,new Vector2(0.45f, 0.0f), 0.1f, 0.1f, UIResourcesManager.instance.UITextures["mobiletouchpause"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {game.PauseGame(null);} ,"",null,1,true,true,null,false),


                    new UIButton(uiStateManager,new Vector2(0.0f, 0.35f), 0.08f, 0.5f, UIResourcesManager.instance.UITextures["mobiletouchleft"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileScrollDelta-=130;} ,"",null,1,false,true,middleDownPanel,false),


                    new UIButton(uiStateManager,new Vector2(0.92f, 0.35f), 0.08f, 0.5f, UIResourcesManager.instance.UITextures["mobiletouchright"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileScrollDelta+=130;} ,"",null,1,false,true,middleDownPanel,false),

                    new UIButton(uiStateManager,new Vector2(0.35f, 0.0f), 0.3f, 0.3f,UIResourcesManager.instance.UITextures["mobiletouchinventory"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window,
                        (ub) => {game.OpenInventory(ub);} ,"",null,1,true,true,middleDownPanel,false),
                };
            }
            else
            {
                uiStateManager.inGameUIs = new List<UIElement>
                {
                    new InGameUI(uiStateManager,UIResourcesManager.instance.sf,game.Window,game._spriteBatch,  game.gamePlayerR,UIResourcesManager.instance.UITextures["hotbartexture"],UIResourcesManager.instance.UITextures["selectedhotbar"]),
                    chatMessageListElement
                };
            }
        }

        public void ConstructPauseMenu()
        {
            uiStateManager.pauseMenuUIs = new List<UIElement>
            {
                // new InGameUI(sf,game.Window,game._spriteBatch, game,UIElement.UITextures["hotbartexture"],UIElement.UITextures["selectedhotbar"])
                new UIImage(uiStateManager,new Vector2(0f,0f),1f,1f,UIResourcesManager.instance.UITextures["menubackgroundtransparent"],game._spriteBatch),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.1f), 0.5f, 0.15f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f), UIResourcesManager.instance.sf,game._spriteBatch,game.Window, (ub)=>game.ResumeGame() ,"Resume Game",null,1),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.3f), 0.5f, 0.15f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f), UIResourcesManager.instance.sf,game._spriteBatch,game.Window, (ub)=>game.QuitGameplay() ,"Quit Game",null,1),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.5f), 0.5f, 0.15f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f), UIResourcesManager.instance.sf,game._spriteBatch,game.Window, (ub)=>
                {
                    if (game.renderPipelineManager is HighDefRenderPipelineManager)
                    {
                        game.effectsManager.LoadCustomPostProcessEffects(game.GraphicsDevice,
                            (game.renderPipelineManager as HighDefRenderPipelineManager).customPostProcessors, game.Content);
                    }

                },"Reload Custom Postprocessing Shaders",null,0.6f),
                new UIButton(uiStateManager, new Vector2(0.25f, 0.7f), 0.5f, 0.15f,
                    UIResourcesManager.instance.UITextures["buttontexture"], new Vector2(0.4f, 0.55f), UIResourcesManager.instance.sf,
                    game._spriteBatch, game.Window,
                    (ub) => BlockResourcesManager.instance. LoadResources(
                        Directory.GetCurrentDirectory() + "/customresourcespack", game.Content, game.GraphicsDevice),
                    "Reload Custom Resource Packs", null, 0.6f)
            };
        }

      /*  public void ConstructStructureOperations()
        {
            var sf = UIResourcesManager.instance.sf;
            uiStateManager.structureOperationsSavingUIs = new List<UIElement>()
            {
                new UIImage(uiStateManager,new Vector2(0.05f, 0.05f), 0.9f, 0.9f,  UIResourcesManager.instance.UITextures["menubackgroundtransparent"], game._spriteBatch),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.1f), 0.5f, 0.1f,  UIResourcesManager.instance.UITextures["menubackgroundtransparent"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, (ub)=>{} ,"Structure Saving",null,1,false,false),
                new InputField(uiStateManager,new Vector2(0.3f, 0.3f), 0.4f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,
                    (ub) => { VoxelWorld.currentWorld.structureOperationsManager.tmpSavingStructureName = ub.text;},"structurename",1,16,false),
                new InputField(uiStateManager,new Vector2(0.1f, 0.45f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window, (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureOrigin.x =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(uiStateManager,new Vector2(0.1f, 0.55f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureOrigin.y =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(uiStateManager,new Vector2(0.1f, 0.65f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureOrigin.z =ub.TryParseInt();} ,"0",1,16,true),

                new InputField(uiStateManager,new Vector2(0.6f, 0.45f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureSize.x =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(uiStateManager,new Vector2(0.6f, 0.55f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureSize.y =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(uiStateManager,new Vector2(0.6f, 0.65f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureSize.z =ub.TryParseInt();} ,"0",1,16,true),


                new UIButton(uiStateManager,new Vector2(0.1f, 0.8f), 0.3f, 0.15f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isShowingStructureSavingBounds = !VoxelWorld
                            .currentWorld.structureOperationsManager.isShowingStructureSavingBounds;} ,"Show Bounds",null,1,false,true),

                new UIButton(uiStateManager,new Vector2(0.6f, 0.8f), 0.3f, 0.15f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager. SaveNewStructure(VoxelWorld.currentWorld.structureOperationsManager.tmpSavingStructureName);} ,"Save Structure",null,1,false,true),


                new UIButton(uiStateManager,new Vector2(0.8f, 0.1f), 0.1f, 0.1f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{uiStateManager.structureOperationsUIsPageID=1; } ,"Next Page" ,null,0.5f),

                new UIButton(uiStateManager,new Vector2(0.1f, 0.1f), 0.1f, 0.1f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{game.CloseStructureOperationsUI(obj); } ,"Close UI" ,null,0.5f),
            };



            uiStateManager.structureOperationsPlacingUIs = new List<UIElement>()
            {
                new UIImage(uiStateManager,new Vector2(0.05f, 0.05f), 0.9f, 0.9f,  UIResourcesManager.instance.UITextures["menubackgroundtransparent"], game._spriteBatch),
                new UIButton(uiStateManager,new Vector2(0.25f, 0.1f), 0.5f, 0.1f,  UIResourcesManager.instance.UITextures["menubackgroundtransparent"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, (ub)=>{} ,"Structure Placing",null,1,false,false),
                new InputField(uiStateManager,new Vector2(0.3f, 0.3f), 0.4f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.tmpPlacingStructureName = ub.text;
                        if (VoxelWorld.currentWorld.structureOperationsManager.allStructureDatas.TryGetValue(VoxelWorld.currentWorld.structureOperationsManager.tmpPlacingStructureName, out var data))
                        {
                            VoxelWorld.currentWorld.structureOperationsManager.curPlacingStructureData =
                                data;
                        }


                    },"structurename",1,16,false),
                new InputField(uiStateManager,new Vector2(0.1f, 0.45f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window, (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curPlacingStructureOrigin.x =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(uiStateManager,new Vector2(0.1f, 0.55f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curPlacingStructureOrigin.y =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(uiStateManager,new Vector2(0.1f, 0.65f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curPlacingStructureOrigin.z =ub.TryParseInt();} ,"0",1,16,true),



                new UIButton(uiStateManager,new Vector2(0.6f, 0.45f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isStructurePlacingInvertX = !VoxelWorld
                            .currentWorld.structureOperationsManager.isStructurePlacingInvertX;
                        ub.text = "Inverse X:" + VoxelWorld.currentWorld.structureOperationsManager
                            .isStructurePlacingInvertX.ToString();
                    } ,"Inverse X:False",null,1,false,true),
                new UIButton(uiStateManager,new Vector2(0.6f, 0.55f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isStructurePlacingInvertY = !VoxelWorld
                            .currentWorld.structureOperationsManager.isStructurePlacingInvertY;
                        ub.text = "Inverse Y:" + VoxelWorld.currentWorld.structureOperationsManager
                            .isStructurePlacingInvertY.ToString();
                    } ,"Inverse Y:False",null,1,false,true),
                new UIButton(uiStateManager,new Vector2(0.6f, 0.65f), 0.3f, 0.1f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isStructurePlacingInvertZ = !VoxelWorld
                            .currentWorld.structureOperationsManager.isStructurePlacingInvertZ;
                        ub.text = "Inverse Z:" + VoxelWorld.currentWorld.structureOperationsManager
                            .isStructurePlacingInvertZ.ToString();
                    } ,"Inverse Z:False",null,1,false,true),




                new UIButton(uiStateManager,new Vector2(0.1f, 0.8f), 0.3f, 0.15f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isShowingStructurePlacingBounds = !VoxelWorld
                            .currentWorld.structureOperationsManager.isShowingStructurePlacingBounds;} ,"Show Bounds",null,1,false,true),

                new UIButton(uiStateManager,new Vector2(0.6f, 0.8f), 0.3f, 0.15f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager. PlaceSavedStructureWithTmpParams();} ,"Place Structure",null,1,false,true),

                new UIButton(uiStateManager,new Vector2(0.1f, 0.1f), 0.1f, 0.1f,  UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{uiStateManager.structureOperationsUIsPageID=0; } ,"Previous Page" ,null,0.5f),
            };
        }*/
        public void ConstructInventory()
        {

            uiStateManager.inventoryUIs = new List<UIElement> { new UIImage(uiStateManager,new Vector2(0.1f, 0.1f), 0.8f, 0.8f, UIResourcesManager.instance.UITextures["menubackgroundtransparent"], game._spriteBatch),
             new UIButton(uiStateManager,new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIResourcesManager.instance.UITextures["menubackgroundtransparent"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, (ub)=>{} ,"Inventory",null,1,false,false),



             new UIButton(uiStateManager,new Vector2(0.35f, 0.7f), 0.3f, 0.1f, UIResourcesManager.instance.UITextures["buttontexture"],new Vector2(0.4f,0.55f),UIResourcesManager.instance.sf,game._spriteBatch,game.Window, (ub)=>{game.OpenInventory(ub);} ,"Close",null,1,false,true),
            };
            int elementCount = 0;
            foreach (var element in Chunk.blockInfosNew)
            {
                uiStateManager.inventoryUIs.Add(new UIButton(uiStateManager, new Vector2(elementCount % 10 * 0.05f + 0.25f, elementCount / 10 * 0.05f + 0.25f), 0.05f, 0.05f, UIResourcesManager.instance.UITextures.ContainsKey("blocktexture" + element.Key) && UIResourcesManager.instance.UITextures["blocktexture" + element.Key] != null ? UIResourcesManager.instance.UITextures["blocktexture" + element.Key] : UIResourcesManager.instance.UITextures["blocktexture-1"],
                    new Vector2(0f, 0f), null, game._spriteBatch, game.Window, (ub) => game.gamePlayerR.gamePlayer.inventoryData[game.gamePlayerR.gamePlayer.currentSelectedHotbar] = (short)element.Key, " ", null, 0f, true
                    ));
                elementCount++;
            }
            foreach (var element in uiStateManager.inventoryUIs)
            {
                element.OnResize(uiStateManager);
            }


        }

        public void ConstructChatMessages()
        {
            ClientGameBase gameClient = game as ClientGameBase;


            InputField chatMessageField = new InputField(uiStateManager, new Vector2(0.0f, 0.95f), 0.8f, 0.05f,
                UIResourcesManager.instance.UITextures["inputfield"], UIResourcesManager.instance.UITextures["inputfieldhighlighted"], UIResourcesManager.instance.sf,
                game._spriteBatch, game.Window, null, "", 0.7f, 64, false, true, 0.01f, true);
            chatMessageField.onEnterPressedAction = (inputField) =>
            {
                if (chatMessageField.text.Length > 0)
                {
                    gameClient.SendChatMessage(inputField, chatMessageField.text);
                    chatMessageField.text = "";
                }
            };
            UIButton sendChatMessageButton = new UIButton(uiStateManager, new Vector2(0.8f, 0.95f), 0.2f, 0.05f,
                UIResourcesManager.instance.UITextures["buttontexture"], new Vector2(0.4f, 0.55f), UIResourcesManager.instance.sf, game._spriteBatch,
                game.Window, (ub) =>
                {
                    if (chatMessageField.text.Length > 0)
                    {
                        gameClient.SendChatMessage(ub, chatMessageField.text);
                        chatMessageField.text = "";
                    }

                }, "Send Message", null, 0.7f, false, true);
            UIButton closeChatUIButton = new UIButton(uiStateManager, new Vector2(0.8f, 0.85f), 0.2f, 0.05f,
                UIResourcesManager.instance.UITextures["buttontexture"], new Vector2(0.4f, 0.55f), UIResourcesManager.instance.sf, game._spriteBatch,
                game.Window, (ub) =>
                {
                    gameClient.CloseChatUI();

                }, "Close", null, 0.7f, false, true);
            uiStateManager.chatMessagesUIs = new List<UIElement>()
            {
                chatMessageField,
                sendChatMessageButton,
                closeChatUIButton
            };
        }
    }
}
