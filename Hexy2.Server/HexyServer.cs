using RabbitMQ.Client;
using System;
using System.Text;
using Hexy2.Shared;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Hexy2.Shared.Message;

namespace Hexy2.Server
{
    public class HexyServer : IDisposable
    {
        private ConnectionFactory _factory = new ConnectionFactory() { HostName = "office", UserName = "user", Password = "user" };
        private IConnection _connection;
        private IModel _channel;
        private Hexy.System _system;

        public HexyServer()
        {
            _connection = _factory.CreateConnection();
            _channel= _connection.CreateModel();

            _channel.QueueDeclare(Queues.I2C_COMMAND_QUEUE, true, false, false, null);
            _channel.QueueDeclare(Queues.STATUS_REPORT_QUEUE, true, false, false, null);

            _system = new Hexy.System();
        }

        static void Main(string[] args)
        {
            using (var server = new HexyServer())
            {
                Console.WriteLine("Startup Completed");
                server.Run();
            }
        }

        public void Run()
        {
            var consumer = new QueueingBasicConsumer(_channel);
            
            _channel.BasicConsume(Queues.I2C_COMMAND_QUEUE, false, consumer);

            while (true)
            {
                BasicDeliverEventArgs e = consumer.Queue.Dequeue();
                                      
                var message = Encoding.UTF8.GetString(e.Body);

                Console.WriteLine("Received Message: {0}:{1}", Queues.I2C_COMMAND_QUEUE, message);

                var command = JsonConvert.DeserializeObject<ServoCommand>(message);

                _system.UpdateServo(command.ServoBoard, command.ServoChannel, command.Degrees);
                PublishReport(_system.RetrieveStatus());

                _channel.BasicAck(e.DeliveryTag, false);
            }
        }

        private void PublishReport(StatusReport report)
        {
            var message = JsonConvert.SerializeObject(report);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish("", Queues.STATUS_REPORT_QUEUE, null, body);
        }

        public int DegreeToPulse(int degrees)
        {
            int servoMin = 150;
            int servoMax = 800;
            int degreeMin = 0;
            int degreeMax = 180;

            if(degrees > degreeMax || degrees < degreeMin)
            {
                return (servoMax - servoMin) / 2 + servoMin;
            }

            var servoCountPerDegree = (servoMax - servoMin) / (degreeMax - degreeMin);

            return servoMin + (servoCountPerDegree * degrees);
        }

        public void Dispose()
        {
            if(_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
                _channel = null;
            }

            if(_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
