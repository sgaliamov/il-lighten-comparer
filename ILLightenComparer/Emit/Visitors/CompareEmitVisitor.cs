using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Visitors
{
    internal sealed class CompareEmitVisitor : IVisitor
    {
        public void Visit(ComparableProperty member, ILEmitter il)
        {
            var isValueType = member.OwnerType.IsValueType;

            var local = il.DeclareLocal(member.MemberType);

            il.Emit(
                  isValueType ? OpCodes.Ldarga_S : OpCodes.Ldarg_S, // todo: use short form for classes
                  1) // x = arg1
              .Emit(OpCodes.Callvirt, member.GetterMethod) // a = x.Prop // todo: call for value types?
              .EmitStore(local)
              .EmitLoadAddress(local) // pa = *a
              .Emit(
                  isValueType ? OpCodes.Ldarga_S : OpCodes.Ldarg_S, // todo: use short form for classes
                  2) // y = arg2 
              .Emit(OpCodes.Callvirt, member.GetterMethod); // b = y.Prop  // todo: call for value types

            EmitCompareToCall(il, member.CompareToMethod);
        }

        public void Visit(ComparableField member, ILEmitter il)
        {
            var isValueType = member.OwnerType.IsValueType;

            il.Emit(
                  isValueType ? OpCodes.Ldarga_S : OpCodes.Ldarg_S, // todo: use short form for classes
                  1) // x = arg1
              .Emit(OpCodes.Ldflda, member.FieldInfo) // a = x.Prop // todo: call for value types?
              .Emit(OpCodes.Ldarg_S, 2) // y = arg2 
              .Emit(OpCodes.Ldfld, member.FieldInfo); // b = y.Prop  // todo: call for value types

            EmitCompareToCall(il, member.CompareToMethod);
        }

        public void Visit(NestedObject member, ILEmitter il)
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
            il.Emit(OpCodes.Call, compareToMethod) // r = pa->CompareTo(b);
              .Emit(OpCodes.Stloc_0) // pop r
              .Emit(OpCodes.Ldloc_0); // push r

            var gotoNext = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, gotoNext) // if(r != 0) return r;
              .Emit(OpCodes.Ldloc_0) // pop r
              .Emit(OpCodes.Ret) // return r
              .MarkLabel(gotoNext);
        }
    }
}
