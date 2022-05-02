using System;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebCastFeed.Operations;
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

            var sqlConnectionString = Environment.GetEnvironmentVariable("XiugouMySqlConnectionString");
            services.AddDbContextPool<XiugouDbContext>(options =>
            {
                options.UseMySQL(sqlConnectionString);
            }, int.Parse(Environment.GetEnvironmentVariable("ConnectionPoolSize") ?? "10"));

            var douyinBaseUrl = Environment.GetEnvironmentVariable("DouyinBaseUrl");
            services.AddScoped<IDouyinClient>(p => new DouyinClient(douyinBaseUrl));

            services.AddScoped<IXiugouRepository, XiugouRepository>();

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
