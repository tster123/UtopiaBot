#pragma warning disable CS8618

namespace ForestLib.Database;

public class RawMessage
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public long Author { get; set; }
    public string MessageContent { get; set; }
    public string Source { get; set; }
    public string ChannelName { get; set; }
    public long ChannelId { get; set; }
    public string? GuildName { get; set; }
    public long? GuildId { get; set; }

    public override string ToString()
    {
        return $"{nameof(Timestamp)}: {Timestamp}, {nameof(ChannelName)}: {ChannelName}, {nameof(MessageContent)}: {MessageContent}";
    }
}