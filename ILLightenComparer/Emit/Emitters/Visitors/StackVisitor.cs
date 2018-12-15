﻿using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors.Comparisons;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors
{
    internal sealed class StackVisitor
    {
        private readonly VariableLoader _loader;

        public StackVisitor(VariableLoader loader)
        {
            _loader = loader;
        }

        public ILEmitter Visit(IHierarchicalComparison comparison, ILEmitter il, Label gotoNextMember)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;
            if (variableType.IsNullable())
            {
                return LoadNullableMembers(
                    il,
                    false,
                    true,
                    variable,
                    gotoNextMember);
            }

            il.LoadArgument(Arg.Context);
            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il.LoadArgument(Arg.SetX)
                     .LoadArgument(Arg.SetY);
        }

        public ILEmitter Visit(IComparableComparison comparison, ILEmitter il, Label gotoNextMember)
        {
            var variable = comparison.Variable;
            var variableType = variable.VariableType;
            var underlyingType = variableType.GetUnderlyingType();
            if (underlyingType.IsValueType)
            {
                if (variableType.IsNullable())
                {
                    return LoadNullableMembers(il, true, false, variable, gotoNextMember);
                }

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
                               .Emit(OpCodes.Brfalse_S, gotoNextMember)
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

        public ILEmitter Visit(ICollectionComparison variable, ILEmitter il, Label gotoNextMember)
        {
            throw new NotImplementedException();
        }

        public ILEmitter Visit(IStaticComparison comparison, ILEmitter il, Label gotoNextMember)
        {
            throw new NotImplementedException();
        }

        private ILEmitter LoadNullableMembers(
            ILEmitter il,
            bool callable,
            bool loadContext,
            IVariable variable,
            Label gotoNextMember)
        {
            var memberType = variable.VariableType;

            variable.Load(_loader, il, Arg.X).Store(memberType, 0, out var nullableX);
            variable.Load(_loader, il, Arg.Y).Store(memberType, 1, out var nullableY);

            var getValueMethod = memberType.GetPropertyGetter(MethodName.Value);
            var underlyingType = memberType.GetUnderlyingType();

            il.CheckNullableValuesForNull(nullableX, nullableY, memberType, gotoNextMember);

            if (loadContext)
            {
                il.LoadArgument(Arg.Context);
            }

            il.LoadAddress(nullableX).Call(getValueMethod);

            if (callable)
            {
                il.Store(underlyingType, out var x).LoadAddress(x);
            }

            il.LoadAddress(nullableY).Call(getValueMethod);

            if (loadContext)
            {
                il.LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);
            }

            return il;
        }
    }
}