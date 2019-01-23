using System.Reflection.Emit;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Comparisons;

namespace ILLightenComparer.Emit.v2.Visitors
{
    internal sealed class StackVisitor
    {
        private readonly VariableLoader _loader;

        public StackVisitor(VariableLoader loader)
        {
            _loader = loader;
        }

        public ILEmitter LoadVariables(HierarchicalComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;

            il.LoadArgument(Arg.Context);
            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il.LoadArgument(Arg.SetX)
                     .LoadArgument(Arg.SetY);
        }

        public ILEmitter LoadVariables(ComparableComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;
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

        public ILEmitter LoadVariables(IStaticComparison comparison, ILEmitter il, Label gotoNext)
        {
            var variable = comparison.Variable;

            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            return il;
        }
    }
}
