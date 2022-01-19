using System;
using System.Collections.Generic;
using System.Text;
using FMOD;
using FMOD.Studio;

namespace Basic_platformer
{
    public static class Audio
    {
        public static FMOD.Studio.System system;
        public static Dictionary<string, Sound> Sounds = new Dictionary<string,Sound>();

        public static void Initialize()
        {
            FMOD.Studio.System.create(out system);
            system.initialize(1024, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
        }

        public static void Update()
            => system.update();

        public static void Finish()
        {
            system.release();
        }
    }
}
