using System;

namespace Frenetic
{
    public class MediatorPlayerSettingsController
    {
        public const string PlayerNameString = "PlayerName";

        public MediatorPlayerSettingsController(PlayerSettings playerSettings, IMediator mediator)
        {
            _playerSettings = playerSettings;
            _mediator = mediator;

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            _mediator.Register(PlayerNameString, PlayerNameCommand);
        }

        #region Delegates
        string PlayerNameCommand(string value)
        {
            if (value == null) // GETTER:
                return _playerSettings.PlayerName;

            // SETTER:
            _playerSettings.PlayerName = value;
            return null;
        }
        #endregion

        PlayerSettings _playerSettings;
        IMediator _mediator;
    }
}
