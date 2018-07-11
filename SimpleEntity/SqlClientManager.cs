using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using System.Data;
using System.Data.SqlClient;

namespace SimpleEntity
{
    class SqlPara
    {
        public string name;
        public object value;

        public SqlPara(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }

    class SqlClientManager : IActiveRecordManager
    {

        private string connectionString;

        private string providerName;


        [ThreadStatic]
        private static List<string> _sqlList;

        private static List<string> _SqlList
        {
            get 
            {
                if (_sqlList == null)
                    _sqlList = new List<string>();

                return _sqlList; 
            }
        }

        [ThreadStatic]
        private static List<SqlPara> _paraList;

        private static List<SqlPara> _ParaList
        {
            get
            {
                if (_paraList == null)
                    _paraList = new List<SqlPara>();

                return _paraList;
            }
        }

        public SqlClientManager(string connectionString, string providerName)
        {
            this.connectionString = connectionString;
            this.providerName = providerName;
        }

        public void Save(object entity)
        {
            Type t = entity.GetType();

            int i = _SqlList.Count;

            string sql = GetInsertSql(t, i);
            
            EntityInfo entityInfo = Util.GetEntityInfo(t);

            foreach(Property p in entityInfo.propertyList)
            {
                _ParaList.Add(new SqlPara("@" + p.columnName + i, p.getMethod.Invoke(entity, null)));
            }

            _SqlList.Add(sql);
        }

        private string GetInsertSql(Type t, int i)
        {
            EntityInfo entityInfo = Util.GetEntityInfo(t);

            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            sb.Append("insert into " + entityInfo.tableName + " ( ");

            foreach(Property p in entityInfo.propertyList)
            {
                sb.Append(p.columnName + ", ");

                sb2.Append("@" + p.columnName + i + ", ");
            }

            sb.Remove(sb.Length - 2, 2);

            sb.Append(" ) values ( ");

            sb.Append(sb2);

            sb.Remove(sb.Length - 2, 2);

            sb.Append(" )");

            return sb.ToString();
        }

        public void Update(object entity)
        {
            Type t = entity.GetType();

            EntityInfo entityInfo = Util.GetEntityInfo(t);

            StringBuilder sb = new StringBuilder();

            sb.Append("update " + entityInfo.tableName + " set ");


            int i = _SqlList.Count;

            object v;

            bool atLeastOneProperty = false;

            foreach (Property p in entityInfo.propertyList)
            {

                if ( p.isPrimaryKey )
                    continue;

                v = p.getMethod.Invoke(entity, null);

                if (v == null)
                    continue;

                sb.Append(p.columnName + " = @" + p.columnName + i + ", ");

                _ParaList.Add(new SqlPara("@" + p.columnName + i, v));

                atLeastOneProperty = true;
   
            }

            if (!atLeastOneProperty)
                throw new SimpleEntityException("至少要为一个 非主键 Property 赋值 才能 Update 。");

            sb.Remove(sb.Length - 2, 2);

            sb.Append(" where " + entityInfo.primaryKey + " = @" + entityInfo.primaryKey + i);

            v = entityInfo.pkGetMethod.Invoke(entity, null);

            if (v == null)
                throw new SimpleEntityException("未对 主键 赋值 。 需要为 主键 赋值 才能 Update 。");

            _SqlList.Add(sb.ToString());
            _ParaList.Add(new SqlPara("@" + entityInfo.primaryKey + i, v));
        }
        
        /// <summary>
        /// 用于将 需要 Update 为 null 的 栏位 Update 为 null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnNames"></param>
        /// <param name="id"></param>
        public void UpdateNull<T>(string[] columnNames, object id)
        {

            Type t = typeof(T);

            EntityInfo entityInfo = Util.GetEntityInfo(t);

            string sql = "update " + entityInfo.tableName + " set ";

            StringBuilder sb = new StringBuilder(sql);

            foreach(string columnName in columnNames)
            {
                sb.Append(columnName + " = null, ");
            }

            sb.Remove(sb.Length - 2, 2);

            sb.Append(" where " + entityInfo.primaryKey + " = @" + entityInfo.primaryKey);

            sql = sb.ToString();

            _SqlList.Add(sql);
            _ParaList.Add(new SqlPara("@" + entityInfo.primaryKey, id));

        }

        public void Delete<T>(object id)
        {

            Type t = typeof(T);

            int i = _SqlList.Count;

            string sql = GetDeleteSql(t, i);

            EntityInfo entityInfo = Util.GetEntityInfo(t);


            _SqlList.Add(sql);
            _ParaList.Add(new SqlPara("@" + entityInfo.primaryKey + i, id));

        }

        private string GetDeleteSql(Type t, int i)
        {
            EntityInfo entityInfo = Util.GetEntityInfo(t);

            return "delete " + entityInfo.tableName + " where " + entityInfo.primaryKey + " = @" + entityInfo.primaryKey + i;
        }

        public void Flush()
        {
            StringBuilder sb = new StringBuilder();

            foreach(string sql in _SqlList)
            {
                sb.Append(sql + "; ");
            }

            using(SqlConnection conn = new SqlConnection(this.connectionString))
            { 
                using(SqlCommand cmd = new SqlCommand(sb.ToString(), conn))
                {

                    foreach(SqlPara para in _ParaList)
                    {
                        cmd.Parameters.AddWithValue(para.name, para.value);
                    }

                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }

            _SqlList.Clear();
            _ParaList.Clear();
        }

        public T Get<T>(object id)
        {
            Type t = typeof(T);

            int i = _SqlList.Count;

            string sql = GetSelectSql(t, i);

            T entity = default(T);

            EntityInfo entityInfo = Util.GetEntityInfo(t);

            object v;

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {

                    cmd.Parameters.AddWithValue("@" + entityInfo.primaryKey + i, id);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = (T)entityInfo.constructorInfo.Invoke(null);

                            foreach (Property p in entityInfo.propertyList)
                            {
                                v = reader[p.columnName];

                                if (v is DBNull)
                                    v = null;

                                p.setMethod.Invoke(entity, new object[] { v });
                            }
                        }
                    }
                }
            }

            return entity;
        }

        private string GetSelectSql(Type t , int i)
        {
            EntityInfo entityInfo = Util.GetEntityInfo(t);

            return "select * from " + entityInfo.tableName + " where " + entityInfo.primaryKey + " = @" + entityInfo.primaryKey + i;
        }

        public List<T> Get<T>(object[] ids)
        {
            Type t = typeof(T);

            EntityInfo entityInfo = Util.GetEntityInfo(t);

            string sql = "select * from " + entityInfo.tableName + " where " + entityInfo.primaryKey + " in ( ";

            StringBuilder sb = new StringBuilder(sql);

            T entity;

            List<T> list = new List<T>();

            object v;

            using (SqlCommand cmd = new SqlCommand())
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    sb.Append("@" + entityInfo.primaryKey + i + ", ");

                    cmd.Parameters.AddWithValue("@" + entityInfo.primaryKey + i, ids[i]);
                }

                sb.Remove(sb.Length - 2, 2);

                sb.Append(" )");

                cmd.CommandText = sb.ToString();

                using (SqlConnection conn = new SqlConnection(this.connectionString))
                {

                    cmd.Connection = conn;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entity = (T)entityInfo.constructorInfo.Invoke(null);

                            foreach (Property p in entityInfo.propertyList)
                            {
                                v = reader[p.columnName];

                                if (v is DBNull)
                                    v = null;

                                p.setMethod.Invoke(entity, new object[] { v });
                            }

                            list.Add(entity);
                        }
                    }
                }
            }

            return list;
            
        }
    }
}
