using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmitLogDirect
{
    using RabbitMQ.Client;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("direct_logs", "direct");

                while (true)
                {
                    var severity = randomSeverity();
                    var message = (args.Length > 1)
                                    ? string.Join(" ", args.Skip(1).ToArray())
                                    : "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("direct_logs", severity, null, body);
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);

                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }

        static string[] logs_level = new string[]
        {
            "info",
            "warning",
            "error"
        };

        static string randomSeverity()
        {
            var random = new Random();
            var log_index = random.Next(logs_level.Length);
            return logs_level[log_index];
        }
    }
}
