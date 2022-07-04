using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using DbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KafkaInfrastructure
{
    public class Consumer : IHostedService
    {
        public ContactContext Context { get; set; }
        private readonly string _queueName = "ReportStatus";

        private ConsumerConfig KafkaConfig => new()
        {
            GroupId = "messageConsumer" + DateTime.Now.Second,
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        public Consumer()
        {

        }



        public Task StartAsync(CancellationToken cancellationToken)
        {

            try
            {
                Task.Run(async () =>
                {
                    using (var consumerBuilder = new ConsumerBuilder
                               <Ignore, string>(KafkaConfig).Build())
                    {
                        consumerBuilder.Subscribe(_queueName);
                        var cancelToken = new CancellationTokenSource();

                        try
                        {
                            while (true)
                            {
                                var consumer = consumerBuilder.Consume
                                    (cancelToken.Token);
                                if (Guid.TryParse(consumer.Message.Value, out Guid reportStatusId))
                                    try
                                    {
                                        await Task.Delay(15 * 1000);
                                        new ReportStatusPrepare(Context, reportStatusId).PrepareReport();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                        throw;
                                    }


                            }
                        }
                        catch (OperationCanceledException)
                        {
                            consumerBuilder.Close();
                        }
                    }
                });


            }
            catch (Exception ex)
            {

            }

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
