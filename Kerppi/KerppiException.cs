using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
