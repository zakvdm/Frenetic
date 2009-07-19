using System;

namespace Frenetic
{
    public interface ISettingsPersister
    {
        void LoadSettings();
        void SaveSettings();
    }
}
