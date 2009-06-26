using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Player;
using Frenetic.Weapons;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerViewTests
    {
        ITexture stubTexture;
        ITextureBank<PlayerTexture> _stubTextureBank;
        ISpriteBatch mockSpriteBatch;
        ICamera stubCamera;
        IRailGunView stubRailGunView;
        PlayerView playerView;
        [SetUp]
        public void SetUp()
        {
            
            stubTexture = MockRepository.GenerateStub<ITexture>();
            _stubTextureBank = MockRepository.GenerateStub<ITextureBank<PlayerTexture>>();
            _stubTextureBank.Stub(x => x[PlayerTexture.Ball]).Return(stubTexture);
            mockSpriteBatch = MockRepository.GenerateMock<ISpriteBatch>();
            stubCamera = MockRepository.GenerateStub<ICamera>();
            stubRailGunView = MockRepository.GenerateStub<IRailGunView>();
            playerView = new PlayerView(_stubTextureBank, mockSpriteBatch, stubCamera, stubRailGunView);
        }

        [Test]
        public void CanAddPlayer()
        {
            IPlayer player = MockRepository.GenerateStub<IPlayer>();
            playerView.AddPlayer(player, null);

            Assert.AreEqual(player, playerView.Players[0]);
        }

        [Test]
        public void CanRemovePlayer()
        {
            IPlayer player = MockRepository.GenerateStub<IPlayer>();
            playerView.AddPlayer(player, MockRepository.GenerateStub<IPlayerSettings>());

            playerView.RemovePlayer(player);

            Assert.AreEqual(0, playerView.Players.Count);
        }

        [Test]
        public void DrawsEachPlayer()
        {
            playerView.AddPlayer(MockRepository.GenerateStub<IPlayer>(), MockRepository.GenerateStub<IPlayerSettings>());
            playerView.AddPlayer(MockRepository.GenerateStub<IPlayer>(), MockRepository.GenerateStub<IPlayerSettings>());
            mockSpriteBatch.Expect(me => me.Draw(Arg<ITexture>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<SpriteEffects>.Is.Anything, Arg<float>.Is.Anything)).
                                Repeat.Twice();

            playerView.Generate();

            mockSpriteBatch.VerifyAllExpectations();
        }

        [Test]
        public void CallsDrawWithCorrectParameters()
        {
            IPlayer player = new Player(null, null);
            IPlayerSettings settings = new NetworkPlayerSettings();
            settings.Texture = PlayerTexture.Ball;
            stubTexture.Stub(x => x.Width).Return(100);
            stubTexture.Stub(x => x.Height).Return(200);
            player.Position = new Vector2(1, 1);
            playerView.AddPlayer(player, settings);

            playerView.Generate();

            mockSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture), Arg<Vector2>.Is.Equal(new Vector2(1, 1)),
                Arg<Rectangle>.Is.Equal(null), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f),
                Arg<Vector2>.Is.Equal(new Vector2(100 / 2f, 200 / 2f)),
                Arg<Vector2>.Is.Equal(new Vector2(1, 1)), Arg<SpriteEffects>.Is.Equal(SpriteEffects.None), Arg<float>.Is.Equal(1f)));
            mockSpriteBatch.AssertWasCalled(x => x.End());
        }

        [Test]
        public void UsesCameraCorrectly()
        {
            IPlayer player = new Player(null, null);
            IPlayerSettings settings = new NetworkPlayerSettings();
            ICamera camera = new Camera(player, new Vector2(100, 100));
            playerView = new PlayerView(_stubTextureBank, mockSpriteBatch, camera, MockRepository.GenerateStub<IRailGunView>());
            playerView.AddPlayer(player, settings);

            playerView.Generate();

            mockSpriteBatch.AssertWasCalled(x => x.Begin(Arg<Matrix>.Is.Equal(camera.TranslationMatrix)));
        }

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
    }
}
