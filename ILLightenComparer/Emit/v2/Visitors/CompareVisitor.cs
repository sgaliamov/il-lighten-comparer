using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Comparisons;
using ILLightenComparer.Emit.v2.Sources;
using ILLightenComparer.Emit.v2.Variables;

namespace ILLightenComparer.Emit.v2.Visitors
{
    internal sealed class CompareVisitor
    {
        private readonly ComparerContext _context;
        private readonly Converter _converter;
        private readonly VariableLoader _loader;
        private readonly MembersProvider _membersProvider;

        public CompareVisitor(ComparerContext context, MembersProvider membersProvider, VariableLoader loader, Converter converter)
        {
            _context = context;
            _membersProvider = membersProvider;
            _loader = loader;
            _converter = converter;
        }

        public ILEmitter Visit(HierarchicalsComparison comparison, ILEmitter il)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            il.LoadArgument(Arg.Context);
            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);
            il.LoadArgument(Arg.SetX)
              .LoadArgument(Arg.SetY);

            if (!variableType.IsValueType && !variableType.IsSealed)
            {
                return EmitCallForDelayedCompareMethod(il, variableType);
            }

            var compareMethod = _context.GetStaticCompareMethod(variableType);

            return il.Emit(OpCodes.Call, compareMethod);
        }

        public ILEmitter Visit(ComparablesComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            if (variableType.IsValueType)
            {
                variable.LoadAddress(_loader, il, Arg.X);
                variable.Load(_loader, il, Arg.Y);
            }
            else
            {
                variable.Load(_loader, il, Arg.X).Store(variableType, 0, out var x);
                variable.Load(_loader, il, Arg.Y)
                        .Store(variableType, 1, out var y)
                        .LoadLocal(x)
                        .Branch(OpCodes.Brtrue_S, out var call)
                        .LoadLocal(y)
                        .Branch(OpCodes.Brfalse_S, gotoNext)
                        .Return(-1)
                        .MarkLabel(call)
                        .LoadLocal(x)
                        .LoadLocal(y);
            }

            return il.Emit(OpCodes.Call, comparison.CompareToMethod);
        }

        public ILEmitter Visit(IntegralsComparison comparison, ILEmitter il)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            if (!variableType.GetUnderlyingType().IsIntegral())
            {
                throw new InvalidOperationException($"Integral type is expected but: {variableType.DisplayName()}.");
            }

            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il.Emit(OpCodes.Sub);
        }

        public ILEmitter Visit(StringsComparison comparison, ILEmitter il)
        {
            var variable = comparison.Variable;

            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il.LoadConstant(comparison.StringComparisonType).Call(Method.StringCompare);
        }

        public ILEmitter Visit(NullableComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            variable.Load(_loader, il, Arg.X).Store(variableType, 0, out var nullableX);
            variable.Load(_loader, il, Arg.Y).Store(variableType, 1, out var nullableY);
            il.EmitCheckNullablesForValue(nullableX, nullableY, variableType, gotoNext);
            var nullableVariable = new NullableVariable(variableType, nullableX, nullableY);

            return _converter
                   .CreateComparison(nullableVariable)
                   .Accept(this, il, gotoNext);
        }

        public ILEmitter LoadVariables(LocalVariable variable, ILEmitter il, Label gotoNext)
        {
            var variableType = variable.VariableType;
            var x = variable.Locals[Arg.X];
            var y = variable.Locals[Arg.Y];

            if (variableType.IsValueType)
            {
                if (variableType.IsNullable())
                {
                    return il.EmitCheckNullablesForValue(x, y, variableType, gotoNext);
                }
            }
            else
            {
                return il.EmitReferenceComparison(x, y, gotoNext);
            }

            return il;
        }

        public ILEmitter LoadVariables(Arguments variable, ILEmitter il, Label gotoNext)
        {
            var variableType = variable.VariableType;
            variable.Load(_loader, il, Arg.X).Store(variableType, 0, out var x);
            variable.Load(_loader, il, Arg.Y).Store(variableType, 1, out var y);

            var variables = new LocalVariable(variableType, x, y);

            LoadVariables(variables, il, gotoNext);

            return il;
        }

        private static ILEmitter EmitCallForDelayedCompareMethod(ILEmitter il, Type type)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(type);

            return il.Emit(OpCodes.Call, delayedCompare);
        }
    }
}
