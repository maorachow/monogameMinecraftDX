using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftNetworking.Client.Rendering.Particle;
using monogameMinecraftNetworking.Client.UI;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftShared;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Rendering.Particle;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Client.Rendering
{
    public class HighDefNetworkingRenderPipelineManager:INetworkClientRenderPipelineManager
    {
        //public EntityRenderer entityRenderer;
       
        public ShadowRenderer shadowRenderer;
        public SSAORenderer ssaoRenderer;
        public SkyboxRenderer skyboxRenderer;
        public GBufferRenderer gBufferRenderer;
        public SSRRenderer ssrRenderer;
        public TextureCube skyboxTex;
        public VolumetricLightRenderer volumetricLightRenderer;

        public PointLightUpdater pointLightUpdater;
        public ContactShadowRenderer contactShadowRenderer;
        public SSIDRenderer ssidRenderer;
        public DeferredShadingRenderer deferredShadingRenderer;
        public RandomTextureGenerator randomTextureGenerator;
        public GlobalMaterialParamsManager globalMaterialParamsManager;
        public BRDFLUTRenderer brdfLUTRenderer;
        public MotionVectorRenderer motionVectorRenderer;
        public TerrainMipmapGenerator terrainMipmapGenerator;
        public FXAARenderer fxaaRenderer;
        public HiZBufferRenderer hiZBufferRenderer;
        public MotionBlurRenderer motionBlurRenderer;
        public HDRCubemapRenderer hdrCubemapRenderer;
       
        public ClientGameBase game { get; set; }
        public IEffectsManager effectsManager { get; set; }
        public List<CustomPostProcessor> customPostProcessors = new List<CustomPostProcessor>();

        public Texture2D particleAtlas;
        public Texture2D environmentHDRITex;
        public Texture2D environmentHDRITexNight;
        public BoundingBoxVisualizationRenderer boundingBoxVisualizationRenderer { get; set; }
        public WalkablePathVisualizationRenderer walkablePathVisualizationRenderer { get; set; }
      public  ChunkRenderer chunkRenderer { get; set; }
      public ParticleRenderer particleRenderer { get; set; }
      public ClientSidePlayersRenderer clientSidePlayersRenderer;
      public ClientSideEntitiesRenderer clientSideEntitiesRenderer;
        public HighDefNetworkingRenderPipelineManager(ClientGameBase game, IEffectsManager em)
        {
            this.game = game;
            effectsManager = em;
        }

        public void InitRenderPipeline()
        {

            environmentHDRITex = game.Content.Load<Texture2D>("environmenthdri");
            environmentHDRITexNight = game.Content.Load<Texture2D>("environmenthdrinight");
            terrainMipmapGenerator = new TerrainMipmapGenerator(game.GraphicsDevice, effectsManager.gameEffects["texturecopyeffect"]);
            brdfLUTRenderer = new BRDFLUTRenderer(game.GraphicsDevice, effectsManager.gameEffects["brdfluteffect"]);
            brdfLUTRenderer.CalculateLUT();
            hdrCubemapRenderer = new HDRCubemapRenderer(game.GraphicsDevice, effectsManager.gameEffects["hdricubeeffect"], environmentHDRITex, effectsManager.gameEffects["hdriirradianceeffect"], effectsManager.gameEffects["hdriprefiltereffect"], environmentHDRITexNight);
            hdrCubemapRenderer.Render(hdrCubemapRenderer.resultCubeCollection, 0);
            hdrCubemapRenderer.Render(hdrCubemapRenderer.resultCubeCollectionNight, 1);

            chunkRenderer = new ChunkRenderer(game, game.GraphicsDevice, effectsManager.gameEffects["blockforwardeffect"], null, game.gameTimeManager);
            pointLightUpdater = new PointLightUpdater(game.gamePlayerR.gamePlayer);
            //    chunkRenderer.SetTexture(terrainTexNoMip, terrainNormal, terrainDepth, terrainTexNoMip, terrainMER);

            BlockResourcesManager.LoadDefaultResources(game.Content, game.GraphicsDevice, chunkRenderer);
            /* gBufferEffect = game.Content.Load<Effect>("gbuffereffect");
             gBufferEntityEffect = game.Content.Load<Effect>("gbufferentityeffect");*/
            particleRenderer = new ClientSideParticleRenderer(chunkRenderer.atlas, chunkRenderer.atlasNormal, chunkRenderer.atlasMER, game.GraphicsDevice,
                effectsManager.gameEffects["gbufferparticleeffect"], game.gamePlayerR.gamePlayer,true);
            BlockResourcesManager.LoadDefaultParticleResources(game.Content, game.GraphicsDevice, particleRenderer);


            clientSidePlayersRenderer = new ClientSidePlayersRenderer(game.Content.Load<Model>("playermodel"),
                effectsManager.gameEffects["gbufferentityeffect"], game.gamePlayerR.gamePlayer,
                game.Content.Load<Texture2D>("steve"), game.networkingClient, game.GraphicsDevice, game._spriteBatch, MultiplayerClientUIUtility.sf, game);
            clientSideEntitiesRenderer = new ClientSideEntitiesRenderer(game.Content.Load<Model>("zombiefbx"), game.Content.Load<Model>("pigfbx"),
                effectsManager.gameEffects["gbufferentityeffect"], game.gamePlayerR.gamePlayer,
                game.Content.Load<Texture2D>("husk"), game.Content.Load<Texture2D>("pig"),game.networkingClient, game.GraphicsDevice, game);
            //   entityRenderer = new EntityRenderer(game.GraphicsDevice, game.gamePlayerR.gamePlayer, effectsManager.gameEffects["entityeffect"], game.Content.Load<Model>("zombiefbx"), game.Content.Load<Texture2D>("husk"), game.Content.Load<Model>("zombiemodelref"), effectsManager.gameEffects["createshadowmapeffect"], null, game.gameTimeManager, game.Content.Load<Model>("playermodel"), game.Content.Load<Texture2D>("steve"));
            gBufferRenderer = new GBufferRenderer(game.GraphicsDevice, effectsManager.gameEffects["gbuffereffect"], effectsManager.gameEffects["gbufferentityeffect"], effectsManager.gameEffects["gbufferdepthpeelingeffect"], game.gamePlayerR.gamePlayer, chunkRenderer, null, particleRenderer, true, clientSidePlayersRenderer, clientSideEntitiesRenderer);
            skyboxRenderer = new SkyboxRenderer(game.GraphicsDevice, effectsManager.gameEffects["skyboxeffect"], null, game.gamePlayerR.gamePlayer, game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skyboxup"), game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skyboxdown"), game.Content.Load<Texture2D>("skybox/skybox"),
               game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnightup"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnightdown"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.gameTimeManager
               );
            skyboxRenderer.skyboxTexture = hdrCubemapRenderer.resultCubeCollection.resultSpecularCubemapMip0;
            skyboxRenderer.skyboxTextureNight = hdrCubemapRenderer.resultCubeCollectionNight.resultSpecularCubemapMip0;
            contactShadowRenderer = new ContactShadowRenderer(game.GraphicsDevice, effectsManager.gameEffects["contactshadoweffect"], gBufferRenderer, game.gameTimeManager, game.gamePlayerR.gamePlayer);
            shadowRenderer = new ShadowRenderer(game.gamePlayerR, game.GraphicsDevice, effectsManager.gameEffects["createshadowmapeffect"], chunkRenderer, null, game.gameTimeManager, clientSidePlayersRenderer, clientSideEntitiesRenderer);
            motionVectorRenderer = new MotionVectorRenderer(game.GraphicsDevice, effectsManager.gameEffects["motionvectoreffect"], gBufferRenderer, game.gamePlayerR.gamePlayer);
            ssaoRenderer = new SSAORenderer(effectsManager.gameEffects["ssaoeffect"], gBufferRenderer, chunkRenderer, game.GraphicsDevice, game.gamePlayerR.gamePlayer, game.Content.Load<Texture2D>("randomnormal"));
            fxaaRenderer = new FXAARenderer(game.GraphicsDevice, effectsManager.gameEffects["fxaaeffect"]);
            motionBlurRenderer = new MotionBlurRenderer(game.GraphicsDevice, effectsManager.gameEffects["motionblureffect"], motionVectorRenderer);
            deferredShadingRenderer = new DeferredShadingRenderer(game.GraphicsDevice, effectsManager.gameEffects["deferredblockeffect"], effectsManager.gameEffects["transparentdeferredblockeffect"], shadowRenderer, ssaoRenderer, game.gameTimeManager, pointLightUpdater, gBufferRenderer, contactShadowRenderer, null, null, effectsManager.gameEffects["deferredblendeffect"], skyboxRenderer, fxaaRenderer, motionBlurRenderer, hdrCubemapRenderer);


            customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess0"));
            customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess1"));
            customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess2"));
            customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess3"));
            effectsManager.LoadCustomPostProcessEffects(game.GraphicsDevice, customPostProcessors, game.Content);
            hiZBufferRenderer = new HiZBufferRenderer(game.GraphicsDevice, effectsManager.gameEffects["hizbuffereffect"], gBufferRenderer, effectsManager.gameEffects["texturecopyraweffect"]);
            ssrRenderer = new SSRRenderer(game.GraphicsDevice, game.gamePlayerR.gamePlayer, gBufferRenderer, effectsManager.gameEffects["ssreffect"], deferredShadingRenderer, effectsManager.gameEffects["texturecopyraweffect"], motionVectorRenderer, hiZBufferRenderer, hdrCubemapRenderer, game.gameTimeManager);
            ssidRenderer = new SSIDRenderer(game.GraphicsDevice, effectsManager.gameEffects["ssideffect"], gBufferRenderer, game.gamePlayerR.gamePlayer, deferredShadingRenderer, effectsManager.gameEffects["texturecopyraweffect"], motionVectorRenderer, hiZBufferRenderer, hdrCubemapRenderer, game.gameTimeManager);

            deferredShadingRenderer.customPostProcessors = customPostProcessors;
            deferredShadingRenderer.ssidRenderer = ssidRenderer;
            deferredShadingRenderer.ssrRenderer = ssrRenderer;
            chunkRenderer.shadowRenderer = shadowRenderer;
            chunkRenderer.SSAORenderer = ssaoRenderer;
         //   entityRenderer.shadowRenderer = shadowRenderer;
            chunkRenderer.lightUpdater = pointLightUpdater;
            shadowRenderer.zombieModel = game.Content.Load<Model>("zombiemodelref");

            volumetricLightRenderer = new VolumetricLightRenderer(game.GraphicsDevice, gBufferRenderer, game._spriteBatch, effectsManager.gameEffects["volumetricmaskblendeffect"], effectsManager.gameEffects["lightshafteffect"], game.gamePlayerR.gamePlayer, game.gameTimeManager);
            chunkRenderer.SSRRenderer = ssrRenderer;
            //volumetricLightRenderer.entityRenderer = entityRenderer;
            boundingBoxVisualizationRenderer = new BoundingBoxVisualizationRenderer();
            boundingBoxVisualizationRenderer.Initialize(environmentHDRITex, game.GraphicsDevice, effectsManager.gameEffects["debuglineeffect"], gBufferRenderer);
            walkablePathVisualizationRenderer = new WalkablePathVisualizationRenderer();
            walkablePathVisualizationRenderer.Initialize(environmentHDRITex, game.GraphicsDevice, effectsManager.gameEffects["debuglineeffect"], gBufferRenderer);
        }


        public void RenderWorld(GameTime gameTime, SpriteBatch sb)
        {
            shadowRenderer.UpdateLightMatrices(game.gamePlayerR.gamePlayer);
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //  GraphicsDevice.RasterizerState = rasterizerState;
            shadowRenderer.RenderShadow(game.gamePlayerR.gamePlayer, ClientSideVoxelWorld.singleInstance.renderingChunks);
            gBufferRenderer.Draw(ClientSideVoxelWorld.singleInstance.renderingChunks);
            ssaoRenderer.Draw();
            hiZBufferRenderer.Draw();
            contactShadowRenderer.Draw();

            volumetricLightRenderer.Draw();
            motionVectorRenderer.Draw();
            shadowRenderer.UpdateLightMatrices(game.gamePlayerR.gamePlayer);

            shadowRenderer.UpdateLightMatrices(game.gamePlayerR.gamePlayer);

            pointLightUpdater.UpdatePointLight();
            deferredShadingRenderer.Draw(game.gamePlayerR.gamePlayer);
            ssidRenderer.Draw(gameTime, sb);

            //    skyboxRenderer.Draw(null);

            //   GraphicsDevice.RasterizerState = rasterizerState1;

            //        chunkRenderer.RenderAllChunksOpq(ChunkManager.chunks, gamePlayer);

            //    entityRenderer.Draw();

            //        chunkRenderer.RenderAllChunksTransparent(ChunkManager.chunks, gamePlayer);
            deferredShadingRenderer.DiffuseBlend(game.gamePlayerR.gamePlayer);
            ssrRenderer.Draw(gameTime);
            deferredShadingRenderer.FinalBlend(game._spriteBatch, volumetricLightRenderer, game.GraphicsDevice, game.gamePlayerR.gamePlayer);
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            game.GraphicsDevice.BlendState = BlendState.Additive;

            /*   if (game.gamePlayerR.gamePlayer is GamePlayer gamePlayer1)
               {
                   if (VoxelWorld.currentWorld.structureOperationsManager != null)
                   {
                       VoxelWorld.currentWorld.structureOperationsManager.DrawStructureSavingBounds(gamePlayer1, this);
                       VoxelWorld.currentWorld.structureOperationsManager.DrawStructurePlacingBounds(gamePlayer1, this);
                   }

                   if (gamePlayer1.curChunk != null)
                   {
                       EntityManager.pathfindingManager.DrawDebuggingPath(new Vector3(0, 0, 0), gamePlayer1, this);

                   }
               } */
            clientSidePlayersRenderer.DrawPlayerNames();


        }

        public void Resize()
        {
            int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            Debug.WriteLine(width);
            Debug.WriteLine(height);
            gBufferRenderer.Resize(width, height, game.GraphicsDevice);



            ssaoRenderer.ssaoTarget = new RenderTarget2D(ssaoRenderer.graphicsDevice, width / 2, height / 2, false, SurfaceFormat.Color, DepthFormat.Depth24);
            volumetricLightRenderer.blendVolumetricMap = new RenderTarget2D(volumetricLightRenderer.device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            volumetricLightRenderer.renderTargetLum = new RenderTarget2D(volumetricLightRenderer.device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            //      ssrRenderer.renderTargetSSR=new RenderTarget2D(ssrRenderer.graphicsDevice, width,height,false,SurfaceFormat.Vector4, DepthFormat.Depth24);
            volumetricLightRenderer.lightShaftTarget = new RenderTarget2D(game.GraphicsDevice, (int)(width / 2f), (int)(height / 2f), false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            contactShadowRenderer.contactShadowRenderTarget = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            motionVectorRenderer.renderTargetMotionVector = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            deferredShadingRenderer.Resize(width, height);
            motionBlurRenderer.processedImage = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            hiZBufferRenderer.ResizeTarget();
            ssidRenderer.renderTargetSSID = new RenderTarget2D(game.GraphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            ssidRenderer.renderTargetSSIDPrev = new RenderTarget2D(game.GraphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            ssrRenderer.renderTargetSSR = new RenderTarget2D(game.GraphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            ssrRenderer.renderTargetSSRPrev = new RenderTarget2D(game.GraphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            foreach (var processor in customPostProcessors)
            {
                processor.processedImage.Dispose();
                processor.processedImage = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            }
            float aspectRatio = game.GraphicsDevice.Viewport.Width / (float)game.GraphicsDevice.Viewport.Height;
            game.gamePlayerR.gamePlayer.cam.aspectRatio = aspectRatio;
            game.gamePlayerR.gamePlayer.cam.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), aspectRatio, 0.1f, 1000f);
        }

    }
}
