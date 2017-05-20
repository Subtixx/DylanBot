using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DylanTwitch;
using TwitchLib.Events.Client;

namespace PointSystem
{
    public static class AdminCommands
    {
        public static void RegisterCommands()
        {
            ChatBot.CommandController.RegisterGlobalCommand("setpoints", OnSetPoints);
        }

        public static bool OnSetPoints(OnChatCommandReceivedArgs args)
        {
            if (!UserDatabase.Users.ContainsKey(args.Command.ChatMessage.Username) ||
                !UserDatabase.Users[args.Command.ChatMessage.Username].Permissions.Contains("edit_points") && !args.Command.ChatMessage.Username.Equals(Settings.Config.Username))
            {
                ChatBot.Client.SendMessage("No permission.");
                return false;
            }

            int amount;
            if (args.Command.ArgumentsAsList.Count != 2 || !int.TryParse(args.Command.ArgumentsAsList[1], out amount))
            {
                ChatBot.Client.SendMessage("Syntax: !setpoints [User] [Amount]");
                return false;
            }

            var target = args.Command.ArgumentsAsList[0];
            if (!UserDatabase.Users.ContainsKey(target))
            {
                ChatBot.Client.SendMessage($"User '{target}' doesn't exist!");
                return false;
            }

            if (!UserDatabase.Users[target].CustomSettings.ContainsKey("points"))
                UserDatabase.Users[target].CustomSettings.Add("points", "0");

            UserDatabase.Users[target].CustomSettings["points"] = amount.ToString();
            ChatBot.Client.SendMessage($"'{target}' Points set to {amount}");

            return true;
        }
    }
}
