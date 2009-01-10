using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public class ScreenFactory : IScreenFactory
    {
        private ScreenManager _screenManager = null;
        public ScreenFactory(ScreenManager screenManager)
        {
            _screenManager = screenManager;
        }
        #region IScreenFactory Members

        public GameplayScreen MakeGameplayScreen(IController serverGameSessionController, IController clientGameSessionController)
        {
            GameplayScreen gameplayScreen = new GameplayScreen(serverGameSessionController, clientGameSessionController);
            _screenManager.AddScreen(gameplayScreen);
            return gameplayScreen;
        }
        public GameplayScreen MakeGameplayScreen(IController clientGameSessionController)
        {
            GameplayScreen gameplayScreen = new GameplayScreen(clientGameSessionController);
            _screenManager.AddScreen(gameplayScreen);
            return gameplayScreen;
        }

        public MainMenuScreen MakeMainMenuScreen()
        {
            MainMenuScreen mainMenuScreen = new MainMenuScreen(_screenManager.GraphicsDevice.Viewport, _screenManager.SpriteBatch, _screenManager.Font,
                            new GameSessionFactory(new Frenetic.Network.Lidgren.LidgrenNetworkSessionFactory()), this);
            _screenManager.AddScreen(mainMenuScreen);
            return mainMenuScreen;
        }
        #endregion
    }
}
