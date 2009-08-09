using System;
using NUnit.Framework;
using Frenetic;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UnitTestLibrary.Test.FakeName;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MediatorTests
    {
        Mediator mediator;
        [SetUp]
        public void SetUp()
        {
            mediator = new Mediator(MockRepository.GenerateStub<log4net.ILog>());
        }

        [Test]
        public void HandlesNonExistentProperties()
        {
            mediator.Set("NonExistentProperty", "hole");
            Assert.IsNull(mediator.Get("NonExistentProperty"));
        }

        [Test]
        public void StripsNameCorrectly()
        {
            var tweakableProp = typeof(TestClass).GetProperty("IntTestProperty");
            mediator.Register(tweakableProp, new TestClass());

            // NOTE: The fullname would be UnitTestLibrary.Test.FakeName.TestClass.IntTestProperty (we take the last bit of the namespace and the property name for readability's sake 
            Assert.AreEqual("FakeName.IntTestProperty", mediator.AvailableProperties[0]);
        }
        [Test]
        public void HoldsAReferenceToDelegates()
        {
            var tweakableProp = typeof(TestClass).GetProperty("IntTestProperty");
            mediator.Register(tweakableProp, new TestClass());
            mediator.Set("FakeName.IntTestProperty", "200");

            tweakableProp = null;
            GC.Collect();

            Assert.AreEqual("200", mediator.Get("FakeName.IntTestProperty"));
        }

        [Test]
        public void CanRegisterAndUseAProperty()
        {
            var tweakableProperty = typeof(TestClass).GetProperty("IntTestProperty");

            mediator.Register(tweakableProperty, new TestClass());

            mediator.Set("FakeName.IntTestProperty", "11");
            Assert.AreEqual("11", mediator.Get("FakeName.IntTestProperty"));
        }

        [Test]
        public void CanHandleStrings()
        {
            var tweakableProp = typeof(TestClass).GetProperty("StringTestProperty");
            mediator.Register(tweakableProp, new TestClass());

            mediator.Set("FakeName.StringTestProperty", "hello");

            Assert.AreEqual("hello", mediator.Get("FakeName.StringTestProperty"));
        }

        [Test]
        public void CanHandleVector2s()
        {
            var tweakableProp = typeof(TestClass).GetProperty("Vector2TestProperty");
            mediator.Register(tweakableProp, new TestClass());

            mediator.Set("FakeName.Vector2TestProperty", "100 200");

            Assert.AreEqual("100 200", mediator.Get("FakeName.Vector2TestProperty"));
        }

        [Test]
        public void CanHandleColors()
        {
            var tweakableProp = typeof(TestClass).GetProperty("ColorTestProperty");
            mediator.Register(tweakableProp, new TestClass());

            mediator.Set("FakeName.ColorTestProperty", "100 200 10");

            Assert.AreEqual("100 200 10", mediator.Get("FakeName.ColorTestProperty"));
        }

        [Test]
        public void CanHandleFloats()
        {
            var tweakProp = typeof(TestClass).GetProperty("FloatTestProperty");
            mediator.Register(tweakProp, new TestClass());

            mediator.Set("FakeName.FloatTestProperty", "12.32231");

            Assert.AreEqual("12.32231", mediator.Get("FakeName.FloatTestProperty"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="This implementation of IMediator does not support Properties of type System.Decimal")]
        public void ThrowsExceptionWhenRegisteringAnUnknownType()
        {
            var tweakProp = typeof(TestClass).GetProperty("InvalidTypeProperty");

            mediator.Register(tweakProp, new TestClass());
        }

        [Test]
        public void CanGetAListOfPossibleCommands()
        {
            var tweakableProp1 = typeof(TestClass).GetProperty("IntTestProperty");
            var tweakableProp2 = typeof(TestClass).GetProperty("StringTestProperty");
            mediator.Register(tweakableProp1, new TestClass());
            mediator.Register(tweakableProp2, new TestClass());

            List<string> possibleProperties = mediator.AvailableProperties;

            Assert.AreEqual(2, possibleProperties.Count);
            Assert.IsTrue(possibleProperties.Exists((searchString) => searchString == "FakeName.IntTestProperty"));
            Assert.IsTrue(possibleProperties.Exists((searchString) => searchString == "FakeName.StringTestProperty"));
        }

        [Test]
        public void IgnoresInvalidInput()
        {
            // NOTE: Just a basic test for now... prolly needs fleshing out at some point
            var tweakProp = typeof(TestClass).GetProperty("Vector2TestProperty");
            TestClass instance = new TestClass();
            mediator.Register(tweakProp, instance);
            instance.Vector2TestProperty = new Vector2(100, 200);

            mediator.Set("FakeName.Vector2TestProperty", "300 ARG! BEEF");

            Assert.AreEqual("100 200", mediator.Get("FakeName.Vector2TestProperty"));
        }

        
    }

    namespace Test.FakeName
    {
        class TestClass
        {
            public int IntTestProperty { get; set; }
            public string StringTestProperty { get; set; }
            public Vector2 Vector2TestProperty { get; set; }
            public Color ColorTestProperty { get; set; }
            public float FloatTestProperty { get; set; }

            public Decimal InvalidTypeProperty { get; set; }
        }
    }
}
