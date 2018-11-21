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

            var compareToMethod = GetCompareToMethod(member.MemberType);

            EmitCompareCall(il, compareToMethod);
        }

        public void Visit(ComparableFieldMember member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            var compareToMethod = GetCompareToMethod(member.MemberType);

            EmitCompareCall(il, compareToMethod);
        }

        public void Visit(NestedObject member, ILEmitter il)
        {
            _stackVisitor.Visit(member, il);

            throw new NotImplementedException();
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

            // if(r != 0) return r;
            var gotoNext = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, gotoNext)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Ret)
              .MarkLabel(gotoNext);
        }

        private static MethodInfo GetCompareToMethod(Type type) => type.GetMethod(
            nameof(IComparable.CompareTo),
            new[] { type });
    }
}
