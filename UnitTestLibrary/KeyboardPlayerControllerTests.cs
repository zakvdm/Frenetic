using System;

using Frenetic;

using NUnit.Framework;
using Frenetic.Physics;
using Rhino.Mocks;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class KeyboardPlayerControllerTests
    {
        [Test]
        public void CanConstruct()
        {
            Player player = new Player(1, null);
            KeyboardPlayerController kpc = new KeyboardPlayerController(player);
        }

        [Test]
        public void ProcessCallsPlayerUpdate()
        {
            IPlayer stubPlayer = MockRepository.GenerateStub<IPlayer>();
            KeyboardPlayerController kpc = new KeyboardPlayerController(stubPlayer);

            kpc.Process();

            stubPlayer.AssertWasCalled(x => x.Update());
        }

    }
}
