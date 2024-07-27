using System.Collections.Generic;

namespace monogameMinecraftShared.Animations
{
    
 public class Animation
    {


        public List<AnimationStep> steps = new List<AnimationStep>();
        public string name { get; private set; }
        public int StepsCount => steps.Count;

        public bool repeats = true;


        public Animation(List<AnimationStep> steps, bool repeat)
        {

            this.steps = steps;
            this.repeats = repeat;
        }
        public AnimationStep GetStep(int index, bool wrapIfOutOfIndex = false)
        {
            if (index >= steps.Count)
            {
                if (wrapIfOutOfIndex)
                {
                    index = index % steps.Count;
                }
                else
                {
                    index = steps.Count - 1;
                }
            }
            return steps[index];
        }


    }
    }
   

