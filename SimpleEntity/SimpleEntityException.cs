using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntity
{
    public class SimpleEntityException : Exception
    {
        internal SimpleEntityException(string message) : base(message)
        {

        }
    }
}
