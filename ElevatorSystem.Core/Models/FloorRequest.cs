using ElevatorSystem.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Core.Models
{
    public class FloorRequest
    {
        public int Floor { get; set; }
        public Direction Direction { get; set; }
    }
}
