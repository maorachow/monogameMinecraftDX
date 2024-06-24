using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace monogameMinecraftDX.Rendering
{
    public class HDRCubemapRenderer
    {
        public GraphicsDevice device;
        public Effect cubemapEffect;
        public Effect cubemapIrradianceEffect;
        public Effect cubemapPrefilterEffect;
        public List<VertexPosition> skyboxVertices;
        public VertexBuffer skyboxVertexBuffer;
        public Texture2D hdriTex;
        public RenderTargetCube resultCubemap;
        public RenderTargetCube resultIrradianceCubemap;
        public RenderTargetCube resultSpecularCubemapMip0;
        public RenderTargetCube resultSpecularCubemapMip1;
        public RenderTargetCube resultSpecularCubemapMip2;
        public RenderTargetCube resultSpecularCubemapMip3;
        public RenderTargetCube resultSpecularCubemapMip4;
        public HDRCubemapRenderer(GraphicsDevice device, Effect cubemapEffect, Texture2D hdriTex, Effect cubemapIrradianceEffect, Effect cubemapPrefilterEffect)
        {
            this.device = device;
            this.cubemapEffect = cubemapEffect;
            this.hdriTex = hdriTex;
            /*       this.hdriTex = new Texture2D(device, 1024, 512, false, SurfaceFormat.Single);
                   using (var stream = File.OpenRead(Directory.GetCurrentDirectory() + "/environmenthdri.hdr"))
                   {
                       ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                       this.hdriTex.SetData(image.Data);
                   }*/

            InitializeVertices();
            InitializeCubeBuffers(device);
            resultCubemap = new RenderTargetCube(device, 512, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            resultSpecularCubemapMip0 = new RenderTargetCube(device, 256, true, SurfaceFormat.Vector4, DepthFormat.Depth24);
            resultSpecularCubemapMip1 = new RenderTargetCube(device, 128, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            resultSpecularCubemapMip2 = new RenderTargetCube(device, 64, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            resultSpecularCubemapMip3 = new RenderTargetCube(device, 32, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            resultSpecularCubemapMip4 = new RenderTargetCube(device, 16, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            resultIrradianceCubemap = new RenderTargetCube(device, 512, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            this.cubemapIrradianceEffect = cubemapIrradianceEffect;
            this.cubemapPrefilterEffect = cubemapPrefilterEffect;
        }
        public void InitializeVertices()
        {
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

        }

        public void InitializeCubeBuffers(GraphicsDevice device)
        {
            skyboxVertexBuffer = new VertexBuffer(device, typeof(VertexPosition), 36, BufferUsage.None);
            skyboxVertexBuffer.SetData(skyboxVertices.ToArray());
        }

        public void Render()
        {
            Matrix captureProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), 1.0f, 0.1f, 10.0f);
            Matrix[] captureViews = new Matrix[6] { Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f)),
             Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f)),
            Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)),
            Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)),
            Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
             Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f))};
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;
            device.DepthStencilState = DepthStencilState.None;
            for (int i = 0; i < 6; i++)
            {
                device.SetRenderTarget(resultCubemap, CubeMapFace.PositiveX + i);
                cubemapEffect.Parameters["View"].SetValue(captureViews[i]);
                cubemapEffect.Parameters["Projection"].SetValue(captureProjection);
                cubemapEffect.Parameters["HDRImageTex"].SetValue(hdriTex);
                device.SetVertexBuffer(skyboxVertexBuffer);
                foreach (var pass in cubemapEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
                }
            }


            // device.SetRenderTarget(null);

            for (int i = 0; i < 6; i++)
            {
                device.SetRenderTarget(resultIrradianceCubemap, CubeMapFace.PositiveX + i);
                cubemapIrradianceEffect.Parameters["View"].SetValue(captureViews[i]);
                cubemapIrradianceEffect.Parameters["Projection"].SetValue(captureProjection);
                cubemapIrradianceEffect.Parameters["HDRImageTex"].SetValue(resultCubemap);
                device.SetVertexBuffer(skyboxVertexBuffer);
                foreach (var pass in cubemapIrradianceEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                device.SetRenderTarget(resultSpecularCubemapMip0, CubeMapFace.PositiveX + i);
                cubemapPrefilterEffect.Parameters["View"].SetValue(captureViews[i]);
                cubemapPrefilterEffect.Parameters["Projection"].SetValue(captureProjection);
                cubemapPrefilterEffect.Parameters["HDRImageTex"].SetValue(resultCubemap);
                cubemapPrefilterEffect.Parameters["roughness"].SetValue(0f);
                device.SetVertexBuffer(skyboxVertexBuffer);
                foreach (var pass in cubemapPrefilterEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                device.SetRenderTarget(resultSpecularCubemapMip1, CubeMapFace.PositiveX + i);
                cubemapPrefilterEffect.Parameters["View"].SetValue(captureViews[i]);
                cubemapPrefilterEffect.Parameters["Projection"].SetValue(captureProjection);
                cubemapPrefilterEffect.Parameters["HDRImageTex"].SetValue(resultCubemap);
                cubemapPrefilterEffect.Parameters["roughness"].SetValue(0.25f);
                device.SetVertexBuffer(skyboxVertexBuffer);
                foreach (var pass in cubemapPrefilterEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                device.SetRenderTarget(resultSpecularCubemapMip2, CubeMapFace.PositiveX + i);
                cubemapPrefilterEffect.Parameters["View"].SetValue(captureViews[i]);
                cubemapPrefilterEffect.Parameters["Projection"].SetValue(captureProjection);
                cubemapPrefilterEffect.Parameters["HDRImageTex"].SetValue(resultCubemap);
                cubemapPrefilterEffect.Parameters["roughness"].SetValue(0.5f);
                device.SetVertexBuffer(skyboxVertexBuffer);
                foreach (var pass in cubemapPrefilterEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                device.SetRenderTarget(resultSpecularCubemapMip3, CubeMapFace.PositiveX + i);
                cubemapPrefilterEffect.Parameters["View"].SetValue(captureViews[i]);
                cubemapPrefilterEffect.Parameters["Projection"].SetValue(captureProjection);
                cubemapPrefilterEffect.Parameters["HDRImageTex"].SetValue(resultCubemap);
                cubemapPrefilterEffect.Parameters["roughness"].SetValue(0.75f);
                device.SetVertexBuffer(skyboxVertexBuffer);
                foreach (var pass in cubemapPrefilterEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                device.SetRenderTarget(resultSpecularCubemapMip4, CubeMapFace.PositiveX + i);
                cubemapPrefilterEffect.Parameters["View"].SetValue(captureViews[i]);
                cubemapPrefilterEffect.Parameters["Projection"].SetValue(captureProjection);
                cubemapPrefilterEffect.Parameters["HDRImageTex"].SetValue(resultCubemap);
                cubemapPrefilterEffect.Parameters["roughness"].SetValue(1f);
                device.SetVertexBuffer(skyboxVertexBuffer);
                foreach (var pass in cubemapPrefilterEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
                }
            }

            device.SetRenderTarget(null);
            int width = resultSpecularCubemapMip1.Size;
            Vector4[] data = new Vector4[width * width];
            //skyboxTexture = new TextureCube(device, 128, false, SurfaceFormat.Color);
            resultSpecularCubemapMip1.GetData(CubeMapFace.PositiveX, data);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveX, 1, null, data, 0, width * width);
            resultSpecularCubemapMip1.GetData(CubeMapFace.PositiveY, data);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveY, 1, null, data, 0, width * width);
            resultSpecularCubemapMip1.GetData(CubeMapFace.PositiveZ, data);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveZ, 1, null, data, 0, width * width);
            resultSpecularCubemapMip1.GetData(CubeMapFace.NegativeX, data);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeX, 1, null, data, 0, width * width);
            resultSpecularCubemapMip1.GetData(CubeMapFace.NegativeY, data);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeY, 1, null, data, 0, width * width);
            resultSpecularCubemapMip1.GetData(CubeMapFace.NegativeZ, data);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeZ, 1, null, data, 0, width * width);

            int width1 = resultSpecularCubemapMip2.Size;

            Vector4[] data1 = new Vector4[width1 * width1];
            //skyboxTexture = new TextureCube(device, 128, false, SurfaceFormat.Color);
            resultSpecularCubemapMip2.GetData(CubeMapFace.PositiveX, data1);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveX, 2, null, data1, 0, width1 * width1);
            resultSpecularCubemapMip2.GetData(CubeMapFace.PositiveY, data1);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveY, 2, null, data1, 0, width1 * width1);
            resultSpecularCubemapMip2.GetData(CubeMapFace.PositiveZ, data1);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveZ, 2, null, data1, 0, width1 * width1);
            resultSpecularCubemapMip2.GetData(CubeMapFace.NegativeX, data1);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeX, 2, null, data1, 0, width1 * width1);
            resultSpecularCubemapMip2.GetData(CubeMapFace.NegativeY, data1);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeY, 2, null, data1, 0, width1 * width1);
            resultSpecularCubemapMip2.GetData(CubeMapFace.NegativeZ, data1);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeZ, 2, null, data1, 0, width1 * width1);


            int width2 = resultSpecularCubemapMip3.Size;

            Vector4[] data2 = new Vector4[width2 * width2];
            //skyboxTexture = new TextureCube(device, 128, false, SurfaceFormat.Color);
            resultSpecularCubemapMip3.GetData(CubeMapFace.PositiveX, data2);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveX, 3, null, data2, 0, width2 * width2);
            resultSpecularCubemapMip3.GetData(CubeMapFace.PositiveY, data2);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveY, 3, null, data2, 0, width2 * width2);
            resultSpecularCubemapMip3.GetData(CubeMapFace.PositiveZ, data2);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveZ, 3, null, data2, 0, width2 * width2);
            resultSpecularCubemapMip3.GetData(CubeMapFace.NegativeX, data2);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeX, 3, null, data2, 0, width2 * width2);
            resultSpecularCubemapMip3.GetData(CubeMapFace.NegativeY, data2);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeY, 3, null, data2, 0, width2 * width2);
            resultSpecularCubemapMip3.GetData(CubeMapFace.NegativeZ, data2);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeZ, 3, null, data2, 0, width2 * width2);

            int width3 = resultSpecularCubemapMip4.Size;

            Vector4[] data3 = new Vector4[width3 * width3];
            //skyboxTexture = new TextureCube(device, 128, false, SurfaceFormat.Color);
            resultSpecularCubemapMip4.GetData(CubeMapFace.PositiveX, data3);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveX, 4, null, data3, 0, width3 * width3);
            resultSpecularCubemapMip4.GetData(CubeMapFace.PositiveY, data3);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveY, 4, null, data3, 0, width3 * width3);
            resultSpecularCubemapMip4.GetData(CubeMapFace.PositiveZ, data3);
            resultSpecularCubemapMip0.SetData(CubeMapFace.PositiveZ, 4, null, data3, 0, width3 * width3);
            resultSpecularCubemapMip4.GetData(CubeMapFace.NegativeX, data3);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeX, 4, null, data3, 0, width3 * width3);
            resultSpecularCubemapMip4.GetData(CubeMapFace.NegativeY, data3);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeY, 4, null, data3, 0, width3 * width3);
            resultSpecularCubemapMip4.GetData(CubeMapFace.NegativeZ, data3);
            resultSpecularCubemapMip0.SetData(CubeMapFace.NegativeZ, 4, null, data3, 0, width3 * width3);
            device.DepthStencilState = DepthStencilState.Default;
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.CullCounterClockwiseFace;
            device.RasterizerState = rasterizerState1;

        }
    }
}
