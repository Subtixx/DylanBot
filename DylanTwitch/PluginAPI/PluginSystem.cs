// ***********************************************************************
// Assembly         : DylanTwitch
// Author           : Subtixx
// Created          : 05-21-2017
//
// Last Modified By : Subtixx
// Last Modified On : 05-21-2017
// ***********************************************************************
// <copyright file="PluginSystem.cs">
//     Copyright © Subtixx 2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TwitchLib.Models.Client;

namespace DylanTwitch
{
    /// <summary>
    /// User Join Event
    /// Gets executed when a user enters the channel chat
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="username">The username.</param>
    public delegate void UserJoinEventHandler(string channel, string username);

    /// <summary>
    /// User Left Event
    /// Gets executed when a user leaves the channel chat
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="username">The username.</param>
    public delegate void UserLeftEventHandler(string channel, string username);

    /// <summary>
    /// User Subscribe Event
    /// Gets executed when a user subscribers to the channel
    /// </summary>
    /// <param name="subscriber">The subscriber.</param>
    public delegate void UserSubscribedEventHandler(Subscriber subscriber);

    /// <summary>
    /// Channel Join Event
    /// Gets executed when the bot joined a channel
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="username">The username.</param>
    public delegate void ChannelJoinedEventHandler(string channel, string username);

    /// <summary>
    /// Message Received Event
    /// Gets executed when a chat message is received
    /// NOTE: DO NOT USE FOR COMMANDS
    /// </summary>
    /// <param name="message">The message.</param>
    public delegate void MessageReceivedEventHandler(ChatMessage message);

    /// <summary>
    /// Main class to interact with the bot.
    /// </summary>
    public static class PluginSystem
    {
        #region API
        /// <summary>
        /// A dictionary containing all loaded plugins
        /// </summary>
        internal static readonly Dictionary<string, Plugin> LoadedPlugins = new Dictionary<string, Plugin>();

        /// <summary>
        /// Register and unregister commands
        /// </summary>
        /// <value>The commands.</value>
        public static CommandController Commands => ChatBot.CommandController;

        /// <summary>
        /// Occurs when [a user joins].
        /// </summary>
        public static event UserJoinEventHandler OnUserJoin;

        /// <summary>
        /// Occurs when [user leaves].
        /// </summary>
        public static event UserLeftEventHandler OnUserLeft;

        /// <summary>
        /// Occurs when [user subscribes].
        /// </summary>
        public static event UserSubscribedEventHandler OnUserSubscribe;

        /// <summary>
        /// Occurs when [the bot joins a channel].
        /// </summary>
        public static event ChannelJoinedEventHandler OnChannelJoin;

        /// <summary>
        /// Occurs when [a message is received].
        /// </summary>
        public static event MessageReceivedEventHandler OnMessageReceived;

        /// <summary>
        /// Sends a message to the channel chat.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public static void SendChatMessage(string channel, string message) => ChatBot.Client.SendMessage(channel, message);

        /// <summary>
        /// Sends a message to the first joined channel chat.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void SendChatMessage(string message) => ChatBot.Client.SendMessage(message);

        /// <summary>
        /// Sends a formatted whisper message to receiver.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        public static void SendWhisperMessage(string receiver, string message) => ChatBot.Client.SendWhisper(receiver, message);
        
        /// <summary>
        /// Determines whether the specified plugin is loaded.
        /// </summary>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <returns><c>true</c> if the specified plugin is loaded; otherwise, <c>false</c>.</returns>
        public static bool IsPluginLoaded(string pluginName)
        {
            return LoadedPlugins.ContainsKey(pluginName);
        }
        #endregion

        #region Events
        /// <summary>
        /// Invokes the OnUserJoin event handler
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="username">The username.</param>
        internal static void UserJoin(string channel, string username)
        {
            OnUserJoin?.Invoke(channel, username);
        }

        /// <summary>
        /// Invokes the OnUserLeft event handler
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="username">The username.</param>
        internal static void UserLeft(string channel, string username)
        {
            OnUserLeft?.Invoke(channel, username);
        }

        /// <summary>
        /// Invokes the OnUserSubscribe event handler
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        internal static void UserSubscribed(Subscriber subscriber)
        {
            OnUserSubscribe?.Invoke(subscriber);
        }

        /// <summary>
        /// Invokes the OnChannelJoin event handler
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="username">The username.</param>
        internal static void ChannelJoin(string channel, string username)
        {
            OnChannelJoin?.Invoke(channel, username);
        }

        /// <summary>
        /// Invokes the OnMessageReceived event handler
        /// </summary>
        /// <param name="message">The message.</param>
        internal static void MessageReceived(ChatMessage message)
        {
            OnMessageReceived?.Invoke(message);
        }
        #endregion

        /// <summary>
        /// Initializes this instance.
        /// Loads all plugins
        /// </summary>
        internal static void Initialize()
        {
            var plugins = Directory.GetFiles("Plugins", "*.dll").ToList();
            plugins.ForEach(file => LoadPlugin(new FileInfo(file)));

            Console.WriteLine($"{plugins.Count} Plugins loaded.");
        }

        /// <summary>
        /// Shutdowns this instance.
        /// Unloads all plugins
        /// </summary>
        internal static void Shutdown()
        {
            foreach (var kvp in LoadedPlugins)
                kvp.Value.Unload();
        }

        /// <summary>
        /// Loads a plugin.
        /// </summary>
        /// <param name="file">The file.</param>
        private static void LoadPlugin(FileInfo file)
        {
            Console.WriteLine($"Loading plugin '{file.Name}'");
            try
            {
                var assembly = Assembly.LoadFrom(file.FullName);

                foreach (var type in assembly.GetTypes())
                    if (type.IsSubclassOf(typeof(Plugin)) && type.IsAbstract == false)
                    {
                        var b = type.InvokeMember(null,
                            BindingFlags.CreateInstance,
                            null, null, null) as Plugin;
                        if (b != null)
                        {
                            b.Load();
                            LoadedPlugins.Add(assembly.FullName, b);
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Plugin '{file.Name}' has thrown an exception:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}