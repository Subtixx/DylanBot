using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DylanTwitch;
using DylanTwitch.Util;
using TwitchLib.Events.Client;

namespace PointSystem
{
    public static class Commands
    {
        public static void Register()
        {
            // Add chat command
            ChatBot.CommandController.RegisterGlobalCommand(PointSystemPlugin.Settings.PointCommand, OnPointCommand);

            AdminCommands.RegisterCommands();
            ArenaCommands.RegisterCommands();
        }

        public static void Unregister()
        {
            ChatBot.CommandController.UnregisterGlobalCommand(PointSystemPlugin.Settings.PointCommand);
        }

        private static bool OnPointCommand(OnChatCommandReceivedArgs args)
        {
            if (!PointSystemPlugin.Settings.Active)
                return false;

            if (UserDatabase.Users.ContainsKey(args.Command.ChatMessage.Username) &&
                UserDatabase.Users[args.Command.ChatMessage.Username].CustomSettings.ContainsKey("points"))
            {
                var points = int.Parse(UserDatabase.Users[args.Command.ChatMessage.Username].CustomSettings["points"]);
                var pointName = points > 1
                    ? PointSystemPlugin.Settings.PointNamePlural
                    : PointSystemPlugin.Settings.PointName;


                ChatBot.Client.SendMessage(
                    TranslateableString.Translate(PointSystemPlugin.Settings.PointAmountMessage,
                        new Dictionary<string, string>
                        {
                            {"$amount", points.ToString()},
                            {"$pointName", pointName}
                        })
                );
            }
            else
            {
                ChatBot.Client.SendMessage(
                    TranslateableString.Translate(PointSystemPlugin.Settings.PointAmountMessage,
                        new Dictionary<string, string>
                        {
                            {"$amount", "0"},
                            {"$pointName", PointSystemPlugin.Settings.PointNamePlural}
                        })
                );
            }
            return true;
        }
    }
}
