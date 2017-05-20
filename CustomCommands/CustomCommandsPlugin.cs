using System;
using System.Collections.Generic;
using System.IO;
using DylanTwitch;
using DylanTwitch.Util;
using Newtonsoft.Json;
using TwitchLib;

namespace CustomCommands
{
    public class CustomCommandsPlugin : Plugin
    {
        private const string SETTINGS_FILE = "Settings/CustomCommands.config";
        public static CustomCommandsSettings Settings;

        public override void Load()
        {
            LoadSettingsFile();

            foreach (var command in Settings.RegisteredCommands)
            {
                ChatBot.CommandController.RegisterGlobalCommand(command.Key, args =>
                {
                    if (Settings.Active)
                    {
                        ChatBot.Client.SendMessage(TranslateableString.Translate(command.Value,
                            new Dictionary<string, string>()
                            {
                                {"$username", args.Command.ChatMessage.Username}
                            }
                        ));
                    }
                    return true;
                });
            }
        }

        private void LoadSettingsFile()
        {
            Settings = File.Exists(SETTINGS_FILE)
                ? JsonConvert.DeserializeObject<CustomCommandsSettings>(File.ReadAllText(SETTINGS_FILE))
                : new CustomCommandsSettings();
        }

        public override void Unload()
        {
            Console.WriteLine("Shutting down CustomCommands Plugin...");

            File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(Settings));

            Console.WriteLine("Shutdown of CustomCommands Plugin completed! Have a nice day!");
        }
    }
}
