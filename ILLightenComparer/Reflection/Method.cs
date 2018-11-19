using System.Collections;
using System.Reflection;

namespace ILLightenComparer.Reflection
{
    internal static class Method
    {
        public static MethodInfo Compare = InterfaceType
                                           .Comparer
                                           .GetMethod(nameof(IComparer.Compare));
    }
}
