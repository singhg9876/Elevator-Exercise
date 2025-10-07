using ElevatorSystem.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Core.State
{
    public class ElevatorDestinationManager
    {
        public Queue<int> Destinations { get; } = new();

        public bool IsIdle => Destinations.Count == 0;

        public void AddDestination(int floor)
        {
            if (!Destinations.Contains(floor))
                Destinations.Enqueue(floor);
        }

        public int PeekNextDestination()
        {
            return Destinations.Count > 0 ? Destinations.Peek() : -1;
        }

        public void RemoveCurrentDestination()
        {
            if (Destinations.Count > 0)
                Destinations.Dequeue();
        }
    }
}
