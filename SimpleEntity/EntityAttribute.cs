using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {

        private string tableName;

        public EntityAttribute(string tableName)
        {
            this.tableName = tableName;
        }

        public string TableName
        {
            get { return this.tableName; }
        }
    }
}
