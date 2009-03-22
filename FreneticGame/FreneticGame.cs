using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Autofac;
using Autofac.Builder;
using Lidgren.Network;
using Frenetic.Network.Lidgren;
using Frenetic.Network;
using Frenetic.Graphics;
using Frenetic.Physics;
using Frenetic.Autofac;
using Frenetic.Level;
using Frenetic.UserInput;

namespace Frenetic
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FreneticGame : Game
    {
        GraphicsDeviceManager graphics;

        ScreenManager screenManager;

        public FreneticGame()
        {
            // initialize the graphics device manager
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_1_1;
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

            Content.RootDirectory = "Content";

            // initialize the gamer-services component
            //   this component enables Live sign-in functionality
            //   and updates the Gamer.SignedInGamers collection.
            //Components.Add(new GamerServicesComponent(this));

            // initialize the screen manager
            screenManager = new ScreenManager(this, Content);
            Components.Add(screenManager);

            // TODO: REMOVE:
            Components.Add(new Frenetic.MyConsole.Components.FPS(this));

            this.IsFixedTimeStep = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Container = BuildContainer();

            // Console:
            _consoleView = CreateConsoleView();
            _consoleController = Container.Resolve<GameConsoleController>();

            CreateMediatorControllers();

            PreloadTextures();

            MainMenuScreen = Container.Resolve<IScreenFactory>().MakeMainMenuScreen(Container);
        }

        protected override void Update(GameTime gameTime)
        {
            _consoleController.Process(gameTime.ElapsedGameTime.Ticks);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            _consoleView.Generate();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (Container != null)
            {
                Container.Dispose();
            }
            base.Dispose(disposing);
        }

        GameConsoleView CreateConsoleView()
        {
            ITexture consoleTexture = Container.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), Content.Load<Texture2D>("Textures/blank")));
            IFont consoleFont = Container.Resolve<IFont>(new TypedParameter(typeof(SpriteFont), Content.Load<SpriteFont>("Fonts/detailsFont")));
            int edgeGap = 4;
            int inputWindowHeight = 24;
            return Container.Resolve<GameConsoleView>(
                            new NamedParameter("inputWindow", new Rectangle(edgeGap, _screenHeight - edgeGap - inputWindowHeight, _screenWidth - 2 * edgeGap, inputWindowHeight)),
                            new NamedParameter("commandWindow", new Rectangle(edgeGap, edgeGap, (_screenWidth / 2) - 30 - edgeGap, _screenHeight - inputWindowHeight - 3 * edgeGap)),
                            new NamedParameter("messageWindow", new Rectangle((_screenWidth / 2) + 30 + edgeGap, edgeGap, (_screenWidth / 2) - 30 - 2 * edgeGap, (_screenHeight / 2) - 2 * edgeGap)),
                            new TypedParameter(typeof(ITexture), consoleTexture), new TypedParameter(typeof(IFont), consoleFont));
        }

        void CreateMediatorControllers()
        {
            PlayerSettings localPlayerSettings = Container.Resolve<PlayerSettings>();
            // NOTE: I'm creating the GameSessionFactory here with the same PlayerSettings that is passed to the MediatorController... This is a little hacky...
            //      At some point all of this needs to move out of the FreneticGame class...
            Container.Resolve<IGameSessionFactory>(new TypedParameter(typeof(PlayerSettings), localPlayerSettings));

            // Mediator Controllers:
            Container.Resolve<MediatorPlayerSettingsController>(new TypedParameter(typeof(PlayerSettings), localPlayerSettings));
            Container.Resolve<MediatorPhysicsSettingsController>();
        }

        void PreloadTextures()
        {
            Container.Resolve<ITextureBank<PlayerTextures>>();
        }

        IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            #region XNA
            // NOTE: This is order sensitive because disposing of the GraphicsDeviceManager also disposes the ContentManager, and we don't want that to happen twice...
            // Needs to be fixed somehow...
            builder.Register<ContentManager>(Content).SingletonScoped();
            builder.Register<XnaContentManager>(new XnaContentManager(Content)).As<IContentManager>().SingletonScoped();
            builder.Register<Game>(this);
            builder.Register<GraphicsDevice>(graphics.GraphicsDevice);
            #endregion

            #region Menus
            builder.Register<ScreenManager>(screenManager).SingletonScoped();
            builder.Register<ScreenFactory>().As<IScreenFactory>().SingletonScoped();
            builder.Register<MainMenuScreen>().SingletonScoped();
            #endregion

            #region Networking
            builder.Register(new NetServer(new NetConfiguration("Frenetic")));
            builder.Register(new NetClient(new NetConfiguration("Frenetic")));
            builder.Register<NetServerWrapper>().As<INetServer>().ContainerScoped();
            builder.Register<NetClientWrapper>().As<INetClient>().ContainerScoped();
            builder.Register<LidgrenServerNetworkSession>().As<IServerNetworkSession>().ContainerScoped();
            builder.Register<LidgrenClientNetworkSession>().As<IClientNetworkSession>().ContainerScoped();
            builder.Register<IncomingMessageQueue>().As<IIncomingMessageQueue>().ContainerScoped();
            builder.Register<OutgoingMessageQueue>().As<IOutgoingMessageQueue>().ContainerScoped();
            builder.Register<XmlMessageSerializer>().As<IMessageSerializer>().ContainerScoped();
            #endregion

            #region Graphics
            builder.Register<Viewport>(graphics.GraphicsDevice.Viewport);
            builder.Register<SpriteFont>(screenManager.Font);
            builder.Register<SpriteBatch>(screenManager.SpriteBatch);
            builder.Register<XnaSpriteBatch>().As<ISpriteBatch>().FactoryScoped();
            builder.Register<XnaTexture>().As<ITexture>().FactoryScoped();
            builder.Register<XnaFont>().As<IFont>().FactoryScoped();
            #endregion

            #region GameSession
            builder.Register<GameSessionFactory>().As<IGameSessionFactory>().SingletonScoped();
            builder.Register<GameSession>().As<IGameSession>().ContainerScoped();
            builder.Register<GameSessionController>().ContainerScoped();
            builder.Register<GameSessionView>().ContainerScoped();
            #endregion

            #region Player
            builder.Register<PlayerSettings>().FactoryScoped();
            builder.Register<Player>().As<IPlayer>().FactoryScoped();
            builder.Register<PlayerView>().FactoryScoped();
            builder.RegisterGeneratedFactory<Player.Factory>(new TypedService(typeof(IPlayer)));
            builder.RegisterGeneratedFactory<PlayerView.Factory>(new TypedService(typeof(PlayerView)));
            builder.Register((c) => (IBoundaryCollider)new WorldBoundaryCollider(_screenWidth, _screenHeight));
            builder.Register<KeyboardPlayerController>().ContainerScoped();
            builder.Register<NetworkPlayerView>().FactoryScoped();

            builder.Register<XnaTextureBank<PlayerTextures>>().As<ITextureBank<PlayerTextures>>().SingletonScoped();
            #endregion

            #region Physics
            builder.RegisterModule(new PhysicsModule() { Gravity = _gravity });
            #endregion

            #region Level
            builder.Register<LevelPiece>().FactoryScoped();
            builder.RegisterGeneratedFactory<LevelPiece.Factory>(new TypedService(typeof(LevelPiece)));
            builder.Register<DumbLevelLoader>().As<ILevelLoader>().ContainerScoped();
            builder.Register<Frenetic.Level.Level>().ContainerScoped();
            builder.Register<LevelController>().ContainerScoped();
            builder.Register<LevelView>().ContainerScoped();
            #endregion

            #region Console
            builder.Register<Mediator>().As<IMediator>().SingletonScoped();
            builder.Register<GameConsole>().As<IGameConsole>().SingletonScoped();
            builder.Register<GameConsoleView>().SingletonScoped();
            builder.Register<GameConsoleController>().SingletonScoped();
            #endregion

            #region Mediator Controllers
            builder.Register<MediatorPlayerSettingsController>().SingletonScoped();
            builder.Register<MediatorPhysicsSettingsController>().SingletonScoped();
            #endregion

            // RAYCASTER:
            builder.Register<DumbRayCaster>().SingletonScoped();
            builder.Register<DumbRayCasterTestController>().ContainerScoped();

            // CAMERA:
            builder.Register((c, p) => (ICamera)new Camera(p.TypedAs<IPlayer>(), new Vector2(_screenWidth, _screenHeight))).ContainerScoped();
            //builder.Register<Camera>().As<ICamera>().FactoryScoped();

            // CROSSHAIR:
            builder.Register<Crosshair>().As<ICrosshair>().ContainerScoped();
            builder.Register<CrosshairView>().ContainerScoped();

            // KEYBOARD:
            builder.Register<XnaKeyboard>().As<IKeyboard>().SingletonScoped();

            // MOUSE:
            builder.Register<FreneticMouse>().As<IMouse>().SingletonScoped();


            return builder.Build();
        }

        MainMenuScreen MainMenuScreen { get; set; }
        GameConsoleView _consoleView;
        GameConsoleController _consoleController;
        IContainer Container { get; set; }
        const int _screenWidth = 800;
        const int _screenHeight = 600;
        Vector2 _gravity = new Vector2(0, 2);
    }
}
