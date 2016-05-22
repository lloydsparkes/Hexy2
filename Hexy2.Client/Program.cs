using Hexy2.Shared;
using Hexy2.Shared.Message;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
                channel.QueueDeclare(Queues.STATUS_REPORT_QUEUE, true, false, false, null);

                var consumer = new QueueingBasicConsumer(channel);

                channel.BasicConsume(Queues.STATUS_REPORT_QUEUE, false, consumer);

                List<ServoCommand> commands = new List<ServoCommand>();
                ServoBoard board = ServoBoard.A;
                for(int i =0; i < 16; i++)
                {
                    commands.Add(new ServoCommand() { ServoBoard = board, Degrees = 90, ServoChannel = i });
                }
                board = ServoBoard.B;
                for (int i = 0; i < 16; i++)
                {
                    commands.Add(new ServoCommand() { ServoBoard = board, Degrees = 90, ServoChannel = i });
                }

                foreach(var command in commands)
                {
                    BasicDeliverEventArgs item = consumer.Queue.DequeueNoWait(null);
                    if(item != null)
                    {
                        var data = Encoding.UTF8.GetString(item.Body);
                        Console.WriteLine(data);

                        channel.BasicAck(item.DeliveryTag, false);
                    }

                    var message = JsonConvert.SerializeObject(command);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", Queues.I2C_COMMAND_QUEUE, null, body);
                }
            }
        }
    }
}
