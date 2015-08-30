using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kerppi.DataModel
{
    interface DBWritable
    {
        void Save();
        void Delete();
    }
}
