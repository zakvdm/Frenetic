using System;
using NUnit.Framework;
using Frenetic;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UnitTestLibrary.Test.FakeName;
using Rhino.Mocks;
using Frenetic.Engine;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MediatorTests
    {
        Mediator mediator;
        [SetUp]
        public void SetUp()
        {
            mediator = new Mediator(DummyLogger.Factory);
        }

        [Test]
        public void HandlesNonExistentProperties()
        {
            mediator.Process("NonExistentProperty", "hole");
            Assert.IsNull(mediator.Process("NonExistentProperty"));
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
            mediator.Process("FakeName.IntTestProperty", "200");

            tweakableProp = null;
            GC.Collect();

            Assert.AreEqual("200", mediator.Process("FakeName.IntTestProperty"));
        }

        [Test]
        public void CanRegisterAndUseAProperty()
        {
            var tweakableProperty = typeof(TestClass).GetProperty("IntTestProperty");

            mediator.Register(tweakableProperty, new TestClass());

            mediator.Process("FakeName.IntTestProperty", "11");
            Assert.AreEqual("11", mediator.Process("FakeName.IntTestProperty"));
        }

        [Test]
        public void CanRegisterAndUseAZeroParameterMethod()
        {
            var method = typeof(TestClass).GetMethod("ZeroParameterTestMethod");

            var testClass = new TestClass();

            mediator.Register(method, "Start", testClass);

            Assert.IsFalse(testClass.ZeroParameterTestMethodCalled);
            mediator.Process("Start");
            Assert.IsTrue(testClass.ZeroParameterTestMethodCalled);
        }

        [Test]
        public void CanRegisterAndUseATwoParameterMethod()
        {
            var method = typeof(TestClass).GetMethod("TwoParameterTestMethod");

            var testClass = new TestClass();

            mediator.Register(method, "Connect", testClass);

            mediator.Process("Connect", "192.168.1.1", 2005);
            Assert.AreEqual("192.168.1.1", testClass.StringTestProperty);
            Assert.AreEqual(2005, testClass.IntTestProperty);
        }

        [Test]
        public void WhenCallingAMethodItTriesToConvertParamatersToAppropriateType()
        {
            var method = typeof(TestClass).GetMethod("TwoParameterTestMethod");
            var testClass = new TestClass();
            mediator.Register(method, "Connect", testClass);

            mediator.Process("Connect", "192.168.1.1", "2005");

            Assert.AreEqual("192.168.1.1", testClass.StringTestProperty);
            Assert.AreEqual(2005, testClass.IntTestProperty);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="This implementation of IMediator does not support Methods with Parameters of type System.Decimal")]
        public void ThrowsExceptionWhenRegisteringMethodWithUnknownParameterType()
        {
            var method = typeof(TestClass).GetMethod("InvalidParameterTestMethod");
            var testClass = new TestClass();
            mediator.Register(method, "InvalidMethod", testClass);

            mediator.Process("InvalidMethod", new System.Decimal(100));
        }

        [Test]
        public void CanHandleStrings()
        {
            var tweakableProp = typeof(TestClass).GetProperty("StringTestProperty");
            mediator.Register(tweakableProp, new TestClass());

            mediator.Process("FakeName.StringTestProperty", "hello");

            Assert.AreEqual("hello", mediator.Process("FakeName.StringTestProperty"));
        }

        [Test]
        public void CanHandleVector2s()
        {
            var tweakableProp = typeof(TestClass).GetProperty("Vector2TestProperty");
            mediator.Register(tweakableProp, new TestClass());

            mediator.Process("FakeName.Vector2TestProperty", "100 200");

            Assert.AreEqual("100 200", mediator.Process("FakeName.Vector2TestProperty"));
        }

        [Test]
        public void CanHandleColors()
        {
            var tweakableProp = typeof(TestClass).GetProperty("ColorTestProperty");
            mediator.Register(tweakableProp, new TestClass());

            mediator.Process("FakeName.ColorTestProperty", "100 200 10");

            Assert.AreEqual("100 200 10", mediator.Process("FakeName.ColorTestProperty"));
        }

        [Test]
        public void CanHandleFloats()
        {
            var tweakProp = typeof(TestClass).GetProperty("FloatTestProperty");
            mediator.Register(tweakProp, new TestClass());

            mediator.Process("FakeName.FloatTestProperty", "12.32231");

            Assert.AreEqual("12.32231", mediator.Process("FakeName.FloatTestProperty"));
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
            var method = typeof(TestClass).GetMethod("TwoParameterTestMethod");
            mediator.Register(tweakableProp1, new TestClass());
            mediator.Register(tweakableProp2, new TestClass());
            mediator.Register(method, "TestCommandName", new TestClass());

            List<string> possibleExecutables = mediator.AvailableProperties;
            possibleExecutables.AddRange(mediator.AvailableActions);

            Assert.AreEqual(3, possibleExecutables.Count);
            Assert.IsTrue(possibleExecutables.Exists((searchString) => searchString == "FakeName.IntTestProperty"));
            Assert.IsTrue(possibleExecutables.Exists((searchString) => searchString == "FakeName.StringTestProperty"));
            Assert.IsTrue(possibleExecutables.Exists((searchString) => searchString == "TestCommandName"));
        }

        [Test]
        public void IgnoresInvalidInputForProperties()
        {
            // NOTE: Just a basic test for now... prolly needs fleshing out at some point
            var tweakProp = typeof(TestClass).GetProperty("Vector2TestProperty");
            TestClass instance = new TestClass();
            mediator.Register(tweakProp, instance);
            instance.Vector2TestProperty = new Vector2(100, 200);

            mediator.Process("FakeName.Vector2TestProperty", "300 ARG! BEEF");

            Assert.AreEqual("100 200", mediator.Process("FakeName.Vector2TestProperty"));
        }

        [Test]
        public void IgnoresInvalidInputForMethods()
        {
            var method = typeof(TestClass).GetMethod("TwoParameterTestMethod");
            var testClass = new TestClass();
            mediator.Register(method, "Connect", testClass);

            mediator.Process("Connect", "192.168.0.1", "100.3");

            Assert.IsNull(testClass.StringTestProperty);
            Assert.AreEqual(0, testClass.IntTestProperty);
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

            public bool ZeroParameterTestMethodCalled { get; set; }

            [Command("Start")]
            public void ZeroParameterTestMethod()
            {
                ZeroParameterTestMethodCalled = true;
            }

            [Command("Connect")]
            public void TwoParameterTestMethod(string stringParameter, int intParameter)
            {
                StringTestProperty = stringParameter;
                IntTestProperty = intParameter;
            }

            [Command("InvalidCommand")]
            public void InvalidParameterTestMethod(Decimal parameter)
            {
            }
        }
    }
}
