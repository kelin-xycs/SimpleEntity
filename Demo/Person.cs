using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SimpleEntity;

namespace Demo
{
    [Entity("Person")]
    public class Person
    {
        [Column("no", IsPrimaryKey=true)]
        public string No
        {
            get;
            set;
        }

        [Column("name")]
        public string Name
        {
            get;
            set;
        }

        [Column("salary")]
        public int? Salary
        {
            get;
            set;
        }

        [Column("create_date")]
        public DateTime? CreateDate
        {
            get;
            set;
        }
    }
}