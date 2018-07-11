using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace SimpleEntity
{
    public class DbContext
    {
        //private string connectionString;
        //private string providerName;

        private IActiveRecordManager activeRecordManager;


        public DbContext(string connectionString, string providerName)
        {

            if (providerName != "System.Data.SqlClient")
                throw new SimpleEntityException("不支持 的 Provider ： " + providerName + " 。");


            //this.connectionString = connectionString;
            //this.providerName = providerName;


            this.activeRecordManager = new SqlClientManager(connectionString, providerName);

        }


        //private IDbConnection GetConnection()
        //{
        //    switch( this.providerName )
        //    {
        //        case "System.Data.SqlClient" :
        //            return new SqlConnection(this.connectionString);
        //        default :
        //            throw new SimpleEntityException("不支持 的 Provider ： " + this.providerName + " 。");
        //    }
        //}


        public void Save(object entity)
        {
            activeRecordManager.Save(entity);
        }

        public void Update(object entity)
        {
            activeRecordManager.Update(entity);
        }

        public void UpdateNull<T>(string[] columnNames, object id)
        {
            activeRecordManager.UpdateNull<T>(columnNames, id);
        }

        public void Delete<T>(object id)
        {
            activeRecordManager.Delete<T>(id);
        }

        public void Flush()
        { 
            activeRecordManager.Flush();
        }

        public T Get<T>(object id)
        {
            return activeRecordManager.Get<T>(id);
        }

        public List<T> Get<T>(object[] ids)
        {
            return activeRecordManager.Get<T>(ids);
        }


    }
}
