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

                il.Emit(OpCodes.Ldarga_S, 1) // x = arg1
                  .CallGetter(member) // a = x.Prop
                  .EmitStore(local)
                  .EmitLoadAddressOf(local) // pa = *a
                  .Emit(OpCodes.Ldarga_S, 2) // y = arg2 
                  .CallGetter(member); // b = y.Prop
            }
            else
            {
                var local = il.DeclareLocal(member.ComparableType);

                il.Emit(OpCodes.Ldarg_1) // x = arg1
                  .CallGetter(member) // a = x.Prop
                  .EmitStore(local)
                  .EmitLoadAddressOf(local) // pa = *a
                  .Emit(OpCodes.Ldarg_2) // y = arg2 
                  .CallGetter(member); // b = y.Prop
            }
        }

        public void Visit(ComparableFieldMember member, ILEmitter il)
        {
            if (member.OwnerType.IsValueType)
            {
                il.Emit(OpCodes.Ldarga_S, 1) // x = arg1
                  .Emit(OpCodes.Ldflda, member.FieldInfo) // a = x.Field 
                  .Emit(OpCodes.Ldarg_2) // y = arg2 
                  .Emit(OpCodes.Ldfld, member.FieldInfo); // b = y.Field
            }
            else
            {
                il.Emit(OpCodes.Ldarg_1) // x = arg1
                  .Emit(OpCodes.Ldflda, member.FieldInfo) // a = x.Field 
                  .Emit(OpCodes.Ldarg_2) // y = arg2 
                  .Emit(OpCodes.Ldfld, member.FieldInfo); // b = y.Field
            }
        }

        public void Visit(StringFiledMember member, ILEmitter il)
        {
            il.Emit(OpCodes.Ldarg_1)
              .Emit(OpCodes.Ldfld, member.FieldInfo)
              .Emit(OpCodes.Ldarg_2)
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
                il.Emit(OpCodes.Ldarg_1)
                  .CallGetter(member)
                  .Emit(OpCodes.Ldarg_2)
                  .CallGetter(member);
            }

            il.Emit(
                OpCodes.Ldc_I4_S,
                (int)_context.Configuration.StringComparisonType); // todo: use short form
        }

        public void Visit(IntegralFiledMember member, ILEmitter il)
        {
            il.Emit(OpCodes.Ldarg_1)
              .Emit(OpCodes.Ldfld, member.FieldInfo)
              .Emit(OpCodes.Ldarg_2)
              .Emit(OpCodes.Ldfld, member.FieldInfo);
        }

        public void Visit(IntegralPropertyMember member, ILEmitter il)
        {
            il.Emit(OpCodes.Ldarg_1)
              .CallGetter(member)
              .Emit(OpCodes.Ldarg_2)
              .CallGetter(member);
        }

        public void Visit(NullablePropertyMember member, ILEmitter il)
        {
            throw new System.NotImplementedException();
        }
    }
}
