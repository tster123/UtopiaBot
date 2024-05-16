using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using ForestBot.Modules;
using ForestLib;

namespace ForestBot;

public class Constants
{
        
    // To add to a server, go to this URL:
    // https://discord.com/api/oauth2/authorize?client_id=916794814303981659&permissions=277025491008&scope=bot
}

public class Program
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _services;
    private readonly MessageHandler _messageHandler;

    private readonly DiscordSocketConfig _socketConfig = new()
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
        AlwaysDownloadUsers = true,
    };

    public Program()
    {
        _configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "DC_")
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        _services = new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(_socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();
        _messageHandler = new MessageHandler();
    }

    static void Main(string[] args)
    {
        new Program().RunAsync().GetAwaiter().GetResult();
    }

    public async Task RunAsync()
    {
        var client = _services.GetRequiredService<DiscordSocketClient>();

        client.Log += LogAsync;

        // Here we can initialize the service that will register and execute our commands
        await _services.GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        client.MessageReceived += _messageHandler.MessageReceivedEvent;

        // Bot token can be provided from the Configuration object we set up earlier
        //await client.LoginAsync(TokenType.Bot, Settings.Instance.DiscordToken);
        //await client.StartAsync();

        Task endpointTask = new IntelEndpoint().StartEndpointAsync(80);

        // Never quit the program until manually forced to.
        await Task.Delay(Timeout.Infinite);
    }

        

    private async Task LogAsync(LogMessage message)
        => Console.WriteLine(message.ToString());

    public static bool IsDebug()
    {
#if DEBUG
        return false;
#else
                return false;
#endif
    }
}