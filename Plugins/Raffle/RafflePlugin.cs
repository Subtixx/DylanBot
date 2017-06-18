using System;
using System.Collections.Generic;
using System.IO;
using DylanTwitch;
using Newtonsoft.Json;
using TwitchLib.Events.Client;

namespace Raffle
{
    public class RafflePlugin : Plugin
    {
        private const string SETTINGS_FILE = "Settings/Raffle.config";
        public static RaffleSettings Settings;

        public override void Load()
        {
            LoadSettingsFile();

            if (Settings.EnterGiveawayCommand != "")
                PluginSystem.Commands.RegisterGlobalCommand(Settings.EnterGiveawayCommand, OnEnterRaffleCommand);

            PluginSystem.Commands.RegisterGlobalCommand("raffle", OnRaffleCommand, new List<string> {"giveaway"});
        }

        private bool OnRaffleCommand(OnChatCommandReceivedArgs arg)
        {
            var commandArgs = arg.Command.ArgumentsAsList;
            var username = arg.Command.ChatMessage.Username;

            var canCloseRaffle = UserDatabase.Users[username].HasPermission("close_raffle");
            var canStartRaffle = UserDatabase.Users[username].HasPermission("start_raffle");
            var canDrawRaffle = UserDatabase.Users[username].HasPermission("draw_raffle");

            if (commandArgs.Count == 0)
            {
                var actions = "enter";
                if (UserDatabase.Users.ContainsKey(username))
                {
                    if(canStartRaffle)
                        actions += "/start";
                    if (canCloseRaffle)
                        actions += "/close";
                    if (canDrawRaffle)
                        actions += "/draw";
                }

                PluginSystem.SendChatMessage($"Syntax: !raffle [Action ({actions})]");
                return false;
            }

            switch (commandArgs[0])
            {
                case "enter":
                    arg.Command.ArgumentsAsList.RemoveAt(0);
                    OnEnterRaffleCommand(arg);
                    break;

                case "start":
                    if (canStartRaffle)
                    {

                    }
                    break;

                case "close":
                    if (canCloseRaffle)
                    {
                        
                    }
                    break;

                case "draw":
                    if (canDrawRaffle)
                    {
                        
                    }
                    break;
            }
            return true;
        }

        private bool OnEnterRaffleCommand(OnChatCommandReceivedArgs arg)
        {
            // TODO
            return true;
        }

        private void LoadSettingsFile()
        {
            Settings = File.Exists(SETTINGS_FILE)
                ? JsonConvert.DeserializeObject<RaffleSettings>(File.ReadAllText(SETTINGS_FILE))
                : new RaffleSettings();
        }

        public override void Unload()
        {
        }
    }
}