using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftShared.Updateables;

namespace monogameMinecraftShared
{
    public interface IGameWithPlayer
    {
        public IGamePlayer gamePlayer { get; }
    }
}
