using System.IO;
using MCDX.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
 
namespace MCDXEditor;

/// <summary>
/// MCDX Game 新的游戏入口
/// </summary>
public class MCDXGame: Game
{
    
    private RootNode rootNode;
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Texture2D awesomeface;
    internal MCDXGame()
    {
        graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferHeight = 1080,
            PreferredBackBufferWidth = 1920,
        };
        IsMouseVisible = true;
        Content.RootDirectory = "Content";
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        MyraEnvironment.Game = this;
        rootNode = new RootNode(this);
        spriteBatch = new SpriteBatch(GraphicsDevice);
        awesomeface = Content.Load<Texture2D>("awesomeface");
        MGCBInvoker.BuildContent("C:/unityMinecraftData/testcontentpack/Content.mgcb", "C:/unityMinecraftData/testcontentpack/bin/foo", "C:/unityMinecraftData/testcontentpack/obj/foo", "C:/unityMinecraftData/testcontentpack/");
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        //渲染管线内容
        //1. 绘制3d世界
        spriteBatch.Begin();
        spriteBatch.Draw(awesomeface,new Vector2(300,300),Color.White);
        spriteBatch.End();
        //2. 绘制UI
        rootNode.Render();
    }
}