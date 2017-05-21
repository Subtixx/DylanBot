using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TwitchLib.Models.Client;

namespace DylanTwitch
{
    public abstract class Plugin
    {
        //public Dictionary<string, object> Settings;
        public Dictionary<string, string> CustomUserSettings = new Dictionary<string, string>();

        public abstract void Load();
        public abstract void Unload();
    }

    public delegate void UserJoinEventHandler(string channel, string username);
    public delegate void UserLeftEventHandler(string channel, string username);
    public delegate void UserSubscribedEventHandler(Subscriber subscriber);
    public delegate void ChannelJoinedEventHandler(string channel, string username);
    public delegate void MessageReceivedEventHandler(ChatMessage message);

    public static class PluginSystem
    {
        public static event UserJoinEventHandler OnUserJoin;
        public static event UserLeftEventHandler OnUserLeft;
        public static event UserSubscribedEventHandler OnUserSubscribe;
        public static event ChannelJoinedEventHandler OnChannelJoin;
        public static event MessageReceivedEventHandler OnMessageReceived;

        internal static void UserJoin(string channel, string username)
        {
            OnUserJoin?.Invoke(channel, username);
        }

        internal static void UserLeft(string channel, string username)
        {
            OnUserLeft?.Invoke(channel, username);
        }

        internal static void UserSubscribed(Subscriber subscriber)
        {
            OnUserSubscribe?.Invoke(subscriber);
        }

        internal static void ChannelJoin(string channel, string username)
        {
            OnChannelJoin?.Invoke(channel, username);
        }

        internal static void MessageReceived(ChatMessage message)
        {
            OnMessageReceived?.Invoke(message);
        }

        public static readonly Dictionary<string, Plugin> LoadedPlugins = new Dictionary<string, Plugin>();

        internal static void Initialize()
        {
            var plugins = Directory.GetFiles("Plugins", "*.dll").ToList();
            plugins.ForEach(file => LoadPlugin(new FileInfo(file)));

            Console.WriteLine($"{plugins.Count} Plugins loaded.");
        }

        internal static void Shutdown()
        {
            foreach (var kvp in LoadedPlugins)
                kvp.Value.Unload();
        }

        public static bool IsPluginLoaded(string pluginName)
        {
            return LoadedPlugins.ContainsKey(pluginName);
        }

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