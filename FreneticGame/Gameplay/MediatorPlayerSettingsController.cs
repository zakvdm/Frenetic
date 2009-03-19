using System;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class MediatorPlayerSettingsController
    {
        public const string PlayerNameString = "PlayerName";
        public const string PlayerColorString = "PlayerColor";

        public MediatorPlayerSettingsController(PlayerSettings playerSettings, IMediator mediator)
        {
            _playerSettings = playerSettings;
            _mediator = mediator;

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            _mediator.Register(PlayerNameString, PlayerNameCommand);
            _mediator.Register(PlayerColorString, PlayerColorCommand);
        }

        #region Delegates
        string PlayerNameCommand(string value)
        {
            if (value == null) // GETTER:
                return _playerSettings.Name;

            // SETTER:
            _playerSettings.Name = value;
            return null;
        }

        string PlayerColorCommand(string value)
        {
            if (value == null) // GETTER:
                return _playerSettings.Color.R + " " + _playerSettings.Color.G + " " + _playerSettings.Color.B;

            // SETTER:
            try
            {
                Color tmpColor = _playerSettings.Color;
                string[] args = value.Split(new char[] { ' ' }, 3);
                tmpColor.R = (byte)(int.Parse(args[0]) % 255);
                tmpColor.G = (byte)(int.Parse(args[1]) % 255);
                tmpColor.B = (byte)(int.Parse(args[2]) % 255);
                _playerSettings.Color = tmpColor;
            }
            catch { }
            return null;
        }
        #endregion

        PlayerSettings _playerSettings;
        IMediator _mediator;
    }
}
