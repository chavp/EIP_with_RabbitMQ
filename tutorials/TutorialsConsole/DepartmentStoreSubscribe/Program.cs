using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepartmentStoreSubscribe
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "stock_items";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, "fanout");

                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queueName, exchange, "");
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queueName, true, consumer);

                    Console.WriteLine(" [*] Waiting for stock items." +
                                      "To exit press CTRL+C");
                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] {0}", message);
                    }
                }
            }
        }
    }
}
