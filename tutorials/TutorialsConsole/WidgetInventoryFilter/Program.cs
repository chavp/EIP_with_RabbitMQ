using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WidgetInventoryFilter
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

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

                var queueName = channel.QueueDeclare("filter_widget", true, false, true, null).QueueName;
                var widgetQueueName = channel.QueueDeclare("widget", true, false, true, null).QueueName;

                channel.QueueBind(queueName, exchange, "#");

                Console.WriteLine(" [*] Waiting for filter new order. " +
                                  "To exit press CTRL+C");

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, false, consumer);

                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    if (message.Contains("widget"))
                    {
                        channel.BasicPublish("", widgetQueueName, null, body);
                        Console.WriteLine(" [x] OK Widget '{0}' Completed", message);
                    }
                    else
                    {
                        Console.WriteLine(" [x] Filter '{0}' Completed", message);
                    }

                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        }
    }
}
