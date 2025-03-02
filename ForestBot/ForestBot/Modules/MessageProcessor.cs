using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestBot.Modules
{
    public class MessageProcessor
    {
        private SocketTextChannel chan;
        public ulong? StartFromMessageId;
        public ulong? EndMessageId;
        public int MaxMessages;

        public ulong? CurrentMessageId { get; private set; }
        public int ProcessedMessages { get; private set; }
        public int EventsSaved { get; private set; }
        private string Timestamp = "";

        public MessageProcessor(SocketTextChannel chan, ulong? startFromMessageId, ulong? endMessageId, int maxMessages)
        {
            this.chan = chan;
            this.StartFromMessageId = startFromMessageId;
            this.EndMessageId = endMessageId;
            this.MaxMessages = maxMessages;
            t = new(ProcessMessages);
        }

        public override string ToString()
        {
            return
                $"MessageProcessor [Channel: {chan.Name}, {nameof(StartFromMessageId)}: {StartFromMessageId}, {nameof(MaxMessages)}: {MaxMessages}, {nameof(CurrentMessageId)}: {CurrentMessageId}, {nameof(ProcessedMessages)}: {ProcessedMessages},  {nameof(EventsSaved)}: {EventsSaved}] ({Timestamp})";
        }

        private Thread t;
        public void Start()
        {
            t.IsBackground = true;
            t.Start();
        }

        public bool IsDone() => !t.IsAlive;

        private readonly MessageHandler handler = new MessageHandler();
        private void ProcessMessages()
        {
            CurrentMessageId = StartFromMessageId;
            while (ProcessedMessages < MaxMessages)
            {
                int numToRead = Math.Min(200, MaxMessages - ProcessedMessages);
                IEnumerable<IMessage> messages;
                if (CurrentMessageId == null)
                {
                    messages = chan.GetMessagesAsync(numToRead).FlattenAsync().Result;
                }
                else
                {
                    messages = chan.GetMessagesAsync(CurrentMessageId.Value, Direction.Before, numToRead).FlattenAsync().Result;
                }

                foreach (IMessage m in messages)
                {
                    EventsSaved += handler.MessageReceivedEvent(m).Result;
                    ProcessedMessages++;
                    if (CurrentMessageId == null || CurrentMessageId.Value > m.Id)
                    {
                        CurrentMessageId = m.Id;
                        Timestamp = m.Timestamp.ToString();
                    }
                }
            }
        }
    }
}
