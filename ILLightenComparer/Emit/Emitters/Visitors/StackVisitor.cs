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
                    variable,
                    false,
                    true,
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
                    return LoadNullableMembers(il, variable, true, false, gotoNextMember);
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
                               .Branch(OpCodes.Brfalse_S, gotoNextMember)
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
                return LoadNullableMembers(il, variable, false, false, gotoNextMember);
            }

            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il;
        }

        public ILEmitter LoadVariables(ArrayItemComparison comparison, ILEmitter il, Label gotoNextMember)
        {
            return comparison.ItemAcceptor.LoadVariables(this, il, gotoNextMember);
        }

        private ILEmitter LoadNullableMembers(
            ILEmitter il,
            IVariable variable,
            bool callable,
            bool loadContext,
            Label gotoNextMember)
        {
            var variableType = variable.VariableType;

            variable.Load(_loader, il, Arg.X).Store(variableType, 0, out var nullableX);
            variable.Load(_loader, il, Arg.Y).Store(variableType, 1, out var nullableY);

            var getValueMethod = variableType.GetPropertyGetter(MethodName.Value);
            var underlyingType = variableType.GetUnderlyingType();

            il.CheckNullableValuesForNull(nullableX, nullableY, variableType, gotoNextMember);

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

        //private static void EmitLoadValues(
        //    ILEmitter il,
        //    ArrayItemComparison member,
        //    LocalBuilder x,
        //    LocalBuilder y,
        //    LocalBuilder index)
        //{
        //    var elementType = member.ElementType;
        //    var underlyingType = elementType.GetUnderlyingType();
        //    var comparisonType = elementType.GetComparisonType();

        //    if (comparisonType == ComparisonType.Hierarchicals)
        //    {
        //        il.LoadArgument(Arg.Context);
        //    }

        //    il.LoadLocal(x)
        //      .LoadLocal(index)
        //      .Call(member.GetItemMethod);
        //    if (comparisonType == ComparisonType.Comparables)
        //    {
        //        il.Store(underlyingType, Arg.X, out var item)
        //          .LoadAddress(item);
        //    }

        //    il.LoadLocal(y)
        //      .LoadLocal(index)
        //      .Call(member.GetItemMethod);

        //    if (comparisonType == ComparisonType.Hierarchicals)
        //    {
        //        il.LoadArgument(Arg.SetX)
        //          .LoadArgument(Arg.SetY);
        //    }
        //}

        //private static void EmitLoadNullableValues(
        //    ILEmitter il,
        //    CollectionComparison comparison,
        //    LocalBuilder x,
        //    LocalBuilder y,
        //    LocalBuilder index,
        //    Label gotoNextMember)
        //{
        //    var elementType = comparison.ElementType;
        //    var underlyingType = elementType.GetUnderlyingType();
        //    var comparisonType = elementType.GetComparisonType();

        //    il.LoadLocal(x)
        //      .LoadLocal(index)
        //      .Call(comparison.GetItemMethod)
        //      .Store(elementType, Arg.X, out var nullableX);
        //    il.LoadLocal(y)
        //      .LoadLocal(index)
        //      .Call(comparison.GetItemMethod)
        //      .Store(elementType, Arg.X, out var nullableY);

        //    il.CheckNullableValuesForNull(nullableX, nullableY, elementType, gotoNextMember);

        //    if (comparisonType == ComparisonType.Hierarchicals)
        //    {
        //        il.LoadArgument(Arg.Context);
        //    }

        //    var getValueMethod = elementType.GetPropertyGetter(MethodName.Value);
        //    il.LoadAddress(nullableX).Call(getValueMethod);
        //    if (comparisonType == ComparisonType.Comparables)
        //    {
        //        il.Store(underlyingType, out var address).LoadAddress(address);
        //    }

        //    il.LoadAddress(nullableY).Call(getValueMethod);

        //    if (comparisonType == ComparisonType.Hierarchicals)
        //    {
        //        il.LoadArgument(Arg.SetX)
        //          .LoadArgument(Arg.SetY);
        //    }
        //}
    }
}
