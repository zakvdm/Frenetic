using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frenetic
{
    public class XnaGame : IGame
    {
        public XnaGame(Game game)
        {
            _game = game;
        }

        #region IGame Members

        public void Exit()
        {
            _game.Exit();
        }

        #endregion

        Game _game;
    }
}
