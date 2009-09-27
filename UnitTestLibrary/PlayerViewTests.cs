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
        IPlayerList playerList;
        ITexture stubTexture;
        ITextureBank<PlayerTexture> _stubTextureBank;
        ISpriteBatch stubSpriteBatch;
        ICamera stubCamera;
        IRailGunView stubRailGunView;
        IBubbleTextDrawer stubBubbleText;
        IPlayer player;
        PlayerView playerView;
        [SetUp]
        public void SetUp()
        {
            playerList = MockRepository.GenerateStub<IPlayerList>();
            playerList.Stub(me => me.Players).Return(new List<IPlayer>());
            stubTexture = MockRepository.GenerateStub<ITexture>();
            _stubTextureBank = MockRepository.GenerateStub<ITextureBank<PlayerTexture>>();
            _stubTextureBank.Stub(x => x[PlayerTexture.Ball]).Return(stubTexture);
            stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            stubCamera = MockRepository.GenerateStub<ICamera>();
            stubRailGunView = MockRepository.GenerateStub<IRailGunView>();
            stubBubbleText = MockRepository.GenerateStub<IBubbleTextDrawer>();
            player = MockRepository.GenerateStub<IPlayer>();
            player.Status = PlayerStatus.Alive;
            player.Stub(me => me.PlayerSettings).Return(new NetworkPlayerSettings());
            player.Stub(me => me.CurrentWeapon).Return(new RailGun(null));
            playerList.Players.Add(player);
            playerView = new PlayerView(playerList, _stubTextureBank, stubSpriteBatch, stubCamera, stubRailGunView, stubBubbleText);
        }

        // EVENT REGISTRATIONS:
        [Test]
        public void RegistersForPlayerAddedEvent()
        {
            playerList.AssertWasCalled(me => me.PlayerAdded += Arg<Action<IPlayer>>.Is.Anything);
        }
        [Test]
        public void RegistersForHealthChangedEventOnNewPlayers()
        {
            var stubPlayer = MockRepository.GenerateStub<IPlayer>();
            playerList.Raise(me => me.PlayerAdded += null, stubPlayer);

            stubPlayer.AssertWasCalled(me => me.HealthChanged += Arg<Action<IPlayer, int>>.Is.Anything);
        }

        [Test]
        public void HandlesPlayerHealthChangedEvent()
        {
            playerList.Raise(me => me.PlayerAdded += null, playerList.Players[0]);
            playerList.Players[0].Raise(me => me.HealthChanged += null, playerList.Players[0], 30);

            stubBubbleText.AssertWasCalled(me => me.AddText(Arg<string>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything));
        }

        [Test]
        public void DrawsEachPlayer()
        {
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.Status = PlayerStatus.Alive;
            player2.Stub(me => me.PlayerSettings).Return(new NetworkPlayerSettings());
            playerList.Players.Add(player2);

            playerView.Generate(1f);

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

            playerView.Generate(1f);

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
            playerView = new PlayerView(playerList, _stubTextureBank, stubSpriteBatch, camera, stubRailGunView, stubBubbleText);

            playerView.Generate(1f);

            stubSpriteBatch.AssertWasCalled(x => x.Begin(Arg<Matrix>.Is.Equal(camera.TranslationMatrix)));
        }

        [Test]
        public void DoesntDrawDeadPlayer()
        {
            player.Status = PlayerStatus.Dead;

            playerView.Generate(1f);

            stubSpriteBatch.AssertWasNotCalled(me => me.Draw(Arg<ITexture>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything, Arg<SpriteEffects>.Is.Anything, Arg<float>.Is.Anything));
        }

        // WEAPON:
        [Test]
        public void DrawsWeaponView()
        {
            stubCamera.Stub(me => me.TranslationMatrix).Return(Matrix.Identity);

            playerView.Generate(1f);

            stubRailGunView.AssertWasCalled(me => me.Draw(Matrix.Identity));
        }

        // BUBBLETEXT:
        [Test]
        public void DrawsBubbleText()
        {
            playerView.Generate(1f);

            stubBubbleText.AssertWasCalled(me => me.DrawText(stubSpriteBatch, 1f));
        }
    }
}
