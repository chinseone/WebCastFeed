using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using MessageProcessor.Interfaces;
using MessageProcessor.Models;

namespace MessageProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _Logger;
        private readonly AmazonSQSClient _SqsClient;
        private readonly IPersistentService _PersistentService;

        public Worker(ILogger<Worker> logger, AmazonSQSClient sqsClient,
            IPersistentService persistentService)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _SqsClient = sqsClient ?? throw new ArgumentNullException(nameof(sqsClient));
            _PersistentService = persistentService ?? throw new ArgumentNullException(nameof(persistentService));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    var request = new ReceiveMessageRequest
                    {
                        QueueUrl = sqsQueueUrl,
                        WaitTimeSeconds = 20
                    };

                     var receiveMessageResponse = await _SqsClient.ReceiveMessageAsync(request).ConfigureAwait(false);

                     if (receiveMessageResponse != null && receiveMessageResponse.Messages != null)
                     {
                        var messages = receiveMessageResponse.Messages;
                        foreach (var message in messages)
                        {
                            try
                            {
                                var awsMessage = JsonSerializer.Deserialize<AWSSnsMessage>(message.Body);
                                var chatMessage = JsonSerializer.Deserialize<ChatMessage>(awsMessage.SerializedRequest);
                                await _PersistentService.ProcessChatMessageAsync(chatMessage);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                     }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
