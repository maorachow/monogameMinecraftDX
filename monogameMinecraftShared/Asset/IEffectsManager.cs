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
    public interface IEffectsManager
    {
        public Dictionary<string, Effect> gameEffects { get; set; }
        public Dictionary<string, Effect> customPostProcessEffects  { get; set; }
        public bool isEffectsLoaded { get; set; }
        public ContentManager contentManager { get; set; }

        public void LoadCustomPostProcessEffects(GraphicsDevice device, List<CustomPostProcessor> customPostProcessors,
            ContentManager cm);

        public void LoadEffects(ContentManager Content);
    }
}
