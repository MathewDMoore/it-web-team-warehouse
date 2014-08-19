using System.Collections.Generic;
using System.Linq;

namespace ApplicationSource.Models
{
    public class OrderDeliveryModel
    {
        public OrderDeliveryModel()
        {
            NotScannedItems = new List<DeliveryOrderItemModel>();
            ScannedItems = new List<DeliveryOrderItemModel>();
        }

        public int DeliveryNumber { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public string Address { get; set; }
        public string Comments { get; set; }
        public string Error { get; set; }
        public bool IsInternal { get; set; }
        public List<DeliveryOrderItemModel> NotScannedItems { get; set; }
        public List<DeliveryOrderItemModel> ScannedItems { get; set; }

        public bool IsVerified
        {
            get { return ScannedItems.Any(x => !x.Verified) && (NotScannedItems.Count == 0 ||  NotScannedItems.All(y => y.NoSerialRequired)) ; }
            set { }
        }
    }
}