using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace monogameMinecraftShared.Animations
{
    public class SingleTexturedAnimatedModel:AnimationBlend
    {
        public Texture texture; 
        public SingleTexturedAnimatedModel(AnimationState[] animationStates, Model model,Texture texture):base(animationStates, model)
        {
           this.texture= texture;

        }

        public override void DrawAnimatedModel(GraphicsDevice device, Matrix world, Matrix view, Matrix projection,
            Effect shader, Dictionary<string, Matrix> optionalParams, Action optionalAction)
        {
            shader.Parameters["TextureE"]?.SetValue(texture);
            base.DrawAnimatedModel( device,  world,  view,  projection,
                 shader, optionalParams,  optionalAction);
        }
    }
}
