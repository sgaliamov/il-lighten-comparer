﻿using System;
using System.Linq;
using System.Reflection.Emit;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Comparisons;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Emitters.Visitors.Collection;
using ILLightenComparer.Extensions;
using ILLightenComparer.Reflection;
using Illuminator;
using Illuminator.Extensions;

namespace ILLightenComparer.Emitters.Visitors
{
    internal sealed class CompareVisitor
    {
        private readonly ArrayVisitor _arrayVisitor;
        private readonly ComparisonsProvider _comparisons;
        private readonly IConfigurationProvider _configurations;
        private readonly Context _context;
        private readonly EnumerableVisitor _enumerableVisitor;
        private readonly VariableLoader _loader = new VariableLoader();
        private readonly MembersProvider _membersProvider;

        public CompareVisitor(
            Context context,
            ComparisonsProvider comparisons,
            IConfigurationProvider configurations)
        {
            _membersProvider = new MembersProvider(configurations);
            _context = context;
            _configurations = configurations;
            _comparisons = comparisons;
            _arrayVisitor = new ArrayVisitor(configurations, this, _loader, comparisons);
            _enumerableVisitor = new EnumerableVisitor(configurations, this, _loader, comparisons);
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

            if (!variableType.IsValueType && !variableType.IsSealed) {
                return EmitCallForDelayedCompareMethod(il, variableType);
            }

            var compareMethod = _context.GetStaticCompareMethod(variableType);

            return il.Call(compareMethod);
        }

        public ILEmitter Visit(ComparablesComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            if (variableType.IsValueType) {
                variable.LoadAddress(_loader, il, Arg.X);
                variable.Load(_loader, il, Arg.Y);
            }
            else {
                variable.Load(_loader, il, Arg.X).Store(variableType, out var x);
                variable.Load(_loader, il, Arg.Y)
                        .Store(variableType, out var y)
                        .LoadLocal(x)
                        .Branch(OpCodes.Brtrue_S, out var call)
                        .LoadLocal(y)
                        .Branch(OpCodes.Brfalse_S, gotoNext)
                        .Return(-1)
                        .MarkLabel(call)
                        .LoadLocal(x)
                        .LoadLocal(y);
            }

            return il.Call(comparison.CompareToMethod);
        }

        public ILEmitter Visit(IntegralsComparison comparison, ILEmitter il)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            if (!variableType.GetUnderlyingType().IsIntegral()) {
                throw new InvalidOperationException($"Integral type is expected but: {variableType.DisplayName()}.");
            }

            return il.Sub(
                il => variable.Load(_loader, il, Arg.X),
                il => variable.Load(_loader, il, Arg.Y));
        }

        public ILEmitter Visit(StringsComparison comparison, ILEmitter il)
        {
            var variable = comparison.Variable;

            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            var stringComparisonType = _configurations.Get(variable.OwnerType).StringComparisonType;

            return il.LoadConstant((int)stringComparisonType).Call(Method.StringCompare);
        }

        public ILEmitter Visit(NullableComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            variable.Load(_loader, il, Arg.X).Store(variableType, out var nullableX);
            variable.Load(_loader, il, Arg.Y).Store(variableType, out var nullableY);
            il.EmitCheckNullablesForValue(nullableX, nullableY, variableType, gotoNext);

            var nullableVariable = new NullableVariable(variableType, variable.OwnerType, nullableX, nullableY);

            return _comparisons
                   .GetComparison(nullableVariable)
                   .Accept(this, il, gotoNext);
        }

        public ILEmitter Visit(ArraysComparison comparison, ILEmitter il, Label gotoNext) => _arrayVisitor.Visit(comparison, il, gotoNext);

        public ILEmitter Visit(EnumerablesComparison comparison, ILEmitter il, Label gotoNext) => _enumerableVisitor.Visit(comparison, il, gotoNext);

        public ILEmitter Visit(MembersComparison comparison, ILEmitter il)
        {
            var variableType = comparison.Variable.VariableType;
            if (variableType.IsPrimitive()) {
                throw new InvalidOperationException($"{variableType.DisplayName()} is not expected.");
            }

            var comparisons = _membersProvider
                              .GetMembers(variableType)
                              .Select(_comparisons.GetComparison);

            foreach (var item in comparisons) {
                using (il.LocalsScope()) {
                    il.DefineLabel(out var gotoNext);

                    item.Accept(this, il, gotoNext);

                    if (item.PutsResultInStack) {
                        il.EmitReturnNotZero(gotoNext);
                    }

                    il.MarkLabel(gotoNext);
                }
            }

            return il.LoadConstant(0);
        }

        public ILEmitter Visit(CustomComparison comparison, ILEmitter il)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;

            il.LoadArgument(Arg.Context);
            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);
            il.LoadArgument(Arg.SetX)
              .LoadArgument(Arg.SetY);

            return EmitCallForDelayedCompareMethod(il, variableType);
        }

        private static ILEmitter EmitCallForDelayedCompareMethod(ILEmitter il, Type type)
        {
            var delayedCompare = Method.DelayedCompare.MakeGenericMethod(type);

            return il.Call(delayedCompare);
        }
    }
}