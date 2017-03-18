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
    [Table("skus")]
    class SKU : InvoiceRow, DBTableCreator, DBWritable, Copyable<SKU>
    {
        [Key]
        public long? Id { get; set; }
        public decimal BuyPrice { get; set; }
        /// <summary>
        /// When selling parts, selling price = BuyPrice * SellPriceFactor. This factor depends on e.g. the current market situation.
        /// </summary>
        public decimal SellPriceFactor { get; set; }
        /// <summary>
        /// Set and got via Timestamp.
        /// </summary>
        public long? UnixTimeModified { get; set; }
        [Editable(false)]
        public DateTime? Timestamp {
            get { return UnixTime.ToDateTime(UnixTimeModified); }
            set { UnixTimeModified = UnixTime.FromDateTime((DateTime?)value); }
        }

        public override string ToString()
        {
            return Code + ": " + Description;
        }

        public override List<ViewModels.SerializableRow> PrintSerializable()
        {
            var list = new List<ViewModels.SerializableRow>();
            list.Add(new ViewModels.SerializableRow(
                this.Code,
                this.Description,
                Decimal.Round(this.BuyPrice * this.SellPriceFactor, 0, MidpointRounding.AwayFromZero).ToString("N2"),
                (new Decimal(1.0)).ToString()
            ));
            return list;
        }

        public SKU()
        {
            Id = null;
            Code = "";
            Description = "";
            BuyPrice = 0;
            SellPriceFactor = 1;
            Timestamp = DateTime.Now;
        }

        public SKU Copy()
        {
            var copy = new SKU();
            copy.Id = this.Id;
            copy.Code = this.Code;
            copy.Description = this.Description;
            copy.BuyPrice = this.BuyPrice;
            copy.SellPriceFactor = this.SellPriceFactor;
            copy.Timestamp = this.Timestamp;
            return copy;
        }

        public void Save()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                this.Timestamp = DateTime.Now;
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
                using (var t = conn.BeginTransaction())
                {
                    conn.Delete(this, t);
                    t.Commit();
                }
            }
        }

        public static IEnumerable<SKU> LoadAll()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                return conn.GetList<SKU>(new { });
            }
        }

        public static IEnumerable<SKU> LoadAllFromFile(string fileName)
        {
            var file = new System.IO.StreamReader(fileName);
            string line = null;
            var skus = new List<SKU>();
            while ((line = file.ReadLine()) != null)
            {
                var sku = new SKU();
                var lineParts = line.Split('|');
                sku.Code = lineParts[0];
                sku.Description = lineParts[1];
                sku.BuyPrice = Convert.ToDecimal(lineParts[2]);
                sku.SellPriceFactor = Convert.ToDecimal(lineParts[3]);
                skus.Add(sku);
            }
            file.Close();
            return skus;
        }

        public static void SaveAllToFile(string fileName, IEnumerable<SKU> skus)
        {
            var file = new System.IO.StreamWriter(fileName);
            string line = null;
            string delim = "|";
            foreach (var sku in skus)
            {
                line = sku.Code + delim + sku.Description + delim + sku.BuyPrice + delim + sku.SellPriceFactor;
                file.WriteLine(line);
            }
            file.Close();
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE skus (
                Id INTEGER PRIMARY KEY,
                Code TEXT UNIQUE NOT NULL,
                Description TEXT,
                BuyPrice REAL,
                SellPriceFactor REAL,
                UnixTimeModified INTEGER
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
