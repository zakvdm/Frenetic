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
    }
}
