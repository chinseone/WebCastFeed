using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;

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

                    services.AddHostedService<Worker>();
                    services.AddSingleton<AmazonSQSClient>(_ =>
                    {
                        var credentials = new BasicAWSCredentials(sqsAccessKey, sqsSecretKey);
                        return new AmazonSQSClient(credentials);
                    });
                });
    }
}
