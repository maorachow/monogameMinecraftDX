using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Rendering.Particle;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

 
   

    namespace monogameMinecraftShared.Rendering
    {
        public class VeryLowDefRenderPipelineManager : IRenderPipelineManager
        {
            public MinecraftGameBase game { get; set; }
            public IEffectsManager effectsManager { get; set; }
            public WalkablePathVisualizationRenderer walkablePathVisualizationRenderer { get; set; }
            public BoundingBoxVisualizationRenderer boundingBoxVisualizationRenderer { get; set; }
            public ChunkRenderer chunkRenderer { get; set; }
            public ParticleRenderer particleRenderer { get; set; }
            public Texture2D environmentHDRITex;
            public Texture2D environmentHDRITexNight;

            public SkyboxRenderer skyboxRenderer;
            public GBufferRenderer gBufferRenderer;
            public EntityRenderer entityRenderer;
            public TerrainMipmapGenerator terrainMipmapGenerator;
            public HDRCubemapRendererLowDef hdrCubemapRenderer;
            public DeferredShadingRendererLowDef deferredShadingRendererLowDef;
            public SSAORenderer ssaoRenderer;

            public Effect chunkForwardEffect;
            public Effect entityForwardEffect;
        public VeryLowDefRenderPipelineManager(MinecraftGameBase game, IEffectsManager em)
            {
                this.game = game;
                effectsManager = em;
            }
            public void InitRenderPipeline()
            {
                environmentHDRITex = game.Content.Load<Texture2D>("environmenthdri");
                environmentHDRITexNight = game.Content.Load<Texture2D>("environmenthdrinight");
                chunkForwardEffect = effectsManager.gameEffects["blockforwardeffect"];
                entityForwardEffect = effectsManager.gameEffects["entityforwardeffect"];
            terrainMipmapGenerator = new TerrainMipmapGenerator(game.GraphicsDevice, effectsManager.gameEffects["texturecopyeffect"]);
                /*     brdfLUTRenderer = new BRDFLUTRenderer(game.GraphicsDevice, effectsManager.gameEffects["brdfluteffect"]);
                     brdfLUTRenderer.CalculateLUT();*/
                hdrCubemapRenderer = new HDRCubemapRendererLowDef(game.GraphicsDevice, effectsManager.gameEffects["hdricubeeffect"], environmentHDRITex, environmentHDRITexNight);
                hdrCubemapRenderer.Render(hdrCubemapRenderer.resultCubeCollection, 0);
                hdrCubemapRenderer.Render(hdrCubemapRenderer.resultCubeCollectionNight, 1);

                chunkRenderer = new ChunkRenderer(game, game.GraphicsDevice, null, null, game.gameTimeManager);

                //    chunkRenderer.SetTexture(terrainTexNoMip, terrainNormal, terrainDepth, terrainTexNoMip, terrainMER);

                BlockResourcesManager.LoadDefaultResources(game.Content, game.GraphicsDevice, chunkRenderer);
                /* gBufferEffect = game.Content.Load<Effect>("gbuffereffect");
                 gBufferEntityEffect = game.Content.Load<Effect>("gbufferentityeffect");*/
                particleRenderer = new ParticleRenderer(chunkRenderer.atlas, chunkRenderer.atlasNormal, chunkRenderer.atlasMER, game.GraphicsDevice,
                  null, game.gamePlayer,false);
                BlockResourcesManager.LoadDefaultParticleResources(game.Content, game.GraphicsDevice, particleRenderer);
                entityRenderer = new EntityRenderer( game.GraphicsDevice, game.gamePlayer, null, game.Content.Load<Model>("zombiefbx"), game.Content.Load<Texture2D>("husk"), game.Content.Load<Model>("zombiemodelref"), null, null, game.gameTimeManager, game.Content.Load<Model>("playermodel"), game.Content.Load<Texture2D>("steve"));
                gBufferRenderer = new GBufferRenderer(game.GraphicsDevice, effectsManager.gameEffects["gbuffereffect"], effectsManager.gameEffects["gbufferentityeffect"], game.gamePlayer, chunkRenderer, entityRenderer, particleRenderer);
                skyboxRenderer = new SkyboxRenderer(game.GraphicsDevice, effectsManager.gameEffects["skyboxeffect"], null, game.gamePlayer, game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skyboxup"), game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skyboxdown"), game.Content.Load<Texture2D>("skybox/skybox"),
                   game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnightup"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnightdown"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.gameTimeManager
                   );
                skyboxRenderer.skyboxTexture = hdrCubemapRenderer.resultCubeCollection;
                skyboxRenderer.skyboxTextureNight = hdrCubemapRenderer.resultCubeCollectionNight;
                ssaoRenderer = new SSAORenderer(effectsManager.gameEffects["ssaoeffect"], gBufferRenderer, chunkRenderer, game.GraphicsDevice, game.gamePlayer, game.Content.Load<Texture2D>("randomnormal"));
                /*        contactShadowRenderer = new ContactShadowRenderer(game.GraphicsDevice, effectsManager.gameEffects["contactshadoweffect"], gBufferRenderer, game.gameTimeManager, game.gamePlayer);
                        shadowRenderer = new ShadowRenderer(game, game.GraphicsDevice, effectsManager.gameEffects["createshadowmapeffect"], chunkRenderer, entityRenderer, game.gameTimeManager);
                        motionVectorRenderer = new MotionVectorRenderer(game.GraphicsDevice, effectsManager.gameEffects["motionvectoreffect"], gBufferRenderer, game.gamePlayer);
                        ssaoRenderer = new SSAORenderer(effectsManager.gameEffects["ssaoeffect"], gBufferRenderer, chunkRenderer, game.GraphicsDevice, game.gamePlayer, game.Content.Load<Texture2D>("randomnormal"));
                        fxaaRenderer = new FXAARenderer(game.GraphicsDevice, effectsManager.gameEffects["fxaaeffect"]);
                        motionBlurRenderer = new MotionBlurRenderer(game.GraphicsDevice, effectsManager.gameEffects["motionblureffect"], motionVectorRenderer);*/
                deferredShadingRendererLowDef = new DeferredShadingRendererLowDef(game.GraphicsDevice, effectsManager.gameEffects["deferredblockeffect"], ssaoRenderer, game.gameTimeManager, gBufferRenderer, skyboxRenderer);


                /*       customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess0"));
                       customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess1"));
                       customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess2"));
                       customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess3"));
                       effectsManager.LoadCustomPostProcessEffects(game.GraphicsDevice, customPostProcessors, game.Content);
                       hiZBufferRenderer = new HiZBufferRenderer(game.GraphicsDevice, effectsManager.gameEffects["hizbuffereffect"], gBufferRenderer, effectsManager.gameEffects["texturecopyraweffect"]);
                       ssrRenderer = new SSRRenderer(game.GraphicsDevice, game.gamePlayer, gBufferRenderer, effectsManager.gameEffects["ssreffect"], deferredShadingRenderer, effectsManager.gameEffects["texturecopyraweffect"], motionVectorRenderer, hiZBufferRenderer, hdrCubemapRenderer, game.gameTimeManager);
                       ssidRenderer = new SSIDRenderer(game.GraphicsDevice, effectsManager.gameEffects["ssideffect"], gBufferRenderer, game.gamePlayer, deferredShadingRenderer, effectsManager.gameEffects["texturecopyraweffect"], motionVectorRenderer, hiZBufferRenderer, hdrCubemapRenderer, game.gameTimeManager);

                       deferredShadingRenderer.customPostProcessors = customPostProcessors;
                       deferredShadingRenderer.ssidRenderer = ssidRenderer;
                       deferredShadingRenderer.ssrRenderer = ssrRenderer;
                       chunkRenderer.shadowRenderer = shadowRenderer;
                       chunkRenderer.SSAORenderer = ssaoRenderer;
                       entityRenderer.shadowRenderer = shadowRenderer;
                       chunkRenderer.lightUpdater = pointLightUpdater;
                       shadowRenderer.zombieModel = game.Content.Load<Model>("zombiemodelref");

                       volumetricLightRenderer = new VolumetricLightRenderer(game.GraphicsDevice, gBufferRenderer, game._spriteBatch, effectsManager.gameEffects["volumetricmaskblendeffect"], effectsManager.gameEffects["lightshafteffect"], game.gamePlayer, game.gameTimeManager);
                       chunkRenderer.SSRRenderer = ssrRenderer;
                       volumetricLightRenderer.entityRenderer = entityRenderer;*/
                boundingBoxVisualizationRenderer = new BoundingBoxVisualizationRenderer();
                boundingBoxVisualizationRenderer.Initialize(environmentHDRITex, game.GraphicsDevice, effectsManager.gameEffects["debuglineeffect"], gBufferRenderer);
                walkablePathVisualizationRenderer = new WalkablePathVisualizationRenderer();
                walkablePathVisualizationRenderer.Initialize(environmentHDRITex, game.GraphicsDevice, effectsManager.gameEffects["debuglineeffect"], gBufferRenderer);
            }

            public void RenderWorld(GameTime gameTime, SpriteBatch sb)
            {

                game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                //  GraphicsDevice.RasterizerState = rasterizerState;
                game.GraphicsDevice.BlendState = BlendState.Opaque;
             //   gBufferRenderer.Draw();
             //   ssaoRenderer.Draw();

              //  deferredShadingRendererLowDef.Draw(game.gamePlayer, game._spriteBatch);
              chunkRenderer.RenderAllChunksLowDefForward(VoxelWorld.currentWorld.chunks,game.gamePlayer, chunkForwardEffect);
              entityRenderer.DrawLowDefForward(entityForwardEffect);
                game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                game.GraphicsDevice.BlendState = BlendState.Additive;

                if (VoxelWorld.currentWorld.structureOperationsManager != null)
                {
                    VoxelWorld.currentWorld.structureOperationsManager.DrawStructureSavingBounds(game.gamePlayer, this);
                    VoxelWorld.currentWorld.structureOperationsManager.DrawStructurePlacingBounds(game.gamePlayer, this);
                }

                if (game.gamePlayer.curChunk != null)
                {
                    EntityManager.pathfindingManager.DrawDebuggingPath(new Vector3(0, 0, 0), game.gamePlayer, this);

                }

            }

            public void Resize()
            {
                int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
                Debug.WriteLine(width);
                Debug.WriteLine(height);
                gBufferRenderer.Resize(width, height, game.GraphicsDevice);



                ssaoRenderer.ssaoTarget = new RenderTarget2D(ssaoRenderer.graphicsDevice, width / 2, height / 2, false, SurfaceFormat.Color, DepthFormat.Depth24);


                deferredShadingRendererLowDef.finalImage = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);

                float aspectRatio = game.GraphicsDevice.Viewport.Width / (float)game.GraphicsDevice.Viewport.Height;
                game.gamePlayer.cam.aspectRatio = aspectRatio;
                game.gamePlayer.cam.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), aspectRatio, 0.1f, 1000f);
            }
        }
    }


