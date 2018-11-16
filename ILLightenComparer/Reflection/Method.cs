using System.Collections;
using System.Reflection;

namespace ILLightenComparer.Reflection
{
    internal static class Method
    {
        public static MethodInfo Compare = InterfaceType
                                           .Comparer
                                           .GetMethod(nameof(IComparer.Compare));

        public static MethodInfo GenericCompare = InterfaceType
                                                  .GenericComparer
                                                  .GetMethod(nameof(IComparer.Compare));

        public static MethodInfo EqualsMethod = InterfaceType
                                                .EqualityComparer
                                                .GetMethod(nameof(IEqualityComparer.Equals));

        public static MethodInfo GenericEquals = InterfaceType
                                                 .GenericEqualityComparer
                                                 .GetMethod(nameof(IEqualityComparer.Equals));

        public static MethodInfo GetHashCodeMethod = InterfaceType
                                                     .EqualityComparer
                                                     .GetMethod(nameof(IEqualityComparer.GetHashCode));

        public static MethodInfo GenericGetHashCode = InterfaceType
                                                      .GenericEqualityComparer
                                                      .GetMethod(nameof(IEqualityComparer.GetHashCode));
    }
}
