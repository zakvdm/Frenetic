using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.UserInput;
using Autofac.Builder;
using Microsoft.Xna.Framework.Input;

namespace Frenetic.Autofac
{
    public class InputModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // KEYBOARD:
            builder.Register<XnaKeyboard>().As<IKeyboard>().SingletonScoped();

            // MOUSE:
            builder.Register<XnaMouse>().As<IMouse>().SingletonScoped();

            // CROSSHAIR:
            builder.Register<Crosshair>().As<ICrosshair>().ContainerScoped();
            builder.Register<CrosshairView>().ContainerScoped();

            builder.Register(container =>
                {
                    var mapping = new KeyMapping();
                    mapping[GameKey.MoveLeft].Keyboard.AddRange(new List<Keys>() { Keys.Left, Keys.A });
                    mapping[GameKey.MoveRight].Keyboard.AddRange(new List<Keys>() { Keys.Right, Keys.D });
                    mapping[GameKey.Jump].Keyboard.AddRange(new List<Keys>() { Keys.Up, Keys.W });
                    mapping[GameKey.Jump].Mouse.Add(MouseKeys.Right);
                    mapping[GameKey.Shoot].Mouse.Add(MouseKeys.Left);
                    mapping[GameKey.RocketLauncher].Keyboard.Add(Keys.Q);
                    mapping[GameKey.RailGun].Keyboard.Add(Keys.E);
                    return mapping;
                }).As<IKeyMapping>().SingletonScoped();
            builder.Register<GameInput>().As<IGameInput>().SingletonScoped();
        }

    }
}
