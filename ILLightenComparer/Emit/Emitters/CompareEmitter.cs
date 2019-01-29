using System;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter = new Converter();
        private readonly VariableLoader _loader = new VariableLoader();

        public CompareEmitter(ComparerContext context)
        {
            _compareVisitor = new CompareVisitor(context, new MembersProvider(context), _loader, _converter);
        }

        public void Emit(Type objectType, ILEmitter il)
        {
            var comparison = _converter.CreateComparison(new ArgumentVariable(objectType));

            comparison.Accept(this, il);
        }

        public ILEmitter Visit(ArraysComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return _compareVisitor.Visit(comparison, il, exit)
                                  .MarkLabel(exit)
                                  .Return(0);
        }

        public ILEmitter Visit(EnumerablesComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return _compareVisitor.Visit(comparison, il, exit)
                                  .MarkLabel(exit)
                                  .Return(0);
        }

        public ILEmitter Visit(MembersComparison comparison, ILEmitter il)
        {
            return _compareVisitor.Visit(comparison, il).Return();
        }

        public ILEmitter Visit(HierarchicalsComparison comparison, ILEmitter il)
        {
            return _compareVisitor.Visit(comparison, il).Return();
        }

        public ILEmitter Visit(IntegralsComparison comparison, ILEmitter il)
        {
            return _compareVisitor.Visit(comparison, il).Return();
        }

        public ILEmitter Visit(StringsComparison comparison, ILEmitter il)
        {
            return _compareVisitor.Visit(comparison, il).Return();
        }

        public ILEmitter Visit(ComparablesComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return _compareVisitor.Visit(comparison, il, exit)
                                  .EmitReturnNotZero(exit)
                                  .MarkLabel(exit)
                                  .Return(0);
        }

        public ILEmitter Visit(NullableComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return _compareVisitor.Visit(comparison, il, exit)
                                  .EmitReturnNotZero(exit)
                                  .MarkLabel(exit)
                                  .Return(0);
        }
    }
}
