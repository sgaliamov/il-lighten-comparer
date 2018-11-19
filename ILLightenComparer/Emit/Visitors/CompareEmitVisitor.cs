using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Visitors
{
    internal sealed class CompareEmitVisitor : IVisitor
    {
        public void Visit(ILGenerator il, PropertyInfo info)
        {
            var isValueType = info.DeclaringType?.IsValueType
                              ?? throw new InvalidOperationException("Can't resolve property owner class.");

            var compareToMethod = info
                                  .PropertyType
                                  .GetMethod(nameof(IComparable.CompareTo), new[] { info.PropertyType })
                                  ?? throw new NotSupportedException(
                                      $"Property {info.DisplayName()} does not implement IComparable.");

            var getMethod = info.GetGetMethod();

            var local = il.DeclareLocal(info.PropertyType); // todo: maybe cache locals to reuse them for same types
            il.Emit(
                isValueType ? OpCodes.Ldarga_S : OpCodes.Ldarg_S, // todo: use short form for classes
                1); // x = arg1
            il.Emit(OpCodes.Callvirt, getMethod); // a = x.Prop // todo: call for value types
            il.Emit(OpCodes.Stloc, local);
            il.Emit(OpCodes.Ldloca, local); // pa = *a

            il.Emit(
                isValueType ? OpCodes.Ldarga_S : OpCodes.Ldarg_S, // todo: use short form for classes
                2); // y = arg2 
            il.Emit(OpCodes.Callvirt, getMethod); // b = y.Prop  // todo: call for value types

            il.Emit(OpCodes.Call, compareToMethod); // r = pa->CompareTo(b)
            il.Emit(OpCodes.Stloc_0); // pop r
            il.Emit(OpCodes.Ldloc_0); // push r

            var gotoNext = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, gotoNext); // if(r == 0) continue
            il.Emit(OpCodes.Ldloc_0); // pop r
            il.Emit(OpCodes.Ret); // return r
            il.MarkLabel(gotoNext); // else
        }

        public void Visit(ILGenerator il, FieldInfo info) { }
    }
}
