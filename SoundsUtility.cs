using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace monogameMinecraftDX
{
    public class SoundsUtility
    {
        public static void PlaySound(Vector3 listenerPos,Vector3 emitPos, SoundEffect effect,float maxDistance)
        {
          
            float volume = (listenerPos - emitPos).Length() / maxDistance;
            
            volume =1-volume;    
            volume=MathHelper.Clamp(volume, 0, 1);
            Debug.WriteLine("play sound" + volume);
           
              effect.Play(volume,0f,0f);
        }
    }
}
