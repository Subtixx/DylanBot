using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DylanTwitch;
using TwitchLib.Events.Client;

namespace PointSystem
{
    public static class Commands
    {
        public static void RegisterCommands()
        {
            // Add chat command
            ChatBot.CommandController.RegisterGlobalCommand("points", OnPointCommand);

            AdminCommands.RegisterCommands();
            ArenaCommands.RegisterCommands();
        }

        public static void Unregister()
        {
            ChatBot.CommandController.UnregisterGlobalCommand("points");
        }

        private static bool OnPointCommand(OnChatCommandReceivedArgs args)
        {
            if (!PointSystemPlugin.Settings.Active)
                return false;

            if (UserDatabase.Users.ContainsKey(args.Command.ChatMessage.Username) &&
                UserDatabase.Users[args.Command.ChatMessage.Username].CustomSettings.ContainsKey("points"))
                ChatBot.Client.SendMessage(
                    $"You have {UserDatabase.Users[args.Command.ChatMessage.Username].CustomSettings["points"]} Points!");
            else
                ChatBot.Client.SendMessage("You have 0 Points!");
            return true;
        }
    }
}
