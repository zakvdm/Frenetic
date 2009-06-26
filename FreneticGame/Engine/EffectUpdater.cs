using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Graphics;

namespace Frenetic
{
    public class EffectUpdater : IController
    {
        public EffectUpdater(IEffect effect)
        {
            _effect = effect;
        }

        #region IController Members

        public void Process(float elapsedSeconds)
        {
            _totalElapsedSeconds += elapsedSeconds;

            _effect.Update(_totalElapsedSeconds, elapsedSeconds);
        }

        #endregion

        IEffect _effect;

        float _totalElapsedSeconds = 0f;
    }
}
