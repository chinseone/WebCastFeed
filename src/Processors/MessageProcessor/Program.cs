using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using MessageProcessor.Implementations;
using MessageProcessor.Interfaces;
using StackExchange.Redis;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Implementations;

namespace MessageProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var sqsAccessKey = Environment.GetEnvironmentVariable("SqsAccessKey") ?? "";
                    var sqsSecretKey = Environment.GetEnvironmentVariable("SqsSecretKey") ?? "";
                    
                    services.AddSingleton<AmazonSQSClient>(_ =>
                    {
                        var credentials = new BasicAWSCredentials(sqsAccessKey, sqsSecretKey);
                        return new AmazonSQSClient(credentials);
                    });

                    var redisConnection = Environment.GetEnvironmentVariable("RedisConnection") ?? "127.0.0.1:6379";
                    services.AddSingleton<IConnectionMultiplexer>(opt =>
                        ConnectionMultiplexer.Connect(redisConnection));

                    services.AddScoped<IXiugouRepository, XiugouRepository>();

                    services.AddSingleton<IPersistentService, PersistentService>();

                    services.AddHostedService<Worker>();
                });
    }
}
