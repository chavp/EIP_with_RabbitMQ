using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognizedOddNumber
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "random_number";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, exchange, "");

                var queueInvalidOddNumber = "invalid_odd_number_queue";
                channel.QueueDeclare(queueInvalidOddNumber, true, false, false, null);

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine(" [*] Waiting for random number." +
                                  "To exit press CTRL+C");
                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        int number = int.Parse(message);
                        if (number % 2 == 0)
                        {
                            Console.WriteLine(" [V] Is Odd {0}", number);
                        }
                        else throw new ApplicationException();
                    }
                    catch
                    {

                        var properties = channel.CreateBasicProperties();
                        properties.SetPersistent(true);

                        channel.BasicPublish("", queueInvalidOddNumber, properties, body);
                        Console.WriteLine(" [x] Invalid Odd Number {0}", message);
                    }
                }
            }
        }
    }
}
