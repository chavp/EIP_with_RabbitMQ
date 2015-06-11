using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUP
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

                Console.WriteLine(" [*] Waiting for CPU instruction." +
                                  "To exit press CTRL+C");

                Task.Run(() =>
                {
                    while (true)
                    {
                        string json = JsonConvert.SerializeObject(new Instruction
                        {
                            Command = "tick",
                            Arguments = Tick().ToString(),
                            Module = "CPU",
                        });
                        var body = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(exchange, "", null, body);
                        Console.Write(".");
                    }
                });

                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var m = JsonConvert.DeserializeObject<Instruction>(message);
                    if (m.Module != "CPU" && m.Command != "tick") 
                    {
                        Console.WriteLine(" [x] {0}", message);
                    }
                }
            }
        }

        static int _tick = 0;
        static int Tick()
        {
            if (_tick == 0) ++_tick;
            else if (_tick == 1) --_tick;
            Thread.Sleep(TimeSpan.FromSeconds(2));
            return _tick;
        }
    }
}
