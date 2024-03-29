﻿/*
    Copyright 2015, 2017, 2024 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace Kerppi
{
    public static class DBHandler
    {
        private const string DBFILENAME = "Kerppi.sqlite";
        public static string Password { private get; set; }

        private static string GetConnectionString()
        {
            return string.Format("Data Source={0};Version=3;Password={1}", DBFILENAME, Password);
        }

        public static IDbConnection Connection()
        {
            Console.WriteLine("Creating new database connection.");
            return new SQLiteConnection(GetConnectionString());
        }

        /// <summary>
        /// Calls all classes implementing DBTableCreator interface and invokes their CreateDBTables method
        /// </summary>
        public static void CreateDBTables()
        {
            Console.WriteLine("Method: CreateDBTables begins.");
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            var interfaceType = typeof(DataModel.IKerppiDBTableCreator);
            var dbCreators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(dbcCandidate => interfaceType.IsAssignableFrom(dbcCandidate) && !dbcCandidate.IsInterface);
            using (var conn = Connection())
            {
                conn.Open();
                Console.WriteLine("DB connection opened.");
                using (var t = conn.BeginTransaction())
                {
                    Execute("CREATE TABLE kerppi_misc (Key TEXT PRIMARY KEY, Value TEXT);", conn, t);
                    Execute("INSERT INTO kerppi_misc (Key, Value) VALUES ('Version', '" + version.ToString() + "');", conn, t);
                    Execute("INSERT INTO kerppi_misc (Key, Value) VALUES ('PrintMargin', '75');", conn, t); // Margin used in prints.
                    Execute("INSERT INTO kerppi_misc (Key, Value) VALUES ('PrintLogoOpacity', '0,1');", conn, t); // Logo opacity used in prints.
                    Execute("INSERT INTO kerppi_misc (Key, Value) VALUES ('VAT', '24');", conn, t); // VAT percent.
                    dbCreators.ToList().ForEach(creator => creator.GetMethod("CreateDBTables").Invoke(null, new object[] { conn, t }));
                    t.Commit();
                }
            }
        }

        /// <summary>
        /// Throws exception if opening database fails, e.g. due to wrong password.
        /// </summary>
        /// <returns>Version string as found in database (after it has been created if that is the case).</returns>
        public static string InitDB()
        {
            Console.WriteLine("Initializing database.");
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);
            string version = null;

            if (System.IO.File.Exists(DBFILENAME))
            {
                Console.WriteLine("Found database file.");
                using (var conn = Connection())
                {
                    conn.Open();
                    // If wrong password, exception will be thrown.
                    version = QueryMisc("Version", conn);
                }
            }
            else
            {
                Console.WriteLine("Database file not found. Creating new one.");
                SQLiteConnection.CreateFile(DBFILENAME);
                CreateDBTables();
                version = QueryMisc("Version");
            }

            return version;
        }

        public static void ChangePassword(string newPassword)
        {
            using (var conn = Connection() as SQLiteConnection)
            {
                conn.Open();
                conn.ChangePassword(newPassword);
                Password = newPassword;
            }
        }

        public static string ExportUnencrypted()
        {
            var now = DateTime.Now;
            string filename = now.Year.ToString() + "_" + now.Month.ToString().PadLeft(2, '0') + "_" + now.Day.ToString().PadLeft(2, '0') + "_Unencrypted_" + DBFILENAME;
            var oldPassword = Password;
            ChangePassword("");
            try
            {
                System.IO.File.Copy(DBFILENAME, filename, true);
                return filename;
            }
            catch
            {
                throw new KerppiException("Tallennus epäonnistui.");
            }
            finally
            {
                ChangePassword(oldPassword);
            }
        }

        /* Note: for some reason, this did not work. Had to resort to using manual delete.
        /// <summary>
        /// Foreign key support is not enabled in SQLite by default. Therefore this should be called before all delete commands so that cascading may happen.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="t"></param>
        public static void PrepareDelete(IDbConnection conn, IDbTransaction t = null)
        {
            var cmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", conn as SQLiteConnection, t as SQLiteTransaction);
            cmd.ExecuteNonQuery();
        }
        */

        public static void Execute(string sql, IDbConnection conn, IDbTransaction t = null)
        {
            var cmd = new SQLiteCommand(sql, conn as SQLiteConnection, t as SQLiteTransaction);
            Console.WriteLine("Executing: " + sql);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Execution complete.");
        }

        public static long? Insert(string sql, List<Tuple<string, DbType, object>> parameters, IDbConnection conn, IDbTransaction t = null)
        {
            var cmd = new SQLiteCommand(sql, conn as SQLiteConnection, t as SQLiteTransaction);
            foreach (var param in parameters)
            {
                cmd.Parameters.Add(param.Item1, param.Item2);
                cmd.Parameters[param.Item1].Value = param.Item3;
            }
            cmd.ExecuteNonQuery();
            cmd = new SQLiteCommand("SELECT last_insert_rowid();", conn as SQLiteConnection, t as SQLiteTransaction);
            var id = cmd.ExecuteScalar();
            return Convert.IsDBNull(id) ? null : (long?)Convert.ToInt64(id);
        }

        public static string QueryMisc (string key, IDbConnection conn = null, IDbTransaction t = null)
        {
            bool createConnection = conn == null;
            string result = null;
            if (createConnection)
            {
                conn = Connection();
                conn.Open();
            }
            try
            {
                var cmd = new SQLiteCommand(
                    "SELECT Value FROM kerppi_misc WHERE (Key = @key);",
                    conn as SQLiteConnection, t as SQLiteTransaction);
                cmd.Parameters.Add("@key", DbType.String);
                cmd.Parameters["@key"].Value = key;
                result = (string)cmd.ExecuteScalar();
            }
            catch (Exception x)
            {
                throw new KerppiException(x.Message, x.InnerException);
            }
            finally
            {
                if (createConnection) conn.Close();
            }
            return result;
        }

        public static void SaveMisc (string key, string value, IDbConnection conn = null, IDbTransaction t = null)
        {
            bool createConnection = conn == null;
            string result = null;
            if (createConnection)
            {
                conn = Connection();
                conn.Open();
            }
            try
            {
                var cmd = new SQLiteCommand(
                    "UPDATE kerppi_misc SET Value = @value WHERE Key = @key;",
                    conn as SQLiteConnection, t as SQLiteTransaction);
                cmd.Parameters.Add("@key", DbType.String);
                cmd.Parameters["@key"].Value = key;
                cmd.Parameters.Add("@value", DbType.String);
                cmd.Parameters["@value"].Value = value;
                result = (string)cmd.ExecuteScalar();
            }
            catch (Exception x)
            {
                throw new KerppiException(x.Message, x.InnerException);
            }
            finally
            {
                if (createConnection) conn.Close();
            }
        }
    }
}
