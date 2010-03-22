using System;
using System.Linq;
using Frenetic.Gameplay;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Player;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerScoreTests
    {
        PlayerScore worst = new PlayerScore() { Deaths = 20, Kills = 4 };
        PlayerScore best = new PlayerScore() { Deaths = 5, Kills = 14 };
        PlayerScore second_worst = new PlayerScore() { Deaths = 8, Kills = 4 };
        PlayerScore second_best = new PlayerScore() { Deaths = 8, Kills = 14 };

        [Test]
        public void OperatorOverloadsWorkForPlayerScore()
        {
            Assert.IsTrue((worst < second_worst) && (second_worst < second_best) && (second_best < best));
            Assert.IsTrue((best > second_best) && (second_best > second_worst) && (second_worst > worst));
        }

        [Test]
        public void CanSortAListOfPlayerScores()
        {
            List<PlayerScore> list = new List<PlayerScore>() { second_worst, best, second_best, worst };

            var sortedList = list.OrderByDescending((p) => p).ToList();

            Assert.AreEqual(best, sortedList[0]);
            Assert.AreEqual(second_best, sortedList[1]);
            Assert.AreEqual(second_worst, sortedList[2]);
            Assert.AreEqual(worst, sortedList[3]);
        }
    }
}
