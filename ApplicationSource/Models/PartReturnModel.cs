using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ApplicationSource.Models
{
    [DataContract]
    public class PartReturnModel
    {
        [DataMember]
        public IList<ReturnLineItem> PartReturnItems { get; set; }
 
    }
}