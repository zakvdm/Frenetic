using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Engine.Console
{
    public class GameConsoleAppender : log4net.Appender.AppenderSkeleton
    {
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            GameConsoleLog.Add(RenderLoggingEvent(loggingEvent));
        }

        public Log<string> GameConsoleLog { get; set; }
    }
}
