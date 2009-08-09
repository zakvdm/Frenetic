using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.UserInput;
using Microsoft.Xna.Framework.Input;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MenuInputStateTests
    {
        IKeyboard stubKeyboard;
        MenuInputState menuInputState;
        [SetUp]
        public void SetUp()
        {
            stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            menuInputState = new MenuInputState(stubKeyboard);
        }

        [Test]
        public void MenuUpWorks()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Up)).Return(true);
            Assert.IsTrue(menuInputState.MenuUp);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.Up)).Return(true);
            Assert.IsFalse(menuInputState.MenuUp);
        }

        [Test]
        public void MenuDownWorks()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Down)).Return(true);
            Assert.IsTrue(menuInputState.MenuDown);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.Down)).Return(true);
            Assert.IsFalse(menuInputState.MenuDown);
        }

        [Test]
        public void MenuSelectWorksWithSpace()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Space)).Return(true);
            Assert.IsTrue(menuInputState.MenuSelect);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.Space)).Return(true);
            Assert.IsFalse(menuInputState.MenuSelect);
        }
        [Test]
        public void MenuSelectWorksWithEnter()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Enter)).Return(true);
            Assert.IsTrue(menuInputState.MenuSelect);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.Enter)).Return(true);
            Assert.IsFalse(menuInputState.MenuSelect);
        }

        [Test]
        public void MenuCancelWorks()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Escape)).Return(true);
            Assert.IsTrue(menuInputState.MenuCancel);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.Escape)).Return(true);
            Assert.IsFalse(menuInputState.MenuCancel);
        }
        [Test]
        public void PauseGameWorks()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Escape)).Return(true);
            Assert.IsTrue(menuInputState.PauseGame);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.Escape)).Return(true);
            Assert.IsFalse(menuInputState.PauseGame);
        }
    }
}
