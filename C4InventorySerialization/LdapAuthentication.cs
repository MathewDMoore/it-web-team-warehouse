using System;
using System.Configuration;
using System.Text;
using System.Collections;
using System.DirectoryServices;

namespace FormsAuth
{
    public class LdapAuthentication
    {
        private String _path;
        private String _filterAttribute;

        public LdapAuthentication(String path)
        {
            _path = path;
        }

        public bool IsAuthenticated(String username, String pwd)
        {
            String domain = ConfigurationManager.AppSettings["Domain"];
            String domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);

            try
            {
                //Bind to the native AdsObject to force authentication.			
                Object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return false;
                }

                //Update the new path to the user in the directory.
                _path = result.Path;
                _filterAttribute = (String) result.Properties["cn"][0];

                bool admin = IsAccountMemberOfGroup(_path, "CN=c4shipping-admins,CN=Users,DC=control4,DC=com");
                bool manager = IsAccountMemberOfGroup(_path, "CN=c4shipping-managers,CN=Users,DC=control4,DC=com");
                bool user = IsAccountMemberOfGroup(_path, "CN=c4shipping-users,CN=Users,DC=control4,DC=com");

                if (admin != true && manager != true && user != true)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user. " + ex.Message);
            }

            return true;
        }

        public static bool IsAccountMemberOfGroup(string account, string group)
        {
            bool found = false;

            using (DirectoryEntry entry = new DirectoryEntry(account))
            {
                entry.RefreshCache(new string[] { "memberOf" });

                foreach (string memberOf in entry.Properties["memberOf"])
                {
                    if (string.Compare(memberOf, group, true) == 0)
                    {
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }

        public String GetGroups()
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + _filterAttribute + ")";
            StringBuilder groupNames = new StringBuilder();

            try
            {
                bool admin = IsAccountMemberOfGroup(_path, "CN=c4shipping-admins,CN=Users,DC=control4,DC=com");
                bool manager = IsAccountMemberOfGroup(_path, "CN=c4shipping-managers,CN=Users,DC=control4,DC=com");
                bool user = IsAccountMemberOfGroup(_path, "CN=c4shipping-users,CN=Users,DC=control4,DC=com");

                if (admin == true)
                {
                    groupNames.Append("Admin");
                    groupNames.Append("|");
                }
                if (user == true)
                {
                    groupNames.Append("User");
                    groupNames.Append("|");
                }
                if (manager == true)
                {
                    groupNames.Append("Manager");
                    groupNames.Append("|");
                }

                if (admin != true || manager != true || user != true)
                {
                    return groupNames.Append("").ToString();
                }

            }

            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            return groupNames.ToString();
        }
    }
}
