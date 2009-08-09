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
using Frenetic.Player;
using System.Windows.Forms;
using Frenetic.Weapons;
using Frenetic.Engine;
using Frenetic.Gameplay.HUD;
using Frenetic.Gameplay;
using Frenetic.Engine.Overlay;

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

            // Disable the title bar and border:
            Form frm = (Form)Form.FromHandle(this.Window.Handle);
            frm.FormBorderStyle = FormBorderStyle.None;
            
            // Disable fixed timestep
            this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

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
            //Components.Add(new Frenetic.MyConsole.Components.EmitterTest(this, graphics));
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

            CreatePhysicsSystem();
            
            // Console:
            _consoleView = Container.Resolve<ConsoleOverlaySetView>();
            _consoleController = Container.Resolve<ConsoleController>
                        (
                            new NamedParameter("inputView", Container.Resolve<InputOverlayView>()), 
                            new NamedParameter("commandConsoleView", Container.Resolve<LogOverlayView<string>>()),
                            new NamedParameter("messageConsoleView", Container.Resolve<LogOverlayView<ChatMessage>>()),
                            new NamedParameter("possibleCommandsView", Container.Resolve<PossibleCommandsLogHudView>())
                        );
            
            // NOTE: order is important here, first register, then set!
            RegisterTweakableProperties();
            LoadSettings();

            PreloadTextures();

            MainMenuScreen = Container.Resolve<IScreenFactory>().MakeMainMenuScreen(Container);
        }

        protected override void Update(GameTime gameTime)
        {
            _consoleController.Process((float)gameTime.ElapsedGameTime.TotalMilliseconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(new Color(20, 20, 20));

            base.Draw(gameTime);

            // NOTE: The console is drawn over everything else, so it's drawn last...
            _consoleView.Generate();
        }

        protected override void Dispose(bool disposing)
        {
            if (Container != null)
            {
                Container.Dispose();
            }
            base.Dispose(disposing);
        }

        void LoadSettings()
        {
            ISettingsPersister settingsSaver = Container.Resolve<ISettingsPersister>();
            settingsSaver.LoadSettings();
        }

        void CreatePhysicsSystem()
        {
            IPhysicsSimulator simulator = Container.Resolve<IPhysicsSimulator>();
        }

        void RegisterTweakableProperties()
        {
            TweakablePropertiesLoader loader = Container.Resolve<TweakablePropertiesLoader>();

            loader.LoadTweakableProperties(Container.Resolve<LocalPlayerSettings>());
            loader.LoadTweakableProperties(Container.Resolve<PhysicsSettings>());
        }

        void PreloadTextures()
        {
            Container.Resolve<ITextureBank<PlayerTexture>>();
        }

        IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            #region XNA
            // NOTE: This is order sensitive because disposing of the GraphicsDeviceManager also disposes the ContentManager, and we don't want that to happen twice...
            // Needs to be fixed somehow...
            builder.Register<ContentManager>(Content).SingletonScoped();
            builder.Register<XnaContentManager>(new XnaContentManager(Content)).As<IContentManager>().SingletonScoped();
            builder.Register<XnaGame>(new XnaGame(this)).As<IGame>().SingletonScoped();
            builder.Register<GraphicsDevice>(graphics.GraphicsDevice).SingletonScoped();
            #endregion

            #region Engine
            builder.Register<Quitter>().SingletonScoped();
            builder.Register<SettingsPersister>().As<ISettingsPersister>().SingletonScoped();

            builder.Register<Frenetic.Engine.Timer>().As<ITimer>().ContainerScoped();

            builder.Register<log4net.ILog>((c, p) => log4net.LogManager.GetLogger(p.TypedAs<Type>())).FactoryScoped();
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
            builder.Register<LidgrenServerMessageSender>().As<IServerMessageSender>().ContainerScoped();
            builder.Register<LidgrenClientNetworkSession>().As<IClientNetworkSession>().ContainerScoped();
            builder.Register<IncomingMessageQueue>().As<IIncomingMessageQueue>().ContainerScoped();
            builder.Register<OutgoingMessageQueue>().As<IOutgoingMessageQueue>().ContainerScoped();
            builder.Register<XmlMessageSerializer>().As<IMessageSerializer>().ContainerScoped();

            builder.Register<LocalClient>().ContainerScoped();
            builder.Register<Client>().FactoryScoped();
            builder.RegisterGeneratedFactory<Client.Factory>(new TypedService(typeof(Client)));
            builder.Register<ServerSideClientFactory>().ContainerScoped();
            builder.Register<ClientSideClientFactory>().ContainerScoped();

            builder.Register<ClientInputSender>().ContainerScoped();
            builder.Register<ClientInputProcessor>().ContainerScoped();

            builder.Register<ClientStateTracker>().As<IClientStateTracker>().ContainerScoped();
            builder.Register<SnapCounter>().As<ISnapCounter>().ContainerScoped();
            builder.Register<ChatLogDifferByReference>().As<IChatLogDiffer>().ContainerScoped();

            builder.Register<NetworkPlayerProcessor>().As<INetworkPlayerProcessor>().ContainerScoped();
            #endregion

            #region Graphics
            builder.Register<Viewport>(graphics.GraphicsDevice.Viewport);
            builder.Register<SpriteFont>(screenManager.Font);
            builder.Register<SpriteBatch>(screenManager.SpriteBatch);
            builder.Register<XnaSpriteBatch>().As<ISpriteBatch>().FactoryScoped();
            builder.Register<XnaTexture>().As<ITexture>().FactoryScoped();
            builder.Register<XnaFont>().As<IFont>().FactoryScoped();
            builder.Register<XnaPrimitiveDrawer>().As<IPrimitiveDrawer>().ContainerScoped();
            #endregion

            #region GameSession
            builder.Register<GameSessionFactory>().As<IGameSessionFactory>().SingletonScoped();
            builder.Register<GameSession>().As<IGameSession>().ContainerScoped();
            builder.Register<GameSessionController>().ContainerScoped();
            builder.Register<GameSessionView>().ContainerScoped();
            #endregion

            #region Player
            builder.Register<PlayerUpdater>().ContainerScoped();
            builder.Register<NetworkPlayerSettings>().As<IPlayerSettings>().FactoryScoped();
            builder.Register<LocalPlayerSettings>().SingletonScoped();
            builder.Register<Frenetic.Player.Player>().As<IPlayer>().FactoryScoped();
            builder.Register<List<IPlayer>>().ContainerScoped();
            builder.Register<PlayerView>().ContainerScoped();
            builder.Register((c) => (IBoundaryCollider)new WorldBoundaryCollider(_screenWidth, _screenHeight));
            builder.Register<KeyboardPlayerController>().ContainerScoped();

            builder.Register<XnaTextureBank<PlayerTexture>>().As<ITextureBank<PlayerTexture>>().SingletonScoped();
            #endregion

            #region Physics
            builder.RegisterModule(new PhysicsModule() { Gravity = _gravity });
            #endregion

            #region Weapons
            builder.RegisterModule(new WeaponsModule() { ContentManager = new ContentManager(this.Services, "Content"), GraphicsDeviceService = this.graphics });
            #endregion

            #region Level
            builder.RegisterModule(new LevelModule());
            #endregion

            #region HUD
            builder.RegisterModule(new OverlaysModule() { ScreenSize = new Vector2(_screenWidth, _screenHeight), InputBoxHeight = 24, ContentManager = this.Content });
            #endregion
            // RAYCASTER:
            //builder.Register<DumbRayCaster>().SingletonScoped();
            //builder.Register<DumbRayCasterTestController>().ContainerScoped();

            // CAMERA:
            builder.Register((c, p) => (ICamera)new Camera(p.TypedAs<IPlayer>(), new Vector2(_screenWidth, _screenHeight))).ContainerScoped();

            // CROSSHAIR:
            builder.Register<Crosshair>().As<ICrosshair>().ContainerScoped();
            builder.Register<CrosshairView>().ContainerScoped();

            // KEYBOARD:
            builder.Register<XnaKeyboard>().As<IKeyboard>().SingletonScoped();

            // MOUSE:
            builder.Register<FreneticMouse>().As<IMouse>().SingletonScoped();

            // Mediator:
            builder.Register<TweakablePropertiesLoader>().SingletonScoped();


            return builder.Build();
        }

        MainMenuScreen MainMenuScreen { get; set; }
        OverlaySetView _consoleView;
        ConsoleController _consoleController;
        IContainer Container { get; set; }
        const int _screenWidth = 800;
        const int _screenHeight = 600;
        Vector2 _gravity = new Vector2(0, 0.0002f);
    }
}
