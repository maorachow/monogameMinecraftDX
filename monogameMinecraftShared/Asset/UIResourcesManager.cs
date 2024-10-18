using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using monogameMinecraftShared.UI;
using Microsoft.Xna.Framework.Audio;
using monogameMinecraftShared.Utility;
// ReSharper disable all StringLiteralTypo
namespace monogameMinecraftShared.Asset
{
    public class UIResourcesManager
    {

        public Dictionary<string, Texture2D> UITextures = new Dictionary<string, Texture2D>();
        public Dictionary<string,SoundEffect> uiSounds= new Dictionary<string, SoundEffect>();
        public SpriteFont sf;
        private static UIResourcesManager _instance;
        private static readonly object locker = new object();

        private UIResourcesManager()
        {

        }
        public static UIResourcesManager instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                      
                        if (_instance == null)
                        {
                            _instance = new UIResourcesManager();
                        }
                    }
                }
                return _instance;
            }
        }
        public void LoadDefaultBlockSpriteResources(MinecraftGameBase game)
        {
            Dictionary<int, string> blockSpriteInfoData = new Dictionary<int, string>
                {
                    { 1, "blocksprites/stone" },
                    { 2, "blocksprites/grass_side_carried" },
                    { 3, "blocksprites/dirt" },
                    { 4, "blocksprites/grass_side_carried" },
                    { 5, "blocksprites/bedrock" },
                    { 6, "blocksprites/log_oak" },
                    { 7, "blocksprites/log_oak" },
                 
                    { 8, "blocksprites/log_oak" },
                    { 9, "blocksprites/leaves_oak_carried" },
                    { 10, "blocksprites/diamond_ore" },
                    { 11, "blocksprites/sand" },
                    { 12, "blocksprites/end_stone" },
                    { 13, "blocksprites/endframe_top" },
                    { 14, "blocksprites/sea_lantern" },
                    { 15, "blocksprites/iron_block" },
                    { 16, "blocksprites/cobblestone" },
                    { 17, "blocksprites/planks_oak" },
                    { 18, "blocksprites/wool_colored_red" },
                    { 19, "blocksprites/wool_colored_green" },
                    { 20, "blocksprites/wool_colored_blue" },
                    { 21, "blocksprites/planks_oak" },
                    { 22, "blocksprites/wool_colored_white" },
                    { 23, "blocksprites/wool_colored_black" },
                    { 100, "blocksprites/water" },
                    { 101, "blocksprites/grass" },
                    { 102, "blocksprites/torch_on" },
                    { 103, "blocksprites/fences" },
                    { 104, "blocksprites/woodendoor" },
                    { 105, "blocksprites/ladder" },
                    { 106, "blocksprites/glass_green" },
                    { 107, "blocksprites/glass_blue" },
                    { 108, "blocksprites/glass_black" },
                    { 109, "blocksprites/glass" },
                    { 110, "blocksprites/glass_white" },
                    { 111, "blocksprites/glass_red" },
                };
            foreach (var item in blockSpriteInfoData)
            {
                try
                {
                    Texture2D sprite = game.Content.Load<Texture2D>(item.Value);

                    //    se.Play(1, 0, 0);
                    if (!UITextures.ContainsKey("blocktexture" + item.Key))
                    {
                      UITextures.Add("blocktexture" + item.Key, sprite);
                    }
                    else
                    {
                       UITextures["blocktexture" + item.Key] = sprite;
                    }
                }
                catch
                {
                    UITextures["blocktexture" + item.Key] = null;
                }
            }

        }
        public void LoadTextures(MinecraftGameBase game)
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



           uiSounds.Clear();

            uiSounds.TryAdd("uiclick", game.Content.Load<SoundEffect>("sounds/uiclick"));
            UITextures = new Dictionary<string, Texture2D> {
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

               UITextures.TryAdd("mobiletouchup", touchUpTex);
              UITextures.TryAdd("mobiletouchdown", touchDownTex);
              UITextures.TryAdd("mobiletouchleft", touchLeftTex);
              UITextures.TryAdd("mobiletouchright", touchRightTex);

             UITextures.TryAdd("mobiletouchuppressed", touchUpPressedTex);
             UITextures.TryAdd("mobiletouchdownpressed", touchDownPressedTex);
              UITextures.TryAdd("mobiletouchleftpressed", touchLeftPressedTex);
            UITextures.TryAdd("mobiletouchrightpressed", touchRightPressedTex);

             UITextures.TryAdd("mobiletouchjump", touchJumpTex);


              UITextures.TryAdd("mobiletouchfly", touchFlyTex);
            UITextures.TryAdd("mobiletouchjumppressed", touchJumpPressedTex);

             UITextures.TryAdd("mobiletouchattack", touchAttackTex);
             UITextures.TryAdd("mobiletouchattackpressed", touchAttackPressedTex);

                UITextures.TryAdd("mobiletouchinteract", touchInteractTex);
                UITextures.TryAdd("mobiletouchinteractpressed", touchInteractPressedTex);

                UITextures.TryAdd("mobiletouchsprint", touchSprintTex);
                UITextures.TryAdd("mobiletouchsprintpressed", touchSprintPressedTex);

                UITextures.TryAdd("mobiletouchpause", touchPauseTex);
                UITextures.TryAdd("mobiletouchinventory", touchInventoryTex);
            }
        }

    }
}
