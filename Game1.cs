using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Mime;
using System.Reflection;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;
using System.Diagnostics;
 
using monogameMinecraftDX;
namespace monogameMinecraft
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
        public VertexBuffer vertexBuffer;
        public Animation animation;
        public AnimationState animationState;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            Window.ClientSizeChanged += OnResize;
            //   Window.KeyDown += OnResize;
         //   TargetElapsedTime = System.TimeSpan.FromMilliseconds(33);
        }

        private void OnResize(object sender, InputKeyEventArgs e)
        {
            gamePlayer.cam.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 480f, 0.1f, 100f);
            Debug.WriteLine(Window.ClientBounds.Size.X + " " + Window.ClientBounds.Size.Y);
        }

        void OnResize(Object sender, EventArgs e)
        {
            gamePlayer.cam.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.DisplayMode.AspectRatio, 0.1f, 100f);
            Debug.WriteLine(GraphicsDevice.Viewport.Width + " " + GraphicsDevice.Viewport.Height);
        }
        protected override void Initialize()
        { 
            gamePlayer=new GamePlayer(new Vector3(10,0,0),new Vector3(11,2,1),this);
            // TODO: Add your initialization logic here
            triangleVertices = new VertexPositionTexture[3];
            triangleVertices[0] = new VertexPositionTexture(new Vector3(0, 20, 0), new Vector2(0.5f,1f));
            triangleVertices[1] = new VertexPositionTexture(new Vector3(-20, -20, 0), new Vector2(0f, 0f));
            triangleVertices[2] = new VertexPositionTexture(new Vector3(20, -20, 0), new Vector2(1f, 0f));

            //Vert buffer
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTexture>(triangleVertices);

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

            animation = new Animation( null,true);
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
            animationState =new AnimationState(animation,model);
            // TODO: use this.Content to load your game content here
        }
        int lastMouseX;
        int lastMouseY;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            var kState=Keyboard.GetState();
            Vector2 playerVec=new Vector2(0f,0f);
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
            animationState.Update((float)gameTime.ElapsedGameTime.TotalSeconds,1,out _,out _);
            gamePlayer.cam.updateCameraVectors();
            var mState=Mouse.GetState();
            gamePlayer.cam.ProcessMouseMovement( mState.X-lastMouseX ,lastMouseY-mState.Y);
            lastMouseY = mState.Y;
            lastMouseX= mState.X;
            gamePlayer.cam.position +=(playerVec.X*gamePlayer.cam.horizontalFront+playerVec.Y * gamePlayer.cam.horizontalRight) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //     Debug.WriteLine(gamePlayer.playerPos);
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
        protected override void Draw(GameTime gameTime)
        {
            gamePlayer.cam.updateCameraVectors();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
        
            // TODO: Add your drawing code here
            Matrix view = gamePlayer.cam.viewMatrix;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            basicEffect.Projection = gamePlayer.cam.projectionMatrix;
            basicEffect.View = view;
            basicEffect.World = world;
            
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                 
            {
                pass.Apply();
               GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);
                                              
            }
            //  DrawModel1(model, world, view, gamePlayer.cam.projectionMatrix);
            animationState.DrawAnimatedModel(world, view, gamePlayer.cam.projectionMatrix);
            base.Draw(gameTime);
        }
    }
}