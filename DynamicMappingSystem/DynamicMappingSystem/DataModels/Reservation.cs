namespace DynamicMappingSystem.DataModels
{
    public class Reservation
    {
        public string Id { get; set; }
        public Customer Customer{ get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
