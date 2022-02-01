﻿using System;
using System.Collections.Generic;
using System.Text;
using FMOD;
using FMOD.Studio;

namespace Basic_platformer
{
    public static class Audio
    {
        public static FMOD.Studio.System system;
        private static Dictionary<string, EventDescription> cachedEventDescriptions = new Dictionary<string,EventDescription>();

        public static void Initialize()
        {
            FMOD.Studio.System.create(out system);
            system.initialize(1024, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);

            Load();
        }

        private static void Load()
        {
            system.loadBankFile("FMOD/Banks/Master.bank", LOAD_BANK_FLAGS.NORMAL, out Bank b);
            system.loadBankFile("FMOD/Banks/Master.strings.bank", LOAD_BANK_FLAGS.NORMAL, out Bank b1);
            system.loadBankFile("FMOD/Banks/GamingAvance.bank", LOAD_BANK_FLAGS.NORMAL, out Bank b2);
        }

        public static EventInstance PlayEvent(string eventName)
        {
            EventInstance instance = CreateEventInstance(eventName);
            instance.start();
            instance.release();
            return instance;
        }

        public static void StopEvent(EventInstance eventInstance, bool allowFadeOut = true)
            => eventInstance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);

        public static void ChangeParameters(EventInstance eventInstance, string paramName, float value)
            => eventInstance.setParameterByName(paramName, value);

        private static EventInstance CreateEventInstance(string eventPath)
        {
            EventDescription eventDesc = GetEventDescription(eventPath);

            if (!eventDesc.isValid())
                throw new Exception("Event Problems on Instance");

            eventDesc.createInstance(out EventInstance instance);
            return instance;
        }

        private static EventDescription GetEventDescription(string path)
        {
            if(cachedEventDescriptions.TryGetValue(path, out EventDescription eventDescription))
                return eventDescription;

            switch (system.getEvent(path, out eventDescription))
            {
                case RESULT.OK:
                    eventDescription.loadSampleData();
                    cachedEventDescriptions.Add(path, eventDescription);
                    return eventDescription;
                default:
#if DEBUG
                    throw new Exception("Event problems");
#endif
            }
        }

        public static void Update()
            => system.update();

        public static void Finish()
        {
            system.unloadAll();
            system.release();
        }
    }
}
