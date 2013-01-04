using System;
using System.Runtime.Serialization;

namespace ApplicationSource.Models
{
    [DataContract]
    public class ReturnLineItem
    {
        [DataMember]
        public string SmartMac { get; set; }
        [DataMember]
        public string DocNum { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool HasErrors
        {
            set { var hasErrors = value; }
            get { return !string.IsNullOrEmpty(ErrorMessage); }
        }

        [DataMember]
        public bool Success
        {
            get;
            set;
        }
    }
}