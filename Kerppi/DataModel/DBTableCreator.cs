using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kerppi.DataModel
{
    interface DBTableCreator
    {
        // This interface is just used for reflection to detect classes supporting the static method:
        // static void CreateDBTables(IDbConnection conn, IDbTransaction t)
    }
}
