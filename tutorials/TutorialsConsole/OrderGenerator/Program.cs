using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "new_orders";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, "topic");

                while (true)
                {
                    var orderID = Guid.NewGuid().ToString();
                    var order_type = random_order_types();
                    var routingKey = string.Format("{0}.{1}", orderID, order_type);
                    var message = "new order " + order_type + " id " + orderID;
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange, routingKey, null, body);
                    Console.WriteLine(" [x] new Order '{0}':'{1}'", routingKey, message);

                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
        }

        static string[] order_types = new string[] { 
            "widget", "gadget", "normal"
        };

        static Random _random = new Random(DateTime.Now.Second);
        static string random_order_types()
        {
            return order_types[_random.Next(order_types.Length)];
        }
    }
}
