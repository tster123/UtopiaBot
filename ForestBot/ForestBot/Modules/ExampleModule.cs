using System.Security.Cryptography.X509Certificates;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace ForestBot.Modules
{
    internal class DoUserCheck : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            // Check if the component matches the target properly.
            if (context.Interaction is not SocketMessageComponent componentContext)
                return Task.FromResult(PreconditionResult.FromError("Context unrecognized as component context."));

            else
            {
                // The approach here entirely depends on how you construct your custom ID. In this case, the format is:
                // unique-name:*,*

                // here the name and wildcards are split by ':'
                var param = componentContext.Data.CustomId.Split(':');

                // here we determine that we should always check for the first ',' present.
                // This will deal with additional wildcards by always selecting the first wildcard present.
                if (param.Length > 1 && ulong.TryParse(param[1].Split(',')[0], out ulong id))
                    return (context.User.Id == id)
                        // If the user ID
                        ? Task.FromResult(PreconditionResult.FromSuccess())
                        : Task.FromResult(PreconditionResult.FromError("User ID does not match component ID!"));

                else return Task.FromResult(PreconditionResult.FromError("Parse cannot be done if no userID exists."));
            }
        }
    }

    public enum ExampleEnum
    {
        First,
        Second,
        Third,
        Fourth,
        [ChoiceDisplay("Twenty First")]
        TwentyFirst
    }

    // Interation modules must be public and inherit from an IInterationModuleBase
    public class ExampleModule : InteractionModuleBase<SocketInteractionContext>
    {
        // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
        public InteractionService Commands { get; set; }

        private InteractionHandler _handler;

        // Constructor injection is also a valid way to access the dependencies
        public ExampleModule(InteractionHandler handler)
        {
            _handler = handler;
        }

        // You can use a number of parameter types in you Slash Command handlers (string, int, double, bool, IUser, IChannel, IMentionable, IRole, Enums) by default. Optionally,
        // you can implement your own TypeConverters to support a wider range of parameter types. For more information, refer to the library documentation.
        // Optional method parameters(parameters with a default value) also will be displayed as optional on Discord.

        // [Summary] lets you customize the name and the description of a parameter
        [SlashCommand("echo", "Repeat the input")]
        public async Task Echo(string echo, [Summary(description: "mention the user")] bool mention = false)
            => await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));

        [SlashCommand("read-messages", "Reads messages from another channel")]
        public async Task ReadMessage(string channel)
        {
            SocketTextChannel? chan = Context.Guild.TextChannels.FirstOrDefault(c => c.Name.ToLower() == channel);
            if (chan == null)
            {
                await RespondAsync($"Unable to find channel [{channel}]");
                return;
            }

            var messages = await chan.GetMessagesAsync(100).FlattenAsync();
            string message = "";
            foreach (IMessage m in messages)
            {
                message += "Message: " + m.Content + "\n";
            }

            await RespondAsync(message);
        }

        [SlashCommand("ping", "Pings the bot and returns its latency.")]
        public async Task GreetUserAsync()
            => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

        [SlashCommand("bitrate", "Gets the bitrate of a specific voice channel.")]
        public async Task GetBitrateAsync([ChannelTypes(ChannelType.Voice, ChannelType.Stage)] IChannel channel)
            => await RespondAsync(text: $"This voice channel has a bitrate of {(channel as IVoiceChannel).Bitrate}");

        // [Group] will create a command group. [SlashCommand]s and [ComponentInteraction]s will be registered with the group prefix
        [Group("test_group", "This is a command group")]
        public class GroupExample : InteractionModuleBase<SocketInteractionContext>
        {
            // You can create command choices either by using the [Choice] attribute or by creating an enum. Every enum with 25 or less values will be registered as a multiple
            // choice option
            [SlashCommand("choice_example", "Enums create choices")]
            public async Task ChoiceExample(ExampleEnum input)
                => await RespondAsync(input.ToString());
        }

        // Use [ComponentInteraction] to handle message component interactions. Message component interaction with the matching customId will be executed.
        // Alternatively, you can create a wild card pattern using the '*' character. Interaction Service will perform a lazy regex search and capture the matching strings.
        // You can then access these capture groups from the method parameters, in the order they were captured. Using the wild card pattern, you can cherry pick component interactions.
        [ComponentInteraction("musicSelect:*,*")]
        public async Task ButtonPress(string id, string name)
        {
            // ...
            await RespondAsync($"Playing song: {name}/{id}");
        }

        // Select Menu interactions, contain ids of the menu options that were selected by the user. You can access the option ids from the method parameters.
        // You can also use the wild card pattern with Select Menus, in that case, the wild card captures will be passed on to the method first, followed by the option ids.
        [ComponentInteraction("roleSelect")]
        public async Task RoleSelect(string[] selections)
        {
            throw new NotImplementedException();
        }

        // With the Attribute DoUserCheck you can make sure that only the user this button targets can click it. This is defined by the first wildcard: *.
        // See Attributes/DoUserCheckAttribute.cs for elaboration.
        [DoUserCheck]
        [ComponentInteraction("myButton:*")]
        public async Task ClickButtonAsync(string userId)
            => await RespondAsync(text: ":thumbsup: Clicked!");

        // This command will greet target user in the channel this was executed in.
        [UserCommand("greet")]
        public async Task GreetUserAsync(IUser user)
            => await RespondAsync(text: $":wave: {Context.User} said hi to you, <@{user.Id}>!");

        // Pins a message in the channel it is in.
        [MessageCommand("pin")]
        public async Task PinMessageAsync(IMessage message)
        {
            // make a safety cast to check if the message is ISystem- or IUserMessage
            if (message is not IUserMessage userMessage)
                await RespondAsync(text: ":x: You cant pin system messages!");

            // if the pins in this channel are equal to or above 50, no more messages can be pinned.
            else if ((await Context.Channel.GetPinnedMessagesAsync()).Count >= 50)
                await RespondAsync(text: ":x: You cant pin any more messages, the max has already been reached in this channel!");

            else
            {
                await userMessage.PinAsync();
                await RespondAsync(":white_check_mark: Successfully pinned message!");
            }
        }
    }
}