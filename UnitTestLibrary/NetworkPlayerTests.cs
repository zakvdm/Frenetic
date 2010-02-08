using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Physics;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetworkPlayerTests
    {
        IPhysicsComponent stubPhysicsComponent;
        NetworkPlayer player;
        [SetUp]
        public void SetUp()
        {
            stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            player = new NetworkPlayer(null, stubPhysicsComponent, null, null, null);
        }

        [Test]
        public void UpdatePositionFromNetworkDoesntMoveCurrentPosition()
        {
            player.Position = new Vector2(100, 200);
            
            player.UpdatePositionFromNetworkWithPrediction(new Vector2(110, 180), 0.1f);

            Assert.AreEqual(new Vector2(100, 200), player.Position);
        }

        [Test]
        public void TestVelocityForSimplePrediction()
        {
            player.Position = new Vector2(10, 15);
            player.LastReceivedPosition = Vector2.Zero;

            player.UpdatePositionFromNetworkWithPrediction(new Vector2(10, 15), 1f);

            Assert.AreEqual(new Vector2(10, 15), stubPhysicsComponent.LinearVelocity);
        }

        [Test]
        public void WorksWhenPredictionWasWrong()
        {
            player.Position = new Vector2(20, 25);
            player.LastReceivedPosition = new Vector2(10, 20);
            // CREATE A PREDICTION:
            player.UpdatePositionFromNetworkWithPrediction(new Vector2(20, 25), 1f);  // prediction: (30, 30)
            // SIMULATE FARSEER'S WORK (i.e. move the Position, and restore LastReceivedPosition):
            player.Position = new Vector2(30, 30);

            var actualReceivedPosition = new Vector2(31, 28);
            player.UpdatePositionFromNetworkWithPrediction(actualReceivedPosition, 1f); // So the estimate was out by (1, -2)

            // NEXT ESTIMATE POSITION SHOULD BE: (31 - 20, 28 - 25)[predicted based on last 2 receipts] + (31 - 30, 28 - 30)[adjustment from wrong previous estimate] = (12, 1)
            Assert.AreEqual(new Vector2(12, 1), stubPhysicsComponent.LinearVelocity);
        }

        [Test]
        public void AdjustsForTime()
        {
            player.Position = new Vector2(20, 25);
            player.LastReceivedPosition = new Vector2(10, 20);

            player.UpdatePositionFromNetworkWithPrediction(new Vector2(20, 25), 0.1f);

            Assert.AreEqual(new Vector2(100, 50), stubPhysicsComponent.LinearVelocity); // NOTE that the smaller the delivery time, the bigger the estimate has to be...
        }
    }
}
