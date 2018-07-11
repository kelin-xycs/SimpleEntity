using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace SimpleEntity
{

    class EntityInfo
    {
        public string tableName;
        public string primaryKey;

        public ConstructorInfo constructorInfo;

        public MethodInfo pkGetMethod;
        public MethodInfo pkSetMethod;

        public List<Property> propertyList;

        //public EntityInfo(string tableName, List<Property> propertyList)
        public EntityInfo(string tableName)
        {
            this.tableName = tableName;
            //this.propertyList = propertyList;
        }
    }

    class Property
    {
        public string columnName;
        public bool isPrimaryKey;
        public MethodInfo getMethod;
        public MethodInfo setMethod;

        public Property(string name, bool isPrimaryKey, MethodInfo getMethod, MethodInfo setMethod)
        {
            this.columnName = name;
            this.isPrimaryKey = isPrimaryKey;
            this.getMethod = getMethod;
            this.setMethod = setMethod;
        }
    }

    class Util
    {

        private static Dictionary<Type, EntityInfo> _entityInfoDic = new Dictionary<Type, EntityInfo>();

        
        public static EntityInfo GetEntityInfo(Type t)
        {
            EntityInfo entityInfo;

            if ( ! _entityInfoDic.TryGetValue(t, out entityInfo) )
            {
                entityInfo = GenEntityInfo(t);

                lock( _entityInfoDic )
                {
                    if ( ! _entityInfoDic.ContainsKey(t) )
                    {
                        _entityInfoDic.Add(t, entityInfo);
                    }
                }
            }

            return entityInfo;
        }

        private static EntityInfo GenEntityInfo(Type t)
        {

            EntityAttribute entityAttribute = t.GetCustomAttribute<EntityAttribute>(true);

            if (entityAttribute == null)
                throw new SimpleEntityException("缺少 [ Entity ] 标记 。 Entity 对象 应有 [ Entity ] 标记 。");

            string tableName = entityAttribute.TableName; ;

            if (string.IsNullOrWhiteSpace(tableName))
                throw new SimpleEntityException("缺少 tableName 。 应在 [ Entity ] 标记 中 指明 tableName ，如： [ Entity( \"tableName\" ) ] 。");

            EntityInfo entityInfo = new EntityInfo(tableName);

            ColumnAttribute columnAttribute;

            //string primaryKey = null;

            List<Property> propertyList = new List<Property>();

            foreach (PropertyInfo p in t.GetProperties())
            {

                columnAttribute = p.GetCustomAttribute<ColumnAttribute>(true);

                if (columnAttribute == null)
                    continue;

                if (string.IsNullOrWhiteSpace(columnAttribute.ColumnName))
                    throw new SimpleEntityException("缺少 columnName 。 应在 [ Column ] 标记 中 指明 columnName ，如： [ Entity( \"columnName\" ) ] 。");


                MethodInfo getM = p.GetGetMethod();
                MethodInfo setM = p.GetSetMethod();

                if (getM == null)
                    throw new SimpleEntityException("Property 应包含 public Get 访问器 。");

                if (setM == null)
                    throw new SimpleEntityException("Property 应包含 public Set 访问器 。");

                if ( columnAttribute.IsPrimaryKey )
                {
                    if (entityInfo.primaryKey != null)
                        throw new SimpleEntityException("只支持 单一列 作为 主键 ， 不支持 多列 联合主键 。");

                    entityInfo.primaryKey = columnAttribute.ColumnName;

                    entityInfo.pkGetMethod = getM;
                    entityInfo.pkSetMethod = setM;
                }

                propertyList.Add(new Property(columnAttribute.ColumnName, columnAttribute.IsPrimaryKey, getM, setM));
            }

            if (propertyList.Count == 0)
                throw new SimpleEntityException("应至少在一个 public Property 上 标注 [ Column ] 标记 。");

            if (entityInfo.primaryKey == null)
                throw new SimpleEntityException("缺少主键 。 应将 一个列 设置为 主键 。");

            ConstructorInfo constructorInfo = t.GetConstructor(Type.EmptyTypes);

            if (constructorInfo == null)
                throw new SimpleEntityException("Entity 对象 需要 有一个 无参数 的 构造函数 。");

            entityInfo.constructorInfo = constructorInfo;
            entityInfo.propertyList = propertyList;

            return entityInfo;
            //return new EntityInfo(tableName, propertyList);
        }

    }
}
