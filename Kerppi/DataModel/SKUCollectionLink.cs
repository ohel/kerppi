/*
    Copyright 2015, 2017, 2024 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using Dapper;
using System.Data;

namespace Kerppi.DataModel
{
    [Table("sku_collection_links")]
    class SKUCollectionLink: IKerppiDBTableCreator
    {
        [Key]
        public long? Id { get; set; }
        public long SKUCollectionId { get; set; }
        public long SKUId { get; set; }

        public static void Create(IDbConnection conn, IDbTransaction t, long skuCollectionId, long skuId)
        {
            conn.Insert(new SKUCollectionLink { Id = null, SKUCollectionId = skuCollectionId, SKUId = skuId }, t);
        }

        public static void Delete(IDbConnection conn, IDbTransaction t, long? skuCollectionId, long? skuId)
        {
            string sql =
                "DELETE FROM sku_collection_links WHERE " +
                (skuCollectionId == null ? "0=0" : ("SKUCollectionId = " + skuCollectionId.ToString())) + " AND " +
                (skuId == null ? "0=0" : ("SKUId = " + skuId.ToString())) + ";";
            DBHandler.Execute(sql, conn, t);
        }

        public static void CreateDBTables(IDbConnection conn, IDbTransaction t)
        {
            string sql = @"
                CREATE TABLE sku_collection_links (
                Id INTEGER PRIMARY KEY,
                SKUId INTEGER,
                SKUCollectionId INTEGER,
                FOREIGN KEY (SKUId) REFERENCES skus(Id), --ON DELETE CASCADE, <-- done manually
                FOREIGN KEY (SKUCollectionId) REFERENCES sku_collection(Id) --ON DELETE CASCADE <-- done manually
                );";
            DBHandler.Execute(sql, conn, t);
        }
    }
}
