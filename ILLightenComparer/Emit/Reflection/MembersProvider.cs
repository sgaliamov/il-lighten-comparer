using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Comparisons;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MembersProvider
    {
        private readonly ComparerContext _context;
        private readonly Converter _converter;

        public MembersProvider(ComparerContext context, Converter converter)
        {
            _context = context;
            _converter = converter;
        }

        public ICompareEmitterAcceptor[] GetMembers(Type type)
        {
            var filtered = Filter(type);
            var sorted = Sort(type, filtered);

            return Convert(sorted);
        }

        private ICompareEmitterAcceptor[] Convert(IEnumerable<MemberInfo> members)
        {
            return members.Select(_converter.CreateMemberComparison).ToArray();
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
            var order = _context.GetConfiguration(ownerType).MembersOrder;

            if (order == null || order.Length == 0)
            {
                return DefaultOrder(members);
            }

            return PredefinedOrder(ownerType, members);
        }

        private IEnumerable<MemberInfo> PredefinedOrder(Type ownerType, IEnumerable<MemberInfo> members)
        {
            var order = _context.GetConfiguration(ownerType).MembersOrder;
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

        private static IEnumerable<MemberInfo> DefaultOrder(IEnumerable<MemberInfo> members)
        {
            return members
                   .OrderBy(x => x.DeclaringType?.FullName ?? string.Empty)
                   .ThenBy(x => x.MemberType)
                   .ThenBy(x => x.Name);
        }

        private bool IncludeFields(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                return true;
            }

            var includeFields = _context.GetConfiguration(memberInfo.DeclaringType).IncludeFields;

            return includeFields && memberInfo.MemberType == MemberTypes.Field;
        }

        private bool IgnoredMembers(MemberInfo memberInfo)
        {
            var ignoredMembers = _context.GetConfiguration(memberInfo.DeclaringType).IgnoredMembers;

            return !ignoredMembers.Contains(memberInfo.Name);
        }
    }
}
