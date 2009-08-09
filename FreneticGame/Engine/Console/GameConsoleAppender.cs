using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Engine.Console
{
    public class GameConsoleAppender : log4net.Appender.AppenderSkeleton
    {
        public GameConsoleAppender()
        {
            System.Console.WriteLine("YAY, I GOT CALLED!");
        }
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            GameConsoleLog.AddMessage(RenderLoggingEvent(loggingEvent));
        }

        public Log<string> GameConsoleLog { get; set; }
    }
}
