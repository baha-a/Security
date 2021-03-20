using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITech.Security.Models
{
    public partial class SecurityDB
    {
        
    }

    public partial class Application
    {
        public static Application All
        {
            get
            {
                return new Application()
                {
                    Id = -1, Name = "All Applications"
                };
            }
        }
    }

    public partial class User
    {
        public IEnumerable<Role> Roles { get; set; }

        public IEnumerable<View> Views { get; set; }

        public IEnumerable<Role> OtherRoles { get; set; }

    }
    public partial class Role
    {
        public IEnumerable<Operation> Operations { get; set; }
        public IEnumerable<ScopedOperation> ScopedOperations { get; set; }
    }
    public partial class UserRole
    {

    }
    public partial class RoleOperation
    {

    }
    public partial class Operation
    {

    }  
    public partial class Model
    {
        
    }
    public partial class View
    {

    }
    public partial class Section
    {

    }
}