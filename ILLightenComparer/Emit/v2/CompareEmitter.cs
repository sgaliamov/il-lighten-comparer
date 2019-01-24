using System;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Visitors;
using ILLightenComparer.Emit.v2.Visitors.Collection;

namespace ILLightenComparer.Emit.v2
{
    internal sealed class CompareEmitter
    {
        private readonly ArrayVisitor _arrayVisitor;
        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter;
        private readonly EnumerableVisitor _enumerableVisitor;
        private readonly VariableLoader _loader = new VariableLoader();
        private readonly VariablesVisitor _variablesVisitor;

        public CompareEmitter(ComparerContext context, Converter converter)
        {
            _compareVisitor = new CompareVisitor(context);
            _variablesVisitor = new VariablesVisitor(_loader, converter);
            _arrayVisitor = new ArrayVisitor(context, _variablesVisitor, _compareVisitor, _loader, converter);
            _enumerableVisitor = new EnumerableVisitor(context, _variablesVisitor, _compareVisitor, _loader, converter);
            _converter = converter;
        }

        public ILEmitter Visit(IComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var gotoNext);
            arguments.Accept(_variablesVisitor, il, gotoNext);

            return comparison.Accept(_compareVisitor, il)
                             .EmitReturnNotZero(gotoNext)
                             .MarkLabel(gotoNext);
        }

        public ILEmitter Visit(ArrayComparison comparison, ILEmitter il)
        {
            return _arrayVisitor.Visit(comparison, il);
        }

        public ILEmitter Visit(EnumerableComparison comparison, ILEmitter il)
        {
            return _enumerableVisitor.Visit(comparison, il);
        }

        public ILEmitter Visit(NullableComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var gotoNext);

            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            variable.Load(_loader, il, Arg.X).Store(variableType, 0, out var nullableX);
            variable.Load(_loader, il, Arg.Y).Store(variableType, 1, out var nullableY);
            il.CheckNullableValuesForNull(nullableX, nullableY, variableType, gotoNext);

            var itemComparison = _converter.CreateNullableVariableComparison(variable, nullableX, nullableY);
            itemComparison.LoadVariables(_stackVisitor, il, gotoNext);

            return itemComparison.Accept(_compareVisitor, il)
                                 .EmitReturnNotZero(gotoNext)
                                 .MarkLabel(gotoNext);
        }

        {
        }
    }
}
