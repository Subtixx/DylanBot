﻿using System.Collections.Generic;
using TwitchLib.Events.Client;
using TwitchLib.Extensions.Client;

namespace DylanTwitch.DefaultCommands
{
    public static class ChannelMod
    {
        public static void Register()
        {
            ChatBot.CommandController.RegisterGlobalCommand("clear", OnClearChatCommand, new List<string> {"prune"}); // <-- aliases
        }

        private static bool OnClearChatCommand(OnChatCommandReceivedArgs args)
        {
            var username = args.Command.ChatMessage.Username;
            if ((!UserDatabase.Users.ContainsKey(username) ||
                 !UserDatabase.Users[username].Permissions.Contains("clear_chat")) &&
                !username.Equals(Settings.Config.Username))
                return true;

            ChatBot.Client.SendMessage($"~ Chat pruned by @{username} ~");
            ChatBot.Client.ClearChat();
            return true;
        }
    }
}