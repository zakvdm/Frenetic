using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Builder;
using Microsoft.Xna.Framework;
using Frenetic.Engine.Overlay;
using Frenetic.Gameplay.HUD;
using Autofac;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Autofac
{
    public class OverlaysModule : Module
    {
        public Vector2 ScreenSize { get; set; }
        public int InputBoxHeight { get; set; }
        public ContentManager ContentManager { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            #region Console
            builder.Register<Mediator>().As<IMediator>().SingletonScoped();
            builder.Register<InputLine>().SingletonScoped();
            builder.Register<CommandConsole>().As<ICommandConsole>().SingletonScoped();
            builder.Register<MessageConsole>().As<IMessageConsole>().SingletonScoped();
            builder.Register<InputOverlayView>().SingletonScoped();
            builder.Register<LogOverlayView<string>>().SingletonScoped();
            builder.Register<LogOverlayView<ChatMessage>>().SingletonScoped();
            builder.Register<PossibleCommandsLogHudView>().SingletonScoped();
            builder.Register<ConsoleController>().SingletonScoped();
            builder.Register<Log<string>>().FactoryScoped();
            builder.Register<Log<ChatMessage>>().FactoryScoped();
            builder.Register<GameStateSender>().ContainerScoped();
            builder.Register<GameStateProcessor>().ContainerScoped();
            
            builder.Register((c, p) => { return CreateConsoleOverlaySetView(c, p); }).SingletonScoped();  // WHY CAN'T I USE THE METHOD NAME DIRECTLY?
            #endregion

            // HUD:
            builder.Register<ScoreOverlayView>().ContainerScoped();
            builder.Register<HudController>().ContainerScoped();

            builder.Register((c, p) => { return CreateHudOverlaySetView(c, p); }).ContainerScoped();
        }

        ConsoleOverlaySetView CreateConsoleOverlaySetView(IContext container, IEnumerable<Parameter> parameters)
        {
            ITexture consoleTexture = container.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), ContentManager.Load<Texture2D>("Textures/blank")));
            IFont consoleFont = container.Resolve<IFont>(new TypedParameter(typeof(SpriteFont), ContentManager.Load<SpriteFont>("Fonts/detailsFont")));
            int edgeGap = OverlaySetView.EDGE_GAP;

            Rectangle consoleWindow = new Rectangle(edgeGap, edgeGap, (int)(ScreenSize.X / 2) - 30 - edgeGap, (int)ScreenSize.Y - InputBoxHeight - 3 * edgeGap);
            Rectangle messageWindow = new Rectangle((int)(ScreenSize.X / 2) + 30 + edgeGap, edgeGap, (int)(ScreenSize.X / 2) - 30 - 2 * edgeGap, (int)(ScreenSize.Y / 2) - 2 * edgeGap);
            Rectangle inputWindow = new Rectangle(edgeGap, (int)ScreenSize.Y - edgeGap - InputBoxHeight, (int)ScreenSize.X - 2 * edgeGap, InputBoxHeight);
            Rectangle possibleCompletionsWindow = new Rectangle((int)(consoleWindow.Right / 2), consoleWindow.Bottom - edgeGap, (int)(messageWindow.Width / 2), 0);

            var fontParameter = new TypedParameter(typeof(IFont), consoleFont);
            var commandConsoleParameter = new TypedParameter(typeof(IConsole<string>), container.Resolve<ICommandConsole>());
            var messageConsoleParameter = new TypedParameter(typeof(IConsole<ChatMessage>), container.Resolve<IMessageConsole>());
            var inputView = container.Resolve<InputOverlayView>(fontParameter, new NamedParameter("inputWindow", inputWindow));
            var commandConsoleView = container.Resolve<LogOverlayView<string>>(fontParameter, commandConsoleParameter, new NamedParameter("logWindow", consoleWindow), new TypedParameter(typeof(Color), Color.White));
            var messageConsoleView = container.Resolve<LogOverlayView<ChatMessage>>(fontParameter, messageConsoleParameter, new NamedParameter("logWindow", messageWindow), new TypedParameter(typeof(Color), Color.Green));
            var possibleCommandsView = container.Resolve<PossibleCommandsLogHudView>(fontParameter, new TypedParameter(typeof(ICommandConsole), container.Resolve<ICommandConsole>()), new NamedParameter("templateWindow", possibleCompletionsWindow), new TypedParameter(typeof(Color), Color.Yellow));

            return new ConsoleOverlaySetView(inputView, commandConsoleView, messageConsoleView, possibleCommandsView, container.Resolve<ISpriteBatch>(), consoleTexture, consoleFont);
        }
        HudOverlaySetView CreateHudOverlaySetView(IContext container, IEnumerable<Parameter> parameters)
        {
            ITexture consoleTexture = container.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), ContentManager.Load<Texture2D>("Textures/blank")));
            IFont consoleFont = container.Resolve<IFont>(new TypedParameter(typeof(SpriteFont), ContentManager.Load<SpriteFont>("Fonts/detailsFont")));
            int edgeGap = OverlaySetView.EDGE_GAP;

            Rectangle scoreWindow = new Rectangle((int)(ScreenSize.X / 2) + 30 + edgeGap, (int)(ScreenSize.Y / 2) + edgeGap, (int)(ScreenSize.X / 2) - 30 - 2 * edgeGap, (int)(ScreenSize.Y / 2) - InputBoxHeight - 3 * edgeGap);
            var scoreView = container.Resolve<ScoreOverlayView>(new TypedParameter(typeof(IFont), consoleFont), new NamedParameter("scoreWindow", scoreWindow));

            return new HudOverlaySetView(scoreView, container.Resolve<ISpriteBatch>(), consoleTexture, consoleFont);
        }

    }
}
