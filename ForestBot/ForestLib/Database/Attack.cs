#pragma warning disable CS8618
namespace ForestLib.Database
{
    public class Attack
    {
        public long Id { get; set; }
        public long GuildId { get; set; }
        public DateTime Timestamp { get; set; }
        public int TimestampSlot { get; set; }
        public long? ParsedFromMessageId { get; set; }
        public string SourceProvince { get; set; }
        public string? TargetProvince { get; set; }
        public string? TargetKingdom { get; set; }
        public string AttackType { get; set; }
        public int Damage { get; set; }
        public int? PlunderGold { get; set; }
        public int? PlunderFood { get; set; }
        public int? PlunderRunes { get; set; }
        public int LostSoldiers { get; set; }
        public int LostOffSpecs { get; set; }
        public int LostElites { get; set; }
        public int LostHorses { get; set; }
        public int Kills { get; set; }
        public int Prisoners { get; set; }
        public bool SpreadPlague { get; set; }
        public int SpecCredits { get; set; }
        public int Peasants { get; set; }
        public double ReturnHours { get; set; }
        public int OffenseSent { get; set; }
        public int GeneralsSent { get; set; }
    }
}
