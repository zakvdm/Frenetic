using System;
using Frenetic.Player;

namespace Frenetic.Gameplay.Weapons
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
