using System;
using DylanTwitch.DefaultCommands;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Events.Services.FollowerService;
using TwitchLib.Models.API.v5.Users;
using TwitchLib.Models.Client;
using TwitchLib.Services;

namespace DylanTwitch
{
    internal class ChatBot
    {
        private const string CLIENT_ID = "27yqxc53mrldhm1mwtobwuqbr7x85f6";

        public static TwitchClient Client;
        public static TwitchPubSub PubSub;
        public static readonly CommandController CommandController = new CommandController();

        public static UserAuthed Channel = null;

        internal ChatBot()
        {
            Settings.Load();

            TwitchAPI.Settings.ClientId = CLIENT_ID;
            TwitchAPI.Settings.AccessToken = Settings.Config.AuthToken;

            UserDatabase.LoadDatabase();

            var credentials = new ConnectionCredentials(Settings.Config.Username, Settings.Config.AuthToken);

            Client = new TwitchClient(credentials, Settings.Config.Username);
            Client.AddChatCommandIdentifier('!');
            Client.AddWhisperCommandIdentifier('!');

            Client.OnJoinedChannel += OnJoinedChannel;
            Client.OnChatCommandReceived += OnChatCommand;
            Client.OnWhisperCommandReceived += OnWhisperCommand;
            Client.OnMessageReceived += OnMessageReceived;
            Client.OnWhisperReceived += OnWhisperReceived;
            Client.OnNewSubscriber += OnNewSubscriber;
            Client.OnReSubscriber += OnReSubcriber;
            Client.OnUserJoined += OnUserJoined;
            Client.OnUserLeft += OnUserLeft;
            Client.OnExistingUsersDetected += OnUserListReceived;

            Client.Connect();

            FollowerService service = new FollowerService();
            service.OnNewFollowersDetected += OnNewFollowersDetected;
            service.StartService();

            PubSub = new TwitchPubSub();
            PubSub.Connect();

            TwitchAPI.Users.v5.GetUser(Settings.Config.AuthToken).ContinueWith(task => Channel = task.Result);

            RegisterGlobalCommands();
        }

        private void OnUserListReceived(object sender, OnExistingUsersDetectedArgs onExistingUsersDetectedArgs)
        {
        }

        private void RegisterGlobalCommands()
        {
            UserCommands.Register();
            ChannelMod.Register();
            ChannelSettings.Register();
        }

        internal void Shutdown()
        {
            Settings.Save();

            UserDatabase.SaveDatabase();

            Client.Disconnect();
            PubSub.Disconnect();
        }

        // Twitch events here
        private void OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            UserDatabase.UserJoined(e.Username);

            // Execute plugin events
            PluginSystem.UserJoin(e.Channel, e.Username);
        }

        private void OnUserLeft(object sender, OnUserLeftArgs e)
        {
            UserDatabase.UserLeft(e.Username);

            // Execute plugin events
            PluginSystem.UserLeft(e.Channel, e.Username);
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            // Execute plugin events
            PluginSystem.ChannelJoin(e.Channel, e.Username);
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // Execute plugin events
            PluginSystem.MessageReceived(e.ChatMessage);
        }

        private void OnChatCommand(object sender, OnChatCommandReceivedArgs e)
        {
            CommandController.ExecuteGlobalCommand(e.Command.Command, e);
        }

        private void OnWhisperCommand(object sender, OnWhisperCommandReceivedArgs e)
        {
            CommandController.ExecuteWhisperCommand(e.Command, e);
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
        }

        private void OnReSubcriber(object sender, OnReSubscriberArgs e)
        {
            PluginSystem.UserReSubscribed(e.ReSubscriber);
        }
        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            // Execute plugin events
            PluginSystem.UserSubscribed(e.Subscriber);
        }

        private void OnNewFollowersDetected(object sender, OnNewFollowersDetectedArgs e)
        {
            PluginSystem.UserFollowed(e);
        }
    }
}