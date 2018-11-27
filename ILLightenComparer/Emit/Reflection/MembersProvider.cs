using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Acceptors;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MembersProvider
    {
        private readonly TypeBuilderContext _context;
        private readonly MemberConverter _converter;

        public MembersProvider(TypeBuilderContext context)
        {
            _context = context;
            _converter = new MemberConverter(_context);
        }

        public IAcceptor[] GetMembers(Type type) => Convert(Sort(Filter(type)));

        private IAcceptor[] Convert(IEnumerable<MemberInfo> members) =>
            members.Select(_converter.Convert).ToArray();

        private IEnumerable<MemberInfo> Filter(IReflect type) =>
            type.GetMembers(BindingFlags.Instance
                            | BindingFlags.FlattenHierarchy
                            | BindingFlags.Public)
                .Where(IgnoredMembers)
                .Where(IncludeFields);

        private IEnumerable<MemberInfo> Sort(IEnumerable<MemberInfo> members)
        {
            var order = _context.Configuration.MembersOrder;

            if (order == null || order.Length == 0)
            {
                return DefaultOrder(members);
            }

            return PredefinedOrder(members);
        }

        private IEnumerable<MemberInfo> PredefinedOrder(IEnumerable<MemberInfo> members)
        {
            var order = _context.Configuration.MembersOrder;
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

        private bool IncludeFields(MemberInfo memberInfo) =>
            memberInfo.MemberType == MemberTypes.Property
            || _context.Configuration.IncludeFields && memberInfo.MemberType == MemberTypes.Field;

        private bool IgnoredMembers(MemberInfo memberInfo) =>
            !_context.Configuration.IgnoredMembers.Contains(memberInfo.Name);
    }
}
