﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.IO;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftShared.UI
{
    [Obsolete]
    public static class UIUtility
    {
        public static SpriteFont sf;

       /* public static void InitGameUI(MinecraftGameBase game)
        {


            sf = game.Content.Load<SpriteFont>("defaultfont");
            Texture2D menubkgrd = game.Content.Load<Texture2D>("menubackground");
            Texture2D menubkgrdTransparent = game.Content.Load<Texture2D>("menubackgroundtransparent");
            Texture2D buttonTex = game.Content.Load<Texture2D>("buttontexture");
            Texture2D inputFieldTex = game.Content.Load<Texture2D>("textfield");
            Texture2D inputFieldTexHighlighted = game.Content.Load<Texture2D>("textfieldhighlighted");
            Texture2D hotbarTex = game.Content.Load<Texture2D>("hotbar");
            Texture2D selectedHotbarTex = game.Content.Load<Texture2D>("selectedhotbar");

            Texture2D blockTex1 = game.Content.Load<Texture2D>("blocksprites/stone");
            Texture2D blockTex2 = game.Content.Load<Texture2D>("blocksprites/grass_side_carried");
            Texture2D blockTex3 = game.Content.Load<Texture2D>("blocksprites/dirt");
            Texture2D blockTex4 = game.Content.Load<Texture2D>("blocksprites/grass_side_carried");
            Texture2D blockTex5 = game.Content.Load<Texture2D>("blocksprites/bedrock");
            Texture2D blockTex6 = game.Content.Load<Texture2D>("blocksprites/log_oak");
            Texture2D blockTex7 = game.Content.Load<Texture2D>("blocksprites/log_oak");
            Texture2D blockTex8 = game.Content.Load<Texture2D>("blocksprites/log_oak");
            Texture2D blockTex9 = game.Content.Load<Texture2D>("blocksprites/leaves_oak_carried");
            Texture2D blockTex12 = game.Content.Load<Texture2D>("blocksprites/end_stone");
            Texture2D blockTex13 = game.Content.Load<Texture2D>("blocksprites/endframe_top");
            Texture2D blockTex14 = game.Content.Load<Texture2D>("blocksprites/sea_lantern");
            Texture2D blockTex102 = game.Content.Load<Texture2D>("blocksprites/torch_on");


         
            UIElement.uiSounds.Clear();

            UIElement.uiSounds.TryAdd("uiclick", game.Content.Load<SoundEffect>("sounds/uiclick"));
            UIElement.UITextures = new Dictionary<string, Texture2D> {
                    { "menubackground" ,menubkgrd},
                    { "inputfield" ,inputFieldTex},
                    { "inputfieldhighlighted" ,inputFieldTexHighlighted},
                    { "menubackgroundtransparent" ,menubkgrdTransparent},
                    { "buttontexture" ,buttonTex},
                    {"hotbartexture",hotbarTex },
                    {"selectedhotbar",selectedHotbarTex },
                    {"blocktexture1",blockTex1 },
                    {"blocktexture2",blockTex2 },
                    { "blocktexture3",blockTex3},
                    { "blocktexture4",blockTex4},
                    { "blocktexture5",blockTex5},
                      { "blocktexture-1",menubkgrdTransparent},
                    {"blocktexture6" ,blockTex6},
                    {"blocktexture7" ,blockTex7},
                    {"blocktexture8" ,blockTex8},
                    { "blocktexture9",blockTex9},
                    { "blocktexture12",blockTex12},
                    { "blocktexture13",blockTex13},
                    { "blocktexture14",blockTex14},
                { "blocktexture102",blockTex102}
            };


            if (game.gamePlatformType == GamePlatformType.VeryLowDefMobile)
            {
                Texture2D touchUpTex = game.Content.Load<Texture2D>("mobiletouch/up");
                Texture2D touchDownTex = game.Content.Load<Texture2D>("mobiletouch/down");
                Texture2D touchLeftTex = game.Content.Load<Texture2D>("mobiletouch/left");
                Texture2D touchRightTex = game.Content.Load<Texture2D>("mobiletouch/right");


                Texture2D touchUpPressedTex = game.Content.Load<Texture2D>("mobiletouch/up_pressed");
                Texture2D touchDownPressedTex = game.Content.Load<Texture2D>("mobiletouch/down_pressed");
                Texture2D touchLeftPressedTex = game.Content.Load<Texture2D>("mobiletouch/left_pressed");
                Texture2D touchRightPressedTex = game.Content.Load<Texture2D>("mobiletouch/right_pressed");



                Texture2D touchJumpTex = game.Content.Load<Texture2D>("mobiletouch/jump");
                Texture2D touchFlyTex = game.Content.Load<Texture2D>("mobiletouch/flyingascend");
                Texture2D touchJumpPressedTex = game.Content.Load<Texture2D>("mobiletouch/jump_pressed");


                Texture2D touchAttackTex = game.Content.Load<Texture2D>("mobiletouch/attack");
                Texture2D touchAttackPressedTex = game.Content.Load<Texture2D>("mobiletouch/attack_pressed");

                Texture2D touchInteractTex = game.Content.Load<Texture2D>("mobiletouch/interact");
                Texture2D touchInteractPressedTex = game.Content.Load<Texture2D>("mobiletouch/interact_pressed");

                Texture2D touchSprintTex = game.Content.Load<Texture2D>("mobiletouch/sprint");
                Texture2D touchSprintPressedTex = game.Content.Load<Texture2D>("mobiletouch/sprint_pressed");

                Texture2D touchPauseTex = game.Content.Load<Texture2D>("mobiletouch/pause");
                Texture2D touchInventoryTex = game.Content.Load<Texture2D>("mobiletouch/inventorybutton");

                UIElement.UITextures.TryAdd("mobiletouchup",touchUpTex);
                UIElement.UITextures.TryAdd("mobiletouchdown", touchDownTex);
                UIElement.UITextures.TryAdd("mobiletouchleft", touchLeftTex);
                UIElement.UITextures.TryAdd("mobiletouchright", touchRightTex);

                UIElement.UITextures.TryAdd("mobiletouchuppressed", touchUpPressedTex);
                UIElement.UITextures.TryAdd("mobiletouchdownpressed", touchDownPressedTex);
                UIElement.UITextures.TryAdd("mobiletouchleftpressed", touchLeftPressedTex);
                UIElement.UITextures.TryAdd("mobiletouchrightpressed", touchRightPressedTex);

                UIElement.UITextures.TryAdd("mobiletouchjump", touchJumpTex);


                UIElement.UITextures.TryAdd("mobiletouchfly", touchFlyTex);
                UIElement.UITextures.TryAdd("mobiletouchjumppressed", touchJumpPressedTex);

                UIElement.UITextures.TryAdd("mobiletouchattack", touchAttackTex);
                UIElement.UITextures.TryAdd("mobiletouchattackpressed", touchAttackPressedTex);

                UIElement.UITextures.TryAdd("mobiletouchinteract", touchInteractTex);
                UIElement.UITextures.TryAdd("mobiletouchinteractpressed", touchInteractPressedTex);

                UIElement.UITextures.TryAdd("mobiletouchsprint", touchSprintTex);
                UIElement.UITextures.TryAdd("mobiletouchsprintpressed", touchSprintPressedTex);

                UIElement.UITextures.TryAdd("mobiletouchpause", touchPauseTex);
                UIElement.UITextures.TryAdd("mobiletouchinventory", touchInventoryTex);
            }
        //    BlockResourcesManager.LoadDefaultUIResources(game.Content, game);

            UIElement.menuUIs = new List<UIElement> {

                new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
             //   new InputField(new Vector2(0.3f, 0.1f), 0.4f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window, null ,"",1,10,false),

        //        new InputField(new Vector2(0.3f, 0.85f), 0.4f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window, null ,"",1,10,false),
                new UIButton(new Vector2(0.3f, 0.3f), 0.4f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, game.InitGameplay ,"Start Game",null,1),
                new UIButton(new Vector2(0.3f, 0.6f), 0.4f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, game.GoToSettings ,"Game Settings",null,1)

            };

            if (game.gamePlatformType == GamePlatformType.HighDefDX)
            {
                UIElement.settingsUIsPage1 = new List<UIElement> {

                new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
                new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderDistance ,"Render Distance : "+GameOptions.renderDistance,GameOptions.UpdateRenderDistanceUIText,1),
                 new UIButton(new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderShadow ,"Render Shadow : "+GameOptions.renderShadow,GameOptions.UpdateRenderShadowUIText,1),
                  new UIButton(new Vector2(0.25f, 0.4f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderFarShadow ,"Render Far Shadow : "+GameOptions.renderFarShadow,GameOptions.UpdateRenderFarShadowUIText,1),
                  new UIButton(new Vector2(0.25f, 0.55f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,GameOptions.ChangeRenderSSAO ,"Render SSAO: "+GameOptions.renderSSAO,GameOptions.UpdateRenderSSAOUIText ,1),
                  new UIButton(new Vector2(0.25f, 0.7f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,GameOptions.ChangeRenderLightShaft ,"Render Light Shaft :"+GameOptions.renderLightShaft,GameOptions.UpdateRenderLightShaftUIText,1 ),
                  new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu",null ,1),
                  new UIButton(new Vector2(0.8f, 0.4f), 0.1f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{UIElement.settingsUIsPageID+=1; } ,"Next Page",null ,0.5f),
            };


                UIElement.settingsUIsPage2 = new List<UIElement> {

                new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
                new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderSSR ,"Render SSR : "+GameOptions.renderSSR,GameOptions.UpdateRenderSSRUIText,1),
                 new UIButton(new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderSSID ,"Render SSID : "+GameOptions.renderSSID,GameOptions.UpdateRenderSSIDUIText,1),
                new UIButton(new Vector2(0.25f, 0.4f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderContactShadow ,"Render Contact Shadow : "+GameOptions.renderContactShadow,GameOptions.UpdateRenderContactShadowUIText,1),
                 new UIButton(new Vector2(0.25f, 0.55f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeShowGraphicsDebug ,"Show Graphics Debug : "+GameOptions.showGraphicsDebug,GameOptions.UpdateShowGraphicsDebugUIText,1),
                     new UIButton(new Vector2(0.25f, 0.7f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderMotionBlur ,"Render Motion Blur : "+GameOptions.renderMotionBlur,GameOptions.UpdateRenderMotionBlurUIText,1),
             //     new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu" ),
                  new UIButton(new Vector2(0.1f, 0.4f), 0.1f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{UIElement.settingsUIsPageID-=1; } ,"Previous Page" ,null,0.5f),
            };
            }else if (game.gamePlatformType == GamePlatformType.LowDefGL)
            {
                UIElement.settingsUIsPage1 = new List<UIElement> {

                new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
                new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderDistance ,"Render Distance : "+GameOptions.renderDistance,GameOptions.UpdateRenderDistanceUIText,1),


                new UIButton(new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,GameOptions.ChangeRenderSSAO ,"Render SSAO: "+GameOptions.renderSSAO,GameOptions.UpdateRenderSSAOUIText ,1),

                new UIButton(new Vector2(0.25f, 0.4f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeShowGraphicsDebug ,"Show Graphics Debug : "+GameOptions.showGraphicsDebug,GameOptions.UpdateShowGraphicsDebugUIText,1),
                new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu",null ,1),
                  new UIButton(new Vector2(0.8f, 0.4f), 0.1f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{UIElement.settingsUIsPageID+=1; } ,"Next Page",null ,0.5f),
            };


                UIElement.settingsUIsPage2 = new List<UIElement> {
                    new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
             //     new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu" ),
                  new UIButton(new Vector2(0.1f, 0.4f), 0.1f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{UIElement.settingsUIsPageID-=1; } ,"Previous Page" ,null,0.5f),
            };
            }else if (game.gamePlatformType == GamePlatformType.VeryLowDefMobile)
            {

                UIElement.settingsUIsPage1 = new List<UIElement> {

                    new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
                    new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderDistance ,"Render Distance : "+GameOptions.renderDistance,GameOptions.UpdateRenderDistanceUIText,1),


                  

                    new UIButton(new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeShowGraphicsDebug ,"Show Graphics Debug : "+GameOptions.showGraphicsDebug,GameOptions.UpdateShowGraphicsDebugUIText,1),
                    new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu",null ,1),
                 
                };
            }

            UIPanel leftDownPanel = new UIPanel(new Vector2(0.05f, 0.6f), 0.3f, 0.3f, true);

            UIPanel rightDownPanel = new UIPanel(new Vector2(0.65f, 0.6f), 0.3f, 0.3f, true);

            UIPanel rightUpPanel = new UIPanel(new Vector2(0.65f, 0.05f), 0.3f, 0.3f, true);

            UIPanel leftUpPanel = new UIPanel(new Vector2(0.05f, 0.05f), 0.3f, 0.3f, true);

            UIPanel middleDownPanel = new UIPanel(new Vector2(0.2f, 0.85f), 0.6f, 0.15f, false);

            if (game.gamePlatformType == GamePlatformType.VeryLowDefMobile)
            {
                UIElement.inGameUIs = new List<UIElement>
                {
                    new InGameUI(sf,game.Window,game._spriteBatch, game.gamePlayerR,UIElement.UITextures["hotbartexture"],UIElement.UITextures["selectedhotbar"]),
                    leftDownPanel,
                    new UIButton(new Vector2(0.333f, 0.0f), 0.333f, 0.333f, UIElement.UITextures["mobiletouchup"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,   (ub) => { PlayerInputManager.mobilemotionVec.Z = 1f;} ,"",null,1,false,true,leftDownPanel,true,UIElement.UITextures["mobiletouchuppressed"]),


                    new UIButton(new Vector2(0.333f, 0.666f), 0.333f, 0.333f, UIElement.UITextures["mobiletouchdown"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,   (ub) => {PlayerInputManager.mobilemotionVec.Z = -1f;} ,"",null,1,false,true,leftDownPanel,true,UIElement.UITextures["mobiletouchdownpressed"]),

                    new UIButton(new Vector2(0.666f, 0.333f), 0.333f, 0.333f, UIElement.UITextures["mobiletouchright"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,   (ub) => { PlayerInputManager.mobilemotionVec.X = 1f;} ,"",null,1,false,true,leftDownPanel,true,UIElement.UITextures["mobiletouchrightpressed"]),

                    new UIButton(new Vector2(0f, 0.333f), 0.333f, 0.333f, UIElement.UITextures["mobiletouchleft"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,   (ub) => { PlayerInputManager.mobilemotionVec.X = -1f;} ,"",null,1,false,true,leftDownPanel,true,UIElement.UITextures["mobiletouchleftpressed"]),



                    rightDownPanel,

                    rightUpPanel,
                    leftUpPanel,
                    middleDownPanel,
                    new UIButton(new Vector2(0.25f, 0.25f), 0.5f, 0.5f, UIElement.UITextures["mobiletouchjump"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobilemotionVec.Y = 1f;} ,"",null,1,false,true,rightDownPanel,true,UIElement.UITextures["mobiletouchjumppressed"]),

                

                    new UIButton(new Vector2(0.25f, 0.0f), 0.5f, 0.5f, UIElement.UITextures["mobiletouchattack"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileIsLMBPressed = true;} ,"",null,1,false,true,leftUpPanel,true,UIElement.UITextures["mobiletouchattackpressed"]),


                    new UIButton(new Vector2(0.25f, 0.5f), 0.5f, 0.5f, UIElement.UITextures["mobiletouchsprint"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileIsSprintingPressed = true;} ,"",null,1,false,true,leftUpPanel,true,UIElement.UITextures["mobiletouchsprintpressed"]),

                    new UIButton(new Vector2(0.25f, 0.0f), 0.5f, 0.5f, UIElement.UITextures["mobiletouchinteract"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileIsRMBPressed =true;} ,"",null,1,false,true,rightUpPanel,true,UIElement.UITextures["mobiletouchinteractpressed"]),

                    new UIButton(new Vector2(0.25f, 0.5f), 0.5f, 0.5f, UIElement.UITextures["mobiletouchfly"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileIsFlyingPressed =true;} ,"",null,1,false,true,rightUpPanel,false),
                    new UIButton(new Vector2(0.45f, 0.0f), 0.1f, 0.1f, UIElement.UITextures["mobiletouchpause"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {game.PauseGame(null);} ,"",null,1,true,true,null,false),


                    new UIButton(new Vector2(0.0f, 0.35f), 0.08f, 0.5f, UIElement.UITextures["mobiletouchleft"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileScrollDelta-=130;} ,"",null,1,false,true,middleDownPanel,false),


                    new UIButton(new Vector2(0.92f, 0.35f), 0.08f, 0.5f, UIElement.UITextures["mobiletouchright"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {PlayerInputManager.mobileScrollDelta+=130;} ,"",null,1,false,true,middleDownPanel,false),

                    new UIButton(new Vector2(0.35f, 0.0f), 0.3f, 0.3f, UIElement.UITextures["mobiletouchinventory"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                        (ub) => {game.OpenInventory(ub);} ,"",null,1,true,true,middleDownPanel,false),
                };
            }
            else
            {
                UIElement.inGameUIs = new List<UIElement>
                {
                    new InGameUI(sf,game.Window,game._spriteBatch,  game.gamePlayerR,UIElement.UITextures["hotbartexture"],UIElement.UITextures["selectedhotbar"]),
                   
                };
            }
            UIElement.pauseMenuUIs = new List<UIElement>
            {
               // new InGameUI(sf,game.Window,game._spriteBatch, game,UIElement.UITextures["hotbartexture"],UIElement.UITextures["selectedhotbar"])
               new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackgroundtransparent"],game._spriteBatch),
                 new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.15f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, (ub)=>game.ResumeGame() ,"Resume Game",null,1),
                  new UIButton(new Vector2(0.25f, 0.3f), 0.5f, 0.15f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, (ub)=>game.QuitGameplay() ,"Quit Game",null,1),
                    new UIButton(new Vector2(0.25f, 0.5f), 0.5f, 0.15f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, (ub)=>
                    {
                        if (game.renderPipelineManager is HighDefRenderPipelineManager)
                        {
                            game.effectsManager.LoadCustomPostProcessEffects(game.GraphicsDevice,
                                (game.renderPipelineManager as HighDefRenderPipelineManager).customPostProcessors, game.Content);
                        }
                   
                    },"Reload Custom Postprocessing Shaders",null,0.6f),
               new UIButton(new Vector2(0.25f, 0.7f), 0.5f, 0.15f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, (ub)=>BlockResourcesManager.LoadResources(Directory.GetCurrentDirectory() + "/customresourcespack",game.Content,game.GraphicsDevice,game.renderPipelineManager.chunkRenderer,game.renderPipelineManager.particleRenderer,game) ,"Reload Custom Resource Packs",null,0.6f)
            };
            InitInventoryUI(game, sf);
            InitStructureOperationsUI(game, sf);
            game.status = GameStatus.Menu;
        }

        public static void InitStructureOperationsUI(MinecraftGameBase game, SpriteFont sf)
        {
            UIElement.structureOperationsSavingUIs = new List<UIElement>()
            {
                new UIImage(new Vector2(0.05f, 0.05f), 0.9f, 0.9f, UIElement.UITextures["menubackgroundtransparent"], game._spriteBatch),
                new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["menubackgroundtransparent"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, (ub)=>{} ,"Structure Saving",null,1,false,false),
                new InputField(new Vector2(0.3f, 0.3f), 0.4f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,
                    (ub) => { VoxelWorld.currentWorld.structureOperationsManager.tmpSavingStructureName = ub.text;},"structurename",1,16,false),
                new InputField(new Vector2(0.1f, 0.45f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window, (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureOrigin.x =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(new Vector2(0.1f, 0.55f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureOrigin.y =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(new Vector2(0.1f, 0.65f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureOrigin.z =ub.TryParseInt();} ,"0",1,16,true),

                new InputField(new Vector2(0.6f, 0.45f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureSize.x =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(new Vector2(0.6f, 0.55f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureSize.y =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(new Vector2(0.6f, 0.65f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curSaveStructureSize.z =ub.TryParseInt();} ,"0",1,16,true),


                new UIButton(new Vector2(0.1f, 0.8f), 0.3f, 0.15f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isShowingStructureSavingBounds = !VoxelWorld
                            .currentWorld.structureOperationsManager.isShowingStructureSavingBounds;} ,"Show Bounds",null,1,false,true),

                new UIButton(new Vector2(0.6f, 0.8f), 0.3f, 0.15f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager. SaveNewStructure(VoxelWorld.currentWorld.structureOperationsManager.tmpSavingStructureName);} ,"Save Structure",null,1,false,true),


                new UIButton(new Vector2(0.8f, 0.1f), 0.1f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{UIElement.structureOperationsUIsPageID=1; } ,"Next Page" ,null,0.5f),

                new UIButton(new Vector2(0.1f, 0.1f), 0.1f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{game.CloseStructureOperationsUI(); } ,"Close UI" ,null,0.5f),
            };



            UIElement.structureOperationsPlacingUIs = new List<UIElement>()
            {
                new UIImage(new Vector2(0.05f, 0.05f), 0.9f, 0.9f, UIElement.UITextures["menubackgroundtransparent"], game._spriteBatch),
                new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["menubackgroundtransparent"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, (ub)=>{} ,"Structure Placing",null,1,false,false),
                new InputField(new Vector2(0.3f, 0.3f), 0.4f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.tmpPlacingStructureName = ub.text;
                        if (VoxelWorld.currentWorld.structureOperationsManager.allStructureDatas.ContainsKey(
                                VoxelWorld.currentWorld.structureOperationsManager.tmpPlacingStructureName))
                        {
                            VoxelWorld.currentWorld.structureOperationsManager.curPlacingStructureData =
                                VoxelWorld.currentWorld.structureOperationsManager.allStructureDatas[
                                    VoxelWorld.currentWorld.structureOperationsManager.tmpPlacingStructureName];
                        }


                    },"structurename",1,16,false),
                new InputField(new Vector2(0.1f, 0.45f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window, (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curPlacingStructureOrigin.x =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(new Vector2(0.1f, 0.55f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curPlacingStructureOrigin.y =ub.TryParseInt();} ,"0",1,16,true),
                new InputField(new Vector2(0.1f, 0.65f), 0.3f, 0.1f, UIElement.UITextures["inputfield"],UIElement.UITextures["inputfieldhighlighted"],sf,game._spriteBatch,game.Window,  (ub) => { VoxelWorld.currentWorld.structureOperationsManager.curPlacingStructureOrigin.z =ub.TryParseInt();} ,"0",1,16,true),



                new UIButton(new Vector2(0.6f, 0.45f), 0.3f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isStructurePlacingInvertX = !VoxelWorld
                            .currentWorld.structureOperationsManager.isStructurePlacingInvertX;
                        ub.text = "Inverse X:" + VoxelWorld.currentWorld.structureOperationsManager
                            .isStructurePlacingInvertX.ToString();
                    } ,"Inverse X:False",null,1,false,true),
                new UIButton(new Vector2(0.6f, 0.55f), 0.3f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isStructurePlacingInvertY = !VoxelWorld
                            .currentWorld.structureOperationsManager.isStructurePlacingInvertY;
                        ub.text = "Inverse Y:" + VoxelWorld.currentWorld.structureOperationsManager
                            .isStructurePlacingInvertY.ToString();
                    } ,"Inverse Y:False",null,1,false,true),
                new UIButton(new Vector2(0.6f, 0.65f), 0.3f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isStructurePlacingInvertZ = !VoxelWorld
                            .currentWorld.structureOperationsManager.isStructurePlacingInvertZ;
                        ub.text = "Inverse Z:" + VoxelWorld.currentWorld.structureOperationsManager
                            .isStructurePlacingInvertZ.ToString();
                    } ,"Inverse Z:False",null,1,false,true),




                new UIButton(new Vector2(0.1f, 0.8f), 0.3f, 0.15f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager.isShowingStructurePlacingBounds = !VoxelWorld
                            .currentWorld.structureOperationsManager.isShowingStructurePlacingBounds;} ,"Show Bounds",null,1,false,true),

                new UIButton(new Vector2(0.6f, 0.8f), 0.3f, 0.15f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,
                    (ub) =>
                    {
                        VoxelWorld.currentWorld.structureOperationsManager. PlaceSavedStructureWithTmpParams();} ,"Place Structure",null,1,false,true),

                new UIButton(new Vector2(0.1f, 0.1f), 0.1f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(obj)=>{UIElement.structureOperationsUIsPageID=0; } ,"Previous Page" ,null,0.5f),
            };
        }
        public static void InitInventoryUI(Game game, SpriteFont sf)
        {
            if (game is MinecraftGameBase)
            {
                MinecraftGameBase minecraftGame=game as MinecraftGameBase;
            UIElement.inventoryUIs = new List<UIElement> { new UIImage(new Vector2(0.1f, 0.1f), 0.8f, 0.8f, UIElement.UITextures["menubackgroundtransparent"], minecraftGame._spriteBatch),
             new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["menubackgroundtransparent"],new Vector2(0.4f,0.55f),sf,minecraftGame._spriteBatch,game.Window, (ub)=>{} ,"Inventory",null,1,false,false),



             new UIButton(new Vector2(0.35f, 0.7f), 0.3f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,minecraftGame._spriteBatch,game.Window, (ub)=>{minecraftGame.OpenInventory(ub);} ,"Close",null,1,false,true),
            };
            int elementCount = 0;
            foreach (var element in Chunk.blockInfosNew)
            {
                UIElement.inventoryUIs.Add(new UIButton(new Vector2(elementCount % 10 * 0.05f + 0.25f, elementCount / 10 * 0.05f + 0.25f), 0.05f, 0.05f, UIElement.UITextures.ContainsKey("blocktexture" + element.Key) && UIElement.UITextures["blocktexture" + element.Key] != null ? UIElement.UITextures["blocktexture" + element.Key] : UIElement.UITextures["blocktexture-1"],
                    new Vector2(0f, 0f), null, minecraftGame._spriteBatch, game.Window, (ub) => minecraftGame.gamePlayerR.gamePlayer.inventoryData[minecraftGame.gamePlayerR.gamePlayer.currentSelectedHotbar] = (short)element.Key, " ", null, 0f, true
                    ));
                elementCount++;
            }
            foreach (var element in UIElement.inventoryUIs)
            {
                element.OnResize();
            }
            } 
           
        }*/

    }

 
}
