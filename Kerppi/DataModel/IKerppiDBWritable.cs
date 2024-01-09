/*
    Copyright 2015, 2017, 2024 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System.Data;

namespace Kerppi.DataModel
{
    interface IKerppiDBWritable
    {
        void Save(IDbConnection conn = null, IDbTransaction t = null);
        void Delete(IDbConnection conn = null, IDbTransaction t = null);
    }
}
