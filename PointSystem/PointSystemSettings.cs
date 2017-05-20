using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Models.Client;

namespace PointSystem
{
    public class PointSystemSettings
    {
        public bool Active = true;
        public int PrimeSubscriptionAmount = 250;
        public int RegularSubscriptionAmount = 500;

        // Arena Settings
        public bool ArenaActive = true;

        public bool ArenaChallengeActive = true;
        public bool ArenaChallengeAllowCustomAmount = false;
        public int ArenaChallengeTimeout = 30;

        public int ArenaChallengeCost = 50;
        public string ArenaChallengeMessage = "$user has challenged $target to a fight! Type $command $user to accept the challenge!";
        public string ArenaChallengeAccepted = "The fight between $user and $target has begun.... Who will be the victor?!";
        public string ArenaChallengeFight = "$user and $target are going head to head in the arena... You can hear their weapons clashing and sparks fly in all directions... Suddenly a sand storm erupt....";
        public string ArneaChallengeOutcome = "The dust finally settled and $results emergerged victorious...";
        public string ArenaChallengeNotActive = "The arena is not open.";
        public string ArenaChallengeSelfError = "It would be pretty stupid to challenge yourself.";
        public string ArenaChallengeTimeoutMsg = "$target didn't accept the fight...";
    }
}
