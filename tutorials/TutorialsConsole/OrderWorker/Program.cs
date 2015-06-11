using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderWorker
{
    using RabbitMQ.Client;
    using System.Threading;

    class Program
    {
        static int _orderNumber = 0;
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("submit_order_queue", true, false, false, null);

                // Simulate submit order
                while (true)
                {
                    var message = SubmitOrder();
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true);

                    channel.BasicPublish("", "submit_order_queue", properties, body);
                    Console.WriteLine(" [x] Sent {0}", message);

                    Thread.Sleep(1000);
                }
            }
        }

        static string SubmitOrder()
        {
            return string.Format("Order #{0}", ++_orderNumber);
        }
    }
}
