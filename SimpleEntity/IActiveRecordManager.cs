using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntity
{
    interface IActiveRecordManager
    {
        void Save(object entity);
        void Update(object entity);
        void UpdateNull<T>(string[] columnNames, object id);
        void Delete<T>(object id);
        void Flush();

        T Get<T>(object id);

        List<T> Get<T>(object[] ids);
    }
}
