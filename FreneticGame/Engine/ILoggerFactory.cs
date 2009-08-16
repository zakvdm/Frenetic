using System;
using log4net;

namespace Frenetic.Engine
{
    public interface ILoggerFactory
    {
        ILog GetLogger(Type containingClassType);
    }

    public class log4netLoggerFactory : ILoggerFactory
    {
        public ILog GetLogger(Type containingClassType)
        {
            return log4net.LogManager.GetLogger(containingClassType);
        }
    }
}
