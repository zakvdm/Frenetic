using System;

namespace Frenetic
{
    class GameSessionView : IView
    {
        public GameSessionView(IGameSession gameSession)
        {
            _gameSession = gameSession;
        }

        #region IView Members

        public void Generate()
        {
            foreach (IView view in _gameSession.Views)
            {
                view.Generate();
            }
        }

        #endregion

        private IGameSession _gameSession;
    }
}
