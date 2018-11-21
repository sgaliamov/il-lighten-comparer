using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Visitors
{
    internal sealed class CompareEmitVisitor : IVisitor
    {
        private static readonly MethodInfo StringCompareMethod = typeof(string).GetMethod(nameof(string.Compare),
            new[] { typeof(string), typeof(string), typeof(StringComparison) });

        private readonly TypeBuilderContext _context;

        public CompareEmitVisitor(TypeBuilderContext context) => _context = context;

        public void Visit(ComparableProperty member, ILEmitter il)
        {
            var local = il.DeclareLocal(member.MemberType);

            var isValueType = member.OwnerType.IsValueType;
            if (isValueType)
            {
                il.Emit(OpCodes.Ldarga_S, 1) // x = arg1
                  .Emit(OpCodes.Call, member.GetterMethod) // a = x.Prop
                  .EmitStore(local)
                  .EmitLoadAddressOf(local) // pa = *a
                  .Emit(OpCodes.Ldarga_S, 2) // y = arg2 
                  .Emit(OpCodes.Call, member.GetterMethod); // b = y.Prop
            }
            else
            {
                il.Emit(OpCodes.Ldarg_1) // x = arg1
                  .Emit(OpCodes.Callvirt, member.GetterMethod) // a = x.Prop
                  .EmitStore(local)
                  .EmitLoadAddressOf(local) // pa = *a
                  .Emit(OpCodes.Ldarg_2) // y = arg2 
                  .Emit(OpCodes.Callvirt, member.GetterMethod); // b = y.Prop
            }

            EmitCompareCall(il, member.CompareToMethod);
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

            EmitCompareCall(il, member.CompareToMethod);
        }

        public void Visit(NestedObject member, ILEmitter il)
        {
            throw new NotImplementedException();
        }

        public void Visit(StringFiledMember member, ILEmitter il)
        {
            il.Emit(OpCodes.Ldarg_1)
              .Emit(OpCodes.Ldfld, member.FieldInfo)
              .Emit(OpCodes.Ldarg_2)
              .Emit(OpCodes.Ldfld, member.FieldInfo)
              .Emit(OpCodes.Ldc_I4_S, (int)_context.Configuration.StringComparisonType); // todo: use short form

            EmitCompareCall(il, StringCompareMethod);
        }

        public void Visit(StringPropertyMember member, ILEmitter il)
        {
            var isValueType = member.OwnerType.IsValueType;
            if (isValueType)
            {
                il.Emit(OpCodes.Ldarga_S, 1)
                  .Emit(OpCodes.Call, member.GetterMethod)
                  .Emit(OpCodes.Ldarga_S, 2)
                  .Emit(OpCodes.Call, member.GetterMethod);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_1)
                  .Emit(OpCodes.Callvirt, member.GetterMethod)
                  .Emit(OpCodes.Ldarg_2)
                  .Emit(OpCodes.Callvirt, member.GetterMethod);
            }

            il.Emit(OpCodes.Ldc_I4_S, (int)_context.Configuration.StringComparisonType); // todo: use short form

            EmitCompareCall(il, StringCompareMethod);
        }

        /// <summary>
        ///     Emits call to a compare method of basic type.
        ///     Expects correct arguments in the stack.
        ///     Push result into Ldloc_0.
        /// </summary>
        private static void EmitCompareCall(ILEmitter il, MethodInfo compareToMethod)
        {
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
