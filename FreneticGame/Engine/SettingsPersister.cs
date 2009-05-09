using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace Frenetic
{
    public class SettingsPersister : ISettingsPersister
    {
        public const string SettingsFileName = "config.cfg";

        public SettingsPersister(IMediator mediator, ICommandConsole commandConsole)
        {
            _mediator = mediator;
            _commandConsole = commandConsole;
        }

        public void SaveSettings()
        {
            using (FileStream stream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
            {
                foreach (string property in _mediator.AvailableProperties)
                {
                    string line = property + " " + _mediator.Get(property);
                    writer.WriteLine(line);
                }

                writer.Close();
            }
        }
        public void LoadSettings()
        {
            using (FileStream stream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    
                    // Use the command console the parse the string and call Set() on the mediator:
                    // NOTE: This is a bit hacky, but probably better than doing the parsing of string commands in two places. I guess I should move ProcessInput out of the CommandConsole eventually
                    line = "/" + line;  // ProcessInput requires that commands are prefixed with "/"
                    _commandConsole.ProcessInput(line);
                }

                reader.Close();
            }
        }

        IMediator _mediator;
        ICommandConsole _commandConsole;

        string _path = Path.Combine(StorageContainer.TitleLocation, SettingsFileName);
    }
}
