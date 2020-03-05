using System;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Builders;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;
using Illuminator;

namespace ILLightenComparer.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly ComparisonResolver _comparisons;

        public CompareEmitter(ComparerProvider provider, IConfigurationProvider configurationProvider) =>
            _comparisons = new ComparisonResolver(provider, configurationProvider);

        public void Emit(Type objectType, ILEmitter il)
        {
            var comparison = _comparisons.GetComparison(new ArgumentVariable(objectType));

            comparison.Accept(this, il);
        }

        public ILEmitter Visit(ArraysComparison comparison, ILEmitter il) => CompareAsCollection(comparison, il);

        public ILEmitter Visit(EnumerablesComparison comparison, ILEmitter il) => CompareAsCollection(comparison, il);

        public ILEmitter Visit(MembersComparison comparison, ILEmitter il) => comparison.Compare(il, default).Return();

        public ILEmitter Visit(HierarchicalsComparison comparison, ILEmitter il) => comparison.Compare(il, default).Return();

        public ILEmitter Visit(IntegralsComparison comparison, ILEmitter il) => comparison.Compare(il, default).Return();

        public ILEmitter Visit(StringsComparison comparison, ILEmitter il) => comparison.Compare(il, default).Return();

        public ILEmitter Visit(ComparablesComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return comparison.Compare(il, exit)
                             .EmitReturnNotZero(exit)
                             .MarkLabel(exit)
                             .Return(0);
        }

        public ILEmitter Visit(NullableComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return comparison.Compare(il, exit)
                             .EmitReturnNotZero(exit)
                             .MarkLabel(exit)
                             .Return(0);
        }

        public ILEmitter Visit(CustomComparison comparison, ILEmitter il) => comparison.Compare(il, default).Return();

        private ILEmitter CompareAsCollection(IComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return comparison.Compare(il, exit)
                             .MarkLabel(exit)
                             .Return(0);
        }
    }
}
