using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Comparisons;
using ILLightenComparer.Emit.v2.Variables;

namespace ILLightenComparer.Emit.v2.Visitors
{
    internal sealed class VariablesVisitor
    {
        private readonly VariableLoader _loader;
        private readonly Converter _converter;

        public VariablesVisitor(VariableLoader loader, Converter converter)
        {
            _loader = loader;
            _converter = converter;
        }

        public ILEmitter LoadVariables(HierarchicalsComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;

            il.LoadArgument(Arg.Context);
            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il.LoadArgument(Arg.SetX)
                     .LoadArgument(Arg.SetY);
        }

        public ILEmitter LoadVariables(SealedComparableVariable variable, ILEmitter il, Label gotoNext)
        {
            var variableType = variable.VariableType;
            var underlyingType = variableType.GetUnderlyingType();
            if (underlyingType.IsValueType)
            {
                variable.LoadAddress(_loader, il, Arg.X);
                return variable.Load(_loader, il, Arg.Y);
            }

            if (underlyingType.IsSealed)
            {
                variable.Load(_loader, il, Arg.X)
                        .Store(underlyingType, 0, out var x);

                return variable.Load(_loader, il, Arg.Y)
                               .Store(underlyingType, 1, out var y)
                               .LoadLocal(x)
                               .Branch(OpCodes.Brtrue_S, out var call)
                               .LoadLocal(y)
                               .Branch(OpCodes.Brfalse_S, gotoNext)
                               .Return(-1)
                               .MarkLabel(call)
                               .LoadLocal(x)
                               .LoadLocal(y);
            }

            il.LoadArgument(Arg.Context);
            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y)
                    .LoadArgument(Arg.SetX)
                    .LoadArgument(Arg.SetY);

            return il;
        }

        public ILEmitter LoadVariables(IComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;

            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il;
        }

        public ILEmitter LoadVariables(ArgumentVariable variable, ILEmitter il, Label gotoNext)
        {
            var variableType = variable.VariableType;
            variable.Load(_loader, il, Arg.X).Store(variableType, 0, out var x);
            variable.Load(_loader, il, Arg.Y).Store(variableType, 1, out var y);

            var variables = new Variables.Variables(variableType, x, y);

            variables.lo

            if (variableType.IsValueType)
            {
                if (variableType.IsNullable())
                {
                    il.CheckNullableValuesForNull(x, y, variableType, gotoNext);
                }
            }
            else
            {
                il.EmitCheckReferenceComparison(x, y, gotoNext);
            }


            _converter.CreateVariable(variable, x, y).;

            return il;
        }

        private void EmitMembersComparison(ILEmitter il, Type objectType)
        {
            if (objectType.GetUnderlyingType().IsPrimitive())
            {
                throw new InvalidOperationException($"{objectType.DisplayName()} is not expected.");
            }

            var members = _membersProvider.GetMembers(objectType);
            foreach (var member in members)
            {
                member.Accept(this, il);
            }
        }
    }
}
