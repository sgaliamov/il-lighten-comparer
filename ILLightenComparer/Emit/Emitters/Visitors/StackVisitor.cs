using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Comparisons;
using ILLightenComparer.Emit.Emitters.Variables;
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

        public ILEmitter LoadVariables(HierarchicalComparison comparison, ILEmitter il, Label gotoNextMember)
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

        public ILEmitter LoadVariables(ComparableComparison comparison, ILEmitter il, Label gotoNextMember)
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

        public ILEmitter LoadVariables(IStaticComparison comparison, ILEmitter il, Label gotoNextMember)
        {
            var variable = comparison.Variable;
            if (variable.VariableType.IsNullable())
            {
                return LoadNullableMembers(il, false, false, variable, gotoNextMember);
            }

            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il;
        }

        public void LoadVariables(
            CollectionComparison comparison,
            ILEmitter il,
            LocalBuilder x,
            LocalBuilder y,
            LocalBuilder index,
            Label gotoNextMember)
        {
            if (comparison.Variable.VariableType.IsNullable())
            {
                EmitLoadNullableValues(il, comparison, x, y, index, gotoNextMember);
            }
            else
            {
                EmitLoadValues(il, comparison, x, y, index);
            }
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

        private static void EmitLoadValues(
            ILEmitter il,
            CollectionComparison member,
            LocalBuilder x,
            LocalBuilder y,
            LocalBuilder index)
        {
            var elementType = member.ElementType;
            var underlyingType = elementType.GetUnderlyingType();
            var comparisonType = elementType.GetComparisonType();

            if (comparisonType == ComparisonType.Hierarchicals)
            {
                il.LoadArgument(Arg.Context);
            }

            il.LoadLocal(x)
              .LoadLocal(index)
              .Call(member.GetItemMethod);
            if (comparisonType == ComparisonType.Comparables)
            {
                il.Store(underlyingType, Arg.X, out var item)
                  .LoadAddress(item);
            }

            il.LoadLocal(y)
              .LoadLocal(index)
              .Call(member.GetItemMethod);

            if (comparisonType == ComparisonType.Hierarchicals)
            {
                il.LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);
            }
        }

        private static void EmitLoadNullableValues(
            ILEmitter il,
            CollectionComparison member,
            LocalBuilder x,
            LocalBuilder y,
            LocalBuilder index,
            Label gotoNextMember)
        {
            var elementType = member.ElementType;
            var underlyingType = elementType.GetUnderlyingType();
            var comparisonType = elementType.GetComparisonType();

            il.LoadLocal(x)
              .LoadLocal(index)
              .Call(member.GetItemMethod)
              .Store(elementType, Arg.X, out var nullableX);
            il.LoadLocal(y)
              .LoadLocal(index)
              .Call(member.GetItemMethod)
              .Store(elementType, Arg.X, out var nullableY);

            il.CheckNullableValuesForNull(nullableX, nullableY, elementType, gotoNextMember);

            if (comparisonType == ComparisonType.Hierarchicals)
            {
                il.LoadArgument(Arg.Context);
            }

            var getValueMethod = elementType.GetPropertyGetter(MethodName.Value);
            il.LoadAddress(nullableX).Call(getValueMethod);
            if (comparisonType == ComparisonType.Comparables)
            {
                il.Store(underlyingType, out var address).LoadAddress(address);
            }

            il.LoadAddress(nullableY).Call(getValueMethod);

            if (comparisonType == ComparisonType.Hierarchicals)
            {
                il.LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);
            }
        }
    }
}
