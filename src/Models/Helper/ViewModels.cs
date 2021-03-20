using System.Collections.Generic;
using System.Net.NetworkInformation;
using ITech.Security.Business;

namespace ITech.Security.Models
{
    public class SectionViewModel
    {
        public IEnumerable<View> Views { get; set; }
        public Section Section { get; set; }
    }

    public class ScopedBoolean
    {
        public bool IsAllowed { get; set; }
        public IEnumerable<string> Scopes { get; set; }

        public static ScopedBoolean Denied
        {
            get
            {
                return new ScopedBoolean()
                {
                    IsAllowed = false,
                    Scopes = null
                };
            }
        }
        public static ScopedBoolean Allowed
        {
            get
            {
                return new ScopedBoolean()
                {
                    IsAllowed = true,
                    Scopes = null
                };
            }
        }
    }
    public class RoleDetailsViewModel
    {
        public Application Application { get; set; }
        public Role Role { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<ScopedOperation> RoleOperations { get; set; }


        public IEnumerable<UserInfoViewModel> OtherUsers { get; set; }
        public IEnumerable<Operation> OtherOperations { get; set; }
    }

    public class UsersViewModel
    {
        public IEnumerable<UserInfoViewModel> UsersInfo { get; set; }

        public IEnumerable<UserInfoViewModel> DbUsersInfo { get; set; }
    }

    public class Pair
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserInfoViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Domain { get; set; }
    }

    public class UserViewModel : UserInfoViewModel
    {
        public List<Pair> Roles { get; set; }

        public Pair Applicaiton { get; set; }
    }
}