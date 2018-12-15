using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Visitors.Comparisons;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MembersProvider
    {
        private readonly ComparerContext _context;
        private readonly MemberConverter _converter = new MemberConverter();

        public MembersProvider(ComparerContext context)
        {
            _context = context;
        }

        public IComparison[] GetMembers(Type type)
        {
            var filtered = Filter(type);
            var sorted = Sort(type, filtered);
            var converted = Convert(sorted);

            return converted;
        }

        private IComparison[] Convert(IEnumerable<MemberInfo> members)
        {
            return members.Select(_converter.Convert).ToArray();
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
