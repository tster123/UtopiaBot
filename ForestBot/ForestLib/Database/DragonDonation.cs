namespace ForestLib.Database
{
    public class DragonDonation
    {
        public long Id { get; set; }
        public long GuildId { get; set; }
        public DateTime Timestamp { get; set; }
        public int TimestampSlot { get; set; }
        public long? ParsedFromMessageId { get; set; }
        public string Province { get; set; }
        public int? Food { get; set; }
        public int? Gold { get; set; }
    }
}
