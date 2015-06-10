using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathRPCClient
{
    using MathRpc.Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Dynamic;

    class MateRPCApi : IDisposable
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private QueueingBasicConsumer consumer;

        public MateRPCApi()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare();
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(replyQueueName, true, consumer);
        }

        public Result Fib(int n)
        {
            var corrId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = corrId;
            props.ContentType = "application/json";

            var message = JsonConvert.SerializeObject(new MathCommand { Math = "fib",  N = n });

            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", "math_rpc_queue", props, messageBytes);

            while (true)
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId == corrId)
                {
                    var result = Encoding.UTF8.GetString(ea.Body);
                    var response = JsonConvert.DeserializeObject<Result>(result);
                    return response;
                }
            }
        }

        public void Close()
        {
            connection.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
