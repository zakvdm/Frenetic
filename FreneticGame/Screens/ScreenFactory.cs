using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        public MessageBoxScreen MakeMessageBoxScreen(string message)
        {
            SpriteFont smallFont = _screenManager.Content.Load<SpriteFont>("Fonts/MessageBox");
            Texture2D blankTexture = _screenManager.Content.Load<Texture2D>("Textures/blank");
            MessageBoxScreen messageBoxScreen = new MessageBoxScreen(message, false, smallFont, _screenManager.Font,
                                                        _screenManager.GraphicsDevice.Viewport, _screenManager.SpriteBatch,
                                                        blankTexture);
            _screenManager.AddScreen(messageBoxScreen);
            return messageBoxScreen;
        }

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
                            this, _screenManager.Game);
            _screenManager.AddScreen(mainMenuScreen);
            return mainMenuScreen;
        }
        #endregion
    }
}
