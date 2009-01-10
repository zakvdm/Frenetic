using System;
using System.Collections.Generic;

namespace Frenetic
{
    public class GameSession : IGameSession 
    {
        public List<IView> Views { get; private set; }
        public List<IController> Controllers { get; private set; }
        public GameSession()
        {
            Views = new List<IView>();
            Controllers = new List<IController>();
        }
        #region IGameSession Members
        #endregion
    }
}
