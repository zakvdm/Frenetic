using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Autofac;

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

        public GameplayScreen MakeGameplayScreen(GameSessionControllerAndView clientGameSessionCandV, GameSessionControllerAndView serverGameSessionCandV)
        {
            GameplayScreen gameplayScreen = new GameplayScreen(clientGameSessionCandV, serverGameSessionCandV, this);
            _screenManager.AddScreen(gameplayScreen);
            return gameplayScreen;
        }

        public MainMenuScreen MakeMainMenuScreen(IContainer container)
        {
            var screen = container.Resolve<MainMenuScreen>();
            _screenManager.AddScreen(screen);
            return screen;
        }
        #endregion
    }
}
