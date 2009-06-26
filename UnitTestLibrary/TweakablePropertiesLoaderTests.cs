using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using System.Reflection;

namespace UnitTestLibrary
{
    // NOTE: These will be relatively slow (about 20 ms each...)
    [TestFixture]
    public class TweakablePropertiesLoaderTests
    {
        IMediator stubMediator;
        TweakablePropertiesLoader loader;
        [SetUp]
        public void SetUp()
        {
            stubMediator = MockRepository.GenerateStub<IMediator>();
            loader = new TweakablePropertiesLoader(stubMediator);
        }

        [Test]
        public void RegistersTweakablePropertiesForASpecificClassInstance()
        {
            TestTweakableClass testClass = new TestTweakableClass();
            PropertyInfo propinfo = testClass.GetType().GetProperty("TestProperty");

            loader.LoadTweakableProperties(testClass);

            stubMediator.AssertWasCalled(me => me.Register(propinfo, testClass));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage="Tweakable property {UnitTestLibrary.IncorrectTweakableClass.NonWritableProperty} is of the wrong type (should be public read/write)")]
        public void ChecksThatPropertyIsReadWrite()
        {
            IncorrectTweakableClass testClass = new IncorrectTweakableClass();
            PropertyInfo propinfo = testClass.GetType().GetProperty("NonWritableProperty");

            loader.LoadTweakableProperties(testClass);
        }
    }

    public class TestStaticTweakableClass
    {
        [Tweakable]
        public static int TestProperty { get; set; }
    }

    public class IncorrectStaticTweakableClass
    {
        [Tweakable]
        public bool NonStaticProperty { get; set; }
    }

    public class TestTweakableClass
    {
        [Tweakable]
        public int TestProperty { get; set; }
    }

    public class IncorrectTweakableClass
    {
        [Tweakable]
        public bool NonWritableProperty { get; private set; }
    }
}
