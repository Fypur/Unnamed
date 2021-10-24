using System;

namespace Basic_platformer
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Platformer())
                game.Run();
        }
    }
}
