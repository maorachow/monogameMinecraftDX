using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace monogameMinecraftShared.Input
{
    public class MouseMovementManager
    {
        public Rectangle windowBounds;
        public bool isMouseLocked=false;
        public float curElapsedTime;
        public readonly float maxUpdatingElapsedTime =0.2f;
        public PlayerInputManager playerInputManager;

        public MouseMovementManager(PlayerInputManager playerInputManager)
        {
            this.playerInputManager= playerInputManager;
        }
       
        public void Update(float deltaTime)
        {
            curElapsedTime += deltaTime;
            if (curElapsedTime > maxUpdatingElapsedTime)
            {
                curElapsedTime = 0f;
                if (isMouseLocked == true)
                {
                    Mouse.SetPosition((int)windowBounds.Width/2, (int)windowBounds.Height/2);
                    playerInputManager.lastMouseState = Mouse.GetState();
                }
            }
           
        }
    }
}
