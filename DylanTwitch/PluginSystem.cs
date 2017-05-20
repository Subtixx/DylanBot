﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DylanTwitch
{
    public abstract class Plugin
    {
        //public Dictionary<string, object> Settings;
        public Dictionary<string, string> CustomUserSettings = new Dictionary<string, string>();

        public abstract void Load();
        public abstract void Unload();
    }

    public static class PluginSystem
    {
        public enum EventType
        {
            UserJoin,
            UserLeave,
            UserSubscribed,
            ChannelJoined,
            Message
        }

        public static Dictionary<string, Plugin> LoadedPlugins = new Dictionary<string, Plugin>();

        private static readonly Dictionary<EventType, List<Func<object[], bool>>> _registeredEvents =
            new Dictionary<EventType, List<Func<object[], bool>>>();

        internal static void Initialize()
        {
            foreach (var val in Enum.GetValues(typeof(EventType)))
                _registeredEvents.Add((EventType) val, new List<Func<object[], bool>>());

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

        // Rework event system to not have "unknown" arguments
        public static void RegisterEvent(EventType evt, Func<object[], bool> func)
        {
            _registeredEvents[evt].Add(func);
        }

        internal static void UnregisterEvent()
        {
            // There is currently no way of unregistering events.
        }

        internal static void ProcessEvent(EventType evt, params object[] args)
        {
            if (!_registeredEvents.ContainsKey(evt))
            {
                Console.WriteLine($"Catastrophic failure. {evt} doesn't exist as event.");
                return;
            }

            foreach (var func in _registeredEvents[evt])
                func(args);
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
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    var exFileNotFound = exSub as FileNotFoundException;
                    if (!string.IsNullOrEmpty(exFileNotFound?.FusionLog))
                    {
                        sb.AppendLine("Fusion Log:");
                        sb.AppendLine(exFileNotFound.FusionLog);
                    }
                }
            }
        }
    }
}