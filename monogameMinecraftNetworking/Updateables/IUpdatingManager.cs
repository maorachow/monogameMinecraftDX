using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Updateables
{
    public interface IUpdatingManager
    {
        public IMultiplayerServer server { get; }
        public void UpdateThread();
        public void Start();
        public void Stop();
    }
}
