using System;
using Microsoft.Xna.Framework;

using Frenetic.Player;
using Frenetic.Graphics;
using System.Collections.Generic;

namespace Frenetic.Weapons
{
    public class RocketLauncherView : BaseWeaponView, IWeaponView
    {
        public RocketLauncherView(IPlayerList playerList)
            : base(playerList)
        { }
        #region IWeaponView Members

        public void Draw(Matrix translationMatrix)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected override void HandleNewPlayer(IPlayer newPlayer)
        {
            this.RocketLaunchers.Add(newPlayer.CurrentWeapon, null);
        }
        
        Dictionary<IWeapon, IEffect> RocketLaunchers = new Dictionary<IWeapon, IEffect>();
    }
}
