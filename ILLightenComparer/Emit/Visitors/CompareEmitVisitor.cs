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
            var compareToMethod = info
                                  .PropertyType
                                  .GetMethod(nameof(IComparable.CompareTo), new[] { info.PropertyType })
                                  ?? throw new NotSupportedException(
                                      $"Property {info.DisplayName()} does not implement IComparable.");

            var getMethod = info.GetGetMethod();

            var local = il.DeclareLocal(info.PropertyType); // todo: maybe cache locals to reuse them for same types
            il.Emit(OpCodes.Ldarg_1); // x = arg1
            il.Emit(OpCodes.Callvirt, getMethod); // a = x.Prop
            il.Emit(OpCodes.Stloc, local);
            il.Emit(OpCodes.Ldloca, local); // pa = *a

            il.Emit(OpCodes.Ldarg_2); // y = arg2
            il.Emit(OpCodes.Callvirt, getMethod); // b = y.Prop

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
