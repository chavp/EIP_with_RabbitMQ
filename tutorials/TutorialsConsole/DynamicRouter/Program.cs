using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRouter
{
    using DynamicRouter.Models;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.IO;

    /// <summary>
    /// Use Messaging Channels > Invalid Message Channel > NumberGenerator
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            LoadDynamicRuleBaseData();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "random_number";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                // Init Message Router
                //foreach (var dynamicRuleBase in _dynamicRuleBaseData)
                //{
                //    channel.QueueDeclare(dynamicRuleBase.Name, true, false, true, null);
                //}

                channel.ExchangeDeclare(exchange, "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, exchange, "");

                var consumer_random = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, true, consumer_random);

                Task.Run(() =>
                {
                    Console.WriteLine(" [*] Waiting for random number." +
                                      "To exit press CTRL+C");

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer_random.Queue.Dequeue();
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [X] Random Number {0} incoming", message);

                        // Decistion Base Elapsed and minimum total service
                        var dynamicRuleBase = (from x in _dynamicRuleBaseData
                                               let minimumDecistionBase = _dynamicRuleBaseData.Min(y => y.CurrentElapsed.TotalMilliseconds)
                                               let xMin = x.CurrentElapsed.TotalMilliseconds
                                               where xMin == minimumDecistionBase
                                               select x).FirstOrDefault();
                        // Message Router
                        if (dynamicRuleBase != null)
                        {
                            channel.BasicPublish("", dynamicRuleBase.Name, null, body);
                            Console.WriteLine(" [X] Rout {0} to {1} completed", message, dynamicRuleBase.Name);
                        }
                    }
                });

                var dynamic_control_router_QueueName = channel.QueueDeclare("dynamic_control_router", true, false, false, null).QueueName;

                var consumer_dynamic_control_router = new QueueingBasicConsumer(channel);
                channel.BasicConsume(dynamic_control_router_QueueName, false, consumer_dynamic_control_router);

                Task.Run(() =>
                {
                    //channel.QueueBind(queueName, exchange, "");

                    Console.WriteLine(" [*] Waiting for control math task." +
                                      "To exit press CTRL+C");

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer_dynamic_control_router.Queue.Dequeue();
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [X] Dynamic Rule Base {0} config", message);

                        var dynamicRule = JsonConvert.DeserializeObject<DynamicRule>(message);

                        lock (_dynamicRuleBaseData)
                        {
                            var oldDynamicRule = (from x in _dynamicRuleBaseData where x.Name == dynamicRule.Name select x).FirstOrDefault();
                            if (oldDynamicRule != null)
                            {
                                oldDynamicRule.CurrentElapsed = dynamicRule.CurrentElapsed;
                                oldDynamicRule.TotalService = dynamicRule.TotalService;
                            }
                            else
                            {
                                _dynamicRuleBaseData.Add(dynamicRule);
                            }

                            SaveDynamicRuleBaseData();
                        }

                        Console.WriteLine(" [x] Udate Dynamic Rule Base {0} Done ", dynamicRule.Name);
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                });

                Console.ReadLine();
            }

            //SaveDynamicRuleBaseData();
        }

        static List<DynamicRule> _dynamicRuleBaseData = null;
        static string _dynamicRuleBasePath = @"data.json";
        static void LoadDynamicRuleBaseData()
        {
            _dynamicRuleBaseData = 
                JsonConvert.DeserializeObject<List<DynamicRule>>(File.ReadAllText(_dynamicRuleBasePath));
        }
        static void SaveDynamicRuleBaseData()
        {
            File.WriteAllText(_dynamicRuleBasePath, JsonConvert.SerializeObject(_dynamicRuleBaseData));
        }
    }
}
