using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ITech.Security.Business
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException() : base() { }

        public ResourceNotFoundException(string msg) : base(msg) { }
    }
    
    public class DeleteNestedResourceException : Exception
    {
        public DeleteNestedResourceException() : base() { }

        public DeleteNestedResourceException(string msg) : base(msg) { }
    }
    
    public class AreadyExistedException : Exception
    {
        public AreadyExistedException() : base() { }

        public AreadyExistedException(string msg) : base(msg) { }
    }
}
