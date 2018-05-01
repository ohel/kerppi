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
        public bool Footer { get; set; }

        public override string ToString()
        {
            return Text.Replace("[DATE]", DateTime.Now.ToShortDateString())
                .Replace("[NL]", Environment.NewLine);
        }

        public StringConstant()
        {
            Id = null;
            Text = "";
            Footer = false;
        }

        public StringConstant Copy()
        {
            var copy = new StringConstant
            {
                Id = Id,
                Text = Text,
                Footer = Footer
            };
            return copy;
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

        public static IEnumerable<string> LoadNonFooterStrings()
        {
            return LoadAllAsStrings(false);
        }

        public static IEnumerable<string> LoadFooterStrings()
        {
            return LoadAllAsStrings(true);
        }

        private static IEnumerable<string> LoadAllAsStrings(bool onlyFooters)
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<StringConstant>().Where(sc => sc.Footer == onlyFooters).Select(sc => sc.ToString());
            }
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE string_constants (
                Id INTEGER PRIMARY KEY,
                Text TEXT NOT NULL,
                Footer INTEGER NOT NULL DEFAULT 0
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
