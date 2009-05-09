using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public class Quitter
    {
        public Quitter(ISettingsPersister settingsSaver, IGame game)
        {
            _settingsSaver = settingsSaver;
            _game = game;
        }

        public void Quit()
        {
            _settingsSaver.SaveSettings();
            _game.Exit();
        }

        ISettingsPersister _settingsSaver;
        IGame _game;
    }
}
