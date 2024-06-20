using System.Collections.Generic;
namespace monogameMinecraftDX
{
    public class AnimationStep
    {
        public float Duration;

        public Dictionary<string, AnimationTransformation> localTransforms = new Dictionary<string, AnimationTransformation>();


        public AnimationStep(Dictionary<string, AnimationTransformation> transforms, float duration)
        {
            this.localTransforms = transforms;
            this.Duration = duration;

        }




        public AnimationTransformation GetBoneLocal(string bone)
        {
            if (localTransforms.ContainsKey(bone))
            {
                return localTransforms[bone];
            }
            else
            {
                return null;
            }

        }

    }
}
