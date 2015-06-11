using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiveNewAdderDeadTask
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
                channel.ExchangeDeclare("dlx", "direct");
                var queueOK = channel.QueueDeclare("dl", false, false, false, null);

                string queueName = queueOK.QueueName;

                channel.QueueBind(queueName, "dlx", routingKey);

                Console.WriteLine(" [*] Waiting for dead new adder task. " +
                                  "To exit press CTRL+C");

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, false, consumer);

                while (true)
                {
                    var ea =
                        (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        }
    }
}
