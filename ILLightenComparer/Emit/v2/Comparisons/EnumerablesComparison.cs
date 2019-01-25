using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;

namespace ILLightenComparer.Emit.v2.Comparisons
{
    internal sealed class EnumerablesComparison : IComparison
    {
        private EnumerablesComparison(IVariable variable)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            
            GetEnumeratorMethod = variable.VariableType.GetMethod(MethodName.GetEnumerator, Type.EmptyTypes)
                                  ?? throw new ArgumentException(nameof(variable));
            
            ElementType = variable.VariableType.GetGenericArguments().FirstOrDefault()
                          ?? throw new ArgumentException(nameof(variable));
            
            EnumeratorType = typeof(IEnumerator<>).MakeGenericType(ElementType);
        }

        public Type ElementType { get; }
        public Type EnumeratorType { get; }
        public MethodInfo GetEnumeratorMethod { get; }
        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
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
