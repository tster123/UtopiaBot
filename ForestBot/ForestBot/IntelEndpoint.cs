using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace ForestBot
{
    public class IntelInput
    {
        public string? data_html { get; set; }
        public string? data_simple { get; set; }
        public string? url { get; set; }
        public string? prov { get; set; }
        public string? key { get; set; }

    }

    public class IntelResult
    {
        public bool success;
    }

    public class IntelEndpoint
    {
        public async Task StartEndpointAsync(int port = 80)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.WebHost.ConfigureKestrel((WebHostBuilderContext context, KestrelServerOptions serverOptions) =>
            {
                serverOptions.Listen(IPAddress.Any, port);
            });
            WebApplication app = builder.Build();

            app.MapPost("/Intel", async (IntelInput input) =>
            {
                
                return Results.Ok(new[] { new IntelResult() { success = true } });
            });

            await app.RunAsync();
        }
    }
}
