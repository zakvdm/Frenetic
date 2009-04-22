using System;

using Frenetic.Level;

using NUnit.Framework;
using Autofac.Builder;
using Autofac;
using Frenetic;
using Microsoft.Xna.Framework.Graphics;
using Rhino.Mocks;
using System.Collections.Generic;
using Frenetic.Graphics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LevelTests
    {
        [Test]
        public void HasACollectionOfLevelPieces()
        {
            Level level = new Level(null);

            Assert.AreEqual(0, level.Pieces.Count);
        }

        [Test]
        public void CanBuildWithAutofac()
        {
            var builder = new ContainerBuilder();
            builder.Register<LevelPiece>().FactoryScoped();
            builder.RegisterGeneratedFactory<LevelPiece.Factory>(new TypedService(typeof(LevelPiece)));
            builder.Register<DumbLevelLoader>().As<ILevelLoader>().SingletonScoped();
            builder.Register<Level>().As<ILevel>().SingletonScoped();
            
            builder.Register<LevelView>();
            ISpriteBatch spriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            builder.Register<ISpriteBatch>(spriteBatch);
            ITexture texture = MockRepository.GenerateStub<ITexture>();
            builder.Register<ITexture>(texture);

            var container = builder.Build();

            LevelView levelView = container.Resolve<LevelView>(new TypedParameter(typeof(ICamera), null));

            Assert.IsNotNull(levelView);
        }

        [Test]
        public void CallsLoadEmptyLevelWhenNotLoaded()
        {
            var stubLevelLoader = MockRepository.GenerateStub<ILevelLoader>();
            Level level = new Level(stubLevelLoader);
            Assert.IsFalse(level.Loaded);

            level.Load();

            Assert.IsTrue(level.Loaded);
            stubLevelLoader.AssertWasCalled(x => x.LoadEmptyLevel(Arg<List<LevelPiece>>.Is.Equal(level.Pieces), Arg<int>.Is.Equal(800), Arg<int>.Is.Equal(600)));
        }
    }
}
