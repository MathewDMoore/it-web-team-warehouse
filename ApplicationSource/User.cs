using System.Collections.Generic;

namespace ApplicationSource
{
    public class User
    {
        public IList<string> Groups { get; set; }
        public string UserName { get; set; }
    }
}