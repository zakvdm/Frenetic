using System;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Frenetic.Graphics.Effects;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class EffectUpdaterTests
    {
        [Test]
        public void UpdatesAllEffects()
        {
            var effect = MockRepository.GenerateStub<IEffect>();
            var lineEffect = MockRepository.GenerateStub<ILineEffect>();
            EffectUpdater effectUpdater = new EffectUpdater(effect, lineEffect);

            effectUpdater.Process(0.2f);

            effect.AssertWasCalled(me => me.Update(0.2f, 0.2f));
            lineEffect.AssertWasCalled(me => me.Update(0.2f, 0.2f));
        }

        [Test]
        public void AddsUpTotalGameTimeCorrectly()
        {
            var effect = MockRepository.GenerateStub<IEffect>();
            EffectUpdater effectUpdater = new EffectUpdater(effect, MockRepository.GenerateStub<ILineEffect>());

            effectUpdater.Process(0.2f);
            effectUpdater.Process(0.4f);

            effect.AssertWasCalled(me => me.Update(0.6f, 0.4f));

        }
    }
}
