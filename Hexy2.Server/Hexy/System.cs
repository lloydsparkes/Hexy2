using Raspberry.IO.Components.Controllers.Pca9685;
using Raspberry.IO.InterIntegratedCircuit;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.Components.Sensors.Compass.Hmc5883;
using Raspberry.IO.Components.Sensors.Accelerometer.Adxl345;
using Raspberry.IO.Components.Sensors.Gyro.L3g4200d;
using Raspberry.IO.Components.Sensors.Pressure.Bmp085;
using UnitsNet;
using Hexy2.Shared.Message;
using System;

namespace Hexy2.Server.Hexy
{
    /// <summary>
    /// Represents all the Devices in the System, as an Attempt to Abstract away
    /// </summary>
    public class System
    {
        private readonly I2cDriver _i2cDriver;

        private readonly Pca9685Connection _servoBoardA;
        private readonly Pca9685Connection _servoBoardB;
        private readonly Hmc5883Connection _compass;
        private readonly Adxl345Connection _accelerometer;
        private readonly L3g4200dConnection _gryo;
        private readonly Bmp085I2cConnection _pressure;

        public System()
        {
             _i2cDriver = new I2cDriver(ConnectorPin.P1Pin03.ToProcessor(), ConnectorPin.P1Pin05.ToProcessor());

            Console.WriteLine("Board A");
            _servoBoardA = new Pca9685Connection(_i2cDriver.Connect(0x40));
            Console.WriteLine("Board B");
            _servoBoardB = new Pca9685Connection(_i2cDriver.Connect(0x41));
            Console.WriteLine("Board Compass");
            _compass = new Hmc5883Connection(_i2cDriver.Connect(Hmc5883Connection.DefaultAddress));
            Console.WriteLine("Board Acc");
            _accelerometer = new Adxl345Connection(_i2cDriver.Connect(Adxl345Connection.DefaultAddress));
            Console.WriteLine("Board Gyro");
            _gryo = new L3g4200dConnection(_i2cDriver.Connect(L3g4200dConnection.DefaultAddress));
            Console.WriteLine("Board Pressure");
            _pressure = new Bmp085I2cConnection(_i2cDriver.Connect(Bmp085I2cConnection.DefaultAddress));


            Console.WriteLine("Board Update Rate");
            _servoBoardA.SetPwmUpdateRate(Frequency.FromHertz(60));
            _servoBoardB.SetPwmUpdateRate(Frequency.FromHertz(60));
        }

        public void UpdateServo(ServoBoard board, int channel, int degrees)
        {
            var pwmChannel = (PwmChannel)channel;
            var pulse = DegreeToPulse(degrees);

            if (board == ServoBoard.A)
            {
                _servoBoardA.SetPwm(pwmChannel, 0, pulse);
            }
            else
            {
                _servoBoardB.SetPwm(pwmChannel, 0, pulse);
            }
        }

        public StatusReport RetrieveStatus()
        {
            var report = new StatusReport();

            var compassData = _compass.GetData();
            report.Compass = new Axis(compassData.X, compassData.Y, compassData.Z);

            var accData = _accelerometer.GetData();
            report.Accelerometer = new Axis(accData.X.MeterPerSecondSquared, accData.Y.MeterPerSecondSquared, accData.Z.MeterPerSecondSquared);

            var gyroData = _gryo.GetData();
            report.Gyro = new Axis(gyroData.X, gyroData.Y, gyroData.Z);

            var envData = _pressure.GetData();
            report.Pressure = envData.Pressure.Bars;
            report.Temperature = envData.Temperature.DegreesCelsius;

            return report;
        } 

        private int DegreeToPulse(int degrees)
        {
            int servoMin = 200;
            int servoMax = 600;
            int degreeMin = 0;
            int degreeMax = 180;

            if (degrees > degreeMax || degrees < degreeMin)
            {
                return (servoMax - servoMin) / 2 + servoMin;
            }

            var servoCountPerDegree = (servoMax - servoMin) / (degreeMax - degreeMin);

            return servoMin + (servoCountPerDegree * degrees);
        }


    }
}
