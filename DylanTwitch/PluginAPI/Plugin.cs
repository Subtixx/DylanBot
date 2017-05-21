using System.Collections.Generic;

namespace DylanTwitch
{
    public abstract class Plugin
    {
        //public Dictionary<string, object> Settings;
        public Dictionary<string, string> CustomUserSettings = new Dictionary<string, string>();

        public abstract void Load();
        public abstract void Unload();
    }
}