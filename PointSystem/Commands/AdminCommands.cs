using DylanTwitch;
using TwitchLib.Events.Client;

namespace PointSystem
{
    public static class AdminCommands
    {
        public static void RegisterCommands()
        {
            // TODO: Maybe do a global !points [set/add/remove] command?
            PluginSystem.Commands.RegisterGlobalCommand($"set{PointSystemPlugin.Settings.PointCommand}", OnSetPoints);
            PluginSystem.Commands.RegisterGlobalCommand($"add{PointSystemPlugin.Settings.PointCommand}", OnAddPoints);
            PluginSystem.Commands.RegisterGlobalCommand($"remove{PointSystemPlugin.Settings.PointCommand}", OnRemovePoints);
        }

        private static bool OnAddPoints(OnChatCommandReceivedArgs args)
        {
            var username  = args.Command.ChatMessage.Username;
            int amount;

            if (!UserDatabase.Users.ContainsKey(username))
                return false; // No user in database found.

            if (UserDatabase.Users[username].HasPermission("add_global_points"))
            {
                if (int.TryParse(args.Command.ArgumentsAsList[0], out amount))
                {
                    foreach (var userKvp in UserDatabase.Users)
                    {
                        var user = userKvp.Value;
                        if (!user.CustomSettings.ContainsKey("points"))
                            user.CustomSettings.Add("points", "0");

                        user.CustomSettings["points"] = (int.Parse(user.CustomSettings["points"]) + amount).ToString();
                    }
                    PluginSystem.SendChatMessage($"{amount} {PointSystemPlugin.Settings.PointNamePlural} added to everybody");

                    return true;
                }
            }
            else if (UserDatabase.Users[username].HasPermission("add_points"))
            {
                if (args.Command.ArgumentsAsList.Count != 2 ||
                    !int.TryParse(args.Command.ArgumentsAsList[1], out amount))
                {
                    PluginSystem.SendChatMessage($"Syntax: !add{PointSystemPlugin.Settings.PointCommand} [User] [Amount]");
                    return false;
                }

                var target = args.Command.ArgumentsAsList[0];
                if (!UserDatabase.Users.ContainsKey(target))
                {
                    PluginSystem.SendChatMessage($"User '{target}' doesn't exist!");
                    return false;
                }

                if (!UserDatabase.Users[target].CustomSettings.ContainsKey("points"))
                    UserDatabase.Users[target].CustomSettings.Add("points", "0");

                UserDatabase.Users[target].CustomSettings["points"] =
                    (int.Parse(UserDatabase.Users[target].CustomSettings["points"]) + amount).ToString();
                PluginSystem.SendChatMessage($"'{target}' {amount} {PointSystemPlugin.Settings.PointNamePlural} added");
            }

            return true;
        }

        private static bool OnRemovePoints(OnChatCommandReceivedArgs args)
        {
            var username = args.Command.ChatMessage.Username;
            int amount;

            if (!UserDatabase.Users.ContainsKey(username))
                return false; // No user in database found.

            if (UserDatabase.Users[username].HasPermission("remove_global_points"))
            {
                if (int.TryParse(args.Command.ArgumentsAsList[0], out amount))
                {
                    foreach (var userKvp in UserDatabase.Users)
                    {
                        var user = userKvp.Value;
                        if (!user.CustomSettings.ContainsKey("points"))
                            user.CustomSettings.Add("points", "0");

                        user.CustomSettings["points"] = (int.Parse(user.CustomSettings["points"]) - amount).ToString();
                    }
                    PluginSystem.SendChatMessage($"{amount} {PointSystemPlugin.Settings.PointNamePlural} removed from everybody");

                    return true;
                }
            }
            else if (UserDatabase.Users[username].HasPermission("remove_points"))
            {
                if (args.Command.ArgumentsAsList.Count != 2 ||
                    !int.TryParse(args.Command.ArgumentsAsList[1], out amount))
                {
                    PluginSystem.SendChatMessage($"Syntax: !remove{PointSystemPlugin.Settings.PointCommand} [User] [Amount]");
                    return false;
                }

                var target = args.Command.ArgumentsAsList[0];
                if (!UserDatabase.Users.ContainsKey(target))
                {
                    PluginSystem.SendChatMessage($"User '{target}' doesn't exist!");
                    return false;
                }

                if (!UserDatabase.Users[target].CustomSettings.ContainsKey("points"))
                    UserDatabase.Users[target].CustomSettings.Add("points", "0");

                UserDatabase.Users[target].CustomSettings["points"] =
                    (int.Parse(UserDatabase.Users[target].CustomSettings["points"]) - amount).ToString();
                PluginSystem.SendChatMessage($"{amount} {PointSystemPlugin.Settings.PointNamePlural} removed from '{target}'");
            }

            return true;
        }

        private static bool OnSetPoints(OnChatCommandReceivedArgs args)
        {
            var username = args.Command.ChatMessage.Username;
            int amount;

            if (!UserDatabase.Users.ContainsKey(username))
                return false; // No user in database found.

            if (UserDatabase.Users[username].HasPermission("set_global_points"))
            {
                if (int.TryParse(args.Command.ArgumentsAsList[0], out amount))
                {
                    foreach (var userKvp in UserDatabase.Users)
                    {
                        var user = userKvp.Value;
                        if (!user.CustomSettings.ContainsKey("points"))
                            user.CustomSettings.Add("points", "0");

                        user.CustomSettings["points"] = amount.ToString();
                    }
                    PluginSystem.SendChatMessage($"{amount} {PointSystemPlugin.Settings.PointNamePlural} set to everybody");

                    return true;
                }
            }
            else if (UserDatabase.Users[username].HasPermission("set_points"))
            {
                if (args.Command.ArgumentsAsList.Count != 2 ||
                    !int.TryParse(args.Command.ArgumentsAsList[1], out amount))
                {
                    PluginSystem.SendChatMessage($"Syntax: !set{PointSystemPlugin.Settings.PointCommand} [User] [Amount]");
                    return false;
                }

                var target = args.Command.ArgumentsAsList[0];
                if (!UserDatabase.Users.ContainsKey(target))
                {
                    PluginSystem.SendChatMessage($"User '{target}' doesn't exist!");
                    return false;
                }

                if (!UserDatabase.Users[target].CustomSettings.ContainsKey("points"))
                    UserDatabase.Users[target].CustomSettings.Add("points", "0");

                UserDatabase.Users[target].CustomSettings["points"] = amount.ToString();
                PluginSystem.SendChatMessage($"'{target}' {amount} {PointSystemPlugin.Settings.PointNamePlural} set");
            }

            return true;
        }
    }
}
