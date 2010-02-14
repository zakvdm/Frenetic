using System;

using Frenetic;

using NUnit.Framework;
using Frenetic.Physics;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.UserInput;
using Frenetic.Player;
using Frenetic.Gameplay.Level;
using Frenetic.Gameplay.Weapons;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameInputTests
    {
        IKeyboard stubKeyboard;
        IMouse stubMouse;
        IKeyMapping stubKeyMapping;
        GameInput gameInput;
        [SetUp]
        public void SetUp()
        {
            stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubMouse = MockRepository.GenerateStub<IMouse>();
            stubKeyMapping = MockRepository.GenerateStub<IKeyMapping>();
            gameInput = new GameInput(stubKeyMapping, stubKeyboard, stubMouse);
        }

        [Test]
        public void ShouldAllowRegisteringMultipleKeysInMapping()
        {
            var keyMapping = new KeyMapping();

            keyMapping[GameKey.MoveRight].Keyboard.Add(Keys.D);
            keyMapping[GameKey.MoveRight].Keyboard.Add(Keys.Right);
            keyMapping[GameKey.MoveRight].Mouse.Add(MouseKeys.Right);

            Assert.AreEqual(2, keyMapping[GameKey.MoveRight].Keyboard.Count);
            Assert.AreEqual(1, keyMapping[GameKey.MoveRight].Mouse.Count);
        }

        [Test]
        public void ShouldReadMappingAndCheckKeyboard()
        {
            stubKeyMapping.Stub(km => km[GameKey.Jump]).Return(new FreneticKeys() { Keyboard = { Keys.Space } });
            stubKeyboard.Stub(k => k.IsKeyDown(Keys.Space)).Return(true);

            Assert.IsTrue(gameInput.IsGameKeyDown(GameKey.Jump));
        }
        [Test]
        public void ShouldCheckMouseForKeyPressesToo()
        {
            stubKeyMapping.Stub(km => km[GameKey.Jump]).Return(new FreneticKeys() { Mouse = { MouseKeys.Right } });
            stubMouse.Stub(m => m.IsKeyDown(MouseKeys.Right)).Return(true);

            Assert.IsTrue(gameInput.IsGameKeyDown(GameKey.Jump));
        }

        [Test]
        public void ShouldCheckEachKeyMapped()
        {
            var keyboardList = new List<Keys>() { Keys.A, Keys.B };
            var mouseList = new List<MouseKeys>() { MouseKeys.Left, MouseKeys.Right };
            stubKeyMapping.Stub(km => km[GameKey.Shoot]).Return(new FreneticKeys() { Keyboard = keyboardList, Mouse = mouseList });

            stubKeyboard.Stub(k => k.IsKeyDown(Keys.B)).Return(true);

            Assert.IsTrue(gameInput.IsGameKeyDown(GameKey.Shoot));

            stubKeyboard.BackToRecord(BackToRecordOptions.All);

            Assert.IsFalse(gameInput.IsGameKeyDown(GameKey.Shoot));
            stubMouse.Stub(m => m.IsKeyDown(MouseKeys.Right)).Return(true);

            Assert.IsTrue(gameInput.IsGameKeyDown(GameKey.Shoot));
        }
    }
}