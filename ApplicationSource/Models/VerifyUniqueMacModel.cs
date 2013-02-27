using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ApplicationSource.Models
{
    [DataContract]
    class VerifyUniqueMacModel
    {
        [DataMember]
        string MacId { get; set; }
    }
}
