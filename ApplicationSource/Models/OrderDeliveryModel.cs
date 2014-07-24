using System.Collections.Generic;

namespace ApplicationSource.Models
{
    public class OrderDeliveryModel
    {
        public string DeliveryNumber { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public AddressModel Address { get; set; }
        public string Comments { get; set; }
        public List<DeliveryOrderItemModel> NotScannedItems { get; set; }
        public List<DeliveryOrderItemModel> ScannedItems { get; set; } 
    }
}