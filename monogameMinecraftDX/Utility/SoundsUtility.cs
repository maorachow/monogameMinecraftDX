using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace monogameMinecraftDX.Utility
{
    public class SoundsUtility
    {

        public static void PlaySound(Vector3 listenerPos, Vector3 emitPos, SoundEffect effect, float maxDistance)
        {

            float volume = (listenerPos - emitPos).Length() / maxDistance;

            volume = 1 - volume;
            volume = MathHelper.Clamp(volume, 0, 1);
            volume = volume * volume;
            //     Debug.WriteLine("play sound" + volume);

            effect.Play(volume, 0f, 0f);
        }
    }
}
