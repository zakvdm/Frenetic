using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public class GameplayScreen : GameScreen
    {
        public GameplayScreen(GameSessionControllerAndView clientGameSessionCandV, GameSessionControllerAndView serverGameSessionCandV, IScreenFactory screenFactory)
        {
            _clientGameSessionController = clientGameSessionCandV.GameSessionController;
            _clientGameSessionView = clientGameSessionCandV.GameSessionView;
            if (serverGameSessionCandV != null)
            {
                _serverGameSessionController = serverGameSessionCandV.GameSessionController;
                _serverGameSessionView = serverGameSessionCandV.GameSessionView;
            }

            _screenFactory = screenFactory;

            // set the transition times
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        public override void Update(GameTime pGameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(pGameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (_serverGameSessionController != null)
                _serverGameSessionController.Process((float)pGameTime.ElapsedGameTime.TotalSeconds);

            if (_clientGameSessionController != null)
                _clientGameSessionController.Process((float)pGameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(MenuInputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (!IsExiting)
            {
                if (input.PauseGame)
                {
                    // If they pressed pause, bring up the pause menu screen.
                    const string message = "Exit the game?";
                    MessageBoxScreen messageBox = _screenFactory.MakeMessageBoxScreen(message);
                    //messageBox.Accepted += (sender, e) => base.ExitScreen();
                    messageBox.Accepted += ExitScreen;
                }
            }
            base.HandleInput(input);
        }
        
        #region Drawing Methods
        public override void Draw(GameTime gameTime)
        {
            if (_serverGameSessionView != null)
                _serverGameSessionView.Generate();

            _clientGameSessionView.Generate();
        }
        #endregion

        #region Event Handlers
        public void ExitScreen(object sender, EventArgs e)
        {
            base.ExitScreen();
        }
        #endregion

        private IController _clientGameSessionController;
        private IController _serverGameSessionController;
        private IView _clientGameSessionView;
        private IView _serverGameSessionView;
        private IScreenFactory _screenFactory;
    }
}
