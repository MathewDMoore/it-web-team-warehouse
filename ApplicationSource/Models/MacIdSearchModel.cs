using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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
