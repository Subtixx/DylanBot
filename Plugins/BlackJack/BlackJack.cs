using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using DylanTwitch;
using DylanTwitch.Util;
using Newtonsoft.Json;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

namespace BlackJack
{
    public class BlackJackPlugin : Plugin
    {
        private const string SETTINGS_FILE = "Settings/BlackJack.config";
        private static BlackJackSettings _settings;

        private readonly Random _rand = new Random();
        private readonly Dictionary<string, int> _scores = new Dictionary<string, int>();
        private readonly List<string> _usernames = new List<string>();

        private GameState _gameState = GameState.Waiting;
        private int _turn;
        private Timer _turnTimer;

        public override void Load()
        {
            LoadSettingsFile();

            PluginSystem.Commands.RegisterGlobalCommand("blackjack", OnBlackJackCommand,
                new List<string> {"jack", "black"});
        }

        private bool OnBlackJackCommand(OnChatCommandReceivedArgs arg)
        {
            switch (arg.Command.ArgumentsAsList[0])
            {
                case "join":
                    Join(arg.Command);
                    break;

                case "draw":
                    Draw(arg.Command);
                    break;

                case "stop":
                    Stop(arg.Command);
                    break;
            }

            return true;
        }

        private void LoadSettingsFile()
        {
            _settings = File.Exists(SETTINGS_FILE)
                ? JsonConvert.DeserializeObject<BlackJackSettings>(File.ReadAllText(SETTINGS_FILE))
                : new BlackJackSettings();
        }

        public override void Unload()
        {
            File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(_settings));
        }

        //#####################################################################
        //############################ START GAME #############################
        //#####################################################################

        private void NextPlayer()
        {
            _turn++;
            PluginSystem.SendChatMessage(
                TranslateableString.Translate(
                    "{$turnPlayer} your turn! Use '!blackjack draw' to get cards and '!blackjack stop' to end your turn.",
                    new Dictionary<string, string>
                    {
                        {"$turnPlayer", _usernames[_turn]}
                    }));

            _turnTimer = new Timer();
            _turnTimer.Elapsed += (sender, args) =>
            {
                _turnTimer.Stop();

                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("{$turnPlayer} turn timeout! You automatically lost.",
                        new Dictionary<string, string>
                        {
                            {"$turnPlayer", _usernames[_turn]}
                        }));

                _scores.Remove(_usernames[_turn]);
                _usernames.RemoveAt(_turn);

                NextPlayer();
            };
            _turnTimer.AutoReset = false;
            _turnTimer.Interval = _settings.TurnTimeout * 1000;
            _turnTimer.Enabled = true;
        }

        private void GameOver()
        {
            var winners = new List<string>();
            var loosers = new List<string>();
            foreach (var score in _scores)
            {
                if (score.Value <= 21)
                {
                    winners.Add(score.Key);
                    if (PluginSystem.IsPluginLoaded("PointSystem") && _settings.EnablePoints)
                    {
                        int points = int.Parse(UserDatabase.Users[score.Key].CustomSettings["points"]);

                        UserDatabase.Users[score.Key].CustomSettings["points"] = (points - _settings.PointBuyIn).ToString();
                    }
                }
                else
                    loosers.Add(score.Key);
            }
            _usernames.Clear();
            _scores.Clear();

            var winnerMsg = "";
            if (winners.Count <= 5)
                for (var i = 0; i <= winners.Count; i++)
                    if (i == winners.Count)
                        winnerMsg += winners[i];
                    else
                        winnerMsg += winners[i] + ", ";
            else
                winnerMsg = winners.Count.ToString();

            var looserMsg = "";
            if (loosers.Count <= 5)
                for (var i = 0; i <= loosers.Count; i++)
                    if (i == loosers.Count)
                        looserMsg += loosers[i];
                    else
                        looserMsg += loosers[i] + ", ";
            else
                looserMsg = loosers.Count.ToString();

            PluginSystem.SendChatMessage(
                TranslateableString.Translate("Winners: {$winner}, Loosers: {$looser}!",
                    new Dictionary<string, string>
                    {
                        {"$winner", winnerMsg},
                        {"$looser", looserMsg}
                    }));

            // TODO: Add cooldown?
            _gameState = GameState.Waiting;
        }

        //#####################################################################
        //######################## COMMAND FUNCTIONS ##########################
        //#####################################################################

        private void Join(ChatCommand command)
        {
            if (_gameState == GameState.Running)
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("A game of blackjack is already running!",
                        new Dictionary<string, string>()));
                return;
            }

            if (PluginSystem.IsPluginLoaded("PointSystem") && _settings.EnablePoints)
            {
                int points = int.Parse(UserDatabase.Users[command.ChatMessage.Username].CustomSettings["points"]);

                if (points < _settings.PointBuyIn)
                {
                    PluginSystem.SendChatMessage(
                        TranslateableString.Translate("Sorry {$user} you don't have enough points to play!",
                            new Dictionary<string, string>
                            {
                                {"$user", command.ChatMessage.Username}
                            }));
                    return;
                }

                UserDatabase.Users[command.ChatMessage.Username].CustomSettings["points"] = (points - _settings.PointBuyIn).ToString();
            }

            if (_usernames.Count == 0)
            {
                var aTimer = new Timer();
                aTimer.Elapsed += (sender, args) =>
                {
                    aTimer.Stop();

                    if (_usernames.Count == 1)
                    {
                        PluginSystem.SendChatMessage(
                            TranslateableString.Translate("Sorry {$user} but nobody wanted to play with you.",
                                new Dictionary<string, string>
                                {
                                    {"$user", command.ChatMessage.Username}
                                }));
                        _usernames.Clear();
                        _scores.Clear();
                    }
                    else
                    {
                        PluginSystem.SendChatMessage(
                            TranslateableString.Translate(
                                "The game starts now! The player closest to (but not over) 21 points wins.",
                                new Dictionary<string, string>()));
                        _turn = -1;
                        _gameState = GameState.Running;
                        NextPlayer();
                    }
                };
                aTimer.AutoReset = false;
                aTimer.Interval = _settings.JoinTimeout * 1000;
                aTimer.Enabled = true;
            }
            _usernames.Add(command.ChatMessage.Username);
            _scores.Add(command.ChatMessage.Username, 0);
        }

        private void Draw(ChatCommand arg)
        {
            if (_gameState != GameState.Running)
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("No blackjack game is running!",
                        new Dictionary<string, string>()));
                return;
            }
            if (!_scores.ContainsKey(arg.ChatMessage.Username))
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("You're not playing!",
                        new Dictionary<string, string>()));
                return;
            }
            if (_usernames[_turn] != arg.ChatMessage.Username)
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("It's not your turn!",
                        new Dictionary<string, string>()));
                return;
            }

            _turnTimer.Stop();

            _scores[arg.ChatMessage.Username] += _rand.Next(2, 10);
            if (_scores[arg.ChatMessage.Username] > 21)
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("You currently have {$value} Points -> Your turn has ended.",
                        new Dictionary<string, string>
                        {
                            {"$value", _scores[arg.ChatMessage.Username].ToString()}
                        }));
                NextPlayer();
            }
            else
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("You currently have {$value} Points -> !blackjack draw/stop",
                        new Dictionary<string, string>
                        {
                            {"$value", _scores[arg.ChatMessage.Username].ToString()}
                        }));
                _turnTimer.Start();
            }
        }

        private void Stop(ChatCommand arg)
        {
            if (_gameState != GameState.Running)
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("No blackjack game is running!",
                        new Dictionary<string, string>()));
                return;
            }
            if (!_scores.ContainsKey(arg.ChatMessage.Username))
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("You're not playing!",
                        new Dictionary<string, string>()));
                return;
            }
            if (_usernames[_turn] != arg.ChatMessage.Username)
            {
                PluginSystem.SendChatMessage(
                    TranslateableString.Translate("It's not your turn!",
                        new Dictionary<string, string>()));
                return;
            }

            _turnTimer.Stop();

            if (_turn < _usernames.Count - 1)
                NextPlayer();
            else
                GameOver();
        }

        private enum GameState
        {
            Waiting,
            Running
        }
    }
}