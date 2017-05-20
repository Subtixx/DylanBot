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
        private static DateTime? ChannelLiveStart = null;

        public static void RegisterCommands()
        {
            // Default commands:
            TwitchAPI.Streams.v5.GetUptime(Settings.Config.ChannelId).ContinueWith(task =>
            {
                if (task.Result.HasValue)
                    ChannelLiveStart = DateTime.Now - task.Result.Value;
            });

            ChatBot.CommandController.RegisterGlobalCommand("uptime", args =>
            {
                if (ChannelLiveStart != null)
                {
                    TimeSpan diff = DateTime.Now.Subtract(ChannelLiveStart.Value);
                    ChatBot.Client.SendMessage(
                        $"Channel went live {ChannelLiveStart:MM/dd/yy H:mm}, uptime: {diff.TotalHours:F2} Hours");
                }
                else
                {
                    TwitchAPI.Streams.v5.GetUptime(Settings.Config.ChannelId).ContinueWith(task =>
                    {
                        if (task.Result.HasValue)
                        {
                            ChannelLiveStart = DateTime.Now - task.Result.Value;
                            TimeSpan diff = DateTime.Now.Subtract(ChannelLiveStart.Value);
                            ChatBot.Client.SendMessage(
                                $"Channel went live {ChannelLiveStart:MM/dd/yy H:mm}, uptime: {diff.TotalHours:F2} Hours");
                        }
                        else
                            ChatBot.Client.SendMessage("The channel is not live!");
                    });
                }
                return true;
            });
            
            ChatBot.CommandController.RegisterGlobalCommand("clear", OnClearChatCommand, new List<string>() { "prune" }); // <-- aliases
            ChatBot.CommandController.RegisterGlobalCommand("setgame", OnSetGameCommand);
        }

        // TODO: Move this to a whisper command maybe?
        private static bool OnSetGameCommand(OnChatCommandReceivedArgs args)
        {
            var username = args.Command.ChatMessage.Username;
            // setgame "" "" <-- would work.
            // setgame Test Programming <-- crashes lol
            var a = CommandController.ParseLine(args.Command.ArgumentsAsString);
            if (a.Count != 2)
            {
                ChatBot.Client.SendMessage("Syntax: !setgame [Status] [Game]");
                return true;
            }

            var status = a[0];
            var game = a[1];

            if ((!UserDatabase.Users.ContainsKey(username) || UserDatabase.Users[username].Permissions.Contains("edit_game")) && !username.Equals(Settings.Config.Username))
            {
                // No permission.
                return true;
            }

            TwitchAPI.Channels.v5.UpdateChannel(Settings.Config.ChannelId, status, game).ContinueWith(task =>
            {
                ChatBot.Client.SendWhisper(args.Command.ChatMessage.Username, $"Status set to: {status} Game set to: {game}");
            });
            return true;
        }

        private static bool OnClearChatCommand(OnChatCommandReceivedArgs args)
        {
            var username = args.Command.ChatMessage.Username;
            if((!UserDatabase.Users.ContainsKey(username) || UserDatabase.Users[username].Permissions.Contains("clear_chat")) && !username.Equals(Settings.Config.Username))
            {
                // No permission.
                return true;
            }

            ChatBot.Client.SendMessage($"~ Chat pruned by @{username} ~");
            ChatBot.Client.ClearChat();
            return true;
        }
    }
}
