/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Dapper;
using System.Collections.Generic;
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

        public static IEnumerable<Task> LoadAllFor(Client client)
        {
            return new List<Task>();
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
        }
    }
}
