using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members.Comparable;
using ILLightenComparer.Emit.Members.Integral;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class StackEmitter
    {
        private readonly TypeBuilderContext _context;

        public StackEmitter(TypeBuilderContext context) => _context = context;

        public void Visit(ComparablePropertyMember member, ILEmitter il)
        {
            if (member.OwnerType.IsValueType)
            {
                var local = il.DeclareLocal(member.ComparableType);

                il.Emit(OpCodes.Ldarga_S, 1)
                  .CallGetter(member)
                  .EmitStore(local)
                  .EmitLoadAddressOf(local)
                  .Emit(OpCodes.Ldarga_S, 2)
                  .CallGetter(member);
            }
            else
            {
                var local = il.DeclareLocal(member.ComparableType);

                il.LoadArgument(1)
                  .CallGetter(member)
                  .EmitStore(local)
                  .EmitLoadAddressOf(local)
                  .LoadArgument(2)
                  .CallGetter(member);
            }
        }

        public void Visit(ComparableFieldMember member, ILEmitter il)
        {
            if (member.OwnerType.IsValueType)
            {
                il.Emit(OpCodes.Ldarga_S, 1)
                  .Emit(OpCodes.Ldflda, member.FieldInfo)
                  .LoadArgument(2)
                  .Emit(OpCodes.Ldfld, member.FieldInfo);
            }
            else
            {
                il.LoadArgument(1)
                  .Emit(OpCodes.Ldflda, member.FieldInfo)
                  .LoadArgument(2)
                  .Emit(OpCodes.Ldfld, member.FieldInfo);
            }
        }

        public void Visit(StringFiledMember member, ILEmitter il)
        {
            il.LoadArgument(1)
              .Emit(OpCodes.Ldfld, member.FieldInfo)
              .LoadArgument(2)
              .Emit(OpCodes.Ldfld, member.FieldInfo)
              .Emit(OpCodes.Ldc_I4_S, (int)_context.Configuration.StringComparisonType); // todo: use short form
        }

        public void Visit(StringPropertyMember member, ILEmitter il)
        {
            if (member.OwnerType.IsValueType)
            {
                il.Emit(OpCodes.Ldarga_S, 1)
                  .CallGetter(member)
                  .Emit(OpCodes.Ldarga_S, 2)
                  .CallGetter(member);
            }
            else
            {
                il.LoadArgument(1)
                  .CallGetter(member)
                  .LoadArgument(2)
                  .CallGetter(member);
            }

            il.Emit(
                OpCodes.Ldc_I4_S,
                (int)_context.Configuration.StringComparisonType); // todo: use short form
        }

        public void Visit(IntegralFiledMember member, ILEmitter il)
        {
            il.LoadArgument(1)
              .Emit(OpCodes.Ldfld, member.FieldInfo)
              .LoadArgument(2)
              .Emit(OpCodes.Ldfld, member.FieldInfo);
        }

        public void Visit(IntegralPropertyMember member, ILEmitter il)
        {
            il.LoadArgument(1)
              .CallGetter(member)
              .LoadArgument(2)
              .CallGetter(member);
        }

        public void Visit(NullablePropertyMember member, ILEmitter il)
        {
            throw new NotImplementedException();
        }
    }
}
