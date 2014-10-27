﻿using System.Collections.Generic;
using Domain;

namespace ApplicationSource.Models
{
    public class ReturnModel
    {
        public int DeliveryNumber { get; set; }
        public List<SerialNumberItem> SelectedList { get; set; }
        //public List<int> Ids { get; set; }
        public bool IsInternal { get; set; }
    }
}