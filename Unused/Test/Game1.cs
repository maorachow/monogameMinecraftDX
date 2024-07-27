using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Diagnostics;
using monogameMinecraftDX.Animations;
using monogameMinecraftDX.Updateables;
namespace monogameMinecraftDX.Test
{


    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D faceTex;
        public Model model;
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        //  private Matrix view = Matrix.CreateLookAt(new Vector3(3, 0, 1), new Vector3(0, 0, 0), -Vector3.UnitY);
        //    private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 480f, 0.1f, 100f);
        public GamePlayer gamePlayer;

        public BasicEffect basicEffect;
        public VertexPositionTexture[] triangleVertices;
        public VertexMatrix4x4Scale[] instancingTransforms;
        public VertexBuffer instancingBuffer;
        public VertexBufferBinding instancingBinding;
        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;
        public Animation animation;
        public AnimationState animationState;
        public Effect billboardEffect;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.SynchronizeWithVerticalRetrace = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            Window.ClientSizeChanged += OnResize;
         //      Window.KeyDown += OnResize;
            //        TargetElapsedTime = System.TimeSpan.FromMilliseconds(0.1);
            IsFixedTimeStep = false;
        }

    /*    private void OnResize(object sender, InputKeyEventArgs e)
        {
            gamePlayer.cam.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 480f, 0.1f, 100f);
            Debug.WriteLine(Window.ClientBounds.Size.X + " " + Window.ClientBounds.Size.Y);
        }*/

        void OnResize(Object sender, EventArgs e)
        {
            gamePlayer.cam.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)GraphicsDevice.Viewport.Width/GraphicsDevice.Viewport.Height, 0.1f, 100f);
            Debug.WriteLine(GraphicsDevice.Viewport.Width + " " + GraphicsDevice.Viewport.Height);
        }
        protected override void Initialize()
        {
            gamePlayer = new GamePlayer(new Vector3(0, 0, 0), new Vector3(1, 2, 1), this);
            // TODO: Add your initialization logic here
            triangleVertices = new VertexPositionTexture[4];
       /*     triangleVertices[0] = new VertexPositionTexture(new Vector3(-0.5f,-0.5f, 0), new Vector2(0.5f, 1f));
            triangleVertices[1] = new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(0f, 0f));
            triangleVertices[2] = new VertexPositionTexture(new Vector3(-0.5f,0.5f, 0), new Vector2(1f, 0f));
            triangleVertices[3] = new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(1f, 0f));*/
            triangleVertices[0].Position = new Vector3(-0.5f, 0.5f, 0);
         

            triangleVertices[1].Position = new Vector3(0.5f, 0.5f, 0);
        

            triangleVertices[2].Position = new Vector3(0.5f, -0.5f, 0);
      

            triangleVertices[3].Position = new Vector3(-0.5f, -0.5f, 0);
        
            ushort[] indicesArray = new ushort[]
            {
                0, 1, 2,
                2, 3, 0
            };
            instancingTransforms = new VertexMatrix4x4Scale[3000000];
            Random rand=new Random();
            for (int i=0;i<instancingTransforms.Length;i++)
            {
                Matrix trans = Matrix.CreateTranslation((rand.NextSingle() * 2f - 1f)*100f, (rand.NextSingle() * 2f - 1f) *100f,
                    (rand.NextSingle() * 2f - 1f) * 100f);
             //   Debug.WriteLine(trans);
                instancingTransforms[i] = new VertexMatrix4x4Scale(new Vector4(trans.M11, trans.M12, trans.M13, trans.M14),
                    new Vector4(trans.M21, trans.M22, trans.M23, trans.M24),
                    new Vector4(trans.M31, trans.M32, trans.M33, trans.M34),
                    new Vector4(trans.M41, trans.M42, trans.M43, trans.M44),new Vector4(rand.NextSingle(), rand.NextSingle(), rand.NextSingle(), rand.NextSingle()),rand.NextSingle() *1f);
            }

            foreach (var VARIABLE in instancingTransforms)
            {
         //       Debug.WriteLine(VARIABLE.row3);
            }
            instancingBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexMatrix4x4Scale), instancingTransforms.Length,
                BufferUsage.WriteOnly);
            instancingBuffer.SetData(instancingTransforms);
            //Vert buffer
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTexture>(triangleVertices);
            indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            indexBuffer.SetData<ushort>(indicesArray);
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1f;

            // Want to see the colors of the vertices, this needs to 
            //   be on

            //Lighting requires normal information which 
            //  VertexPositionColor does not have
            //If you want to use lighting and VPC you need to create a 
            // custom def
            basicEffect.LightingEnabled = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            faceTex = this.Content.Load<Texture2D>("awesomeface");
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = faceTex;
            model = Content.Load<Model>("zombiefbx");
          
            billboardEffect = Content.Load<Effect>("testbillboardeffect");
            animation = new Animation(null, true);
            animation.steps = new System.Collections.Generic.List<AnimationStep> {
                new AnimationStep(new System.Collections.Generic.Dictionary<string, AnimationTransformation> {
                    { "body", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 40f, 0f), new Vector3(1f, 1f, 1f)) },
                    { "leftArm", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 40f, 0f), new Vector3(1f, 1f, 1f)) },
                    { "leftLeg",new AnimationTransformation(new Vector3(0f,0.1f,0f),new Vector3(0f, 40f, 0f), new Vector3(1f, 1f, 1f)) }
                }, 0.5f),
                new AnimationStep(new System.Collections.Generic.Dictionary<string, AnimationTransformation> {
                    { "leftArm", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -40f, 0f),  new Vector3(1f, 1f, 1f)) },
                    { "leftLeg", new AnimationTransformation(new Vector3(0f, -0.1f, 0f),new Vector3(0f, -40f, 0f), new Vector3(1f, 1f, 1f)) },
                    { "body", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -40f, 0f), new Vector3(1f, 1f, 1f)) } }, 0.5f) };
            animationState = new AnimationState(animation, model);
            // TODO: use this.Content to load your game content here
        }
        int lastMouseX;
        int lastMouseY;
        float prevFPS = 0f;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            var kState = Keyboard.GetState();
            Vector2 playerVec = new Vector2(0f, 0f);
            if (kState.IsKeyDown(Keys.A))
            {
                playerVec.Y = -1f;
            }

            if (kState.IsKeyDown(Keys.D))
            {
                playerVec.Y = 1f;
            }

            if (kState.IsKeyDown(Keys.W))
            {
                playerVec.X = 1f;
            }

            if (kState.IsKeyDown(Keys.S))
            {
                playerVec.X = -1f;
            }
            animationState.Update((float)gameTime.ElapsedGameTime.TotalSeconds, 1, out _, out _);
            gamePlayer.cam.updateCameraVectors();
            var mState = Mouse.GetState();
            gamePlayer.cam.ProcessMouseMovement(mState.X - lastMouseX, lastMouseY - mState.Y);
            lastMouseY = mState.Y;
            lastMouseX = mState.X;
            gamePlayer.cam.position += (playerVec.X * gamePlayer.cam.horizontalFront + playerVec.Y * gamePlayer.cam.horizontalRight) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float curFps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            float deltaFps = Math.Abs(curFps - prevFPS);
         //   Window.Title = deltaFps < 20f ? deltaFps.ToString() : "delta fps more than 20";
            prevFPS = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            //     Debug.WriteLine(gamePlayer.position);
            //     Debug.WriteLine(gamePlayer.cam.Pitch+" "+ gamePlayer.cam.Yaw);
            //    Debug.WriteLine(gamePlayer.cam.position + " " + gamePlayer.cam.front+" "+gamePlayer.cam.up);
            base.Update(gameTime);

        }

        /*  public void DrawModel1(Model model, Matrix world, Matrix view, Matrix projection)
          {
              Matrix[] sharedDrawBoneMatrices = new Matrix[model.Bones.Count];
              Draw(world, view, projection);
              void Draw(Matrix world, Matrix view, Matrix projection)
              {
                  int count = model.Bones.Count;
                  if (sharedDrawBoneMatrices == null || sharedDrawBoneMatrices.Length < count)
                  {
                      sharedDrawBoneMatrices = new Matrix[count];
                  }

                  CopyAbsoluteBoneTransformsTo(sharedDrawBoneMatrices);
                  foreach (ModelMesh mesh in model.Meshes)
                  {
                      foreach (Effect effect in mesh.Effects)
                      {
                          IEffectMatrices obj = (effect as IEffectMatrices) ?? throw new InvalidOperationException();
                          obj.World = sharedDrawBoneMatrices[mesh.ParentBone.Index] * world;
                          obj.View = view;
                          obj.Projection = projection;
                      }

                      mesh.Draw();
                  }
              }

              void CopyAbsoluteBoneTransformsTo(Matrix[] destinationBoneTransforms)
              {
                  if (destinationBoneTransforms == null)
                  {
                      throw new ArgumentNullException("destinationBoneTransforms");
                  }

                  if (destinationBoneTransforms.Length < model.Bones.Count)
                  {
                      throw new ArgumentOutOfRangeException("destinationBoneTransforms");
                  }

                  int count = model.Bones.Count;
                  for (int i = 0; i < count; i++)
                  {
                      ModelBone modelBone = model.Bones[i];
                      Matrix localTrans;
                      if (animationState.GetBoneTransformLocal(modelBone.Name)!=null)
                      {
                          localTrans = animationState.GetBoneTransformLocal(modelBone.Name).ToMatrix();
                      }
                      else
                      {
                          localTrans = AnimationTransformation.Identity.ToMatrix();
                      }


                      if (modelBone.Parent == null)
                      {
                          destinationBoneTransforms[i] = localTrans*modelBone.Transform;
                          continue;
                      }

                      int index = modelBone.Parent.Index;
                      destinationBoneTransforms[i]=Matrix.Multiply(localTrans*modelBone.Transform,  destinationBoneTransforms[index] );
                  }
              }

          }*/
        /*  private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
          {
              int count = model.Bones.Count;
              animationState.curStep.GetBone("");
              Matrix[] sharedDrawBoneMatrices=animationState.GetAllBoneTransforms();
              if (sharedDrawBoneMatrices == null)
              {
                  return;
              }

              foreach (ModelMesh mesh in model.Meshes)
              {
                  foreach (Effect effect in mesh.Effects)
                  {
                      IEffectMatrices obj = (effect as IEffectMatrices) ?? throw new InvalidOperationException();
                      obj.World = sharedDrawBoneMatrices[mesh.ParentBone.Index] * world;
                      obj.View = view;
                      obj.Projection = projection;
                  }

                  mesh.Draw();
              }
          }
        */
        RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None};
        protected override void Draw(GameTime gameTime)
        {
            gamePlayer.cam.updateCameraVectors();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(instancingBuffer, 0, 1),new VertexBufferBinding(vertexBuffer,0));
            GraphicsDevice.Indices = indexBuffer;
            // TODO: Add your drawing code here
          
           
            
            GraphicsDevice.RasterizerState = rasterizerState;
            //     basicEffect.Projection = gamePlayer.cam.projectionMatrix;
            //   basicEffect.View = view;
            //     basicEffect.World = world;
            billboardEffect.Parameters["World"]? .SetValue(world);
            billboardEffect.Parameters["View"].SetValue(gamePlayer.cam.viewMatrix);
            billboardEffect.Parameters["Projection"].SetValue(gamePlayer.cam.projectionMatrix);
            billboardEffect.Parameters["Texture"]?.SetValue(faceTex);
            foreach (EffectPass pass in billboardEffect.CurrentTechnique.Passes)

            {
                pass.Apply();
                GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList,0, 0, 6, 3000000);

            }
            //  DrawModel1(model, world, view, gamePlayer.cam.projectionMatrix);
            animationState.DrawAnimatedModel(world, gamePlayer.cam.viewMatrix, gamePlayer.cam.projectionMatrix);
            base.Draw(gameTime);
        }
    }

    public struct VertexMatrix4x4Scale : IVertexType
    {
        public Vector4 row0;
        public Vector4 row1;
        public Vector4 row2;
        public Vector4 row3;
        public Vector4 uvWidthCorner;
        public float scale;
        public static readonly VertexDeclaration VertexDeclaration;
        public VertexMatrix4x4Scale(Vector4 r0, Vector4 r1, Vector4 r2, Vector4 r3,Vector4 uvWidthCorner,float scale )
        {
            row0 = r0;
            row1 = r1;
            row2 = r2;
            row3 = r3;
            this.uvWidthCorner= uvWidthCorner;
            this.scale = scale;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        static VertexMatrix4x4Scale()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(sizeof(float)*4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate,3),
                new VertexElement(sizeof(float)*8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate,4),
                new VertexElement(sizeof(float)*12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5),
                new VertexElement(sizeof(float)*16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 6),
                new VertexElement(sizeof(float)*20, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 7)
            };
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }
}