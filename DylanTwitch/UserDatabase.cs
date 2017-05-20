using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TwitchLib;

namespace DylanTwitch
{
    public static class UserDatabase
    {
        private const string DatabasePath = "Database\\UserDatabase.db";

        public static Dictionary<string, User> Users = new Dictionary<string, User>();

        public static List<string> DefaultEditorPerms = new List<string>()
        {
            "edit_game",
            "clear_chat"
        };

        internal static async void LoadDatabase()
        {
            if (File.Exists(DatabasePath))
            {
                try
                {
                    Users =
                        JsonConvert.DeserializeObject<Dictionary<string,User>>(File.ReadAllText(DatabasePath));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                var t = await TwitchAPI.Users.v5.GetUser(Settings.Config.AuthToken); // <-- ChannelId
                Settings.Config.ChannelId = t.Id;

                if (!Users.ContainsKey(Settings.Config.Username))
                    Users.Add(Settings.Config.Username, new User(Settings.Config.Username));

                if (t.Partnered)
                {
                    List<TwitchLib.Models.API.v5.Subscriptions.Subscription> allSubs = await TwitchAPI.Channels.v5.GetAllSubscribers(Settings.Config.ChannelId);
                    allSubs.ForEach(s =>
                    {
                        if (!Users.ContainsKey(s.User.Name)) // Add all subscribers to our new userDatabase
                            Users.Add(s.User.Name, new User(s.User.Name));
                    });
                }

                /*var editors = await TwitchAPI.Channels.v5.GetChannelEditors(Settings.Config.ChannelId, ChatBot.OAUTH_TOKEN);
                foreach (var editor in editors.Editors)
                {
                    if (!Users.ContainsKey(editor.Name))
                    {
                        // Add user to database with permissions
                        User u = new User(editor.Name);
                        u.Permissions.AddRange(DefaultEditorPerms);

                        Users.Add(editor.Name, u);
                    }
                    else
                    {
                        // Add all default permissions to user
                        User u = Users[editor.Name];
                        foreach (var editorPerm in DefaultEditorPerms)
                        {
                            if(!u.Permissions.Contains(editorPerm))
                                u.Permissions.Add(editorPerm);
                        }
                    }
                }*/

                var followers = await TwitchAPI.Channels.v5.GetChannelFollowers(Settings.Config.ChannelId);
                foreach (var follower in followers.Follows) // Add all followers to our new userDatabase
                {
                    if (!Users.ContainsKey(follower.User.Name))
                        Users.Add(follower.User.Name, new User(follower.User.Name));
                }

                SaveDatabase();
            }
        }

        internal static void SaveDatabase()
        {
            foreach (var keyValuePair in Users)
            {
                if(keyValuePair.Value.JoinDateTime.HasValue)
                    keyValuePair.Value.TotalWatchTime += DateTime.Now.Subtract(keyValuePair.Value.JoinDateTime.Value).TotalSeconds;
            }

            File.WriteAllText(DatabasePath, JsonConvert.SerializeObject(Users));
        }

        internal static void UserJoined(string eUsername)
        {
            if (!Users.ContainsKey(eUsername))
            {
                Users.Add(eUsername, new User(eUsername, DateTime.Now));
            }
            else
            {
                Users[eUsername].JoinDateTime = DateTime.Now;
            }
        }

        internal static void UserLeft(string eUsername)
        {
            if(Users[eUsername].JoinDateTime.HasValue)
                Users[eUsername].TotalWatchTime += DateTime.Now.Subtract(Users[eUsername].JoinDateTime.Value).TotalSeconds;
        }
    }
}
