using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingWorker
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("submit_order_queue", true, false, false, null);

                channel.BasicQos(0, 1, false);
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume("submit_order_queue", false, consumer);

                Console.WriteLine(" [*] Waiting for submit order. " +
                                  "To exit press CTRL+C");
                while (true)
                {
                    var ea =
                        (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Shipped {0}", message);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        }
    }
}
