using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Player;

namespace Frenetic.Gameplay.Level
{
    public interface IPlayerRespawner
    {
        void RespawnPlayer(IPlayer player);
    }
}
