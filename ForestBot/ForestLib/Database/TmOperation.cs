
#pragma warning disable CS8618
namespace ForestLib.Database
{
    public class TmOperation
    {
        public long Id { get; set; }
        public long GuildId { get; set; }
        public DateTime Timestamp { get; set; }
        public int TimestampSlot { get; set; }
        public long? ParsedFromMessageId { get; set; }
        public string SourceProvince { get; set; }
        public string? TargetProvince { get; set; }
        public string? TargetKingdom { get; set; }
        public string OpName { get; set; }
        public bool Success { get; set; }
        public int? Damage { get; set; }
        public int? ThievesSent { get; set; }
        public int? SelfCasualties { get; set; }
        public bool Reflected { get; set; }
    }
}
