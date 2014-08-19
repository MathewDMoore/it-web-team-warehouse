namespace Domain
{
    public class DeliveryOrderItemsQuery
    {
        public int DocNum { get; set; }
        public string ServerLocation { get; set; } 
        public string Username { get; set; }
        public bool IsInternal { get; set; }
    }
}