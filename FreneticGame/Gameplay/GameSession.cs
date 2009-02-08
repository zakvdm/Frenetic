using System;
using System.Collections.Generic;

namespace Frenetic
{
    public class GameSession : IGameSession 
    {
        
        public GameSession()
        {
            Views = new List<IView>();
            Controllers = new List<IController>();
        }

        public List<IView> Views { get; private set; }
        public List<IController> Controllers { get; private set; }

        public IView GameSessionView { get; set; }
        public IController GameSessionController { get; set; }
    }
}
