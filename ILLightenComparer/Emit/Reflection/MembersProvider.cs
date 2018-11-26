using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MembersProvider
    {
        private readonly MemberConverter _converter = new MemberConverter();

        public IMember[] GetMembers(Type type, CompareConfiguration configuration) =>
            type.GetMembers(
                    BindingFlags.Instance
                    | BindingFlags.FlattenHierarchy
                    | BindingFlags.Public)
                .Where(memberInfo => IgnoredMembers(memberInfo, configuration.IgnoredMembers))
                .Where(memberInfo => IncludeFields(memberInfo, configuration.IncludeFields))
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
