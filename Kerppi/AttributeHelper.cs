/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System.Linq;

namespace Kerppi
{
    public static class AttributeHelper
    {
        public static T GetAttribute<T>()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return (T)assembly.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }
    }
}
