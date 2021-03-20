//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ITech.Security.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.UserRoles = new HashSet<UserRole>();
        }
    
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Created { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime Modified { get; set; }
        public bool IsDeleted { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Domain { get; set; }
    
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}