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
        List<IPlayer> playerList;
        ITexture stubTexture;
        ITextureBank<PlayerTexture> _stubTextureBank;
        ISpriteBatch stubSpriteBatch;
        ICamera stubCamera;
        IRailGunView stubRailGunView;
        IPlayer player;
        PlayerView playerView;
        [SetUp]
        public void SetUp()
        {
            playerList = new List<IPlayer>();
            stubTexture = MockRepository.GenerateStub<ITexture>();
            _stubTextureBank = MockRepository.GenerateStub<ITextureBank<PlayerTexture>>();
            _stubTextureBank.Stub(x => x[PlayerTexture.Ball]).Return(stubTexture);
            stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            stubCamera = MockRepository.GenerateStub<ICamera>();
            stubRailGunView = MockRepository.GenerateStub<IRailGunView>();
            player = MockRepository.GenerateStub<IPlayer>();
            player.Status = PlayerStatus.Alive;
            player.Stub(me => me.PlayerSettings).Return(new NetworkPlayerSettings());
            player.Stub(me => me.CurrentWeapon).Return(new RailGun(null));
            playerList.Add(player);
            playerView = new PlayerView(playerList, _stubTextureBank, stubSpriteBatch, stubCamera, stubRailGunView);
        }

        [Test]
        public void DrawsEachPlayer()
        {
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.Status = PlayerStatus.Alive;
            player2.Stub(me => me.PlayerSettings).Return(new NetworkPlayerSettings());
            playerList.Add(player2);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(me => me.Draw(Arg<ITexture>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<SpriteEffects>.Is.Anything, Arg<float>.Is.Anything),
                o => o.Repeat.Twice());
        }

        [Test]
        public void CallsDrawWithCorrectParameters()
        {
            player.PlayerSettings.Texture = PlayerTexture.Ball;
            stubTexture.Stub(x => x.Width).Return(100);
            stubTexture.Stub(x => x.Height).Return(200);
            player.Position = new Vector2(1, 1);

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
            ICamera camera = new Camera(player, new Vector2(100, 100));
            playerView = new PlayerView(playerList, _stubTextureBank, stubSpriteBatch, camera, stubRailGunView);

            playerView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin(Arg<Matrix>.Is.Equal(camera.TranslationMatrix)));
        }

        [Test]
        public void DoesntDrawDeadPlayer()
        {
            player.Status = PlayerStatus.Dead;

            playerView.Generate();

            stubSpriteBatch.AssertWasNotCalled(me => me.Draw(Arg<ITexture>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<SpriteEffects>.Is.Anything, Arg<float>.Is.Anything));
        }

        // WEAPON:
        [Test]
        public void DrawsWeaponView()
        {
            stubCamera.Stub(me => me.TranslationMatrix).Return(Matrix.Identity);

            playerView.Generate();

            stubRailGunView.AssertWasCalled(me => me.Draw(Matrix.Identity));
        }
    }
}
