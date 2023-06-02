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
                if (args.Length >= 1 && args[0] != "run"){
                    Platformer.InitLevel = args[0];
                    Console.WriteLine(args[0]);
                }

                if(!(Environment.CurrentDirectory.EndsWith("Debug/net6.0") || Environment.CurrentDirectory.EndsWith("Debug\\net6.0")))
                    Environment.CurrentDirectory = Environment.CurrentDirectory + "/bin/x64/Debug/net6.0";
#endif

                game.Run();
            }
        }
    }
}
