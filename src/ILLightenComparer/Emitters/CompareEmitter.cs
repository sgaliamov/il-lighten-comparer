using System;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using ILLightenComparer.Shared;

namespace ILLightenComparer.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter;

        public CompareEmitter(Context context, IConfigurationProvider configurationProvider)
        {
            var membersProvider = new MembersProvider(configurationProvider);
            _converter = new Converter(configurationProvider);
            _compareVisitor = new CompareVisitor(context, configurationProvider, membersProvider, _converter);
        }

        public void Emit(Type objectType, ILEmitter il)
        {
            var comparison = _converter.CreateComparison(new ArgumentVariable(objectType));

            comparison.Accept(this, il);
        }

        public ILEmitter Visit(ArraysComparison comparison, ILEmitter il)
        {
            return CompareAsCollection(comparison, il);
        }

        public ILEmitter Visit(EnumerablesComparison comparison, ILEmitter il)
        {
            return CompareAsCollection(comparison, il);
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

        public ILEmitter Visit(CustomComparison comparison, ILEmitter il)
        {
            return _compareVisitor.Visit(comparison, il).Return();
        }

        private ILEmitter CompareAsCollection(IComparison comparison, ILEmitter il)
        {
            il.DefineLabel(out var exit);

            return comparison.Accept(_compareVisitor, il, exit)
                             .MarkLabel(exit)
                             .Return(0);
        }
    }
}
