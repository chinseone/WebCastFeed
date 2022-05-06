using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebCastFeed.WebSocket;

namespace WebCastFeed
{
    public class Program
    {
        private static IHost _Host;

        public static async Task Main(string[] args)
        {
            _Host = CreateHostBuilder(args).Build();

            await InitializeAsync().ConfigureAwait(false);

            await _Host.RunAsync();
        }

        private static async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            // Create a new scope
            using (var scope = _Host.Services.CreateScope())
            {
                // Get and initialize the RoomService
                var webSocketClient = scope.ServiceProvider.GetRequiredService<IWebSocketClient>();
                await webSocketClient.InitializeAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
