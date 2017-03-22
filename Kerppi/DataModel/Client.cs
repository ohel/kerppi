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
        public long? DefaultPayerContactId { get; set; }
        private Contact _defaultPayer = null;
        [Editable(false)]
        public Contact DefaultPayer { get { return _defaultPayer; } set { _defaultPayer = value; DefaultPayerContactId = ((Contact)value)?.Id ?? null; } }
        public string ContactPersonName { get; set; }
        public string ContactPersonPostalAddress { get; set; }
        public string ContactPersonPostalCode { get; set; }
        public string ContactPersonContactInfo { get; set; }

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
            PostalAddress = null;
            PostalCode = null;
            ContactInfo = null;
            Information = null;
            DefaultPayerContactId = null;
            ContactPersonName = null;
            ContactPersonPostalAddress = null;
            ContactPersonPostalCode = null;
            ContactPersonContactInfo = null;
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
            copy.DefaultPayerContactId = DefaultPayerContactId;
            copy.DefaultPayer = DefaultPayer?.Copy();
            copy.ContactPersonName = ContactPersonName;
            copy.ContactPersonPostalAddress = ContactPersonPostalAddress;
            copy.ContactPersonPostalCode = ContactPersonPostalCode;
            copy.ContactPersonContactInfo = ContactPersonContactInfo;
            return copy;
        }

        public void Save(IDbConnection conn = null, IDbTransaction t = null)
        {
            if (conn == null)
            {
                using (var c = DBHandler.Connection())
                {
                    c.Open();
                    if (Id == null)
                        c.Insert(this);
                    else
                        c.Update(this);
                }
            }
            else
            {
                if (Id == null)
                    conn.Insert(this, t);
                else
                    conn.Update(this, t);
            }
        }

        public void Delete(IDbConnection conn = null, IDbTransaction t = null)
        {
            if (conn == null)
            {
                using (var c = DBHandler.Connection())
                {
                    c.Open();
                    c.Delete(this);
                }
            }
            else
            {
                conn.Delete(this, t);
            }
        }

        public static IEnumerable<Client> LoadAll()
        {
            return LoadAllWhereOnlyActive();
        }

        public static IEnumerable<Client> LoadAllActive()
        {
            return LoadAllWhereOnlyActive(true);
        }

        private static IEnumerable<Client> LoadAllWhereOnlyActive(bool onlyActive = false)
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                var results = conn.Query<Client, Contact, Client>(@"
                    SELECT c.*, con.* FROM clients c
                    LEFT JOIN contacts con ON c.DefaultPayerContactId = con.Id" +
                    (onlyActive ? " WHERE c.Active = 1;" : ";"),
                    (c, con) => {
                        c.DefaultPayer = con;
                        return c;
                    });

                return results;
            }
        }

        public static void RemoveDefaultPayer(long? contactId, IDbConnection conn, IDbTransaction t)
        {
            if (contactId != null)
            {
                DBHandler.Execute("UPDATE clients SET DefaultPayerContactId = NULL WHERE DefaultPayerContactId = " + contactId, conn, t);
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
                CREATE TABLE clients (
                Id INTEGER PRIMARY KEY,
                IdCode TEXT UNIQUE NOT NULL,
                Active INTEGER NOT NULL DEFAULT 1,
                Certificate INTEGER NOT NULL DEFAULT 0,
                Name TEXT NOT NULL,
                PostalAddress TEXT,
                PostalCode TEXT,
                ContactInfo TEXT,
                Information TEXT,
                DefaultPayerContactId INTEGER,
                ContactPersonName TEXT,
                ContactPersonPostalAddress TEXT,
                ContactPersonPostalCode TEXT,
                ContactPersonContactInfo TEXT,
                FOREIGN KEY (DefaultPayerContactId) REFERENCES contacts
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
