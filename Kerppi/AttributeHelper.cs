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
