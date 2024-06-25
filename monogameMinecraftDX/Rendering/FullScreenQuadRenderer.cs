using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace monogameMinecraftDX.Rendering
{
    public class FullScreenQuadRenderer
    {

        public static VertexPositionTexture[] quadVertices =
        {

            new VertexPositionTexture(new Vector3(-1.0f,  1.0f, 0.0f),new Vector2(  0.0f, 0.0f)),
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 0.0f),new Vector2(  0.0f, 1.0f)),
            new VertexPositionTexture(new Vector3(1.0f,  1.0f, 0.0f),new Vector2(1.0f, 1.0f)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f, 0.0f),new Vector2(1.0f, 0.0f))
         
        

            


       //     new VertexPositionTexture(new Vector3(-1.0f,  1.0f,0f),new Vector2( 0.0f, 1.0f)), 
             ,
        

        //    new VertexPositionTexture(new Vector3(1.0f, -1.0f,0f),new Vector2(1.0f, 0.0f)) ,  

        
        };
        public static bool isVertsInited = false;
        public static bool isQuadBuffersInited = false;

        public ushort[] quadIndices =
        {
                0, 1, 2,
                2, 3, 0
        };

        public static IndexBuffer quadIndexBuffer;

        public static VertexBuffer quadVertexBuffer;
        public void InitializeVertices()
        {
            if (isVertsInited == true) { return; }
            quadVertices = new VertexPositionTexture[4];

            quadVertices[0].Position = new Vector3(-1, 1, 0);
            quadVertices[0].TextureCoordinate = new Vector2(0, 0);

            quadVertices[1].Position = new Vector3(1, 1, 0);
            quadVertices[1].TextureCoordinate = new Vector2(1, 0);

            quadVertices[2].Position = new Vector3(1, -1, 0);
            quadVertices[2].TextureCoordinate = new Vector2(1, 1);

            quadVertices[3].Position = new Vector3(-1, -1, 0);
            quadVertices[3].TextureCoordinate = new Vector2(0, 1);
            isVertsInited = true;
        }
        public void InitializeQuadBuffers(GraphicsDevice device)
        {
            if (isQuadBuffersInited == true)
            {
                return;
            }
            quadVertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), 6, BufferUsage.None);

            quadVertexBuffer.SetData(quadVertices);
            quadIndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.None);
            quadIndexBuffer.SetData(quadIndices);
            isQuadBuffersInited = true;
        }

        static BoundingFrustum playerViewProjFrustum = new BoundingFrustum(Matrix.Identity * Matrix.Identity);
        public void SetCameraFrustum(Camera camera, Effect effect)
        {



            /*      Matrix view = camera.viewMatrixOrigin;
                  Matrix proj = camera.projectionMatrix;
                  Matrix vp = view * proj;

                  // 将camera view space 的平移置为0，用来计算world space下相对于相机的vector  
                  Matrix cview = view;
                  cview.Translation = new Vector3(0.0f, 0.0f, 0.0f);
                  Matrix cviewProj = cview * proj;

                  // 计算viewProj逆矩阵，即从裁剪空间变换到世界空间  
                  Matrix cviewProjInv = Matrix.Invert(cviewProj);
                 */
            var near = 0.1f;
            playerViewProjFrustum.Matrix = camera.viewMatrixOrigin * camera.projectionMatrix;
            Vector3[] corners = playerViewProjFrustum.GetCorners();
            Vector3 topLeftCorner = corners[0];
            Vector3 topRightCorner = corners[1];
            Vector3 bottomLeftCorner = corners[3];
            Vector3 cameraXExtent = topRightCorner - topLeftCorner;
            Vector3 cameraYExtent = bottomLeftCorner - topLeftCorner;

            //  Vector4 topLeftCorner1 = Vector4.Transform(new Vector4(-1.0f, 1.0f, -1, 1f), cviewProjInv);
            // Vector4 topRightCorner1 = Vector4.Transform(new Vector4(1.0f, 1.0f, -1, 1f), cviewProjInv);
            //  Vector4 bottomLeftCorner1 = Vector4.Transform(new Vector4(-1.0f, -1.0f, -1, 1f), cviewProjInv);

            // 计算相机近平面上方向向量
            //   Vector4 cameraXExtent1 = topRightCorner1 - topLeftCorner1;
            //   Vector4 cameraYExtent1 = bottomLeftCorner1 - topLeftCorner1;
            //      Debug.WriteLine("corners:"+(corners[0] - camera.position)+" "+ (corners[1] - camera.position) + " " + (corners[2] - camera.position) + " " + (corners[3] - camera.position));
            //      Debug.WriteLine("corners1:" + (topLeftCorner1) + " " + (topRightCorner1) + " " + (corners[2] - camera.position) + " " + (bottomLeftCorner1));
            effect.Parameters["ProjectionParams2"]?.SetValue(new Vector4(1.0f / near, camera.position.X, camera.position.Y, camera.position.Z));
            effect.Parameters["CameraViewTopLeftCorner"]?.SetValue(topLeftCorner);
            effect.Parameters["CameraViewXExtent"]?.SetValue(cameraXExtent);
            effect.Parameters["CameraViewYExtent"]?.SetValue(cameraYExtent);
            effect.Parameters["CameraPos"]?.SetValue(camera.position);
        }
        public void RenderQuad(GraphicsDevice device, RenderTarget2D target, Effect quadEffect, bool isPureWhite = false, bool isRenderingOnDcreen = false, bool clearColor = true)
        {
            if (isRenderingOnDcreen == false)
            {
                device.SetRenderTarget(target);
                if (clearColor == true)
                {
                    device.Clear(Color.Transparent);
                }

            }
            if (isPureWhite)
            {

                device.Clear(Color.White);
                device.SetRenderTarget(null);
                device.Clear(Color.CornflowerBlue);
                return;
            }

            device.SetVertexBuffer(quadVertexBuffer);
            device.Indices = quadIndexBuffer;
            //     RasterizerState rasterizerState = new RasterizerState();
            //  rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;
            device.BlendState = BlendState.AlphaBlend;
            foreach (var pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4);
            }
            //    graphicsDevice.Clear(Color.White);
            if (isRenderingOnDcreen == false)
            {
                device.SetRenderTarget(null);
                device.Clear(Color.CornflowerBlue);
            }

        }
        public static RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None };

        public void RenderQuad(GraphicsDevice device, RenderTarget2D target, RenderTarget2D target2, Effect quadEffect, bool clearColor = true)
        {


            device.SetRenderTargets(target, target2);
            if (clearColor == true)
            {
                device.Clear(Color.Transparent);
            }






            device.SetVertexBuffer(quadVertexBuffer);
            device.Indices = quadIndexBuffer;

            device.RasterizerState = rasterizerState;
            device.BlendState = BlendState.AlphaBlend;
            foreach (var pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4);
            }
            //    graphicsDevice.Clear(Color.White);

            device.SetRenderTarget(null);
            device.Clear(Color.CornflowerBlue);


        }
        public void RenderQuadPureColor(GraphicsDevice device, RenderTarget2D target, Color color)
        {

            device.SetRenderTarget(target);
            device.Clear(color);



            device.SetRenderTarget(null);
            device.Clear(Color.CornflowerBlue);
            return;




        }
    }
}
