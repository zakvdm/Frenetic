using System;
using Lidgren.Network;
using Frenetic.Network;
using Frenetic.Network.Lidgren;
using Frenetic.Player;

namespace Frenetic
{
    public class GameSessionController : IController
    {
        public GameSessionController(IGameSession gameSession)
        {
            _gameSession = gameSession;
        }

        #region IController Members
        public void Process(float elapsedTime)
        {
            // Update all gamesession controllers:
            foreach (IController controller in _gameSession.Controllers)
            {
                controller.Process(elapsedTime);
            }
        }
        #endregion

        IGameSession _gameSession;
    }
}
