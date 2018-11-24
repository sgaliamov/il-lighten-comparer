using System.Reflection.Emit;
using ILLightenComparer.Emit.Members.Comparable;
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

        private static void EmitCheckForZero(ILEmitter il)
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
