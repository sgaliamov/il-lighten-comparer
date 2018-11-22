using System.Reflection.Emit;
using ILLightenComparer.Emit.Members.Comparable;
using ILLightenComparer.Emit.Members.Integral;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly StackEmitter _stackVisitor;

        public CompareEmitter(TypeBuilderContext context) => _stackVisitor = new StackEmitter(context);

        public void Visit(IComparableMember member, ILEmitter il)
        {
            member.Accept(_stackVisitor, il);

            il.Emit(OpCodes.Call, member.CompareToMethod);

            EmitCheckToZero(il);
        }

        public void Visit(IIntegralMember member, ILEmitter il)
        {
            member.Accept(_stackVisitor, il);

            il.Emit(OpCodes.Sub);

            EmitCheckToZero(il);
        }

        private static void EmitCheckToZero(ILEmitter il)
        {
            var gotoNext = il.DefineLabel();

            il.Emit(OpCodes.Stloc_0)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Brfalse_S, gotoNext)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Ret)
              .MarkLabel(gotoNext);
        }
    }
}
