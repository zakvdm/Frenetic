using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Frenetic;

using NUnit.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class WorldTests
    {
        [Test]
        public void CanCreateWorldModel()
        {
            World world = new World();
            Assert.IsNotNull(world);
        }

        [Test]
        public void CanAddPlayerToWorld()
        {
            World world = new World();
            Player player = new Player(1);
            world.AddPlayer(player);

            Assert.AreEqual(player, world.Players[0]);
        }
    }
}
