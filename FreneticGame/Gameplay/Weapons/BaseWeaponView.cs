using System;
using Frenetic.Player;

namespace Frenetic.Weapons
{
    public abstract class BaseWeaponView
    {
        public BaseWeaponView(IPlayerList playerList)
        {
            playerList.PlayerAdded += HandleNewPlayer;
        }

        protected abstract void HandleNewPlayer(IPlayer newPlayer);
    }
}
