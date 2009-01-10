using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Frenetic;

using NUnit.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GamerTests
    {
        [Test]
        public void CanMakeGamer()
        {
            Gamer gamer = new Gamer();
        }
    }
}
