using System;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

namespace UnitTestLibrary
{
    [TestFixture]
    public class SettingsPersisterTests
    {
        [Test]
        public void CanSaveAndReloadSettings()
        {
            IMediator stubMediator = MockRepository.GenerateStub<IMediator>();
            SettingsPersister saver = new SettingsPersister(stubMediator, new CommandConsole(stubMediator, new Log<string>()));
            stubMediator.Stub(me => me.AvailableProperties).Return(new List<string>() { "Setting1", "Setting2" });
            stubMediator.Stub(me => me.Get("Setting1")).Return("5");
            stubMediator.Stub(me => me.Get("Setting2")).Return("100 200");

            saver.SaveSettings();
            saver.LoadSettings();

            stubMediator.AssertWasCalled(me => me.Set("Setting1", "5"));
            stubMediator.AssertWasCalled(me => me.Set("Setting2", "100 200"));
        }

        [Test]
        public void HandlesNonExistentSettingsFile()
        {
            IMediator stubMediator = MockRepository.GenerateStub<IMediator>();
            SettingsPersister saver = new SettingsPersister(stubMediator, new CommandConsole(stubMediator, new Log<string>()));
            stubMediator.Stub(me => me.AvailableProperties).Return(new List<string>() { "Setting1" });
            stubMediator.Stub(me => me.Get("Setting1")).Return("5");

            string path = Path.Combine(StorageContainer.TitleLocation, SettingsPersister.SettingsFileName);
            File.Delete(path);

            saver.LoadSettings();

            stubMediator.AssertWasNotCalled(me => me.Set(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }
    }
}
