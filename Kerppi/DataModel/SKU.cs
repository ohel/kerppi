/*
    Copyright 2015, 2017, 2018 Olli Helin / GainIT
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

        public override List<ViewModels.SerializableRow> PrintSerializable(bool onlyChildren = false)
        {
            var list = new List<ViewModels.SerializableRow>();
            list.Add(new ViewModels.SerializableRow(
                Code,
                Description,
                Decimal.Round(BuyPrice * SellPriceFactor, 0, MidpointRounding.AwayFromZero).ToString("N2"),
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
            copy.Id = Id;
            copy.Code = Code;
            copy.Description = Description;
            copy.BuyPrice = BuyPrice;
            copy.SellPriceFactor = SellPriceFactor;
            copy.Timestamp = Timestamp;
            return copy;
        }

        public void Save(IDbConnection conn = null, IDbTransaction t = null)
        {
            Timestamp = DateTime.Now;
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
                    using (var tr = c.BeginTransaction())
                    {
                        c.Delete(this, tr);
                        SKUCollectionLink.Delete(c, tr, null, Id);
                        tr.Commit();
                    }
                }
            }
            else
            {
                conn.Delete(this, t);
                SKUCollectionLink.Delete(conn, t, null, Id);
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
