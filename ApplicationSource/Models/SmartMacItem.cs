using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ApplicationSource.Models
{
    [DataContract]
    public class SmartMacItem
    {
        [DataMember]
        public string DeliveryNumber { get; set; }
        [DataMember]
        public string SmartMac { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public bool HasErrors
        {
            set { var hasErrors = value; }
            get { return !string.IsNullOrEmpty(ErrorMessage); }
        }
    }
}
