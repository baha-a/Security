using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITech.Security.Models
{
    public class ScopedOperation
    {
        public string Scope { get; set; }
        public Role Role { get; set; }
        public Operation Operation { get; set; }
    }
}