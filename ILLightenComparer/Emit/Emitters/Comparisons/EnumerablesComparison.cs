using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class EnumerablesComparison : IComparison
    {
        private EnumerablesComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));

            ElementType = variable
                          .VariableType
                          .GetGenericArguments()
                          .SingleOrDefault()
                          ?? throw new ArgumentException(nameof(variable));

            // todo: use read enumerator, not virtual
            EnumeratorType = typeof(IEnumerator<>).MakeGenericType(ElementType);

            GetEnumeratorMethod = typeof(IEnumerable<>)
                                  .MakeGenericType(ElementType)
                                  .GetMethod(MethodName.GetEnumerator, Type.EmptyTypes);
        }

        public Type ElementType { get; }
        public Type EnumeratorType { get; }
        public MethodInfo GetEnumeratorMethod { get; }
        public IVariable Variable { get; }
        public bool ResultInStack => false;

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static EnumerablesComparison Create(IVariable variable)
        {
            var variableType = variable.VariableType;
            if (variableType.ImplementsGeneric(typeof(IEnumerable<>)) && !variableType.IsArray)
            {
                return new EnumerablesComparison(variable);
            }

            return null;
        }
    }
}
