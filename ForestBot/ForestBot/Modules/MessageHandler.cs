using Discord.WebSocket;
using Discord;
using ForestLib;
using ForestLib.Database;
using Microsoft.EntityFrameworkCore;

namespace ForestBot.Modules
{
    public class MessageHandler
    {
        public BotParser parser = new BotParser();

        public async Task<int> MessageReceivedEvent(IMessage arg)
        {
            RawMessage message = new RawMessage
            {
                Id = (long) arg.Id,
                Author = (long)arg.Id,
                ChannelId = (long)arg.Channel.Id,
                ChannelName = arg.Channel.Name,
                MessageContent = arg.Content,
                Source = arg.Source.ToString(),
                Timestamp = arg.Timestamp.UtcDateTime,
            };
            if (arg.Channel is IGuildChannel c)
            {
                message.GuildName = c.Guild.Name;
                message.GuildId = (long)c.Guild.Id;
            }

            try
            {
                ForestContext context = new ForestContext();
                if (await context.RawMessages.CountAsync(m => m.Id == message.Id) == 0)
                {
                    await context.RawMessages.AddAsync(message);

                    var ops = parser.ParseOps(message.Timestamp, message.GuildId ?? 0, message.Id, message.MessageContent);
                    foreach (var op in ops)
                    {
                        await context.Operations.AddAsync(op);
                    }

                    await context.SaveChangesAsync();
                    return 1;
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
