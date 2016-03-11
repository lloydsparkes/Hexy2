using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexy2.Shared.Message
{
    public class StatusReport
    {
        public Axis Compass { get; set; }
        public Axis Accelerometer { get; set; }
        public Axis Gyro { get; set; }
        public double Pressure { get; set; }
        public double Temperature { get; set; }
    }
}
