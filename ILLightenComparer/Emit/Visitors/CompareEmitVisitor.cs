using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ILLightenComparer.Emit.Visitors
{
    internal sealed class CompareEmitVisitor : IVisitor
    {
        public void Visit(ILGenerator il, PropertyInfo info)
        {
            var comparable = info
                             .PropertyType
                             .GetInterfaces()
                             .FirstOrDefault(type => type.FullName == typeof(IComparable).FullName)
                             ?? throw new NotSupportedException(
                                 $"Property {info.DeclaringType}.{info.Name} must implement {nameof(IComparable)}");

            var compareToMethod = comparable.GetMethod(nameof(IComparable.CompareTo));
            var getMethod = info.GetGetMethod();

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, getMethod);

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, getMethod);

            var gotoNext = il.DefineLabel();
            il.Emit(OpCodes.Callvirt, compareToMethod);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Brfalse_S, gotoNext);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(gotoNext);
            il.Emit(OpCodes.Pop);
        }

        public void Visit(ILGenerator il, FieldInfo info) { }
    }
}
