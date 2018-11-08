using System.Collections;
using System.Reflection;

namespace ILightenComparer.Reflection
{
    internal static class Method
    {
        public static MethodInfo Compare = Interface
                                           .Comparer
                                           .GetMethod(nameof(IComparer.Compare));

        public static MethodInfo GenericCompare = Interface
                                                  .GenericComparer
                                                  .GetMethod(nameof(IComparer.Compare));

        public static MethodInfo EqualsMethod = Interface
                                                .EqualityComparer
                                                .GetMethod(nameof(IEqualityComparer.Equals));

        public static MethodInfo GenericEquals = Interface
                                                 .GenericEqualityComparer
                                                 .GetMethod(nameof(IEqualityComparer.Equals));

        public static MethodInfo GetHashCodeMethod = Interface
                                                     .EqualityComparer
                                                     .GetMethod(nameof(IEqualityComparer.GetHashCode));

        public static MethodInfo GenericGetHashCode = Interface
                                                      .GenericEqualityComparer
                                                      .GetMethod(nameof(IEqualityComparer.GetHashCode));
    }
}
