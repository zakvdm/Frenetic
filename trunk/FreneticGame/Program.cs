using System;

namespace Frenetic
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (FreneticGame game = new FreneticGame())
            {
                game.Run();
            }
        }
    }
}

