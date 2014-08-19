using System.Collections.Generic;

namespace ApplicationSource.Models
{
    public class ReturnModel
    {
        public List<int> Ids { get; set; }
        
        public bool IsInternal { get; set; }
    }
}