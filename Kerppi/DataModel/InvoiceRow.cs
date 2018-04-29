/*
    Copyright 2015, 2017, 2018 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System.Collections.Generic;

namespace Kerppi.DataModel
{
    abstract class InvoiceRow
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(Code) ? (Code + ": ") : "") + Description;
        }

        public abstract List<ViewModels.SerializableRow> PrintSerializable(bool onlyChildren = false);
    }
}
