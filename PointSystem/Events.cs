using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DylanTwitch;
using TwitchLib.Models.Client;

namespace PointSystem
{
    public static class Events
    {
        public static void RegisterEvents()
        {
            PluginSystem.RegisterEvent(PluginSystem.EventType.UserSubscribed, OnNewSubscriber);
        }

        /// <summary>
        /// Unregisters all events.
        /// </summary>
        /// Not possible currently <see cref="PluginSystem">PluginSystem</see>
        public static void Unregister()
        {
            //PluginSystem.UnregisterEvent();
        }

        private static bool OnNewSubscriber(object[] args)
        {
            if (!PointSystemPlugin.Settings.Active)
                return false;

            var user = (Subscriber)args[0];
            var username = user.DisplayName;
            var prime = user.IsTwitchPrime;

            if (UserDatabase.Users.ContainsKey(username) &&
                UserDatabase.Users[username].CustomSettings.ContainsKey("points"))
                UserDatabase.Users[username].CustomSettings["points"] += prime
                    ? PointSystemPlugin.Settings.PrimeSubscriptionAmount
                    : PointSystemPlugin.Settings.RegularSubscriptionAmount;
            return true;
        }
    }
}
