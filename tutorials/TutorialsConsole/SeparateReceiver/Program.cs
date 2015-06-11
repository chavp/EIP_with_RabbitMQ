using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeparateReceiver
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    class Program
    {
        private static string[] data_types = new string[] { 
            "query",
            "price_quote",
            "purchase_order"
        };

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.BasicQos(0, 1, false);

                foreach (var queue in data_types)
                {
                    channel.QueueDeclare(queue, true, false, false, null);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queue, false, consumer);

                    Task.Run(() =>
                    {
                        while (true)
                        {
                            var ea =
                                (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            //Console.WriteLine(" [x] Received {0} {1}", queue, message);

                            Console.WriteLine(" [x] Process " + queue + " Done " + ea.DeliveryTag);

                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    });
                }

                Console.WriteLine(" [*] Waiting for messages. " +
                                      "To exit press CTRL+C");
                Console.ReadLine();
            }
        }
    }
}
