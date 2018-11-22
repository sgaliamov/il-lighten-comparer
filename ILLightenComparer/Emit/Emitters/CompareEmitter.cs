using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter : IMemvberVisitor
    {
        private readonly MembersStacker _stackVisitor;

        public CompareEmitter(TypeBuilderContext context) => _stackVisitor = new MembersStacker(context);

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

            EmitCompareCall(il, member.CompareToMethod);
        }

        public void Visit(StringPropertyMember member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            EmitCompareCall(il, member.CompareToMethod);
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
            il.Emit(OpCodes.Call, compareToMethod);

            EmitCheckToZero(il);
        }

        private static void EmitCheckToZero(ILEmitter il)
        {
            var gotoNext = il.DefineLabel();

            il.Emit(OpCodes.Stloc_0)
                .Emit(OpCodes.Ldloc_0)
                .Emit(OpCodes.Brfalse_S, gotoNext)
                .Emit(OpCodes.Ldloc_0)
                .Emit(OpCodes.Ret)
                .MarkLabel(gotoNext);
        }
    }
}
