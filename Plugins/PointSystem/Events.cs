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
        public static void Register()
        {
            PluginSystem.OnUserSubscribe += OnNewSubscriber;
        }

        public static void Unregister()
        {
            PluginSystem.OnUserSubscribe -= OnNewSubscriber;
        }

        private static void OnNewSubscriber(Subscriber subscriber)
        {
            if (!PointSystemPlugin.Settings.Active)
                return;
            
            var username = subscriber.DisplayName;
            var prime = subscriber.IsTwitchPrime;

            if (UserDatabase.Users.ContainsKey(username) &&
                UserDatabase.Users[username].CustomSettings.ContainsKey("points"))
                UserDatabase.Users[username].CustomSettings["points"] += prime
                    ? PointSystemPlugin.Settings.PrimeSubscriptionAmount
                    : PointSystemPlugin.Settings.RegularSubscriptionAmount;
        }
    }
}
