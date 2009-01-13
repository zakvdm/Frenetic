using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameSessionTests
    {
        [Test]
        public void CanConstructGameSession()
        {
            GameSession gameSession = new GameSession();
            Assert.IsNotNull(gameSession);
        }
    }
}
