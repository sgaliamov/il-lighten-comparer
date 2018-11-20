using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Visitors
{
    internal sealed class CompareEmitVisitor : IVisitor
    {
        public void Visit(ComparableProperty info, ILEmitter il)
        {
            var isValueType = info.OwnerType?.IsValueType
                              ?? throw new InvalidOperationException("Can't resolve property owner class.");

            var local = il.GetLocal(info.MemberType);
            il.Emit(
                isValueType ? OpCodes.Ldarga_S : OpCodes.Ldarg_S, // todo: use short form for classes
                1); // x = arg1
            il.Emit(OpCodes.Callvirt, info.GetterMethod); // a = x.Prop // todo: call for value types?
            il.EmitStore(local);
            il.EmitLoadAddress(local); // pa = *a

            il.Emit(
                isValueType ? OpCodes.Ldarga_S : OpCodes.Ldarg_S, // todo: use short form for classes
                2); // y = arg2 
            il.Emit(OpCodes.Callvirt, info.GetterMethod); // b = y.Prop  // todo: call for value types

            EmitCompareToCall(il, info.CompareToMethod);
        }

        public void Visit(ComparableField info, ILEmitter il) { }

        public void Visit(NestedObject info, ILEmitter il)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Emits call to CompareTo method of basic type.
        ///     Expects in stack:
        ///     - address of left value;
        ///     - right value.
        ///     Push result into Ldloc_0.
        /// </summary>
        private static void EmitCompareToCall(ILEmitter il, MethodInfo compareToMethod)
        {
            il.Emit(OpCodes.Call, compareToMethod); // r = pa->CompareTo(b);
            il.Emit(OpCodes.Stloc_0); // pop r
            il.Emit(OpCodes.Ldloc_0); // push r

            var gotoNext = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, gotoNext); // if(r != 0) return r;
            il.Emit(OpCodes.Ldloc_0); // pop r
            il.Emit(OpCodes.Ret); // return r
            il.MarkLabel(gotoNext);
        }
    }
}
