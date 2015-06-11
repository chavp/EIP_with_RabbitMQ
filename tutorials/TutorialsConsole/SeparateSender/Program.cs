using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeparateSender
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
                foreach (var queue in data_types)
                {
                    channel.QueueDeclare(queue, true, false, false, null);
                }

                while (true)
                {
                    var message = randomData();
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true);

                    foreach (var queue in data_types)
                    {
                        if (message == queue)
                        {
                            channel.BasicPublish("", queue, properties, body);
                            Console.WriteLine(" [x] Sent {0}", message);
                            break;
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                
            }
        }

        private static string[] data_types = new string[] { 
            "query",
            "price_quote",
            "purchase_order"
        };

        private static string randomData()
        {
            var random = new Random();
            var data_index = random.Next(data_types.Length);
            return data_types[data_index];
        }
    }
}
