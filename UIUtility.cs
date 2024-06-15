using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace monogameMinecraft
{
    public static class UIUtility
    {
        public static void InitGameUI(MinecraftGame game)
        {

            SpriteFont sf;
            sf = game.Content.Load<SpriteFont>("defaultfont");
            Texture2D menubkgrd = game.Content.Load<Texture2D>("menubackground");
            Texture2D buttonTex = game.Content.Load<Texture2D>("buttontexture");
            Texture2D hotbarTex = game.Content.Load<Texture2D>("hotbar");
            Texture2D selectedHotbarTex = game.Content.Load<Texture2D>("selectedhotbar");

            Texture2D blockTex1 = game.Content.Load<Texture2D>("blocksprites/stone");
            Texture2D blockTex2 = game.Content.Load<Texture2D>("blocksprites/grass_side_carried");
            Texture2D blockTex3 = game.Content.Load<Texture2D>("blocksprites/dirt");
            Texture2D blockTex4= game.Content.Load<Texture2D>("blocksprites/grass_side_carried");
            Texture2D blockTex5 = game.Content.Load<Texture2D>("blocksprites/bedrock");
            Texture2D blockTex6 = game.Content.Load<Texture2D>("blocksprites/log_oak");
            Texture2D blockTex7 = game.Content.Load<Texture2D>("blocksprites/log_oak");
            Texture2D blockTex8 = game.Content.Load<Texture2D>("blocksprites/log_oak");
            Texture2D blockTex9 = game.Content.Load<Texture2D>("blocksprites/leaves_oak_carried");
            Texture2D blockTex12 = game.Content.Load<Texture2D>("blocksprites/end_stone");
            Texture2D blockTex13 = game.Content.Load<Texture2D>("blocksprites/endframe_top");
            Texture2D blockTex14 = game.Content.Load<Texture2D>("blocksprites/sea_lantern");
            Texture2D blockTex102 = game.Content.Load<Texture2D>("blocksprites/torch_on");
            UIElement.UITextures = new Dictionary<string, Texture2D> {
                    { "menubackground" ,menubkgrd},
                    { "buttontexture" ,buttonTex},
                    {"hotbartexture",hotbarTex },
                    {"selectedhotbar",selectedHotbarTex },
                    {"blocktexture1",blockTex1 },
                    {"blocktexture2",blockTex2 },
                    { "blocktexture3",blockTex3},
                    { "blocktexture4",blockTex4},
                    { "blocktexture5",blockTex5},
                    {"blocktexture6" ,blockTex6},
                    {"blocktexture7" ,blockTex7},
                    {"blocktexture8" ,blockTex8},
                    { "blocktexture9",blockTex9},
                    { "blocktexture12",blockTex12},
                    { "blocktexture13",blockTex13},
                    { "blocktexture14",blockTex14},
                { "blocktexture102",blockTex102}
            };
            UIElement.menuUIs = new List<UIElement> {
                
                new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
                new UIButton(new Vector2(0.3f, 0.3f), 0.4f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, game.InitGameplay ,"Start Game",null,1),
                new UIButton(new Vector2(0.3f, 0.6f), 0.4f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, game.GoToSettings ,"Game Settings",null,1)

            };

            UIElement.settingsUIsPage1 = new List<UIElement> {

                new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
                new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderDistance ,"Render Distance : "+GameOptions.renderDistance,GameOptions.UpdateRenderDistanceUIText,1),
                 new UIButton(new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderShadow ,"Render Shadow : "+GameOptions.renderShadow,GameOptions.UpdateRenderShadowUIText,1),
                  new UIButton(new Vector2(0.25f, 0.4f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderFarShadow ,"Render Far Shadow : "+GameOptions.renderFarShadow,GameOptions.UpdateRenderFarShadowUIText,1),
                  new UIButton(new Vector2(0.25f, 0.55f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,GameOptions.ChangeRenderSSAO ,"Render SSAO: "+GameOptions.renderSSAO,GameOptions.UpdateRenderSSAOUIText ,1),
                  new UIButton(new Vector2(0.25f, 0.7f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,GameOptions.ChangeRenderLightShaft ,"Render Light Shaft :"+GameOptions.renderLightShaft,GameOptions.UpdateRenderLightShaftUIText,1 ),
                  new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu",null ,1),
                  new UIButton(new Vector2(0.8f, 0.4f), 0.1f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(UIButton obj)=>{UIElement.settingsUIsPageID+=1; } ,"Next Page",null ,0.5f),
            };


            UIElement.settingsUIsPage2 = new List<UIElement> {

                new UIImage(new Vector2(0f,0f),1f,1f,UIElement.UITextures["menubackground"],game._spriteBatch),
                new UIButton(new Vector2(0.25f, 0.1f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderSSR ,"Render SSR : "+GameOptions.renderSSR,GameOptions.UpdateRenderSSRUIText,1),
                 new UIButton(new Vector2(0.25f, 0.25f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderSSID ,"Render SSID : "+GameOptions.renderSSID,GameOptions.UpdateRenderSSIDUIText,1),
                new UIButton(new Vector2(0.25f, 0.4f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderContactShadow ,"Render Contact Shadow : "+GameOptions.renderContactShadow,GameOptions.UpdateRenderContactShadowUIText,1),
                 new UIButton(new Vector2(0.25f, 0.55f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeShowGraphicsDebug ,"Show Graphics Debug : "+GameOptions.showGraphicsDebug,GameOptions.UpdateShowGraphicsDebugUIText,1),
                     new UIButton(new Vector2(0.25f, 0.7f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window, GameOptions.ChangeRenderMotionBlur ,"Render Motion Blur : "+GameOptions.renderMotionBlur,GameOptions.UpdateRenderMotionBlurUIText,1),
             //     new UIButton(new Vector2(0.25f, 0.85f), 0.5f, 0.1f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,game.GoToMenuFromSettings ,"Return To Menu" ),
                  new UIButton(new Vector2(0.1f, 0.4f), 0.1f, 0.2f, UIElement.UITextures["buttontexture"],new Vector2(0.4f,0.55f),sf,game._spriteBatch,game.Window,(UIButton obj)=>{UIElement.settingsUIsPageID-=1; } ,"Previous Page" ,null,0.5f),
            };
            UIElement.inGameUIs = new List<UIElement>
            {
                new InGameUI(sf,game.Window,game._spriteBatch, game,UIElement.UITextures["hotbartexture"],UIElement.UITextures["selectedhotbar"])
            };
            game.status = GameStatus.Menu;
        }
        
    }
}
