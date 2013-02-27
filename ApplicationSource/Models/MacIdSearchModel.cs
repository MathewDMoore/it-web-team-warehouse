using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ApplicationSource.Models
{
    [DataContract]
    class MacIdSearchModel
    {
        [DataMember]
        IList<MacIdItem> MacIdSearchItems { get; set; }
    }
}
