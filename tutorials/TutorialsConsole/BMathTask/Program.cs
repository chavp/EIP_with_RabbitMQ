using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMathTask
{
    using DynamicRouter.Models;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Diagnostics;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var myDynamicRule = new DynamicRule
            {
                Name = "b_queue",
                CurrentElapsed = TimeSpan.Zero,
                TotalService = 0
            };

            var randomElapsedProcess = new Random(DateTime.Now.Second);

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(myDynamicRule.Name, true, false, false, null);
                channel.BasicQos(0, 1, false);

                UdpateDynamicRule(myDynamicRule, channel);

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(myDynamicRule.Name, false, consumer);

                Console.WriteLine(" [*] Waiting for Number for A-Math Provider. " +
                                  "To exit press CTRL+C");
                while (true)
                {
                    var ea =
                        (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] A-Math Process {0}", message);

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Thread.Sleep(randomElapsedProcess.Next(1000));

                    myDynamicRule.CurrentElapsed = stopwatch.Elapsed;
                    ++myDynamicRule.TotalService;

                    UdpateDynamicRule(myDynamicRule, channel);

                    Console.WriteLine(" [x] Process Done " + ea.DeliveryTag);
                    channel.BasicAck(ea.DeliveryTag, false);

                }
            }
        }

        static void UdpateDynamicRule(DynamicRule myDynamicRule, IModel channel)
        {
            var updateControl = JsonConvert.SerializeObject(myDynamicRule);
            var updateControlBody = Encoding.UTF8.GetBytes(updateControl);
            channel.BasicPublish("", "dynamic_control_router", null, updateControlBody);
        }
    }
}
