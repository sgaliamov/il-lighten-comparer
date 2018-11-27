using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members.Integral;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly StackEmitter _stackVisitor;

        public CompareEmitter(TypeBuilderContext context) => _stackVisitor = new StackEmitter(context);

        public ILEmitter Visit(IComparableMember member, ILEmitter il)
        {
            member.Accept(_stackVisitor, il);

            il.Emit(OpCodes.Call, member.CompareToMethod);

            EmitCheckForZero(il);

            return il;
        }

        public ILEmitter Visit(IIntegralMember member, ILEmitter il)
        {
            member.Accept(_stackVisitor, il);

            il.Emit(OpCodes.Sub);

            EmitCheckForZero(il);

            return il;
        }

        public ILEmitter Visit(INullableMember member, ILEmitter il)
        {
            var propertyType = member.MemberType;

            il.LoadProperty(member, 2) // var n2 = &arg2
              .DeclareLocal(propertyType, out var n2, 0)
              .Store(n2)
              .LoadAddress(n2)
              // var secondHasValue = n2->HasValue
              .Call(propertyType, member.HasValueMethod)
              .DeclareLocal(typeof(bool), out var secondHasValue)
              .Store(secondHasValue)
              // var n1 = &arg1
              .LoadProperty(member, 1)
              .DeclareLocal(propertyType, out var n1, 1)
              .Store(n1)
              .LoadAddress(n1)
              // if n1->HasValue goto firstHasValue
              .Call(propertyType, member.HasValueMethod)
              .Branch(OpCodes.Brtrue_S, out var firstHasValue)
              // if n2->HasValue goto returnZero
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brfalse_S, out var next)
              // else return -1
              .Emit(OpCodes.Ldc_I4_M1)
              .Emit(OpCodes.Ret)
              // firstHasValue:
              .MarkLabel(firstHasValue)
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brtrue_S, out var getValues)
              // return 1
              .Emit(OpCodes.Ldc_I4_1)
              .Emit(OpCodes.Ret)
              // getValues: load values
              .MarkLabel(getValues)
              .LoadAddress(n1)
              .Call(propertyType, member.GetValueMethod)
              .DeclareLocal(member.MemberType.GetUnderlyingType(), out var local)
              .Store(local)
              .LoadAddress(local)
              .LoadAddress(n2)
              .Call(propertyType, member.GetValueMethod)
              // compare
              .Emit(OpCodes.Call, member.CompareToMethod)
              .Emit(OpCodes.Stloc_0)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Brfalse_S, next)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Ret)
              .MarkLabel(next);

            return il;
        }

        private static void EmitCheckForZero(ILEmitter il)
        {
            il.Emit(OpCodes.Stloc_0)
              .Emit(OpCodes.Ldloc_0)
              .Branch(OpCodes.Brfalse_S, out var gotoNext)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Ret)
              .MarkLabel(gotoNext);
        }
    }
}
