using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Display
{
    using MessageBus.Models;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var ticks = 0;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "message-bus";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, "fanout");

                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queueName, exchange, "");
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine(" [*] Waiting for Display instruction." +
                                  "To exit press CTRL+C");

                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var m = JsonConvert.DeserializeObject<Instruction>(message);
                    if (m.Module == "CPU" && m.Command == "tick")
                    {
                        ++ticks;
                        Console.WriteLine(" [x] {0}", ticks);
                    }
                }
            }
        }
    }
}
