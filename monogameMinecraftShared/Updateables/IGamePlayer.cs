using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace monogameMinecraftShared.Updateables
{
    public interface IGamePlayer
    {
        public Camera cam { get; }
        public Vector3 position { get; set; }

        public int currentSelectedHotbar { get; }
        public short[] inventoryData { get; }

        public void ResetPlayerInputValues();
        
        public void ProcessPlayerInputs(Vector3 dir, float deltaTime, KeyboardState kState, MouseState mState, MouseState prevMouseState, bool isFlyingPressed, bool isSpeedUpPressed, bool isLMBPressed, bool isRMBPressed, float scrollDelta);
    }
}
