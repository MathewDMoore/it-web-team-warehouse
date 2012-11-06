namespace Domain
{
    public class Delivery
    {
        public string DeliveryNumber { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public Address Address { get; set; }
        public string Comments { get; set; }
    }
}