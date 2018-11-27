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

        public IAcceptor[] GetMembers(Type type) =>
            type.GetMembers(
                    BindingFlags.Instance
                    | BindingFlags.FlattenHierarchy
                    | BindingFlags.Public)
                .Where(memberInfo => IgnoredMembers(memberInfo, _context.Configuration.IgnoredMembers))
                .Where(memberInfo => IncludeFields(memberInfo, _context.Configuration.IncludeFields))
                .OrderBy(x => x.MemberType) // todo: use functor from settings
                .ThenBy(x => x.Name)
                .Select(_converter.Convert)
                .ToArray();

        private static bool IgnoredMembers(MemberInfo memberInfo, ICollection<string> ignoredMembers) =>
            !ignoredMembers.Contains(memberInfo.Name);

        private static bool IncludeFields(MemberInfo memberInfo, bool includeFields) =>
            memberInfo.MemberType == MemberTypes.Property
            || includeFields && memberInfo.MemberType == MemberTypes.Field;
    }
}
