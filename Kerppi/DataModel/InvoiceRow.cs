using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kerppi.DataModel
{
    abstract class InvoiceRow
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return (String.IsNullOrEmpty(Code) ? (Code + ": ") : "") + Description;
        }

        public abstract List<ViewModels.SerializableRow> PrintSerializable();
    }
}
