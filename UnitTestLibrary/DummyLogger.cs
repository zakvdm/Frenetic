using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Engine;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    public static class DummyLogger
    {
        static DummyLogger()
        {
            Factory.Stub(me => me.GetLogger(Arg<Type>.Is.Anything)).Return(MockRepository.GenerateStub<log4net.ILog>());
        }
        public static ILoggerFactory Factory = MockRepository.GenerateStub<ILoggerFactory>();
    }
}
