using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftDX.Utility;
using monogameMinecraftDX.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftDX.Asset;

using Microsoft.Xna.Framework;
using System.Diagnostics;
using monogameMinecraftDX.Rendering.Particle;
using monogameMinecraftDX.Updateables;

namespace monogameMinecraftDX.Rendering
{
    public class RenderPipelineManager
    {
        public EntityRenderer entityRenderer;
        public ChunkRenderer chunkRenderer;
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
       public ParticleRenderer particleRenderer;
        public MinecraftGame game;
        public EffectsManager effectsManager;
        public List<CustomPostProcessor> customPostProcessors = new List<CustomPostProcessor>();

        public Texture2D particleAtlas;
        public Texture2D environmentHDRITex;
        public Texture2D environmentHDRITexNight;
        public BoundingBoxVisualizationRenderer boundingBoxVisualizationRenderer;
        public WalkablePathVisualizationRenderer walkablePathVisualizationRenderer;
        public RenderPipelineManager(MinecraftGame game,EffectsManager em)
        {
            this.game = game;
            this.effectsManager = em;
        }

        public void InitRenderPipeline()
        {

            environmentHDRITex = game.Content.Load<Texture2D>("environmenthdri");
            environmentHDRITexNight = game.Content.Load<Texture2D>("environmenthdrinight");
            terrainMipmapGenerator = new TerrainMipmapGenerator(game.GraphicsDevice, effectsManager.gameEffects["texturecopyeffect"]);
            brdfLUTRenderer = new BRDFLUTRenderer(game.GraphicsDevice, effectsManager.gameEffects["brdfluteffect"]);
            brdfLUTRenderer.CalculateLUT();
            hdrCubemapRenderer = new HDRCubemapRenderer(game.GraphicsDevice, effectsManager.gameEffects["hdricubeeffect"], environmentHDRITex, effectsManager.gameEffects["hdriirradianceeffect"], effectsManager.gameEffects["hdriprefiltereffect"], environmentHDRITexNight);
            hdrCubemapRenderer.Render(hdrCubemapRenderer.resultCubeCollection,0);
            hdrCubemapRenderer.Render(hdrCubemapRenderer.resultCubeCollectionNight,1);
            
            chunkRenderer = new ChunkRenderer(this.game, game.GraphicsDevice, effectsManager.gameEffects["blockforwardeffect"], null, game.gameTimeManager);
            pointLightUpdater = new PointLightUpdater(game.gamePlayer);
            //    chunkRenderer.SetTexture(terrainTexNoMip, terrainNormal, terrainDepth, terrainTexNoMip, terrainMER);

            BlockResourcesManager.LoadDefaultResources(game.Content, game.GraphicsDevice, chunkRenderer);
            /* gBufferEffect = game.Content.Load<Effect>("gbuffereffect");
             gBufferEntityEffect = game.Content.Load<Effect>("gbufferentityeffect");*/
            particleRenderer = new ParticleRenderer(chunkRenderer.atlas, chunkRenderer.atlasNormal, chunkRenderer.atlasMER, game.GraphicsDevice,
                effectsManager.gameEffects["gbufferparticleeffect"], game);
            BlockResourcesManager.LoadDefaultParticleResources(game.Content,game.GraphicsDevice,particleRenderer);
            entityRenderer = new EntityRenderer(this.game, game.GraphicsDevice, game.gamePlayer, effectsManager.gameEffects["entityeffect"], game.Content.Load<Model>("zombiefbx"), game.Content.Load<Texture2D>("husk"), game.Content.Load<Model>("zombiemodelref"), effectsManager.gameEffects["createshadowmapeffect"], null, game.gameTimeManager);
            gBufferRenderer = new GBufferRenderer(this.game.GraphicsDevice, effectsManager.gameEffects["gbuffereffect"], effectsManager.gameEffects["gbufferentityeffect"], game.gamePlayer, chunkRenderer, entityRenderer, particleRenderer);
            skyboxRenderer = new SkyboxRenderer(game.GraphicsDevice, effectsManager.gameEffects["skyboxeffect"], null, game.gamePlayer, game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skyboxup"), game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skybox"), game.Content.Load<Texture2D>("skybox/skyboxdown"), game.Content.Load<Texture2D>("skybox/skybox"),
               game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnightup"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.Content.Load<Texture2D>("skybox/skyboxnightdown"), game.Content.Load<Texture2D>("skybox/skyboxnight"), game.gameTimeManager
               );
            skyboxRenderer.skyboxTexture = hdrCubemapRenderer.resultCubeCollection.resultSpecularCubemapMip0;
            skyboxRenderer.skyboxTextureNight = hdrCubemapRenderer.resultCubeCollectionNight.resultSpecularCubemapMip0;
            contactShadowRenderer = new ContactShadowRenderer(game.GraphicsDevice, effectsManager.gameEffects["contactshadoweffect"], gBufferRenderer, game.gameTimeManager, game.gamePlayer);
            shadowRenderer = new ShadowRenderer(this.game, game.GraphicsDevice, effectsManager.gameEffects["createshadowmapeffect"], chunkRenderer, entityRenderer, game.gameTimeManager);
            motionVectorRenderer = new MotionVectorRenderer(this.game.GraphicsDevice, effectsManager.gameEffects["motionvectoreffect"], gBufferRenderer, game.gamePlayer);
            ssaoRenderer = new SSAORenderer(effectsManager.gameEffects["ssaoeffect"], gBufferRenderer, chunkRenderer, this.game.GraphicsDevice, game.gamePlayer, game.Content.Load<Texture2D>("randomnormal"));
            fxaaRenderer = new FXAARenderer(game.GraphicsDevice, effectsManager.gameEffects["fxaaeffect"]);
            motionBlurRenderer = new MotionBlurRenderer(game.GraphicsDevice, effectsManager.gameEffects["motionblureffect"], motionVectorRenderer);
            deferredShadingRenderer = new DeferredShadingRenderer(game.GraphicsDevice, effectsManager.gameEffects["deferredblockeffect"], shadowRenderer, ssaoRenderer, game.gameTimeManager, pointLightUpdater, gBufferRenderer, contactShadowRenderer, null, null, effectsManager.gameEffects["deferredblendeffect"], skyboxRenderer, fxaaRenderer, motionBlurRenderer, hdrCubemapRenderer);


            customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess0"));
            customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess1"));
            customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess2"));
            customPostProcessors.Add(new CustomPostProcessor(game.GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess3"));
            effectsManager.LoadCustomPostProcessEffects(game.GraphicsDevice, customPostProcessors, game.Content);
            hiZBufferRenderer = new HiZBufferRenderer(game.GraphicsDevice, effectsManager.gameEffects["hizbuffereffect"], gBufferRenderer, effectsManager.gameEffects["texturecopyraweffect"]);
            ssrRenderer = new SSRRenderer(game.GraphicsDevice, game.gamePlayer, gBufferRenderer, effectsManager.gameEffects["ssreffect"], deferredShadingRenderer, effectsManager.gameEffects["texturecopyraweffect"], motionVectorRenderer, hiZBufferRenderer,hdrCubemapRenderer,game.gameTimeManager);
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
            volumetricLightRenderer.entityRenderer = entityRenderer;
            boundingBoxVisualizationRenderer = new BoundingBoxVisualizationRenderer();
            boundingBoxVisualizationRenderer.Initialize(environmentHDRITex, game.GraphicsDevice, effectsManager.gameEffects["debuglineeffect"],gBufferRenderer);
            walkablePathVisualizationRenderer=new WalkablePathVisualizationRenderer();
            walkablePathVisualizationRenderer.Initialize(environmentHDRITex, game.GraphicsDevice, effectsManager.gameEffects["debuglineeffect"], gBufferRenderer);
        }


        public void RenderWorld(GameTime gameTime, SpriteBatch sb)
        {
            shadowRenderer.UpdateLightMatrices(game.gamePlayer);
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //  GraphicsDevice.RasterizerState = rasterizerState;
            shadowRenderer.RenderShadow(game.gamePlayer);
            gBufferRenderer.Draw();
            ssaoRenderer.Draw();
            hiZBufferRenderer.Draw();
            contactShadowRenderer.Draw();
          
            volumetricLightRenderer.Draw();
            motionVectorRenderer.Draw();
            shadowRenderer.UpdateLightMatrices(game.gamePlayer);

            shadowRenderer.UpdateLightMatrices(game.gamePlayer);
          
            pointLightUpdater.UpdatePointLight();
            deferredShadingRenderer.Draw(game.gamePlayer);
            ssidRenderer.Draw(gameTime, sb);
            
            //    skyboxRenderer.Draw(null);

            //   GraphicsDevice.RasterizerState = rasterizerState1;
           
            //        chunkRenderer.RenderAllChunksOpq(ChunkManager.chunks, gamePlayer);

            //    entityRenderer.Draw();

            //        chunkRenderer.RenderAllChunksTransparent(ChunkManager.chunks, gamePlayer);
            deferredShadingRenderer.DiffuseBlend(game.gamePlayer);
            ssrRenderer.Draw(gameTime);
            deferredShadingRenderer.FinalBlend(game._spriteBatch, volumetricLightRenderer, game.GraphicsDevice, game.gamePlayer);
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            game.GraphicsDevice.BlendState=BlendState.Additive;

            if (VoxelWorld.currentWorld.structureOperationsManager != null)
            {
                VoxelWorld.currentWorld.structureOperationsManager.DrawStructureSavingBounds(game.gamePlayer,this);
                VoxelWorld.currentWorld.structureOperationsManager.DrawStructurePlacingBounds(game.gamePlayer, this);
            }

            if (game.gamePlayer.curChunk != null)
            {
                EntityManager.pathfindingManager.DrawDebuggingPath(new Vector3(0,0,0), game.gamePlayer, this);

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
            volumetricLightRenderer.blendVolumetricMap = new RenderTarget2D(volumetricLightRenderer.device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            volumetricLightRenderer.renderTargetLum = new RenderTarget2D(volumetricLightRenderer.device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            //      ssrRenderer.renderTargetSSR=new RenderTarget2D(ssrRenderer.graphicsDevice, width,height,false,SurfaceFormat.Vector4, DepthFormat.Depth24);
            volumetricLightRenderer.lightShaftTarget = new RenderTarget2D(game.GraphicsDevice, (int)((float)width / 2f), (int)((float)height / 2f), false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            contactShadowRenderer.contactShadowRenderTarget = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            motionVectorRenderer.renderTargetMotionVector = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            deferredShadingRenderer.renderTargetLum = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            deferredShadingRenderer.finalImage = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            deferredShadingRenderer.renderTargetLumSpec = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            deferredShadingRenderer.renderTargetLumAllDiffuse = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            motionBlurRenderer.processedImage = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            hiZBufferRenderer.ResizeTarget();
            ssidRenderer.renderTargetSSID = new RenderTarget2D(game.GraphicsDevice, width / 2, height / 2, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            ssidRenderer.renderTargetSSIDPrev = new RenderTarget2D(game.GraphicsDevice, width / 2, height / 2, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            ssrRenderer.renderTargetSSR = new RenderTarget2D(game.GraphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            ssrRenderer.renderTargetSSRPrev = new RenderTarget2D(game.GraphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            foreach (var processor in customPostProcessors)
            {
                processor.processedImage.Dispose();
                processor.processedImage = new RenderTarget2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            }
            float aspectRatio = game.GraphicsDevice.Viewport.Width / (float)game.GraphicsDevice.Viewport.Height;
            game.gamePlayer.cam.aspectRatio = aspectRatio;
            game.gamePlayer.cam.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), aspectRatio, 0.1f, 1000f);
        }

    }
}
