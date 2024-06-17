using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;
namespace monogameMinecraftDX
{
    public class AnimationStep
    {
        public float Duration;

        public Dictionary<string,AnimationTransformation> localTransforms = new Dictionary<string,AnimationTransformation>();


        public AnimationStep(Dictionary<string, AnimationTransformation> transforms,float duration) {
        this.localTransforms = transforms;
            this.Duration = duration;
           
        }
      
       
     
        
        public AnimationTransformation GetBoneLocal(string bone)
        {
            if (localTransforms.ContainsKey(bone))
            {
            return localTransforms[bone];
            }
            else {
                return null;
            }
         
        }
      
    }
}
