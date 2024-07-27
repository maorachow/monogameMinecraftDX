using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
 
using monogameMinecraftShared.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace monogameMinecraftShared.Asset
{
    public class EffectsManager:IEffectsManager
    {
        public Dictionary<string, Effect> gameEffects { get; set; }
        public Dictionary<string, Effect> customPostProcessEffects { get; set; }
        public bool isEffectsLoaded { get; set; }
        public ContentManager contentManager {get; set; }


        public EffectsManager()
        {
            gameEffects = new Dictionary<string, Effect>();
        customPostProcessEffects = new Dictionary<string, Effect>();
        isEffectsLoaded = false;
        }
        public void LoadCustomPostProcessEffects(GraphicsDevice device, List<CustomPostProcessor> customPostProcessors, ContentManager cm)
        {
            if (contentManager != null)
            {
                contentManager.Dispose();
            }
            contentManager = new ContentManager(cm.ServiceProvider, AppDomain.CurrentDomain.BaseDirectory + "CustomEffects");
    //        Debug.WriteLine(contentManager.GetGraphicsDevice().ToString());
            customPostProcessEffects.Clear();

            foreach (var processor in customPostProcessors)
            {

                processor.postProcessEffect = null;


            }
            Effect e0;
            Effect e1;
            Effect e2;
            Effect e3;
            try
            {
                e0 = contentManager.Load<Effect>("postprocess0");

            }
            catch
            {
                e0 = null;
            }
            try
            {
                e1 = contentManager.Load<Effect>("postprocess1");
            }
            catch
            {
                e1 = null;
            }
            try
            {
                e2 = contentManager.Load<Effect>("postprocess2");
            }
            catch
            {
                e2 = null;
            }
            try
            {
                e3 = contentManager.Load<Effect>("postprocess3");
            }
            catch
            {
                e3 = null;
            }
            customPostProcessEffects.Add("postprocess0", e0);
            customPostProcessEffects.Add("postprocess1", e1);
            customPostProcessEffects.Add("postprocess2", e2);
            customPostProcessEffects.Add("postprocess3", e3);
            foreach (var processor in customPostProcessors)
            {
                if (customPostProcessEffects.ContainsKey(processor.effectNameInDic) && customPostProcessEffects[processor.effectNameInDic] != null)
                {
                    processor.LoadEffect(customPostProcessEffects[processor.effectNameInDic]);
                }

            }

            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;
            foreach (var processor in customPostProcessors)
            {
                processor.processedImage = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            }
        }
        public void LoadEffects(ContentManager Content)
        {


            if (isEffectsLoaded == true) { return; }

            gameEffects.Clear();
            gameEffects.TryAdd("blockforwardeffect", Content.Load<Effect>("blockeffect"));
            gameEffects.TryAdd("createshadowmapeffect", Content.Load<Effect>("createshadowmapeffect"));
            gameEffects.TryAdd("entityeffect", Content.Load<Effect>("entityeffect"));

            gameEffects.TryAdd("gbuffereffect", Content.Load<Effect>("gbuffereffect"));
            gameEffects.TryAdd("gbufferentityeffect", Content.Load<Effect>("gbufferentityeffect"));
            gameEffects.TryAdd("ssaoeffect", Content.Load<Effect>("ssaoeffect"));
            gameEffects.TryAdd("lightshafteffect", Content.Load<Effect>("lightshafteffect"));
            gameEffects.TryAdd("skyboxeffect", Content.Load<Effect>("skyboxeffect"));
            gameEffects.TryAdd("ssreffect", Content.Load<Effect>("ssreffect"));
            gameEffects.TryAdd("ssideffect", Content.Load<Effect>("ssideffect"));
            gameEffects.TryAdd("deferredblockeffect", Content.Load<Effect>("deferredblockeffect"));
            gameEffects.TryAdd("contactshadoweffect", Content.Load<Effect>("contactshadoweffect"));
            gameEffects.TryAdd("deferredblendeffect", Content.Load<Effect>("deferredblendeffect"));
            gameEffects.TryAdd("brdfluteffect", Content.Load<Effect>("brdfluteffect"));
            gameEffects.TryAdd("motionvectoreffect", Content.Load<Effect>("motionvectoreffect"));
            gameEffects.TryAdd("texturecopyraweffect", Content.Load<Effect>("texturecopyraweffect"));
            gameEffects.TryAdd("texturecopyeffect", Content.Load<Effect>("texturecopyeffect"));
            gameEffects.TryAdd("hizbuffereffect", Content.Load<Effect>("hizbuffereffect"));
            gameEffects.TryAdd("fxaaeffect", Content.Load<Effect>("fxaaeffect"));
            gameEffects.TryAdd("motionblureffect", Content.Load<Effect>("motionblureffect"));
            gameEffects.TryAdd("gbuffereffect", Content.Load<Effect>("gbuffereffect"));
            gameEffects.TryAdd("volumetricmaskblendeffect", Content.Load<Effect>("volumetricmaskblend"));
            gameEffects.TryAdd("hdricubeeffect", Content.Load<Effect>("hdricubeeffect"));
            gameEffects.TryAdd("hdriirradianceeffect", Content.Load<Effect>("hdriirradianceeffect"));
            gameEffects.TryAdd("hdriprefiltereffect", Content.Load<Effect>("hdriprefiltereffect"));
            gameEffects.TryAdd("gbufferparticleeffect", Content.Load<Effect>("gbufferparticleeffect"));
            gameEffects.TryAdd("debuglineeffect", Content.Load<Effect>("debuglineeffect"));
            isEffectsLoaded = true;
        }


        /*   public static Effect CompileFX(GraphicsDevice gd, string sourceFilePath)
           {
               //   string sourceFile = Directory.GetCurrentDirectory() + "/tmp.txt";
               //  File.WriteAllText(sourceFile, fxcode);

               EffectImporter importer = new EffectImporter();

               EffectContent content = importer.Import(sourceFilePath, null);
               if (content == null)
               {
                   System.Diagnostics.Debug.WriteLine("effect file not found:" + sourceFilePath);
                   return null;    
               }
               EffectProcessor processor = new EffectProcessor();
               processor.DebugMode = EffectProcessorDebugMode.Debug;
                   Debug.WriteLine(content.Identity.SourceFilename);
               PipelineManager pm = new PipelineManager(Directory.GetCurrentDirectory(), Directory.GetCurrentDirectory(), Directory.GetCurrentDirectory());
                   pm.Profile = GraphicsProfile.Reach;
                   pm.Platform = TargetPlatform.Windows;

                //   pm.BuildContent(sourceFilePath);
                   PipelineProcessorContext ppc = new PipelineProcessorContext(pm, new PipelineBuildEvent());
                   //       ppc.TargetProfile = GraphicsProfile.HiDef;
                   //    ppc.TargetPlatform = TargetPlatform.Windows;
                   CompiledEffectContent cecontent = processor.Process(content, ppc) ;
         //      ContentCompiler compiler = new ContentCompiler();

               return new Effect(gd, cecontent.GetEffectCode());


           }*/
    }
}
