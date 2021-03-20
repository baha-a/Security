using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace ITech.Security.Business
{
    public class ActiveDirectoryUser
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsSame(ActiveDirectoryUser u)
        {
            return FirstName.Equals(u.FirstName);
        }
    }

    public static class ActiveDirectory
    {
        private static IList<ActiveDirectoryUser> CachedData = null;

        public static void ResetCachedData()
        {
            CachedData = null;
        }

        public static ActiveDirectoryUser GetUser(string userName, string domain)
        {
            return GetUsers(domain).SingleOrDefault(x => x.UserName.ToLower() == userName.ToLower());
        }

        public static IList<ActiveDirectoryUser> GetUsers(string domain)
        {
            if (CachedData != null)
                return CachedData;

            List<ActiveDirectoryUser> Accounts = new List<ActiveDirectoryUser>();

            using (var context = new PrincipalContext(ContextType.Domain, domain))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    putTheResultIntoTheList(Accounts, searcher.FindAll());
                }
            }

            return Accounts.OrderBy(x => x.UserName.ToLower()).ToList();
        }

        private static void putTheResultIntoTheList(List<ActiveDirectoryUser> Accounts, PrincipalSearchResult<Principal> users)
        {
            foreach (var result in users)
                Accounts.Add(parseUser(result.GetUnderlyingObject() as DirectoryEntry));
        }

        private static ActiveDirectoryUser parseUser(DirectoryEntry de)
        {
            return new ActiveDirectoryUser()
            {
                UserName = de.Properties["samAccountName"].Value + "",
                FirstName = de.Properties["givenName"].Value + "",
                LastName = de.Properties["sn"].Value + "",
            };
        }
    }
}