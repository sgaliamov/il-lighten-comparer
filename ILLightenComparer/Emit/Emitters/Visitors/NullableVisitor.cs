using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class NullableVisitor
    {
        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter;
        private readonly VariableLoader _loader;
        private readonly StackVisitor _stackVisitor;

        public NullableVisitor(
            StackVisitor stackVisitor,
            CompareVisitor compareVisitor,
            VariableLoader loader,
            Converter converter)
        {
            _stackVisitor = stackVisitor;
            _compareVisitor = compareVisitor;
            _loader = loader;
            _converter = converter;
        }

        public ILEmitter Visit(NullableComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);

            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            variable.Load(_loader, il, Arg.X).Store(variableType, 0, out var nullableX);
            variable.Load(_loader, il, Arg.Y).Store(variableType, 1, out var nullableY);

            // todo: move implementation here
            il.CheckNullableValuesForNull(nullableX, nullableY, variableType, gotoNextMember);

            var itemComparison = _converter.CreateNullableVariableComparison(variable, nullableX, nullableY);

            itemComparison.LoadVariables(_stackVisitor, il, gotoNextMember);

            return itemComparison.Accept(_compareVisitor, il)
                                 .EmitReturnNotZero(gotoNextMember)
                                 .MarkLabel(gotoNextMember);
        }
    }
}
