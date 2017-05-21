using System;
using System.Collections.Generic;
using System.Timers;
using DylanTwitch;
using DylanTwitch.Util;
using TwitchLib.Events.Client;

namespace PointSystem
{
    public static class ArenaCommands
    {
        //TODO: Move this somewhere logically.
        public static Dictionary<string, string> ActiveChallengeRequests = new Dictionary<string, string>();
        public static Dictionary<string, string> ActiveChallenges = new Dictionary<string, string>();

        public static void RegisterCommands()
        {
            PluginSystem.Commands.RegisterGlobalCommand("challenge", OnChallengeCommand);
        }

        private static bool OnChallengeCommand(OnChatCommandReceivedArgs arg)
        {
            if (!PointSystemPlugin.Settings.ArenaActive || !PointSystemPlugin.Settings.ArenaChallengeActive)
            {
                PluginSystem.SendChatMessage(PointSystemPlugin.Settings.ArenaChallengeNotActive);
                return false;
            }

            var username = arg.Command.ChatMessage.Username.ToLower();

            if (ActiveChallenges.ContainsKey(username) || ActiveChallenges.ContainsValue(username))
            {
                PluginSystem.SendChatMessage(PointSystemPlugin.Settings.ArenaChallengeAlreadyFighting);
                return false;
            }

            if (arg.Command.ArgumentsAsList.Count < 1)
            {
                PluginSystem.SendChatMessage("Syntax: !challenge [Target]");
                return false;
            }

            var target = arg.Command.ArgumentsAsList[0].ToLower();

            if (!ActiveChallengeRequests.ContainsKey(target) && PointSystemPlugin.Settings.ArenaChallengeCostIsBet &&
                PointSystemPlugin.Settings.ArenaChallengeAllowCustomAmount && arg.Command.ArgumentsAsList.Count != 2)
            {
                PluginSystem.SendChatMessage("Syntax: !challenge [Target] or !challenge [Target] [Amount]");
                return false;
            }

            if (username.Equals(target))
            {
                PluginSystem.SendChatMessage(PointSystemPlugin.Settings.ArenaChallengeSelfError);
                return false;
            }

            if (PointSystemPlugin.Settings.ArenaChallengeCost > 0)
            {
                if (!UserDatabase.Users.ContainsKey(username) || !UserDatabase.Users.ContainsKey(target))
                {
                    PluginSystem.SendChatMessage(PointSystemPlugin.Settings.ArenaChallengeNotEnoughPoints);
                    return false;
                }

                if (int.Parse(UserDatabase.Users[username].CustomSettings["points"]) <
                    PointSystemPlugin.Settings.ArenaChallengeCost ||
                    int.Parse(UserDatabase.Users[target].CustomSettings["points"]) <
                    PointSystemPlugin.Settings.ArenaChallengeCost)
                {
                    PluginSystem.SendChatMessage(PointSystemPlugin.Settings.ArenaChallengeNotEnoughPoints);
                    return false;
                }
            }

            if (ActiveChallengeRequests.ContainsKey(target))
            {
                if (UserDatabase.Users.ContainsKey(username) && UserDatabase.Users.ContainsKey(target))
                {
                    UserDatabase.Users[username].CustomSettings["points"] =
                    (int.Parse(UserDatabase.Users[username].CustomSettings["points"]) -
                     PointSystemPlugin.Settings.ArenaChallengeCost).ToString();

                    UserDatabase.Users[target].CustomSettings["points"] =
                    (int.Parse(UserDatabase.Users[target].CustomSettings["points"]) -
                     PointSystemPlugin.Settings.ArenaChallengeCost).ToString();

                    // Challenge accept
                    ActiveChallengeRequests.Remove(target); // Remove challenge request
                    ActiveChallenges.Add(target, username); // Add active challenge

                    PluginSystem.SendChatMessage(
                        TranslateableString.Translate(PointSystemPlugin.Settings.ArenaChallengeAccepted,
                            new Dictionary<string, string>
                            {
                                // We switch target and username here since the person who wrote !challenge in the first place
                                // Was target, not username.
                                {"$user", target},
                                {"$target", username}
                            }));

                    var aTimer = new Timer();
                    aTimer.Elapsed += (sender, args) =>
                    {
                        aTimer.Stop();

                        PluginSystem.SendChatMessage(
                            TranslateableString.Translate(PointSystemPlugin.Settings.ArenaChallengeFight,
                                new Dictionary<string, string>
                                {
                                    // We switch target and username here since the person who wrote !challenge in the first place
                                    // Was target, not username.
                                    {"$user", target},
                                    {"$target", username}
                                }));
                    };
                    aTimer.AutoReset = false;
                    aTimer.Interval = 5000;
                    aTimer.Enabled = true;

                    var aTimer2 = new Timer();
                    aTimer2.Elapsed += (sender2, args2) =>
                    {
                        aTimer2.Stop();

                        var winnerUsername = new Random().Next(100) <= 50 ? username : target;
                        if (PointSystemPlugin.Settings.ArenaChallengeCostIsBet)
                            UserDatabase.Users[winnerUsername].CustomSettings["points"] =
                            (int.Parse(UserDatabase.Users[winnerUsername].CustomSettings["points"]) +
                             PointSystemPlugin.Settings.ArenaChallengeCost * 2).ToString();

                        PluginSystem.SendChatMessage(
                            TranslateableString.Translate(PointSystemPlugin.Settings.ArneaChallengeOutcome,
                                new Dictionary<string, string>
                                {
                                    {"$results", winnerUsername}
                                }));
                        ActiveChallenges.Remove(target); // Remove active challenge
                    };
                    aTimer2.AutoReset = false;
                    aTimer2.Interval = 10000;
                    aTimer2.Enabled = true;
                }
            }
            else
            {
                // Challenge start
                ActiveChallengeRequests.Add(username, target);
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate(PointSystemPlugin.Settings.ArenaChallengeMessage,
                        new Dictionary<string, string>
                        {
                            {"$user", username},
                            {"$target", target},
                            {"$command", "!challenge"}
                        }));
                var aTimer = new Timer();
                aTimer.Elapsed += (sender, args) =>
                {
                    aTimer.Stop();

                    if (ActiveChallengeRequests.ContainsKey(username))
                    {
                        ActiveChallengeRequests.Remove(username);

                        PluginSystem.SendChatMessage(
                            TranslateableString.Translate(PointSystemPlugin.Settings.ArenaChallengeTimeoutMsg,
                                new Dictionary<string, string>
                                {
                                    {"$target", target}
                                }));
                    }
                };
                aTimer.AutoReset = false;
                aTimer.Interval = PointSystemPlugin.Settings.ArenaChallengeTimeout * 1000;
                aTimer.Enabled = true;
            }
            return true;
        }

        //$user has challenged $target to a fight! Type $command $user to accept the challenge!
        //The fight between $user and $target has begun.... Who will be the victor?!
        //$target didn't accept the fight... Either because he didn't have enough currency or he declined.

        //$user and $target are going head to head in the arena... You can hear their weapons clashing and sparks fly in all directions... Suddenly a sand storm erupt....
        //The dust finally settled and $results emergerged victorious...
        //You are still recovering... You will be out of commission for $cooldown!
        //$user looks like you have recovered! Want to go again?! Type $command [name] to start!
    }
}