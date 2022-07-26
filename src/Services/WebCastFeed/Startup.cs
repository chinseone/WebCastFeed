using System;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;
using WebCastFeed.Operations;
using WebCastFeed.WebSocket;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Implementations;
using Xiugou.Http;

namespace WebCastFeed
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // var sqlConnectionString = Environment.GetEnvironmentVariable("XiugouMySqlConnectionString");
            // services.AddDbContextPool<XiugouDbContext>(options =>
            // {
            //     options.UseMySQL(sqlConnectionString);
            // }, int.Parse(Environment.GetEnvironmentVariable("ConnectionPoolSize") ?? "10"));

            var douyinBaseUrl = Environment.GetEnvironmentVariable("DouyinBaseUrl");
            services.AddScoped<IDouyinClient>(p => new DouyinClient(douyinBaseUrl));

            var redisConnection = Environment.GetEnvironmentVariable("RedisConnection") ?? "127.0.0.1:6379";
            services.AddSingleton<IConnectionMultiplexer>(opt =>
                ConnectionMultiplexer.Connect(redisConnection));

            services.AddScoped<IXiugouRepository, XiugouRepository>();

            services.AddSingleton(sp =>
                CreateWebSocketClient(CancellationToken.None));

            services.AddSingleton<OperationExecutor>();
            services.AddScoped<ValidateDouyinWebhookOperation>();
            services.AddScoped<HandleLiveFeedOperation>();
            services.AddScoped<CreateTicketOperation>();
            services.AddScoped<UpdateTicketOperation>();
            services.AddScoped<GetTicketByCodeOperation>();
            services.AddScoped<DouyinStartGameOperation>();
            services.AddScoped<DouyinStopGameOperation>();
            services.AddScoped<GetAllTicketsOperation>();
            services.AddScoped<GetActiveSessionIdByAnchorIdOperation>();
            services.AddScoped<CreateH5ProfileOperation>();
            services.AddScoped<GetH5ProfileOperation>();
            services.AddScoped<OnboardOfficialTicketsOperation>();
            services.AddScoped<GetAllH5ProfilesOperation>();
            services.AddScoped<ResetAllTicketsOperation>();
        }

        private static IWebSocketClient CreateWebSocketClient(CancellationToken cancellationToken)
        {
            var serverShutdownTime = int.Parse(Environment.GetEnvironmentVariable("ServerShutdownTime") ?? "3");
            EventHandler<TransportClosedEventArgs> transportClosedHandler = async (obj, args) =>
            {
                // Kill the whole program if a websocket transport closes
                // await _Host?.StopAsync(TimeSpan.FromSeconds(serverShutdownTime));
            };

            var websocketUri = Environment.GetEnvironmentVariable("WebSocketServerUri") ?? "ws://localhost:6000/douyin/chat";
            var maxRetries = int.Parse(Environment.GetEnvironmentVariable("WebSocketConnectionMaxRetries") ?? "5");
            var transportFactory = new WebSocketTransportFactory(websocketUri, 443, maxRetries, transportClosedHandler);

            return new WebSocketClient(transportFactory);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
