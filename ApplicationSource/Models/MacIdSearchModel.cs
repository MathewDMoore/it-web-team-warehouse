using System.Runtime.Serialization;

namespace ApplicationSource.Models
{

    [DataContract]
    public class MacIdSearchModel
    {
        [DataMember]
        public string SerialCode { get; set; }
        [DataMember]
        public string ProductId { get; set; }
        [DataMember]
        public string Color { get; set; }
    }
}
