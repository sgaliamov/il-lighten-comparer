using System.Reflection.Emit;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Visitors
{
    internal sealed class PushToStackVisitor : IVisitor
    {
        private readonly TypeBuilderContext _context;

        public PushToStackVisitor(TypeBuilderContext context) => _context = context;

        public void Visit(ComparablePropertyMember member, ILEmitter il)
        {
            var isValueType = member.OwnerType.IsValueType;
            if (isValueType)
            {
                var local = il.DeclareLocal(member.MemberType);

                il.Emit(OpCodes.Ldarga_S, 1) // x = arg1
                  .Emit(OpCodes.Call, member.GetterMethod) // a = x.Prop
                  .EmitStore(local)
                  .EmitLoadAddressOf(local) // pa = *a
                  .Emit(OpCodes.Ldarga_S, 2) // y = arg2 
                  .Emit(OpCodes.Call, member.GetterMethod); // b = y.Prop
            }
            else
            {
                var local = il.DeclareLocal(member.MemberType);

                il.Emit(OpCodes.Ldarg_1) // x = arg1
                  .Emit(OpCodes.Callvirt, member.GetterMethod) // a = x.Prop
                  .EmitStore(local)
                  .EmitLoadAddressOf(local) // pa = *a
                  .Emit(OpCodes.Ldarg_2) // y = arg2 
                  .Emit(OpCodes.Callvirt, member.GetterMethod); // b = y.Prop
            }
        }

        public void Visit(ComparableFieldMember member, ILEmitter il)
        {
            var isValueType = member.OwnerType.IsValueType;
            if (isValueType)
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

            il.Emit(
                OpCodes.Ldc_I4_S,
                (int)_context.Configuration.StringComparisonType); // todo: use short form
        }
    }
}
