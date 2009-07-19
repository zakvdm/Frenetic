using System;
using Frenetic.UserInput;
using Microsoft.Xna.Framework.Input;

namespace Frenetic.Gameplay.HUD
{
    public class HudController : IController
    {
        public HudController(ScoreOverlayView scoreView, IKeyboard keyboard)
        {
            _scoreView = scoreView;
            _keyboard = keyboard;
        }
        #region IController Members

        public void Process(float elapsedSeconds)
        {
            _scoreView.Visible = _keyboard.IsKeyDown(Keys.Tab);
        }

        #endregion

        ScoreOverlayView _scoreView;
        IKeyboard _keyboard;
    }
}
