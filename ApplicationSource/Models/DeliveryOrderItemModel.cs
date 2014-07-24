﻿using System;

namespace ApplicationSource.Models
{
    public class DeliveryOrderItemModel
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string RealItemCode { get; set; }
        public string AltText { get; set; }
        public int SerialNum { get; set; }
        public string ReturnedByUser { get; set; }
        public string SerialCode { get; set; }
        public bool NoSerialization { get; set; }
        public bool SmartCodeOnly { get; set; }
        public string ProductId { get; set; }
        public string Color { get; set; }   
        public string ProductGroup { get; set; }   
    }
}