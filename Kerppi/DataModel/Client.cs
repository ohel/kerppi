/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Kerppi.DataModel
{
    [Table("clients")]
    class Client : DBTableCreator, DBWritable, Copyable<Client>
    {
        [Key]
        public long? Id { get; set; }
        /// <summary>
        /// Social security number, unique birthday date or something.
        /// </summary>
        public string IdCode { get; set; }
        /// <summary>
        /// For example, deceased clients are not active anymore.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// For example, some clients need a certificate of their visit.
        /// </summary>
        public bool Certificate { get; set; }
        public string Name { get; set; }
        public string PostalAddress { get; set; }
        /// <summary>
        /// Holds both postal code and city.
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Generic contact information, for example a telephone number.
        /// </summary>
        public string ContactInfo { get; set; }
        /// <summary>
        /// Holds some client specific information that may be used for various purposes.
        /// </summary>
        public string Information { get; set; }

        public override string ToString()
        {
            return Name + " (" + IdCode + ")";
        }

        public Client()
        {
            Id = null;
            IdCode = "";
            Active = true;
            Certificate = false;
            Name = "";
            PostalAddress = "";
            PostalCode = "";
            ContactInfo = "";
            Information = "";
        }

        public Client Copy()
        {
            var copy = new Client();
            copy.Id = Id;
            copy.IdCode = IdCode;
            copy.Active = Active;
            copy.Certificate = Certificate;
            copy.Name = Name;
            copy.PostalAddress = PostalAddress;
            copy.PostalCode = PostalCode;
            copy.ContactInfo = ContactInfo;
            copy.Information = Information;
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

        public static IEnumerable<Client> LoadAll()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<Client>(new { });
            }
        }

        public static IEnumerable<Client> LoadAllActive()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<Client>(new { Active = 1 });
            }
        }

        public static bool Exists(long? id)
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<Client>(new { Id = id }).Count() > 0;
            }
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE Clients (
                Id INTEGER PRIMARY KEY,
                IdCode TEXT UNIQUE NOT NULL,
                Active INTEGER NOT NULL DEFAULT 1,
                Certificate INTEGER NOT NULL DEFAULT 0,
                Name TEXT NOT NULL,
                PostalAddress TEXT,
                PostalCode TEXT,
                ContactInfo TEXT,
                Information TEXT
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
