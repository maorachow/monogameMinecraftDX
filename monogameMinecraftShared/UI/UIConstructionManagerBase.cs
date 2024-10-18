using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftShared.UI
{
    public class UIConstructionManagerBase
    {
        public UIStateManager uiStateManager;
        public MinecraftGameBase game;

        public UIConstructionManagerBase(UIStateManager uiStateManager)
        {
            this.uiStateManager = uiStateManager;
          
        }

        public virtual void ConstructAll()
        {

        }
    }
}
