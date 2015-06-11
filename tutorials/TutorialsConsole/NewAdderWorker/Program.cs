using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAdderWorker
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var routingKey = "new_adder_task_queue";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var arguments = new Dictionary<String, Object>();
                arguments.Add("x-message-ttl", 1000);
                arguments.Add("x-dead-letter-exchange", "dlx");

                channel.QueueDeclare(routingKey, true, false, false, arguments);

                channel.BasicQos(0, 1, false);
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(routingKey, false, consumer);

                Console.WriteLine(" [*] Waiting for new adder task. " +
                                  "To exit press CTRL+C");
                while (true)
                {
                    var ea =
                        (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    Console.WriteLine(" [x] Done " + ea.DeliveryTag);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        }
    }
}
