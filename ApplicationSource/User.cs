using System.Collections.Generic;
using System.Security.Principal;

namespace ApplicationSource
{
    public class User:IIdentity
    {

        public User(string name, string username, bool isAuthenticated)
        {
            Name = name;
            UserName = username;
            IsAuthenticated = isAuthenticated;
        }

        public IList<string> Groups { get; set; }
        public string UserName { get; set; }
        public string Name { get; }
        public string AuthenticationType { get; }
        public bool IsAuthenticated { get; }
    }
}