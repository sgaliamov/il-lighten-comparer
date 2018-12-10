using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class ObjectWalker
    {
        private readonly HashSet<Member> _cache = new HashSet<Member>();
        private readonly Stack<Member> _toWalk = new Stack<Member>();

        public ObjectWalker(Member root)
        {
            Schedule(root);
        }

        public Member Current { get; private set; }

        public ObjectWalker GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (_toWalk.Count == 0)
            {
                return false;
            }

            Current = _toWalk.Pop();
            if (Current.Value == null || Current.ValueType.IsPrimitive() || Current.ValueType.IsNullable())
            {
                return true;
            }

            var members = Current
                          .ValueType
                          .GetMembers(BindingFlags.Instance
                                      | BindingFlags.FlattenHierarchy
                                      | BindingFlags.Public)
                          .Where(x => x.MemberType == MemberTypes.Property
                                      || x.MemberType == MemberTypes.Field);

            foreach (var info in members)
            {
                switch (info)
                {
                    case FieldInfo fieldInfo:
                        Schedule(new Member(
                            Current.Value,
                            fieldInfo,
                            fieldInfo.FieldType,
                            fieldInfo.GetValue(Current.Value)));
                        break;

                    case PropertyInfo propertyInfo:
                        Schedule(new Member(
                            Current.Value,
                            propertyInfo,
                            propertyInfo.PropertyType,
                            propertyInfo.GetValue(Current.Value)));
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            return true;
        }

        private void Schedule(Member toSchedule)
        {
            if (_cache.Contains(toSchedule))
            {
                return;
            }

            _cache.Add(toSchedule);

            if (toSchedule.ValueType.IsAssignableFrom(typeof(IEnumerable)))
            {
                throw new NotImplementedException();
                //uint index = 0;
                //foreach (var item in (IEnumerable)value)
                //{
                //    Schedule(new Member(value, item, index++));
                //}
            }

            _toWalk.Push(toSchedule);
        }
    }

    internal sealed class Member
    {
        public Member(object parent, MemberInfo memberInfo, Type valueType, object value)
        {
            Parent = parent;
            MemberInfo = memberInfo;
            ValueType = valueType;
            Value = value;
        }

        public Member(object value)
            : this(null, null, value.GetType(), value) { }

        //public Member(object parent, Type valueType, object value, uint index)
        //    : this(parent, null, valueType, value) => Index = index;

        public uint? Index { get; }

        //public bool IsCollection => Index.HasValue;
        public MemberInfo MemberInfo { get; }
        public object Parent { get; }
        public object Value { get; }
        public Type ValueType { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is Member other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Index.GetHashCode();
                hashCode = (hashCode * 397) ^ (MemberInfo != null ? MemberInfo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Parent != null ? Parent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool Equals(Member other)
        {
            return Index == other.Index
                   && Equals(MemberInfo, other.MemberInfo)
                   && Equals(Parent, other.Parent)
                   && Equals(Value, other.Value);
        }
    }
}
