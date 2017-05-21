using System;
using System.IO;
using DylanTwitch;
using Newtonsoft.Json;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

namespace PointSystem
{
    public class PointSystemPlugin : Plugin
    {
        private const string SETTINGS_FILE = "Settings/PointSystem.config";
        public static PointSystemSettings Settings;

        public override void Load()
        {
            LoadSettingsFile();
            AddCustomUserSettings();

            Events.Register();
            Commands.Register();
        }

        private void AddCustomUserSettings()
        {
            // Custom user settings with default value of 0
            CustomUserSettings.Add("points", "0");
            foreach (var user in UserDatabase.Users)
            {
                if(!user.Value.CustomSettings.ContainsKey("points"))
                    user.Value.CustomSettings.Add("points", "0");
            }
        }

        private void LoadSettingsFile()
        {
            Settings = File.Exists(SETTINGS_FILE)
                ? JsonConvert.DeserializeObject<PointSystemSettings>(File.ReadAllText(SETTINGS_FILE))
                : new PointSystemSettings();
        }

        public override void Unload()
        {
            Console.WriteLine("Shutting down PointSystem Plugin...");
            
            Commands.Unregister();
            Events.Unregister();

            File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(Settings));

            Console.WriteLine("Shutdown of PointSystem Plugin completed! Have a nice day!");
        }
    }
}