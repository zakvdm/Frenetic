using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

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

        public GameplayScreen MakeGameplayScreen(GameSessionControllerAndView serverGameSessionCandV, GameSessionControllerAndView clientGameSessionCandV)
        {
            GameplayScreen gameplayScreen = new GameplayScreen(serverGameSessionCandV, clientGameSessionCandV);
            _screenManager.AddScreen(gameplayScreen);
            return gameplayScreen;
        }
        public GameplayScreen MakeGameplayScreen(GameSessionControllerAndView clientGameSessionCandV)
        {
            GameplayScreen gameplayScreen = new GameplayScreen(clientGameSessionCandV);
            _screenManager.AddScreen(gameplayScreen);
            return gameplayScreen;
        }

        public MainMenuScreen MakeMainMenuScreen()
        {
            MainMenuScreen mainMenuScreen = new MainMenuScreen(
                            _screenManager.GraphicsDevice.Viewport, 
                            _screenManager.SpriteBatch, 
                            _screenManager.Font,
                            new GameSessionFactory
                                (
                                    new Frenetic.Network.Lidgren.LidgrenNetworkSessionFactory(), 
                                    _screenManager.GraphicsDevice, new ContentManager(_screenManager.Game.Services)
                                ), 
                            this);
            _screenManager.AddScreen(mainMenuScreen);
            return mainMenuScreen;
        }
        #endregion
    }
}
