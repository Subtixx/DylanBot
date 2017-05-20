using System;
using TwitchLib;
using TwitchLib.Events.Client;

namespace DylanTwitch.DefaultCommands
{
    public static class UserCommands
    {
        private static DateTime? ChannelLiveStart;

        public static void RegisterCommands()
        {
            ChatBot.CommandController.RegisterGlobalCommand("watchtime", OnWatchTimeCommand);

            TwitchAPI.Streams.v5.GetUptime(Settings.Config.ChannelId).ContinueWith(task =>
            {
                if (task.Result.HasValue)
                    ChannelLiveStart = DateTime.Now - task.Result.Value;
            });
            ChatBot.CommandController.RegisterGlobalCommand("uptime", OnUptimeCommand);
        }

        private static bool OnUptimeCommand(OnChatCommandReceivedArgs args)
        {
            if (ChannelLiveStart != null)
            {
                var diff = DateTime.Now.Subtract(ChannelLiveStart.Value);
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
                        var diff = DateTime.Now.Subtract(ChannelLiveStart.Value);
                        ChatBot.Client.SendMessage(
                            $"Channel went live {ChannelLiveStart:MM/dd/yy H:mm}, uptime: {diff.TotalHours:F2} Hours");
                    }
                    else
                    {
                        ChatBot.Client.SendMessage("The channel is not live!");
                    }
                });
            }
            return true;
        }

        private static bool OnWatchTimeCommand(OnChatCommandReceivedArgs args)
        {
            var user = UserDatabase.Users[args.Command.ChatMessage.Username];
            if (user.JoinDateTime.HasValue)
            {
                user.TotalWatchTime += DateTime.Now.Subtract(user.JoinDateTime.Value).TotalSeconds;
                user.JoinDateTime = DateTime.Now;
            }

            ChatBot.Client.SendMessage(
                $"{user.TwitchUsername} current watch time: {Math.Round(user.TotalWatchTime / 60 / 60, 2):F2} Hours");
            return true;
        }
    }
}