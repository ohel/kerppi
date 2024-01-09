/*
    Copyright 2015, 2017, 2018, 2024 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kerppi.DataModel
{
    [Table("tasks")]
    class Task : IKerppiDBTableCreator, IKerppiDBWritable, IKerppiCopyable<Task>
    {
        [Key]
        public long? Id { get; set; }
        public long? ClientId { get; set; }
        [Editable(false)]
        public Client Client { get; set; }
        public long? UnixTimeCreated { get; set; }
        [Editable(false)]
        public DateTime? TimestampCreated { get { return UnixTime.ToDateTime(UnixTimeCreated); } set { UnixTimeCreated = UnixTime.FromDateTime(value); } }
        /// <summary>
        /// Serialized invoice data.
        /// </summary>
        public string Invoice { get; set; }

        #region Task and invoice specific properties
        public long? UnixTimeInvoiced { get; set; }
        public long? UnixTimeDelivered { get; set; }
        public long? UnixTimeFinished { get; set; }
        [Editable(false)]
        public DateTime? TimestampInvoiced { get { return UnixTime.ToDateTime(UnixTimeInvoiced); } set { UnixTimeInvoiced = UnixTime.FromDateTime(value); } }
        [Editable(false)]
        public DateTime? TimestampDelivered { get { return UnixTime.ToDateTime(UnixTimeDelivered); } set { UnixTimeDelivered = UnixTime.FromDateTime(value); } }
        [Editable(false)]
        public DateTime? TimestampFinished { get { return UnixTime.ToDateTime(UnixTimeFinished); } set { UnixTimeFinished = UnixTime.FromDateTime(value); } }
        public string InvoiceNumber { get; set; }
        #endregion

        public override string ToString()
        {
            return Id.ToString();
        }

        public Task()
        {
            Id = null;
            ClientId = null;
            Invoice = null;
            InvoiceNumber = null;
            TimestampCreated = null;
            TimestampInvoiced = null;
            TimestampDelivered = null;
            TimestampFinished = null;
            Client = new Client();
        }

        public Task Copy()
        {
            var copy = new Task
            {
                Id = Id,
                ClientId = ClientId,
                Invoice = Invoice,
                InvoiceNumber = InvoiceNumber,
                TimestampCreated = TimestampCreated,
                TimestampInvoiced = TimestampInvoiced,
                TimestampDelivered = TimestampDelivered,
                TimestampFinished = TimestampFinished,
                Client = Client.Copy()
            };
            return copy;
        }

        public void Save(bool alwaysAsNew, IDbConnection conn = null, IDbTransaction t = null)
        {
            if (alwaysAsNew) Id = null;
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

        public void Save(IDbConnection conn = null, IDbTransaction t = null)
        {
            Save(false, conn, t);
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

        public static IEnumerable<Task> LoadAll()
        {
            return LoadAllWhereClientId();
        }

        public static IEnumerable<Task> LoadAllFor(Client client)
        {
            return LoadAllWhereClientId(client.Id ?? 0);
        }

        /// <summary>
        /// Prints all relevant information the client has provided about themselves.
        /// This is to comply with General Data Protection Regulation.
        /// </summary>
        public static string[] GetPrintableTaskDataFor(Client client)
        {
            var tasks = LoadAllFor(client);

            return tasks.Select(t => t.TimestampCreated.ToString()).ToArray();
        }

        private static IEnumerable<Task> LoadAllWhereClientId(long id = 0)
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                var results = conn.Query<Task, Client, Contact, Task>(@"
                    SELECT t.*, c.*, con.* FROM tasks t
                    LEFT JOIN clients c ON t.ClientId = c.Id
                    LEFT JOIN contacts con ON c.DefaultPayerContactId = con.Id" +
                    (id == 0 ? "" : " WHERE t.ClientId = " + id) + ";",
                    (t, c, con) => {
                        t.Client = c;
                        c.DefaultPayer = con;
                        return t;
                    });

                return results;
            }
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE tasks (
                Id INTEGER PRIMARY KEY,
                ClientId INTEGER,
                Invoice TEXT,
                InvoiceNumber TEXT,
                UnixTimeCreated TEXT,
                UnixTimeInvoiced TEXT,
                UnixTimeDelivered TEXT,
                UnixTimeFinished TEXT,
                FOREIGN KEY (ClientId) REFERENCES clients
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
