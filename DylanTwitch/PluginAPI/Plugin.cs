using System.Collections.Generic;

namespace DylanTwitch
{
    public abstract class Plugin
    {
        //public Dictionary<string, object> Settings;
        /// <summary>
        /// Custom Settings that get applied to users
        /// </summary>
        public Dictionary<string, string> CustomUserSettings = new Dictionary<string, string>();

        /// <summary>
        /// Load Method when plugin gets loaded
        /// TODO: Should maybe be renamed to Activate/Deactivate
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Unload Method when plugin should unload
        /// TODO: Should maybe be renamed to Deactivate/Activate
        /// </summary>
        public abstract void Unload();
    }
}