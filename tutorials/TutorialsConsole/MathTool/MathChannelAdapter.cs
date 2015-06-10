using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathTool
{
    using Math.Lib;
    using MathRpc.Messages;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class MathChannelAdapter
        : IMath
    {

        private ConnectionFactory factory;
        //private IModel channel;
        //private string replyQueueName;
       // private QueueingBasicConsumer consumer;

        public MathChannelAdapter()
        {
            factory = new ConnectionFactory() { HostName = "localhost" };
        }

        public int Fib(int n)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var replyQueueName = channel.QueueDeclare();
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(replyQueueName, true, consumer);
                var corrId = Guid.NewGuid().ToString();
                var props = channel.CreateBasicProperties();
                props.ReplyTo = replyQueueName;
                props.CorrelationId = corrId;
                props.ContentType = "application/json";

                var message = JsonConvert.SerializeObject(new MathCommand { Math = "fib", N = n });

                var messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", "math_rpc_queue", props, messageBytes);

                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId == corrId)
                {
                    var result = Encoding.UTF8.GetString(ea.Body);
                    var response = JsonConvert.DeserializeObject<Result>(result);
                    return response.N;
                }

                return 0;
            }
        }
    }
}
