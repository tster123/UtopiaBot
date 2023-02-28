using Discord.WebSocket;
using Discord;
using ForestLib.Database;

namespace ForestBot.Modules
{
    public class MessageHandler
    {
        public async Task MessageReceivedEvent(SocketMessage arg)
        {
            RawMessage message = new RawMessage
            {
                Id = arg.Id,
                Author = arg.Id,
                ChannelId = arg.Channel.Id,
                ChannelName = arg.Channel.Name,
                MessageContent = arg.Content,
                Source = arg.Source.ToString(),
                Timestamp = arg.Timestamp.UtcDateTime,
            };
            if (arg.Channel is SocketGuildChannel c)
            {
                message.GuildName = c.Guild.Name;
                message.GuildId = c.Guild.Id;
            }

            try
            {
                ForestContext context = new ForestContext();
                await context.RawMessages.AddAsync(message);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}
