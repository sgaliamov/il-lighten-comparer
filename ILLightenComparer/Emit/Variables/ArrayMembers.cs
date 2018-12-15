using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Variables
{
    internal sealed class ArrayFieldVariable : FieldVariable, IArrayAcceptor, IArgumentVariable
    {
        private ArrayFieldVariable(FieldInfo fieldInfo) : base(fieldInfo)
        {
            GetLengthMethod = VariableType.GetPropertyGetter(MethodName.ArrayLength);
            GetItemMethod = VariableType.GetMethod(MethodName.ArrayGet, new[] { typeof(int) });
            ElementType = VariableType.GetUnderlyingType().GetElementType();
        }

        public bool LoadContext => false;
        public MethodInfo GetLengthMethod { get; }
        public Type ElementType { get; }
        public MethodInfo GetItemMethod { get; }

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

        public static ArrayFieldVariable Create(MemberInfo memberInfo)
        {
            var info = memberInfo as FieldInfo;
            if (info == null)
            {
                return null;
            }

            var underlyingType = info.FieldType.GetUnderlyingType();
            if (underlyingType.IsArray && underlyingType.GetArrayRank() == 1)
            {
                return new ArrayFieldVariable(info);
            }

            return null;
        }
    }

    internal sealed class ArrayPropertyVariable : PropertyVariable, IArrayAcceptor, IArgumentVariable
    {
        private ArrayPropertyVariable(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            GetLengthMethod = VariableType.GetPropertyGetter(MethodName.ArrayLength);
            GetItemMethod = VariableType.GetMethod(MethodName.ArrayGet, new[] { typeof(int) });
            ElementType = VariableType.GetUnderlyingType().GetElementType();
        }

        public bool LoadContext => false;
        public MethodInfo GetLengthMethod { get; }
        public Type ElementType { get; }
        public MethodInfo GetItemMethod { get; }

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

        public static ArrayPropertyVariable Create(MemberInfo memberInfo)
        {
            var info = memberInfo as PropertyInfo;
            if (info == null)
            {
                return null;
            }

            var underlyingType = info.PropertyType.GetUnderlyingType();
            if (underlyingType.IsArray && underlyingType.GetArrayRank() == 1)
            {
                return new ArrayPropertyVariable(info);
            }

            return null;
        }
    }
}
