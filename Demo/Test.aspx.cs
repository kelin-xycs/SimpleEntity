using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Text;

using SimpleEntity;

namespace Demo
{

    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            DbContext dbContext = new DbContext(s.ConnectionString, s.ProviderName);

            Person person = new Person();

            person.No = "001";

            person.Salary = 2500;
            person.CreateDate = DateTime.Now;

            dbContext.Update(person);

            dbContext.Flush();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            DbContext dbContext = new DbContext(s.ConnectionString, s.ProviderName);

            Person person = new Person();

            person.No = "001";
            person.Name = "小明";
            person.Salary = 2000;
            person.CreateDate = DateTime.Now;

            dbContext.Save(person);

            person = new Person();

            person.No = "002";
            person.Name = "小刚";
            person.Salary = 3000;
            person.CreateDate = DateTime.Now;

            dbContext.Save(person);

            person = new Person();

            person.No = "003";
            person.Name = "小红";
            person.Salary = 4000;
            person.CreateDate = DateTime.Now;

            dbContext.Save(person);

            dbContext.Flush();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            DbContext dbContext = new DbContext(s.ConnectionString, s.ProviderName);

            dbContext.Delete<Person>("001");

            dbContext.Flush();
        }

        protected void btnGet_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            DbContext dbContext = new DbContext(s.ConnectionString, s.ProviderName);

            Person person = dbContext.Get<Person>("001");

            if (person == null)
            {
                litPerson.Text = "No : 001 的 Person 不存在 。";
            }
            else
            {
                ShowPerson(person);
            }
        }

        private void ShowPerson(Person person)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Person<br />");
            sb.Append("{<br />");
            sb.Append("No : " + person.No + "<br />");
            sb.Append("Name : " + person.Name + "<br />");
            sb.Append("Salary : " + person.Salary + "<br />");
            sb.Append("CreateDate : " + person.CreateDate + "<br />");
            sb.Append("}<br />");

            litPerson.Text = sb.ToString();
        }

        protected void btnGetList_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            DbContext dbContext = new DbContext(s.ConnectionString, s.ProviderName);

            List<Person> personList = dbContext.Get<Person>(new object[] {"001", "002", "003"});

            ShowPersonList(personList);
        }

        private void ShowPersonList(List<Person> personList)
        {
            StringBuilder sb = new StringBuilder();

            foreach(Person person in personList)
            {
                sb.Append("Person<br />");
                sb.Append("{<br />");
                sb.Append("No : " + person.No + "<br />");
                sb.Append("Name : " + person.Name + "<br />");
                sb.Append("Salary : " + person.Salary + "<br />");
                sb.Append("CreateDate : " + person.CreateDate + "<br />");
                sb.Append("}<br />");
            }
            
            litPerson.Text = sb.ToString();
        }

        protected void btnUpdateNull_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            DbContext dbContext = new DbContext(s.ConnectionString, s.ProviderName);

            dbContext.UpdateNull<Person>(new string[] { "create_date" }, "001");

            dbContext.Flush();
        }
    }
}