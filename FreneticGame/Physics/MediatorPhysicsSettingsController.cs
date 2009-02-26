using System;

namespace Frenetic.Physics
{
    public class MediatorPhysicsSettingsController
    {
        public const string GravityString = "Gravity";

        public MediatorPhysicsSettingsController(PhysicsSettings physicsSettings, IMediator mediator)
        {
            _physicsSettings = physicsSettings;
            _mediator = mediator;

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            _mediator.Register(GravityString, GravityCommand);
        }

        #region Delegates
        string GravityCommand(string value)
        {
            if (value == null) // GETTER:
                return _physicsSettings.Gravity.ToString();

            // SETTER:
            try
            {
                _physicsSettings.Gravity = Convert.ToSingle(value);
            }
            catch { }
            return null;
        }
        #endregion

        PhysicsSettings _physicsSettings;
        IMediator _mediator;
    }
}
