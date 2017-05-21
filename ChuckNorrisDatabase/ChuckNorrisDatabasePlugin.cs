using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DylanTwitch;
using Newtonsoft.Json;
using TwitchLib.Events.Client;

namespace ChuckNorrisDatabase
{
    public class JokeResult
    {
        public class ValueClass
        {
            [JsonProperty("id")]
            public int Id;

            [JsonProperty("joke")]
            public string Joke;

            [JsonProperty("categories")]
            public List<object> Categories { get; set; }
        }

        [JsonProperty("type")]
        public string Type;
        [JsonProperty("value")]
        public ValueClass Value;

    }

    public class ChuckNorrisDatabasePlugin : Plugin
    {
        private const string NameJokeUrl = "http://api.icndb.com/jokes/random?firstName=$firstName&lastName=$lastName";
        private const string CategoryJokeUrl = "http://api.icndb.com/jokes/random?limitTo=[$cats]";
        private const string RandomJokeUrl = "http://api.icndb.com/jokes/random";

        public override void Load()
        {
            PluginSystem.Commands.RegisterGlobalCommand("chuck", OnJokeCommand);
        }

        private bool OnJokeCommand(OnChatCommandReceivedArgs args)
        {
            if (args.Command.ArgumentsAsList.Count == 0)
            {
                using (WebClient web = new WebClient())
                {
                    var result = JsonConvert.DeserializeObject<JokeResult>(web.DownloadString(RandomJokeUrl));
                    if (result != null && result.Type == "success")
                    {
                        PluginSystem.SendChatMessage(WebUtility.HtmlDecode(result.Value.Joke));
                    }
                    else
                    {
                        PluginSystem.SendChatMessage("I was unable to find any jokes :/");
                    }
                }
                return true;
            }
            else if (args.Command.ArgumentsAsList.Count == 3 && args.Command.ArgumentsAsList[0] == "name")
            {
                var names = CommandController.ParseLine(args.Command.ArgumentsAsString);
                if (names.Count != 3)
                {
                    PluginSystem.SendChatMessage("Syntax: !joke [FirstName] [LastName]");
                    return false;
                }

                using (WebClient web = new WebClient())
                {
                    var result = JsonConvert.DeserializeObject<JokeResult>(web.DownloadString(CategoryJokeUrl.Replace("$firstName", names[1]).Replace("$lastName", names[2])));
                    if (result != null && result.Type == "success")
                    {
                        PluginSystem.SendChatMessage(WebUtility.HtmlDecode(result.Value.Joke));
                    }
                    else
                    {
                        PluginSystem.SendChatMessage("I was unable to find any jokes :/");
                    }
                }
                return true;
            }
            /*else if (args.Command.ArgumentsAsList.Count >= 1)
            {
                var cats = CommandController.ParseLine(args.Command.ArgumentsAsString);
                if (cats.Count == 0)
                {
                    PluginSystem.SendChatMessage("Syntax: !joke [Category] (Category)");
                    return false;
                }

                var include = "";
                foreach (var cat in cats)
                {
                    var c = cat.Replace(",", "").Replace("\"", "");
                    if (include != "")
                        include += "," + c;
                    else include += c;
                }
                using (WebClient web = new WebClient())
                {
                    var json = web.DownloadString(CategoryJokeUrl.Replace("$cats", include));
                    var result = JsonConvert.DeserializeObject<JokeResult>(json);
                    if (result != null && result.Type == "success")
                    {
                        PluginSystem.SendChatMessage(WebUtility.HtmlDecode(result.Value.Joke));
                    }
                    else
                    {
                        PluginSystem.SendChatMessage("I was unable to find any jokes :/");
                    }
                }
            }*/
            return false;
        }

        public override void Unload()
        {
            PluginSystem.Commands.UnregisterGlobalCommand("chuck");
        }
    }
}