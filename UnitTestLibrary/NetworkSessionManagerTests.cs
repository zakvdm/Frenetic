using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Network;
using Autofac.Builder;
using Frenetic.Network.Lidgren;
using Autofac;
using Lidgren.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetworkSessionManagerTests
    {
        [Test]
        public void CanBuildNetworkSessionsWithAutofac()
        {
            var builder = new ContainerBuilder();
            builder.Register(MockRepository.GenerateStub<INetServer>()).SingletonScoped();
            builder.Register(MockRepository.GenerateStub<INetClient>()).SingletonScoped();
            builder.Register(x => new NetConfiguration("Frenetic")).FactoryScoped();
            builder.Register<LidgrenServerNetworkSession>();
            builder.Register<LidgrenClientNetworkSession>();
            builder.Register(MockRepository.GenerateStub<IMessageSerializer>()).SingletonScoped();
            
            var container = builder.Build();

            var serverNetworkSession = container.Resolve<LidgrenServerNetworkSession>();
            Assert.IsNotNull(serverNetworkSession);

            var clientNetworkSession = container.Resolve<LidgrenClientNetworkSession>();
            Assert.IsNotNull(clientNetworkSession);
        }
        
        [Test]
        public void CorrectlyStartsServerNetworkSession()
        {
            IServerNetworkSession stubServerNetworkSession = MockRepository.GenerateStub<IServerNetworkSession>();
            NetworkSessionManager networkSessionManager = new NetworkSessionManager(null, stubServerNetworkSession);

            networkSessionManager.Start(20);

            stubServerNetworkSession.AssertWasCalled(x => x.Create(20));
        }

        [Test]
        public void CorrectlyJoinsNetworkSession()
        {
            IClientNetworkSession stubClientNetworkSession = MockRepository.GenerateStub<IClientNetworkSession>();
            NetworkSessionManager networkSessionManager = new NetworkSessionManager(stubClientNetworkSession, null);

            networkSessionManager.Join(30);

            stubClientNetworkSession.AssertWasCalled(x => x.Join(Arg<int>.Is.Equal(30)));
        }

        // TODO: Fix this test
        // [Test]
        public void CorrectlyShutsDownClientSession()
        {
            IClientNetworkSession stubClientNetworkSession = MockRepository.GenerateStub<IClientNetworkSession>();
            new NetworkSessionManager(stubClientNetworkSession, null);
            

            stubClientNetworkSession.AssertWasCalled(x => x.Shutdown(Arg<string>.Is.Anything));
        }
    }
}
