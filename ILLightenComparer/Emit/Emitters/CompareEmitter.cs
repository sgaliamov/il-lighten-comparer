using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly ArrayVisitor _arrayVisitor;
        private readonly CompareVisitor _compareVisitor;
        private readonly EnumerableVisitor _enumerableVisitor;
        private readonly VariableLoader _loader = new VariableLoader();
        private readonly StackVisitor _stackVisitor;

        public CompareEmitter(ComparerContext context, Converter converter)
        {
            _compareVisitor = new CompareVisitor(context);
            _stackVisitor = new StackVisitor(_loader);
            _arrayVisitor = new ArrayVisitor(context, _stackVisitor, _compareVisitor, _loader, converter);
            _enumerableVisitor = new EnumerableVisitor(context, _stackVisitor, _compareVisitor, _loader, converter);
        }

        public ILEmitter Visit(IMemberComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);
            comparison.LoadVariables(_stackVisitor, il, gotoNextMember);

            return comparison.Accept(_compareVisitor, il)
                             .EmitReturnNotZero(gotoNextMember)
                             .MarkLabel(gotoNextMember);
        }

        public ILEmitter Visit(ArrayComparison comparison, ILEmitter il)
        {
            return _arrayVisitor.Visit(comparison, il);
        }

        public void EmitArgumentsReferenceComparison(ILEmitter il)
        {
            il.LoadArgument(Arg.X) // x == y
              .LoadArgument(Arg.Y)
              .Branch(OpCodes.Bne_Un_S, out var checkY)
              .Return(0)
              .MarkLabel(checkY)
              // y != null
              .LoadArgument(Arg.Y)
              .Branch(OpCodes.Brtrue_S, out var checkX)
              .Return(1)
              .MarkLabel(checkX)
              // x != null
              .LoadArgument(Arg.X)
              .Branch(OpCodes.Brtrue_S, out var next)
              .Return(-1)
              .MarkLabel(next);
        }

        public ILEmitter Visit(EnumerableComparison comparison, ILEmitter il)
        {
            return _enumerableVisitor.Visit(comparison, il);
        }
    }
}
