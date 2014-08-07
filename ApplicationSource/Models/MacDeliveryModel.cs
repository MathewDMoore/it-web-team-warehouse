using System.Runtime.Serialization;

namespace ApplicationSource.Models
{
    [DataContract]
    public class MacDeliveryModel
    {
        [DataMember]
        public int DeliveryNumber { get; set; }
        [DataMember(IsRequired = false)]
        public string Error { get; set; }        
    }
}