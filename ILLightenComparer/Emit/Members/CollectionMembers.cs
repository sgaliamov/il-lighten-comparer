using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class CollectionFieldMember : FieldMember, ICollectionAcceptor, IArgumentsMember
    {
        private CollectionFieldMember(FieldInfo fieldInfo) : base(fieldInfo) { }

        public bool LoadContext => true;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il)
        {
            return visitor.Visit(this, il, gotoNextMember);
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

        public static CollectionFieldMember Create(MemberInfo memberInfo)
        {
            var info = memberInfo as FieldInfo;
            if (info == null)
            {
                return null;
            }

            var underlyingType = info.FieldType.GetUnderlyingType();

            var isCollection = underlyingType.ImplementsGeneric(typeof(ICollection<>), underlyingType);

            return isCollection
                       ? new CollectionFieldMember(info)
                       : null;
        }
    }

    internal sealed class CollectionPropertyMember : PropertyMember, ICollectionAcceptor, IArgumentsMember
    {
        private CollectionPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo) { }

        public bool LoadContext => true;

        public ILEmitter LoadMembers(StackEmitter visitor, Label gotoNextMember, ILEmitter il)
        {
            return visitor.Visit(this, il, gotoNextMember);
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

        public static CollectionPropertyMember Create(MemberInfo memberInfo)
        {
            var info = memberInfo as PropertyInfo;
            if (info == null)
            {
                return null;
            }

            var underlyingType = info.PropertyType.GetUnderlyingType();

            var isCollection = underlyingType.ImplementsGeneric(typeof(ICollection<>), underlyingType);

            return isCollection
                       ? new CollectionPropertyMember(info)
                       : null;
        }
    }
}
