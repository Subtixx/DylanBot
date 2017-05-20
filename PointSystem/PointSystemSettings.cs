namespace PointSystem
{
    public class PointSystemSettings
    {
        public bool Active = true;

        public string PointName = "point";
        public string PointNamePlural = "points";
        public string PointCommand = "points";

        public string PointAmountMessage = "You have $amount $pointName";

        public int PrimeSubscriptionAmount = 250;
        public int RegularSubscriptionAmount = 500;

        // Arena Settings
        public bool ArenaActive = true;

        public bool ArenaChallengeActive = true;
        public bool ArenaChallengeAllowCustomAmount = false;
        public bool ArenaChallengeCostIsBet = false;
        public int ArenaChallengeTimeout = 30;

        public int ArenaChallengeCost = 50;

        public string ArenaChallengeNotEnoughPoints = "Not enough money!";
        public string ArenaChallengeAlreadyFighting = "You're already in a challenge!";
        public string ArenaChallengeMessage = "$user has challenged $target to a fight! Type $command $user to accept the challenge!";
        public string ArenaChallengeAccepted = "The fight between $user and $target has begun.... Who will be the victor?!";
        public string ArenaChallengeFight = "$user and $target are going head to head in the arena... You can hear their weapons clashing and sparks fly in all directions... Suddenly a sand storm erupt....";
        public string ArneaChallengeOutcome = "The dust finally settled and $results emergerged victorious...";
        public string ArenaChallengeNotActive = "The arena is not open.";
        public string ArenaChallengeSelfError = "It would be pretty stupid to challenge yourself.";
        public string ArenaChallengeTimeoutMsg = "$target didn't accept the fight...";
    }
}
