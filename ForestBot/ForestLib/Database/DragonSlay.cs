namespace ForestLib.Database
{
    public class DragonSlay
    {
        public long Id { get; set; }
        public long GuildId { get; set; }
        public DateTime Timestamp { get; set; }
        public int TimestampSlot { get; set; }
        public long? ParsedFromMessageId { get; set; }
        public string Province { get; set; }
        public int Troops { get; set; }
        public int Points { get; set; }
    }
}
