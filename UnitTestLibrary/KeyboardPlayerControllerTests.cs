﻿using System;

using Frenetic;

using NUnit.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class KeyboardPlayerControllerTests
    {
        [Test]
        public void CanConstruct()
        {
            Player player = new Player(1);
            KeyboardPlayerController kpc = new KeyboardPlayerController(player);
        }

    }
}