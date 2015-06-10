using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmitLogTopic
{
    using RabbitMQ.Client;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("topic_logs", "topic");

                    while (true)
                    {
                        var routingKey = randomRoutingKey();
                        var message = (args.Length > 1)
                                         ? string.Join(" ", args.Skip(1).ToArray())
                                         : "Hello World!";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("topic_logs", routingKey, null, body);
                        Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);

                        Thread.Sleep(TimeSpan.FromSeconds(3));
                    }
                }
            }
        }

        static string[] speeds = new string[] { 
            "lazy", "normal", "fast" 
        };
        static string[] colours = new string[] { 
            "red", "orange", "yellow", "green", "blue", "indigo", "violet"
        };
        static string[] species = new string[] { 
            "rabbit", "cat", "dog", "tortoise"
        };

        static string randomRoutingKey()
        {
            var random = new Random();
            var speeds_index = random.Next(speeds.Length);
            var colours_index = random.Next(colours.Length);
            var species_index = random.Next(species.Length);

            return string.Format("{0}.{1}.{2}",
                speeds[speeds_index], colours[colours_index], species[species_index]);
        }
    }
}
