using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathRPCServer
{
    using MathRpc.Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Diagnostics;
    using System.Dynamic;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("math_rpc_queue", false, false, false, null);
                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("math_rpc_queue", false, consumer);
                    Console.WriteLine(" [x] Awaiting Math RPC requests");

                    while (true)
                    {
                        int response = 0;
                        TimeSpan elapsed = TimeSpan.Zero;
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var props = ea.BasicProperties;
                        props.ContentType = "application/json";
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;

                        try
                        {
                            var message = Encoding.UTF8.GetString(body);
                            var cmd = JsonConvert.DeserializeObject<MathCommand>(message);

                            if (cmd.Math == "fib")
                            {
                                Console.WriteLine(" [.] fib({0})", cmd.N);
                                var timeWatcher = Stopwatch.StartNew();
                                response = fib(cmd.N);
                                elapsed = timeWatcher.Elapsed;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" [.] " + e.Message);
                        }
                        finally
                        {
                            var message = JsonConvert.SerializeObject(new Result { N = response, Elapsed = elapsed });
                            var responseBytes = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish("", props.ReplyTo, replyProps,
                                                 responseBytes);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                }
            }
        }

        private static int fib(int n)
        {
            if (n == 0 || n == 1) return n;
            return fib(n - 1) + fib(n - 2);
        }
    }
}
