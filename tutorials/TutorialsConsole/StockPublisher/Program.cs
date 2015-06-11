using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPublisher
{
    using RabbitMQ.Client;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            string data_path = @"D:\projects\Git\EIP_with_RabbitMQ\tutorials\TutorialsConsole\data\items.json";
            string data_json = File.ReadAllText(data_path);

            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "stock_items";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, "fanout");

                while (true)
                {
                    var items_json = GetItems();
                    var body = Encoding.UTF8.GetBytes(items_json);
                    channel.BasicPublish(exchange, "", null, body);
                    Console.WriteLine(" [x] Update {0}", items_json);

                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
        }

        private static string GetItems()
        {
            string data_path = @"D:\projects\Git\EIP_with_RabbitMQ\tutorials\TutorialsConsole\data\items.json";
            string data_json = File.ReadAllText(data_path);

            return data_json;
        }
    }
}
