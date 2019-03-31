using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Config;
using ILLightenComparer.Emitters.Variables;
using ILLightenComparer.Extensions;

namespace ILLightenComparer.Reflection
{
    internal sealed class MembersProvider
    {
        private readonly IConfigurationProvider _configurations;

        public MembersProvider(IConfigurationProvider configurations)
        {
            _configurations = configurations;
        }

        public IVariable[] GetMembers(Type type)
        {
            var filtered = Filter(type);
            var sorted = Sort(type, filtered);

            return Convert(sorted);
        }

        private IEnumerable<MemberInfo> Filter(IReflect type)
        {
            return type.GetMembers(BindingFlags.Instance
                                   | BindingFlags.FlattenHierarchy
                                   | BindingFlags.Public)
                       .Where(IncludeFields)
                       .Where(IgnoredMembers);
        }

        private IEnumerable<MemberInfo> Sort(Type ownerType, IEnumerable<MemberInfo> members)
        {
            var order = _configurations.Get(ownerType).MembersOrder;

            if (order == null || order.Length == 0)
            {
                return DefaultOrder(members);
            }

            return PredefinedOrder(ownerType, members);
        }

        private IEnumerable<MemberInfo> PredefinedOrder(Type ownerType, IEnumerable<MemberInfo> members)
        {
            var order = _configurations.Get(ownerType).MembersOrder;
            var dictionary = members.ToDictionary(x => x.Name);

            foreach (var item in order)
            {
                if (!dictionary.TryGetValue(item, out var memberInfo))
                {
                    continue;
                }

                dictionary.Remove(item);

                yield return memberInfo;
            }

            foreach (var item in DefaultOrder(dictionary.Values))
            {
                yield return item;
            }
        }

        private bool IncludeFields(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                return true;
            }

            var includeFields = _configurations.Get(memberInfo.DeclaringType).IncludeFields;

            return includeFields && memberInfo.MemberType == MemberTypes.Field;
        }

        private bool IgnoredMembers(MemberInfo memberInfo)
        {
            var ignoredMembers = _configurations.Get(memberInfo.DeclaringType).IgnoredMembers;

            return !ignoredMembers.Contains(memberInfo.Name);
        }

        private static IVariable Create(MemberInfo memberInfo)
        {
            return PropertyMemberVariable.Create(memberInfo)
                   ?? FieldMemberVariable.Create(memberInfo)
                   ?? throw new ArgumentException(
                       $"Can't create member variable from {memberInfo.DisplayName()}",
                       nameof(memberInfo));
        }

        private static IVariable[] Convert(IEnumerable<MemberInfo> members)
        {
            return members.Select(Create).ToArray();
        }

        private static IEnumerable<MemberInfo> DefaultOrder(IEnumerable<MemberInfo> members)
        {
            return members
                   .OrderBy(x => x.DeclaringType?.FullName ?? string.Empty)
                   .ThenBy(x => x.MemberType)
                   .ThenBy(x => x.Name);
        }
    }
}
