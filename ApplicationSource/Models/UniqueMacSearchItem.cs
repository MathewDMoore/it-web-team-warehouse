using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ApplicationSource.Models
{
    [DataContract]
    public class UniqueMacSearchItem
    {
        [DataMember]
        private IList<string> items { get; set; }
    }
}