using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Graphics.Effects;

namespace Frenetic
{
    public class EffectUpdater : IController
    {
        public EffectUpdater(IEffect effect, ILineEffect lineEffect)
        {
            this.effect = effect;
            this.lineEffect = lineEffect;
        }

        #region IController Members

        public void Process(float elapsedSeconds)
        {
            _totalElapsedSeconds += elapsedSeconds;

            effect.Update(_totalElapsedSeconds, elapsedSeconds);
            lineEffect.Update(_totalElapsedSeconds, elapsedSeconds);
        }

        #endregion

        IEffect effect;
        ILineEffect lineEffect;

        float _totalElapsedSeconds = 0f;
    }
}
