using System;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using ILLightenComparer.Extensions;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly CompareVisitor _compareVisitor;
        private readonly ComparisonsProvider _comparisons;

        public CompareEmitter(Context context, IConfigurationProvider configurationProvider)
        {
            _comparisons = new ComparisonsProvider(configurationProvider);
            _compareVisitor = new CompareVisitor(context, _comparisons, configurationProvider);
        }

        public void Emit(Type objectType, ILEmitter il)
        {
            var comparison = _comparisons.GetComparison(new ArgumentVariable(objectType));

            comparison.Accept(this, il);
        }

        public ILEmitter Visit(ArraysComparison comparison, ILEmitter il) => CompareAsCollection(comparison, il);

        public ILEmitter Visit(EnumerablesComparison comparison, ILEmitter il) => CompareAsCollection(comparison, il);

        public ILEmitter Visit(MembersComparison comparison, ILEmitter il) => _compareVisitor.Visit(comparison, il).Return();

        public ILEmitter Visit(HierarchicalsComparison comparison, ILEmitter il) => _compareVisitor.Visit(comparison, il).Return();

        public ILEmitter Visit(IntegralsComparison comparison, ILEmitter il) => _compareVisitor.Visit(comparison, il).Return();

        public ILEmitter Visit(StringsComparison comparison, ILEmitter il) => _compareVisitor.Visit(comparison, il).Return();

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

        public ILEmitter Visit(CustomComparison comparison, ILEmitter il) => _compareVisitor.Visit(comparison, il).Return();

        private ILEmitter CompareAsCollection(IComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return comparison.Accept(_compareVisitor, il, exit)
                             .MarkLabel(exit)
                             .Return(0);
        }
    }
}
