﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class ArrayFieldMember : FieldMember, IArrayAcceptor, IArgumentsMember
    {
        private ArrayFieldMember(FieldInfo fieldInfo) : base(fieldInfo)
        {
            CountMethod = MemberType.GetMethod(MethodName.ArrayLength, Type.EmptyTypes);
            GetItemMethod = MemberType.GetMethod(MethodName.ArrayGet, new[] { typeof(int) });
        }

        public bool LoadContext => false;
        public MethodInfo CountMethod { get; }
        public MethodInfo GetItemMethod { get; }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Load(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMember(this, il, arg);
        }

        public ILEmitter LoadAddress(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMemberAddress(this, il, arg);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareCallVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public static ArrayFieldMember Create(MemberInfo memberInfo)
        {
            var info = memberInfo as FieldInfo;
            if (info == null)
            {
                return null;
            }

            var underlyingType = info.FieldType.GetUnderlyingType();

            return underlyingType.IsArray
                       ? new ArrayFieldMember(info)
                       : null;
        }
    }

    internal sealed class ArrayPropertyMember : PropertyMember, IArrayAcceptor, IArgumentsMember
    {
        private ArrayPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            CountMethod = MemberType.GetMethod(MethodName.ArrayLength, Type.EmptyTypes);
            GetItemMethod = MemberType.GetMethod(MethodName.ArrayGet, new[] { typeof(int) });
        }

        public bool LoadContext => false;
        public MethodInfo CountMethod { get; }
        public MethodInfo GetItemMethod { get; }

        public ILEmitter LoadMembers(StackEmitter visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public ILEmitter Load(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMember(this, il, arg);
        }

        public ILEmitter LoadAddress(MemberLoader visitor, ILEmitter il, ushort arg)
        {
            return visitor.LoadMemberAddress(this, il, arg);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter Accept(CompareCallVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return visitor.Visit(this, il, gotoNext);
        }

        public static ArrayPropertyMember Create(MemberInfo memberInfo)
        {
            var info = memberInfo as PropertyInfo;
            if (info == null)
            {
                return null;
            }

            var underlyingType = info.PropertyType.GetUnderlyingType();

            return underlyingType.IsArray
                       ? new ArrayPropertyMember(info)
                       : null;
        }
    }
}