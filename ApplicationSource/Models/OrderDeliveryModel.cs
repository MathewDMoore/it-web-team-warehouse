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
            ActiveKits = new Dictionary<string, List<DeliveryOrderItemModel>>();
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
        public Dictionary<string, List<DeliveryOrderItemModel>> ActiveKits { get; set; }
        public bool IsVerified
        {
            get { return (NotScannedItems.Any(x => x.Verified) || ScannedItems.Any(x => x.Verified)); }
            set { }
        }
    }
}