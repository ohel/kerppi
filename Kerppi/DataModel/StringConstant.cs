/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Kerppi.DataModel
{
    [Table("string_constants")]
    class StringConstant : DBTableCreator, DBWritable, Copyable<StringConstant>
    {
        [Key]
        public long? Id { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return Text.Replace("[DATE]", DateTime.Now.ToShortDateString());
        }

        public StringConstant()
        {
            Id = null;
            Text = "";
        }

        public StringConstant Copy()
        {
            var copy = new StringConstant();
            copy.Id = this.Id;
            copy.Text = this.Text;
            return copy;
        }

        public void Save()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                if (Id == null)
                    conn.Insert(this);
                else
                    conn.Update(this);
            }
        }

        public void Delete()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                conn.Delete(this);
            }
        }

        public void Save(IDbConnection conn, IDbTransaction t)
        {
            if (Id == null)
                conn.Insert(this, t);
            else
                conn.Update(this, t);
        }

        public void Delete(IDbConnection conn, IDbTransaction t)
        {
            conn.Delete(this, t);
        }

        public static IEnumerable<StringConstant> LoadAll()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<StringConstant>();
            }
        }

        public static IEnumerable<string> LoadAllAsStrings()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<StringConstant>().Select(sc => sc.ToString());
            }
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE string_constants (
                Id INTEGER PRIMARY KEY,
                Text TEXT NOT NULL
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
