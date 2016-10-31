using System.Runtime.Serialization;

namespace ApplicationSource.Models
{
    [DataContract]
    public class ReturnLineItem
    {
        [DataMember]
        public string SmartCode { get; set; }
        [DataMember]
        public string DocNum { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool HasErrors
        {
            set {  }
            get { return !string.IsNullOrEmpty(ErrorMessage); }
        }

        [DataMember]
        public bool Success
        {
            get;
            set;
        }

        public string MacId
        {
            get
            {
                return SmartCode.Length >= 29 ? SmartCode.Remove(SmartCode.Length - 17, 17) : SmartCode;
            }

        }
    }
}