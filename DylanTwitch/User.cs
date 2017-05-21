// ***********************************************************************
// Assembly         : DylanTwitch
// Author           : Subtixx
// Created          : 05-19-2017
//
// Last Modified By : Subtixx
// Last Modified On : 05-19-2017
// ***********************************************************************
// <copyright file="User.cs">
//     Copyright © Subtixx 2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DylanTwitch
{
    /// <summary>
    /// Class to save informations about a User.
    /// </summary>
    public class User
    {
        /// <summary>
        /// This users twitch username
        /// </summary>
        public string TwitchUsername;

        /// <summary>
        /// This users permissions
        /// </summary>
        public List<string> Permissions;

        /// <summary>
        /// Custom settings applied by plugins
        /// </summary>
        public Dictionary<string, string> CustomSettings;

        public double TotalWatchTime; // in seconds

        internal DateTime? JoinDateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="eUsername">The username.</param>
        /// <param name="joinDate"></param>
        public User(string eUsername, DateTime? joinDate = null)
        {
            TwitchUsername = eUsername;
            Permissions = new List<string>();
            CustomSettings = new Dictionary<string, string>();
            TotalWatchTime = 0;
            JoinDateTime = joinDate;

            foreach (var loadedPlugin in PluginSystem.LoadedPlugins)
            {
                foreach (var valueCustomUserSetting in loadedPlugin.Value.CustomUserSettings)
                {
                    if(!CustomSettings.ContainsKey(valueCustomUserSetting.Key))
                        CustomSettings.Add(valueCustomUserSetting.Key, valueCustomUserSetting.Value);
                    else
                    {
                        Console.WriteLine($"ERROR: Plugin {loadedPlugin.Key} added duplicated user setting {valueCustomUserSetting.Key}");
                    }
                }
            }
        }

        public bool HasPermission(string permission)
        {
            if (TwitchUsername.Equals(Settings.Config.Username))
                return true; // Bot has all permissions.

            return Permissions.Contains(permission);
        }
    }
}
