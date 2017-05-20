using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DylanTwitch
{
    public class Config
    {
        public string AuthToken = "";
        public string Username = "";
        public string ChannelId = "";
    }

    public static class Settings
    {
        public static Config Config;
        internal static string SettingsFile = "DylanBot.config";

        public static void Load()
        {
            if(File.Exists(SettingsFile))
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(SettingsFile));
            else
                Config = new Config();
        }

        public static void Save()
        {
#if !DEBUG
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(Config));
#endif
        }
    }
}
