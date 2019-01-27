using System;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Shared;
using ILLightenComparer.Emit.v2.Variables;
using ILLightenComparer.Emit.v2.Visitors;
using ILLightenComparer.Emit.v2.Visitors.Collection;

namespace ILLightenComparer.Emit.v2
{
    internal sealed class CompareEmitter
    {
        private readonly CompareVisitor _compareVisitor;
        private readonly Converter _converter = new Converter();
        private readonly VariableLoader _loader = new VariableLoader();

        public CompareEmitter(ComparerContext context)
        {
            _compareVisitor = new CompareVisitor(context);
        }

        public void Emit(Type objectType, ILEmitter il)
        {
            InitFirstLocalToKeepComparisonsResult(il);

            var comparison = _converter.CreateComparison(new ArgumentVariable(objectType));

            il.DefineLabel(out var exit);

            comparison.Accept(_compareVisitor, il, exit)
                      .EmitReturnNotZero(exit)
                      .MarkLabel(exit)
                      .Return(0);
        }

        // todo: refactor
        private static void InitFirstLocalToKeepComparisonsResult(ILEmitter il)
        {
            il.DeclareLocal(typeof(int), 0, out _);
        }
    }
}
