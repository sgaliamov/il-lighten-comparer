﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class ComparableFieldMember : FieldMember, IComparableAcceptor, IComparableMember
    {
        private ComparableFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

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

        public static ComparableFieldMember Create(MemberInfo memberInfo)
        {
            var info = memberInfo as FieldInfo;
            if (info == null)
            {
                return null;
            }

            var underlyingType = info.FieldType.GetUnderlyingType();

            var isComparable = underlyingType.ImplementsGeneric(typeof(IComparable<>), underlyingType);

            return isComparable
                       ? new ComparableFieldMember(info)
                       : null;
        }
    }

    internal sealed class ComparablePropertyMember : PropertyMember, IComparableAcceptor, IComparableMember
    {
        private ComparablePropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

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

        public static ComparablePropertyMember Create(MemberInfo memberInfo)
        {
            var info = memberInfo as PropertyInfo;
            if (info == null)
            {
                return null;
            }

            var underlyingType = info.PropertyType.GetUnderlyingType();

            var isComparable = underlyingType.ImplementsGeneric(typeof(IComparable<>), underlyingType);

            return isComparable
                       ? new ComparablePropertyMember(info)
                       : null;
        }
    }
}
