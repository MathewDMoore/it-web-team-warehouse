using System.Collections.Generic;

namespace ApplicationSource.Models
{
    public class OrderDeliveryModel
    {
        public OrderDeliveryModel()
        {
            NotScannedItems = new List<DeliveryOrderItemModel>();
            ScannedItems = new List<DeliveryOrderItemModel>();
        }

        public string DeliveryNumber { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public string Address { get; set; }
        public string Comments { get; set; }
        public List<DeliveryOrderItemModel> NotScannedItems { get; set; }
        public List<DeliveryOrderItemModel> ScannedItems { get; set; } 
    }
}