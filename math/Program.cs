using System;

namespace math
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Game1 theGame = new Game1();
        static void Main(string[] args)
        {
            using (Game1 game = theGame)
            {
                game.Run();
            }
        }
    }
#endif
}

