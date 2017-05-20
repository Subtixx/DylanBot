using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Events.Client;

namespace DylanTwitch.DefaultCommands
{
    public static class UserCommands
    {
        public static void RegisterCommands()
        {
            ChatBot.CommandController.RegisterGlobalCommand("watchtime", OnWatchTimeCommand);
        }

        private static bool OnWatchTimeCommand(OnChatCommandReceivedArgs args)
        {
            User user = UserDatabase.Users[args.Command.ChatMessage.Username];
            if (user.JoinDateTime.HasValue)
            {
                user.TotalWatchTime += DateTime.Now.Subtract(user.JoinDateTime.Value).TotalSeconds;
                user.JoinDateTime = DateTime.Now;
            }

            ChatBot.Client.SendMessage($"{user.TwitchUsername} current watch time: {Math.Round(user.TotalWatchTime / 60 / 60, 2):F2} Hours");
            return true;
        }
    }
}
