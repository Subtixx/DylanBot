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
        private const string RandomJokeUrl = "http://api.icndb.com/jokes/random";

        public override void Load()
        {
            ChatBot.CommandController.RegisterGlobalCommand("joke", OnJokeCommand);
        }

        private bool OnJokeCommand(OnChatCommandReceivedArgs onChatCommandReceivedArgs)
        {
            using (WebClient web = new WebClient())
            {
                var result = JsonConvert.DeserializeObject<JokeResult>(web.DownloadString(RandomJokeUrl));
                if (result != null && result.Type == "success")
                {
                    ChatBot.Client.SendMessage(WebUtility.HtmlDecode(result.Value.Joke));
                }
                else
                {
                    ChatBot.Client.SendMessage("I was unable to find any jokes :/");
                }
            }
            return false;
        }

        public override void Unload()
        {
            ChatBot.CommandController.UnregisterGlobalCommand("joke");
        }
    }
}