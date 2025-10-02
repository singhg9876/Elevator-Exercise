namespace ElevatorApi.Models
{
    /// <summary>
    /// Represents a request made by a user for an elevator at a specific floor and direction.
    /// </summary>    
    public class FloorRequest
    {
        public int Floor { get; set; }
        public Direction Direction { get; set; }
    }
}
