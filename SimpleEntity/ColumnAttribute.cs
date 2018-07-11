using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        private string columnName;

        public ColumnAttribute(string columnName)
        {
            this.columnName = columnName;
        }

        public string ColumnName
        {
            get { return this.columnName; }
        }

        private bool isPrimaryKey = false;
        public bool IsPrimaryKey
        {
            get { return this.isPrimaryKey; }
            set { this.isPrimaryKey = value; }
        }
    }
}
