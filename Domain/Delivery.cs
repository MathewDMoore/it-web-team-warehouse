namespace Domain
{
    public class Delivery
    {
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public int DeliveryNumber { get; set; }
        public string Address { get; set; }
        public string Comments { get; set; }
        public bool IsIrDelivery{ get; set; }
    }
}