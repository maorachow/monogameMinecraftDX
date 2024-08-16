using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using monogameMinecraftDX.Updateables;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.World;
using System.Collections.Generic;

namespace monogameMinecraftShared.Rendering
{
    public class SkyboxRenderer
    {
        public GraphicsDevice device;
        public Effect skyboxEffect;

        public TextureCube skyboxTexture;
        public TextureCube skyboxTextureNight;
        public VertexBuffer skyboxVertexBuffer;
        public List<VertexPosition> skyboxVertices;
        public IGamePlayer player;
        public float curDateTime = 0f;
        public GameTimeManager gametimeManager;
        public SkyboxRenderer(GraphicsDevice device, Effect skyboxEffect, TextureCube skyboxTex, IGamePlayer player, Texture2D skyboxTexPX, Texture2D skyboxTexPY, Texture2D skyboxTexPZ, Texture2D skyboxTexNX, Texture2D skyboxTexNY, Texture2D skyboxTexNZ
            , Texture2D skyboxTexPXN, Texture2D skyboxTexPYN, Texture2D skyboxTexPZN, Texture2D skyboxTexNXN, Texture2D skyboxTexNYN, Texture2D skyboxTexNZN, GameTimeManager gametimeManager
            )
        {
            this.device = device;
            this.skyboxEffect = skyboxEffect;

            skyboxTexture = skyboxTex;
            this.player = player;
            this.gametimeManager = gametimeManager;
            skyboxVertices = new List<VertexPosition> {
           new VertexPosition( new Vector3(  -1.0f, -1.0f, -1.0f)),
    new VertexPosition(new Vector3(  1.0f, -1.0f, -1.0f)),
   new VertexPosition(  new Vector3( 1.0f,  1.0f, -1.0f)),
   new VertexPosition(new Vector3(   1.0f,  1.0f, -1.0f)),
   new VertexPosition( new Vector3( -1.0f,  1.0f, -1.0f)),
   new VertexPosition( new Vector3( -1.0f, -1.0f, -1.0f)),

   new VertexPosition(new Vector3(-1.0f, -1.0f,  1.0f)),
   new VertexPosition(new Vector3(   1.0f, -1.0f,  1.0f)),
   new VertexPosition( new Vector3(  1.0f,  1.0f,  1.0f)),
   new VertexPosition( new Vector3(  1.0f,  1.0f,  1.0f)),
  new VertexPosition(new Vector3(   -1.0f,  1.0f,  1.0f)),
  new VertexPosition( new Vector3(  -1.0f, -1.0f,  1.0f)),

  new VertexPosition(  new Vector3( -1.0f,  1.0f,  1.0f)),
   new VertexPosition(new Vector3(  -1.0f,  1.0f, -1.0f)),
   new VertexPosition(new Vector3(  -1.0f, -1.0f, -1.0f)),
   new VertexPosition(new Vector3(  -1.0f, -1.0f, -1.0f)),
  new VertexPosition( new Vector3(  -1.0f, -1.0f,  1.0f)),
  new VertexPosition( new Vector3(  -1.0f,  1.0f,  1.0f)),

  new VertexPosition( new Vector3(   1.0f,  1.0f,  1.0f )),
  new VertexPosition( new Vector3(   1.0f,  1.0f, -1.0f)),
   new VertexPosition(new Vector3(   1.0f, -1.0f, -1.0f)),
   new VertexPosition( new Vector3(  1.0f, -1.0f, -1.0f)),
  new VertexPosition( new Vector3(   1.0f, -1.0f,  1.0f)),
  new VertexPosition( new Vector3(   1.0f,  1.0f,  1.0f)),

   new VertexPosition(new Vector3(  -1.0f, -1.0f, -1.0f)),
  new VertexPosition( new Vector3(   1.0f, -1.0f, -1.0f)),
new VertexPosition(   new Vector3(   1.0f, -1.0f,  1.0f)),
   new VertexPosition(new Vector3(   1.0f, -1.0f,  1.0f)),
  new VertexPosition(  new Vector3( -1.0f, -1.0f,  1.0f)),
  new VertexPosition(  new Vector3( -1.0f, -1.0f, -1.0f)),

  new VertexPosition( new Vector3(  -1.0f,  1.0f, -1.0f)),
   new VertexPosition(new Vector3(   1.0f,  1.0f, -1.0f)),
  new VertexPosition( new Vector3(   1.0f,  1.0f,  1.0f)),
  new VertexPosition( new Vector3(   1.0f,  1.0f,  1.0f)),
  new VertexPosition( new Vector3(  -1.0f,  1.0f,  1.0f)),
  new VertexPosition( new Vector3(  -1.0f,  1.0f, -1.0f) ),
            };
            skyboxVertexBuffer = new VertexBuffer(device, typeof(VertexPosition), 36, BufferUsage.None);
            skyboxVertexBuffer.SetData(skyboxVertices.ToArray());
            int width = skyboxTexPX.Width;
            int height = skyboxTexPX.Height;
            Color[] data = new Color[width * height];
        /*    skyboxTexture = new TextureCube(device, 128, false, SurfaceFormat.Color);
            skyboxTexPX.GetData(data);
            skyboxTexture.SetData(CubeMapFace.PositiveX, data);
            skyboxTexPY.GetData(data);
            skyboxTexture.SetData(CubeMapFace.PositiveY, data);
            skyboxTexPZ.GetData(data);
            skyboxTexture.SetData(CubeMapFace.PositiveZ, data);
            skyboxTexNX.GetData(data);
            skyboxTexture.SetData(CubeMapFace.NegativeX, data);
            skyboxTexNY.GetData(data);
            skyboxTexture.SetData(CubeMapFace.NegativeY, data);
            skyboxTexNZ.GetData(data);
            skyboxTexture.SetData(CubeMapFace.NegativeZ, data);

            skyboxTextureNight = new TextureCube(device, 128, false, SurfaceFormat.Color);
            skyboxTexPXN.GetData(data);
            skyboxTextureNight.SetData(CubeMapFace.PositiveX, data);
            skyboxTexPYN.GetData(data);
            skyboxTextureNight.SetData(CubeMapFace.PositiveY, data);
            skyboxTexPZN.GetData(data);
            skyboxTextureNight.SetData(CubeMapFace.PositiveZ, data);
            skyboxTexNXN.GetData(data);
            skyboxTextureNight.SetData(CubeMapFace.NegativeX, data);
            skyboxTexNYN.GetData(data);
            skyboxTextureNight.SetData(CubeMapFace.NegativeY, data);
            skyboxTexNZN.GetData(data);
            skyboxTextureNight.SetData(CubeMapFace.NegativeZ, data);*/

        }
        RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None };
        RasterizerState rasterizerState1 = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace };
        public void Draw(RenderTarget2D renderTarget, bool keepRenderTarget = false)
        {
            device.SetRenderTarget(renderTarget);


            device.RasterizerState = rasterizerState;
            device.DepthStencilState = DepthStencilState.None;
            skyboxEffect.Parameters["World"].SetValue(Matrix.CreateScale(50f) * Matrix.CreateTranslation(player.cam.position));
            skyboxEffect.Parameters["View"].SetValue(player.cam.viewMatrix);
            skyboxEffect.Parameters["Projection"].SetValue(player.cam.projectionMatrix);
            skyboxEffect.Parameters["SkyBoxTexture"].SetValue(skyboxTexture);
            skyboxEffect.Parameters["SkyBoxTextureNight"].SetValue(skyboxTextureNight);

            //  Debug.WriteLine(gametimeManager.dateTime);

            skyboxEffect.Parameters["mixValue"].SetValue(gametimeManager.skyboxMixValue);
            skyboxEffect.Parameters["CameraPosition"].SetValue(player.cam.position);
            device.SetVertexBuffer(skyboxVertexBuffer);
            foreach (var pass in skyboxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
            }
            device.DepthStencilState = DepthStencilState.Default;

            device.RasterizerState = rasterizerState1;
            if (keepRenderTarget == false)
            {
                device.SetRenderTarget(null);
            }

        }
    }
}
