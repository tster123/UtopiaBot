namespace ForestLib
{

    public enum ActionType
    {
        Attack,
        Spell,
        Thievery
    }
    public class Action
    {
        public ActionType Type;
        public string OpName;
        public bool Success;
        public int Damage;
        public int ThievesLost;
        public int WizardsLost;
        public int HorsesLost;
        public int OffSpecsLost;
        public int ElitesLost;
        public int AttackKills;
        public int PrisonersGained;
        public int PeasantsGained;
        public double ReturnHours;
        public int SpecCredits;
        public int BuildCredits;
        public int Promotions;
        public int OffenseSent;
        public int GeneralsSent;
    }
    public class BotParser
    {
    }
}