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

        public void Visit(ComparableFieldMember member, ILEmitter il)
        {
            il.LoadFieldAddress(member, 1)
              .LoadField(member, 2);
        }

        public void Visit(ComparablePropertyMember member, ILEmitter il)
        {
            il.LoadProperty(member, 1)
              .DeclareLocal(member.ComparableType, out var local)
              .Store(local)
              .LoadAddress(local)
              .LoadProperty(member, 2);
        }

        public void Visit(StringFiledMember member, ILEmitter il)
        {
            var comparisonType = (int)_context.Configuration.StringComparisonType;

            il.LoadField(member, 1)
              .LoadField(member, 2)
              .Emit(OpCodes.Ldc_I4_S, comparisonType); // todo: use short form
        }

        public void Visit(StringPropertyMember member, ILEmitter il)
        {
            var comparisonType = (int)_context.Configuration.StringComparisonType;

            il.LoadProperty(member, 1)
              .LoadProperty(member, 2)
              .Emit(OpCodes.Ldc_I4_S, comparisonType); // todo: use short form
        }

        public void Visit(IntegralFiledMember member, ILEmitter il)
        {
            il.LoadField(member, 1)
              .LoadField(member, 2);
        }

        public void Visit(IntegralPropertyMember member, ILEmitter il)
        {
            il.LoadProperty(member, 1)
              .LoadProperty(member, 2);
        }

        public void Visit(NullablePropertyMember member, ILEmitter il)
        {
            //var hasValue = il.DeclareLocal(typeof(bool));

            //il.LoadArgument(2)
            //  .Call(member, member.HasValueMethod)
            //  .EmitStore(hasValue)
            //  .LoadArgument(1)
            //  .Call(member, member.HasValueMethod)
            //IL_000A:  call        System.Nullable<>.get_HasValue
            //IL_000F:  brtrue.s    IL_0018
            //IL_0011:  ldloc.0     // n2HasValue
            //IL_0012:  brfalse.s   IL_0016
            //IL_0014:  ldc.i4.m1   
            //IL_0015:  ret         
            //IL_0016:  ldc.i4.0    
            //IL_0017:  ret         
            //IL_0018:  ldloc.0     // n2HasValue
            //IL_0019:  brtrue.s    IL_001D
            //IL_001B:  ldc.i4.1    
            //IL_001C:  ret         
            //IL_001D:  call        System.Collections.Generic.Comparer<>.get_Default
            //IL_0022:  ldarga.s    00 
            //IL_0024:  call        System.Nullable<>.get_Value
            //IL_0029:  ldarga.s    01 
            //IL_002B:  call        System.Nullable<>.get_Value
            //IL_0030:  callvirt    System.Collections.Generic.Comparer<>.Compare
            //IL_0035:  ret  
        }
    }
}
