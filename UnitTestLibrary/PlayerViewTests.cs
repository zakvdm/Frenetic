using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Player;
using Frenetic.Weapons;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerViewTests
    {
        ITexture stubTexture;
        ITextureBank<PlayerTexture> _stubTextureBank;
        ISpriteBatch stubSpriteBatch;
        ICamera stubCamera;
        RailGunView.Factory railgunViewFactoryDelegate;
        IRailGunView stubRailGunView;
        IPlayer player;
        PlayerView playerView;
        [SetUp]
        public void SetUp()
        {
            stubTexture = MockRepository.GenerateStub<ITexture>();
            _stubTextureBank = MockRepository.GenerateStub<ITextureBank<PlayerTexture>>();
            _stubTextureBank.Stub(x => x[PlayerTexture.Ball]).Return(stubTexture);
            stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            stubCamera = MockRepository.GenerateStub<ICamera>();
            stubRailGunView = MockRepository.GenerateStub<IRailGunView>();
            railgunViewFactoryDelegate = () => stubRailGunView;
            player = MockRepository.GenerateStub<IPlayer>();
            player.IsAlive = true;
            playerView = new PlayerView(_stubTextureBank, stubSpriteBatch, stubCamera, railgunViewFactoryDelegate);
        }

        [Test]
        public void AddPlayerAlsoAddsWeaponView()
        {
            bool factoryWasUsed = false;
            railgunViewFactoryDelegate = () => { factoryWasUsed = true; return stubRailGunView; };
            playerView = new PlayerView(_stubTextureBank, stubSpriteBatch, stubCamera, railgunViewFactoryDelegate);
            playerView.AddPlayer(player, null);

            Assert.AreEqual(player, playerView.Players[0]);
            Assert.IsTrue(factoryWasUsed);
        }

        [Test]
        public void CanRemovePlayer()
        {
            playerView.AddPlayer(player, MockRepository.GenerateStub<IPlayerSettings>());

            playerView.RemovePlayer(player);

            Assert.AreEqual(0, playerView.Players.Count);
        }

        [Test]
        public void DrawsEachPlayer()
        {
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.IsAlive = true;
            playerView.AddPlayer(player, MockRepository.GenerateStub<IPlayerSettings>());
            playerView.AddPlayer(player2, MockRepository.GenerateStub<IPlayerSettings>());

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(me => me.Draw(Arg<ITexture>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<SpriteEffects>.Is.Anything, Arg<float>.Is.Anything),
                o => o.Repeat.Twice());
        }

        [Test]
        public void CallsDrawWithCorrectParameters()
        {
            IPlayerSettings settings = new NetworkPlayerSettings();
            settings.Texture = PlayerTexture.Ball;
            stubTexture.Stub(x => x.Width).Return(100);
            stubTexture.Stub(x => x.Height).Return(200);
            player.Position = new Vector2(1, 1);
            playerView.AddPlayer(player, settings);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture), Arg<Vector2>.Is.Equal(new Vector2(1, 1)),
                Arg<Rectangle>.Is.Equal(null), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f),
                Arg<Vector2>.Is.Equal(new Vector2(100 / 2f, 200 / 2f)),
                Arg<Vector2>.Is.Equal(new Vector2(1, 1)), Arg<SpriteEffects>.Is.Equal(SpriteEffects.None), Arg<float>.Is.Equal(1f)));
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }

        [Test]
        public void UsesCameraCorrectly()
        {
            IPlayerSettings settings = new NetworkPlayerSettings();
            ICamera camera = new Camera(player, new Vector2(100, 100));
            playerView = new PlayerView(_stubTextureBank, stubSpriteBatch, camera, () => MockRepository.GenerateStub<IRailGunView>());
            playerView.AddPlayer(player, settings);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin(Arg<Matrix>.Is.Equal(camera.TranslationMatrix)));
        }

        [Test]
        public void DoesntDrawDeadPlayer()
        {
            player.IsAlive = false;
            playerView.AddPlayer(player, MockRepository.GenerateStub<IPlayerSettings>());

            playerView.Generate();

            stubSpriteBatch.AssertWasNotCalled(me => me.Draw(Arg<ITexture>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<SpriteEffects>.Is.Anything, Arg<float>.Is.Anything));
        }

        // WEAPON:
        [Test]
        public void DrawsPlayerWeapon()
        {
            RailGun railGun = new RailGun(null);
            stubCamera.Stub(me => me.TranslationMatrix).Return(Matrix.Identity);
            var stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubPlayer.Stub(me => me.CurrentWeapon).Return(railGun);
            playerView.AddPlayer(stubPlayer, MockRepository.GenerateStub<IPlayerSettings>());

            playerView.Generate();

            stubRailGunView.AssertWasCalled(me => me.Draw(railGun, Matrix.Identity));
        }
        [Test]
        public void DrawsSeparateWeaponViewForEachPlayer()
        {
            var stubRailGunView1 = MockRepository.GenerateStub<IRailGunView>();
            var stubRailGunView2 = MockRepository.GenerateStub<IRailGunView>();
            Queue<IRailGunView> railgun_views = new Queue<IRailGunView>();
            railgun_views.Enqueue(stubRailGunView1);
            railgun_views.Enqueue(stubRailGunView2);
            railgunViewFactoryDelegate = () => railgun_views.Dequeue();
            playerView = new PlayerView(_stubTextureBank, stubSpriteBatch, stubCamera, railgunViewFactoryDelegate);
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.IsAlive = true;
            playerView.AddPlayer(player, MockRepository.GenerateStub<IPlayerSettings>());
            playerView.AddPlayer(player2, MockRepository.GenerateStub<IPlayerSettings>());

            playerView.Generate();

            stubRailGunView2.AssertWasCalled(me => me.Draw(Arg<IRailGun>.Is.Anything, Arg<Matrix>.Is.Anything));
            stubRailGunView1.AssertWasCalled(me => me.Draw(Arg<IRailGun>.Is.Anything, Arg<Matrix>.Is.Anything));
        }
    }
}
