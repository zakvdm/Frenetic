using System;
using Autofac;
using Autofac.Builder;

namespace Frenetic.Autofac
{
    class GameSessionModule : Module
    {
        public bool IsAServer { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<GameSessionController>().SingletonScoped();
            builder.Register<GameSessionView>().SingletonScoped();

            if (IsAServer)
            {
                builder.Register((c, p) =>
                    {
                        IGameSession gameSession = new GameSession();
                        var gameSessionController = c.Resolve<GameSessionController>
                            (
                            new TypedParameter(typeof(IPlayer), null),
                            new TypedParameter(typeof(ICamera), null),
                            new TypedParameter(typeof(bool), true)
                            );
                        var gameSessionView = c.Resolve<GameSessionView>();
                        gameSession.GameSessionController = gameSessionController;
                        gameSession.GameSessionView = gameSessionView;
                        return gameSession;
                    });
            }
            else
            {
                builder.Register((c, p) =>
                    {
                        IGameSession gameSession = new GameSession();
                        var gameSessionController = c.Resolve<GameSessionController>
                            (
                            new TypedParameter(typeof(IPlayer), c.Resolve<IPlayer>(new NamedParameter("LocalPlayer", true))),
                            new TypedParameter(typeof(bool), false)
                            );
                        var gameSessionView = c.Resolve<GameSessionView>();
                        gameSession.GameSessionController = gameSessionController;
                        gameSession.GameSessionView = gameSessionView;
                        return gameSession;
                    });
            }
        }

    }
}
