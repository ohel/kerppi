/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;

namespace Kerppi
{
    class KerppiException : Exception
    {
        public KerppiException()
        {
        }

        public KerppiException(string message)
            : base(message)
        {
        }

        public KerppiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
