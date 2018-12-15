using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Variables
{
    internal sealed class HierarchicalFieldVariable : FieldVariable, IHierarchicalAcceptor, IArgumentsVariable
    {
        private HierarchicalFieldVariable(FieldInfo fieldInfo) : base(fieldInfo) { }

        public bool LoadContext => true;

        public ILEmitter LoadVariables(StackEmitter visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMember(this, il, arg);
        }

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMemberAddress(this, il, arg);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareCallVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static HierarchicalFieldVariable Create(MemberInfo memberInfo)
        {
            return memberInfo is FieldInfo info
                   && !info
                       .FieldType
                       .GetUnderlyingType()
                       .IsPrimitive()
                       ? new HierarchicalFieldVariable(info)
                       : null;
        }
    }

    internal sealed class HierarchicalPropertyVariable : PropertyVariable, IHierarchicalAcceptor, IArgumentsVariable
    {
        private HierarchicalPropertyVariable(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public bool LoadContext => true;

        public ILEmitter LoadVariables(StackEmitter visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMember(this, il, arg);
        }

        public ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMemberAddress(this, il, arg);
        }

        public ILEmitter Accept(CompareCallVisitor visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public static HierarchicalPropertyVariable Create(MemberInfo memberInfo)
        {
            return memberInfo is PropertyInfo info
                   && !info
                       .PropertyType
                       .GetUnderlyingType()
                       .IsPrimitive()
                       ? new HierarchicalPropertyVariable(info)
                       : null;
        }
    }
}
