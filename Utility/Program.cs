using System;

namespace Platformer
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new Platformer())
            {
#if DEBUG
                if (args.Length >= 1)
                    Platformer.InitLevel = args[0];
#endif

                game.Run();
            }
        }
    }
}
