/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace Kerppi.DataModel
{
    [Table("contacts")]
    class Contact : DBTableCreator, DBWritable, Copyable<Contact>
    {
        [Key]
        public long? Id { get; set; }
        public string Name { get; set; }
        public string PostalAddress { get; set; }
        /// <summary>
        /// Holds both postal code and city.
        /// </summary>
        public string PostalCode { get; set; }
        public string DefaultInfo { get; set; }
        public string AdditionalInfo { get; set; }
        public bool Payer { get; set; }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(Name) ? "" : (Name + " | ")) +
                (string.IsNullOrEmpty(DefaultInfo) ? "" : (DefaultInfo.Substring(0, Math.Min(DefaultInfo.Length, 30)) +
                (DefaultInfo.Length < 31 ? "" : "…")));
        }

        public Contact()
        {
            Id = null;
            Name = null;
            PostalAddress = "";
            PostalCode = "";
            DefaultInfo = "";
            AdditionalInfo = "";
        }

        public Contact Copy()
        {
            var copy = new Contact();
            copy.Id = Id;
            copy.Name = Name;
            copy.PostalAddress = PostalAddress;
            copy.PostalCode = PostalCode;
            copy.DefaultInfo = DefaultInfo;
            copy.AdditionalInfo = AdditionalInfo;
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

        public static IEnumerable<Contact> LoadAllPayers()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<Contact>(new { Payer = 1 });
            }
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE contacts (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                PostalAddress TEXT,
                PostalCode TEXT,
                DefaultInfo TEXT,
                AdditionalInfo TEXT,
                Payer INTEGER NOT NULL DEFAULT 0
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
