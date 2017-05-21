using System;
using System.IO;
using System.Linq;
using DylanTwitch;
using Newtonsoft.Json;
using TwitchLib.Events.Services.FollowerService;
using TwitchLib.Models.Client;

namespace LatestSaver
{
    public class LatestSaverPlugin : Plugin
    {
        private const string SETTINGS_FILE = "Settings/LatestSaver.config";
        public static LatestSaverSettings Settings;

        private string latestFollowerSavePath = "Plugins\\LatestSaver\\LatestFollower.txt";
        private string latestSubscriberSavePath = "Plugins\\LatestSaver\\LatestSubscriber.txt";

        public override void Load()
        {
            LoadSettingsFile();

            if (!Directory.Exists("Plugins\\LatestSaver\\"))
                Directory.CreateDirectory("Plugins\\LatestSaver\\");
            
            if(!File.Exists(latestFollowerSavePath))
                File.WriteAllText(latestFollowerSavePath, "");

            if(!File.Exists(latestSubscriberSavePath))
                File.WriteAllText(latestSubscriberSavePath, "");

            PluginSystem.OnUserSubscribe += OnUserSubscribe;
            PluginSystem.OnUserFollowed += OnUserFollowed;
            PluginSystem.OnUserReSubscribed += OnUserReSubscribe;

            if (Settings.SaveLatestSubscriber)
                if (PluginSystem.ChannelUser != null && !PluginSystem.ChannelUser.Partnered)
                    Console.WriteLine(
                        "WARNING: Saving latest subscriber is not supported since the channel is not partnered!");
        }

        private void OnUserReSubscribe(Subscriber subscriber)
        {
            if (Settings.Active && Settings.SaveLatestResubscriber)
                OnUserSubscribe(subscriber);
        }

        private void OnUserFollowed(OnNewFollowersDetectedArgs args)
        {
            if (Settings.Active && Settings.SaveLatestFollower)
                File.WriteAllText(latestFollowerSavePath, args.NewFollowers.LastOrDefault());
        }

        private void OnUserSubscribe(Subscriber subscriber)
        {
            if (Settings.Active && Settings.SaveLatestSubscriber)
                File.WriteAllText(latestSubscriberSavePath, subscriber.DisplayName);
        }

        private void LoadSettingsFile()
        {
            Settings = File.Exists(SETTINGS_FILE)
                ? JsonConvert.DeserializeObject<LatestSaverSettings>(File.ReadAllText(SETTINGS_FILE))
                : new LatestSaverSettings();
        }

        public override void Unload()
        {
            File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(Settings));
        }
    }
}