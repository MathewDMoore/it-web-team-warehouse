﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ApplicationSource.Models
{
    [DataContract]
    public class VerifyUniqueMacModel
    {
        [DataMember]
        public string MacId { get; set; }

        [DataMember]
        public string ProductGroup { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool IsUnique { get; set; }

        [DataMember]
        public int ErrorDeliveryNumber { get; set; }

        [DataMember]
        public string SerialCode { get; set; }

        [DataMember]
        public int Id { get; set; }
        
        [DataMember]
        public bool IsInternal { get; set; } 
        [DataMember]
        public bool AlreadyScanned { get; set; } 
        
        [DataMember]
        public int SerialNum { get; set; }        
        
        [DataMember]
        public int DocNum { get; set; }

    }
}
