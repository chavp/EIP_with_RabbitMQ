using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAdderTask
{
    using RabbitMQ.Client;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var routingKey = "new_adder_task_queue";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.QueueDeclare(routingKey, true, false, false, null);

                while (true)
                {
                    var message = randomNumber().ToString();
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true);

                    channel.BasicPublish("", routingKey, properties, body);
                    Console.WriteLine(" [x] Sent Adder Task {0}", message);

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
        }

        static Random _ran = new Random(DateTime.Now.Second);
        static int randomNumber()
        {
            return _ran.Next(int.MaxValue);
        }
    }
}
