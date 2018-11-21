using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Members;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Visitors
{
    internal sealed class CompareEmitVisitor : IVisitor
    {
        private static readonly MethodInfo StringCompareMethod = typeof(string)
            .GetMethod(
                nameof(string.Compare),
                new[] { typeof(string), typeof(string), typeof(StringComparison) });

        private readonly PushToStackVisitor _stackVisitor;

        public CompareEmitVisitor(TypeBuilderContext context) => _stackVisitor = new PushToStackVisitor(context);

        public void Visit(ComparablePropertyMember member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            EmitCompareCall(il, member.CompareToMethod);
        }

        public void Visit(ComparableFieldMember member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            EmitCompareCall(il, member.CompareToMethod);
        }

        public void Visit(StringFiledMember member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            EmitCompareCall(il, StringCompareMethod);
        }

        public void Visit(StringPropertyMember member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            EmitCompareCall(il, StringCompareMethod);
        }

        public void Visit(IntegralFiledMember member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            il.Emit(OpCodes.Sub);

            EmitCheckToZero(il);
        }

        public void Visit(IntegralPropertyMember member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            il.Emit(OpCodes.Sub);

            EmitCheckToZero(il);
        }

        /// <summary>
        ///     Emits call to a compare method of a basic type.
        ///     Expects correct arguments in the stack.
        ///     Push result into Ldloc_0.
        /// </summary>
        private static void EmitCompareCall(ILEmitter il, MethodInfo compareToMethod)
        {
            il.Emit(OpCodes.Call, compareToMethod)
              .Emit(OpCodes.Stloc_0)
              .Emit(OpCodes.Ldloc_0);

            EmitCheckToZero(il);
        }

        private static void EmitCheckToZero(ILEmitter il)
        {
            var gotoNext = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, gotoNext)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Ret)
              .MarkLabel(gotoNext);
        }
    }
}
