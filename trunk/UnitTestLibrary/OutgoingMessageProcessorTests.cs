using System;
using System.Text;

using Frenetic;
using Lidgren.Network;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class OutgoingMessageProcessorTests
    {
        //[Test]
        public void CallsSendMessageOnNetworkSessionCorrectly()
        {
            var stubNetworkSession = MockRepository.GenerateStub<INetworkSession>();
            OutgoingMessageProcessor outgoingMP = new OutgoingMessageProcessor(stubNetworkSession);

            outgoingMP.Process("Hello baby!");

            //stubNetworkSession.AssertWasCalled(x => x.Send(Arg<string>.Is.Equal("Hello baby!"), 
            //                                        Arg<NetChannel>.Is.Equal(NetChannel.Unreliable)));
        }
    }
}
