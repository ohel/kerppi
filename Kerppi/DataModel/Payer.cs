using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Kerppi.DataModel
{
    [Table("payers")]
    class Payer : DBTableCreator, DBWritable, Copyable<Payer>
    {
        [Key]
        public long? Id { get; set; }
        public string Name { get; set; }
        public string PostalAddress { get; set; }
        /// <summary>
        /// Holds both postal code and city.
        /// </summary>
        public string PostalCode { get; set; }
        public string DefaultContact { get; set; }

        public override string ToString()
        {
            return (String.IsNullOrEmpty(Name) ? "" : (Name + " | ")) +
                (String.IsNullOrEmpty(DefaultContact) ? "" : (DefaultContact.Substring(0, Math.Min(DefaultContact.Length, 20)) + "…"));
        }

        public Payer()
        {
            Id = null;
            Name = null;
            PostalAddress = "";
            PostalCode = "";
            DefaultContact = "";
        }

        public Payer Copy()
        {
            var copy = new Payer();
            copy.Id = this.Id;
            copy.Name = this.Name;
            copy.PostalAddress = this.PostalAddress;
            copy.PostalCode = this.PostalCode;
            copy.DefaultContact = this.DefaultContact;
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

        public static IEnumerable<Payer> LoadAll()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<Payer>();
            }
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE payers (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                PostalAddress TEXT,
                PostalCode TEXT,
                DefaultContact TEXT
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
