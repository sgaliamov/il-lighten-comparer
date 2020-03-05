using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors.Collection;
using ILLightenComparer.Reflection;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Comparisons
{
    internal sealed class EnumerablesComparison : IComparison
    {
        private readonly EnumerableVisitor _enumerableVisitor;

        private EnumerablesComparison(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));

            ElementType = variable
                          .VariableType
                          .FindGenericInterface(typeof(IEnumerable<>))
                          .GetGenericArguments()
                          .SingleOrDefault()
                          ?? throw new ArgumentException(nameof(variable));

            // todo: use read enumerator, not virtual
            EnumeratorType = typeof(IEnumerator<>).MakeGenericType(ElementType);

            GetEnumeratorMethod = typeof(IEnumerable<>)
                                  .MakeGenericType(ElementType)
                                  .GetMethod(MethodName.GetEnumerator, Type.EmptyTypes);

            _enumerableVisitor = new EnumerableVisitor(configurations, comparisons);
        }

        public Type ElementType { get; }
        public Type EnumeratorType { get; }
        public MethodInfo GetEnumeratorMethod { get; }
        public IVariable Variable { get; }
        public bool PutsResultInStack => false;

        public ILEmitter Accept(ILEmitter il, Label gotoNext) => _enumerableVisitor.Visit(this, il, gotoNext);

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il) => visitor.Visit(this, il);

        public static EnumerablesComparison Create(
            ComparisonResolver comparisons,
            IConfigurationProvider configurations,
            IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.ImplementsGeneric(typeof(IEnumerable<>)) && !variableType.IsArray) {
                return new EnumerablesComparison(comparisons, configurations, variable);
            }

            return null;
        }
    }
}
