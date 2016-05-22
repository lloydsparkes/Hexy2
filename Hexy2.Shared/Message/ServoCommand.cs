using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexy2.Shared.Message
{
    public class ServoCommand
    { 
        public ServoBoard ServoBoard { get; set; }

        public int ServoChannel { get; set; }

        public int Degrees { get; set; }
    }
}
