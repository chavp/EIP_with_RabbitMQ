using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberGenerator
{
    using RabbitMQ.Client;
    using System.Threading;

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

                while (true)
                {
                    var message = randomNumber().ToString();
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange, "", null, body);
                    Console.WriteLine(" [x] Random {0}", message);

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
