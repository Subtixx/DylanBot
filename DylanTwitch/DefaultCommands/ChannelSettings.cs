using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Extensions.Client;

namespace DylanTwitch.DefaultCommands
{
    public static class ChannelSettings
    {
        public static void RegisterCommands()
        {
            ChatBot.CommandController.RegisterWhisperCommand("setgame", OnSetGameCommand);
        }
        
        private static bool OnSetGameCommand(OnWhisperCommandReceivedArgs args)
        {
            var username = args.WhisperMessage.Username;

            var a = CommandController.ParseLine(args.ArgumentsAsString);
            if (a.Count != 2)
            {
                ChatBot.Client.SendWhisper(username, "Syntax: !setgame [Status] [Game]");
                return true;
            }

            var status = a[0];
            var game = a[1];

            if ((!UserDatabase.Users.ContainsKey(username) || !UserDatabase.Users[username].Permissions.Contains("edit_game")) && !username.Equals(Settings.Config.Username))
            {
                // No permission.
                return true;
            }

            TwitchAPI.Channels.v5.UpdateChannel(Settings.Config.ChannelId, status, game).ContinueWith(task =>
            {
                ChatBot.Client.SendWhisper(username, $"Status set to: {status} Game set to: {game}");
            });
            return true;
        }

        
    }
}
