using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftShared.Asset
{
    public class LowDefEffectsManager:IEffectsManager
    {

        public Dictionary<string, Effect> gameEffects { get; set; }
        public Dictionary<string, Effect> customPostProcessEffects { get; set; }
        public bool isEffectsLoaded { get; set; }
        public ContentManager contentManager { get; set; }


        public LowDefEffectsManager()
        {
            gameEffects = new Dictionary<string, Effect>();
            customPostProcessEffects = new Dictionary<string, Effect>();
            isEffectsLoaded = false;
        }
        public void LoadCustomPostProcessEffects(GraphicsDevice device, List<CustomPostProcessor> customPostProcessors, ContentManager cm)
        {
         
        }
        public void LoadEffects(ContentManager Content)
        {


            if (isEffectsLoaded == true) { return; }

            gameEffects.Clear();

            try
            {
                gameEffects.TryAdd("blockforwardeffect", Content.Load<Effect>("blockforwardeffect"));
                gameEffects.TryAdd("entityforwardeffect", Content.Load<Effect>("entityforwardeffect"));
            }
            catch
            {
                // ignored
            }
            gameEffects.TryAdd("ssaoeffect", Content.Load<Effect>("ssaoeffect"));

            gameEffects.TryAdd("skyboxeffect", Content.Load<Effect>("skyboxeffect"));

            gameEffects.TryAdd("deferredblockeffect", Content.Load<Effect>("deferredblockeffect"));

            gameEffects.TryAdd("gbuffereffect", Content.Load<Effect>("gbuffereffect"));
            try
            {
                gameEffects.TryAdd("gbufferdepthpeelingeffect", Content.Load<Effect>("gbufferdepthpeelingeffect"));
               
                gameEffects.TryAdd("gbufferparticleeffect", Content.Load<Effect>("gbufferparticleeffect"));
            }
            catch
            {
                // ignored
            }

            gameEffects.TryAdd("gbufferentityeffect", Content.Load<Effect>("gbufferentityeffect"));
        
      


       
            gameEffects.TryAdd("texturecopyraweffect", Content.Load<Effect>("texturecopyraweffect"));
            gameEffects.TryAdd("texturecopyeffect", Content.Load<Effect>("texturecopyeffect"));
        
            gameEffects.TryAdd("gbuffereffect", Content.Load<Effect>("gbuffereffect"));
         
            gameEffects.TryAdd("hdricubeeffect", Content.Load<Effect>("hdricubeeffect"));
            gameEffects.TryAdd("hdriirradianceeffect", Content.Load<Effect>("hdriirradianceeffect"));
            gameEffects.TryAdd("hdriprefiltereffect", Content.Load<Effect>("hdriprefiltereffect"));
            
            gameEffects.TryAdd("debuglineeffect", Content.Load<Effect>("debuglineeffect"));
            isEffectsLoaded = true;
        }

    }
}
