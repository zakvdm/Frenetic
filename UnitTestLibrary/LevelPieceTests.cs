using System;

using Frenetic.Level;

using Autofac.Builder;
using Autofac;

using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LevelPieceTests
    {
        [Test]
        public void CanMakeALevelPieceFactoryWithAutofac()
        {
            var builder = new ContainerBuilder();
            builder.Register<LevelPiece>().FactoryScoped();
            builder.RegisterGeneratedFactory<LevelPiece.Factory>(new TypedService(typeof(LevelPiece)));
            var container = builder.Build();

            var levelPieceFactory = container.Resolve<LevelPiece.Factory>();

            LevelPiece levelPiece1 = levelPieceFactory(new Vector2(100, 200), new Vector2(1, 2));
            LevelPiece levelPiece2 = levelPieceFactory(new Vector2(300, 400), new Vector2(3, 4));

            Assert.AreEqual(new Vector2(100, 200), levelPiece1.Position);
            Assert.AreEqual(new Vector2(1, 2), levelPiece1.Size);
            Assert.AreEqual(new Vector2(300, 400), levelPiece2.Position);
            Assert.AreEqual(new Vector2(3, 4), levelPiece2.Size);
        }
    }
}
