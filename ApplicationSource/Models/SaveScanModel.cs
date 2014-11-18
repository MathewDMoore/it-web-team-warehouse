using Domain;

namespace ApplicationSource.Models
{
    public class SaveScanModel
    {
        public string ErrorMessage { get; set; }
        public DeliveryOrderItemModel UpdatedItem { get; set; }
    }
}