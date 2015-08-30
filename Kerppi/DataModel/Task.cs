using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Kerppi.DataModel
{
    [Table("tasks")]
    class Task : DBTableCreator, DBWritable, Copyable<Task>
    {
        public Task()
        {
        }

        public Task Copy()
        {
            return new Task();
        }

        public void Save()
        {
        }

        public void Delete()
        {
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
        }

        public static IEnumerable<Task> LoadAllFor(Client client)
        {
            return new List<Task>();
        }
    }
}
