using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hexy2.Shared;
using RabbitMQ.Client.Events;
using Raspberry.IO.Components.Controllers.Pca9685;
using Raspberry.IO.InterIntegratedCircuit;
using Raspberry.IO.GeneralPurpose;
using UnitsNet;
using System.Threading;
using Newtonsoft.Json;
using Hexy2.Shared.Message;

namespace Hexy2.Server
{
    public class HexyServer : IDisposable
    {
        private ConnectionFactory _factory = new ConnectionFactory() { HostName = "office", UserName = "user", Password = "user" };
        private IConnection _connection;
        private IModel _channel;

        private Pca9685Connection _driver;

        public HexyServer()
        {
            _connection = _factory.CreateConnection();
            _channel= _connection.CreateModel();

            _channel.QueueDeclare(Queues.I2C_COMMAND_QUEUE, true, false, false, null);

            var i2cDriver = new I2cDriver(ConnectorPin.P1Pin03.ToProcessor(), ConnectorPin.P1Pin05.ToProcessor());
            var i2cConnection = i2cDriver.Connect(0x40);
            _driver = new Pca9685Connection(i2cConnection);

            _driver.SetPwmUpdateRate(Frequency.FromHertz(60));
        }

        static void Main(string[] args)
        {
            using (var server = new HexyServer())
            {
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

                var pwmChannel = (PwmChannel)command.ServoChannel;
                var pulse = DegreeToPulse(command.Degrees);

                _driver.SetPwm(pwmChannel, 0, pulse);

                _channel.BasicAck(e.DeliveryTag, false);
            }
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
