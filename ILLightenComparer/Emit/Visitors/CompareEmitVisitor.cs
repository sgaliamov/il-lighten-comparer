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
            if (isValueType)
            {
                il.Emit(OpCodes.Ldarga_S, 1); // x = arg1
            }
            else
            {
                il.Emit(OpCodes.Ldarg_1); // x = arg1
            }

            var local = il.DeclareLocal(member.MemberType);

            il.Emit(
                  isValueType ? OpCodes.Call : OpCodes.Callvirt,
                  member.GetterMethod) // a = x.Prop
              .EmitStore(local)
              .EmitLoadAddressOf(local) // pa = *a
              .Emit(
                  isValueType ? OpCodes.Ldarga_S : OpCodes.Ldarg_S, // todo: use short form for classes
                  2) // y = arg2 
              .Emit(
                  isValueType ? OpCodes.Call : OpCodes.Callvirt,
                  member.GetterMethod); // b = y.Prop
            
            EmitCompareToCall(il, member.CompareToMethod);
        }

        public void Visit(ComparableField member, ILEmitter il)
        {
            var isValueType = member.OwnerType.IsValueType;
            if (isValueType)
            {
                il.Emit(OpCodes.Ldarga_S, 1); // x = arg1
            }
            else
            {
                il.Emit(OpCodes.Ldarg_1); // x = arg1
            }

            il.Emit(OpCodes.Ldflda, member.FieldInfo) // a = x.Field 
              .Emit(OpCodes.Ldarg_2) // y = arg2 
              .Emit(OpCodes.Ldfld, member.FieldInfo); // b = y.Field

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
            // r = pa->CompareTo(b);
            il.Emit(OpCodes.Call, compareToMethod)
              .Emit(OpCodes.Stloc_0)
              .Emit(OpCodes.Ldloc_0);

            // if(r != 0) return r;
            var gotoNext = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, gotoNext)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Ret)
              .MarkLabel(gotoNext);
        }
    }
}
