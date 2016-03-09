using Hexy2.Shared;
using Hexy2.Shared.Message;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hexy2.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "office", UserName = "user", Password = "user" };

            using (var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(Queues.I2C_COMMAND_QUEUE, true, false, false, null);

                int i = 0;

                while (true)
                {
                    i = i + 10;

                    if(i >= 180)
                    {
                        i = 0;
                    }

                    Thread.Sleep(1000);

                    var command = new ServoCommand() { Degrees = i, ServoChannel = 0 };
                    var message = JsonConvert.SerializeObject(command);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", Queues.I2C_COMMAND_QUEUE, null, body);
                }
            }
        }
    }
}
