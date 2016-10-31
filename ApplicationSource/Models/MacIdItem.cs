using System.Runtime.Serialization;

namespace ApplicationSource.Models
{
    [DataContract]
    public class MacIdItem
    {
        [DataMember]
        public string DeliveryNumber { get; set; }
        [DataMember]
        public string MacId { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public bool HasErrors
        {
            set { }
            get { return !string.IsNullOrEmpty(ErrorMessage); }
        }
        [DataMember]
        public bool IsIRDelivery { get; set; }
    }
}
