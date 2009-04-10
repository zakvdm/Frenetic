using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Physics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MediatorPhysicsSettingsControllerTests
    {
        string gravityPropertyName = "Gravity";

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mediator();
            _physicsSettings = new PhysicsSettings();

            new MediatorPhysicsSettingsController(_physicsSettings, _mediator);
        }
        Mediator _mediator;
        PhysicsSettings _physicsSettings;

        [Test]
        public void GravityPropertyRegisteredAndWorks()
        {
            Assert.AreEqual(0, _physicsSettings.Gravity);

            _mediator.Do(gravityPropertyName, "100");

            Assert.AreEqual(100, _physicsSettings.Gravity);
            Assert.AreEqual(100, Convert.ToInt32(_mediator.Get(gravityPropertyName)));
        }

        [Test]
        public void GravityPropertyChecksForValidInput()
        {
            Assert.AreEqual(0, _physicsSettings.Gravity);

            _mediator.Do(gravityPropertyName, "gibberish");

            Assert.AreEqual(0, _physicsSettings.Gravity);
        }
    }
}
