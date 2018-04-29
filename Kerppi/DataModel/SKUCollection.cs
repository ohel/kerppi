/*
    Copyright 2015, 2017, 2018 Olli Helin / GainIT
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
    [Table("sku_collections")]
    class SKUCollection: InvoiceRow, DBTableCreator, DBWritable, Copyable<SKUCollection>
    {
        [Key]
        public long? Id { get; set; }
        [Editable(false)]
        public List<SKU> SKUs { get; set; }

        public override string ToString()
        {
            return Code + ": " + Description + " (" + string.Join(", ", (SKUs.Select(sku => sku.Code))) + ")";
        }

        public override List<ViewModels.SerializableRow> PrintSerializable(bool onlyChildren = false)
        {
            var printout = SKUs.Select(sku => sku.PrintSerializable(onlyChildren)).Select(s => s.ElementAt(0)).ToList();
            if (onlyChildren) return printout;

            var sum = printout
                .Select(sku => { decimal d; return decimal.TryParse(sku.Price, out d) ? d : (decimal?)null; })
                .Where(d => d.HasValue)
                .Select(d => d.Value)
                .Sum();

            printout.ForEach(sku => sku.PrivateUseOnly = true);

            printout.Insert(0, new ViewModels.SerializableRow(
                Code,
                Description,
                sum.ToString(),
                (new Decimal(1.0)).ToString()
            ));

            return printout;
        }

        public SKUCollection()
        {
            Id = null;
            Code = "";
            Description = "";
            SKUs = new List<SKU>();
        }

        public SKUCollection Copy()
        {
            var copy = new SKUCollection();
            copy.Id = Id;
            copy.Code = Code;
            copy.Description = Description;
            copy.SKUs = SKUs.Select(sku => sku.Copy()).ToList();
            return copy;
        }

        public void Save(IDbConnection conn = null, IDbTransaction t = null)
        {
            Save(false, conn, t);
        }

        public void Save(bool noLinkUpdates, IDbConnection conn = null, IDbTransaction t = null)
        {
            if (conn == null)
            {
                using (var c = DBHandler.Connection())
                {
                    c.Open();
                    using (var tr = c.BeginTransaction())
                    {
                        SaveUsing(c, tr, noLinkUpdates);
                        tr.Commit();
                    }
                }
            }
            else
            {
                SaveUsing(conn, t, noLinkUpdates);
            }
        }

        private void SaveUsing(IDbConnection conn, IDbTransaction t, bool noLinkUpdates)
        {
            long? collectionId = null;
            if (Id == null)
            {
                var parameters = new List<Tuple<string, DbType, object>>();
                var paramCode = new Tuple<string, DbType, object>("@Code", DbType.String, Code);
                var paramName = new Tuple<string, DbType, object>("@Description", DbType.String, Description);
                parameters.Add(paramCode);
                parameters.Add(paramName);
                string sql = "INSERT INTO sku_collections (Code, Description) VALUES (@Code, @Description);";
                collectionId = DBHandler.Insert(sql, parameters, conn, t);
            }
            else
            {
                collectionId = Id;
                conn.Update(this, t);
            }
            if (!noLinkUpdates)
            {
                SKUCollectionLink.Delete(conn, t, (long)collectionId, null);
                SKUs.ForEach(sku => SKUCollectionLink.Create(conn, t, (long)collectionId, (long)sku.Id));
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
                        DeleteUsing(c, tr);
                        tr.Commit();
                    }
                }
            }
            else
            {
                DeleteUsing(conn, t);
            }
        }

        private void DeleteUsing(IDbConnection conn, IDbTransaction t)
        {
            conn.Delete(this, t);
            SKUCollectionLink.Delete(conn, t, Id, null);
        }

        public static IEnumerable<SKUCollection> LoadAll()
        {
            using (var conn = DBHandler.Connection())
            {
                conn.Open();
                var lookup = new Dictionary<long?, SKUCollection>();
                conn.Query<SKUCollection, SKU, SKUCollection>(@"
                    SELECT c.*, s.* FROM sku_collections c
                    LEFT JOIN sku_collection_links l ON l.SKUCollectionId = c.Id
                    LEFT JOIN skus s ON s.Id = l.SKUId;",
                    (c, s) => {
                        var skuCollection = new SKUCollection();
                        // Note the out parameter copy, SKUs will be added to existing one.
                        if (!lookup.TryGetValue(c.Id, out skuCollection)) {
                            lookup.Add(c.Id, skuCollection = c);
                        }
                        if (s != null)
                        {
                            skuCollection.SKUs.Add(s);
                        }
                        return skuCollection;
                    });

                return lookup.Values;
            }
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE sku_collections (
                Id INTEGER PRIMARY KEY,
                Code TEXT UNIQUE NOT NULL,
                Description TEXT
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
