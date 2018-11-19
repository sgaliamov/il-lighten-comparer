using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ILLightenComparer.Reflection
{
    internal sealed class MembersProvider
    {
        public MemberInfo[] GetMembers(Type type, CompareConfiguration configuration) =>
            type.GetMembers(
                    BindingFlags.Instance
                    //| BindingFlags.FlattenHierarchy
                    | BindingFlags.Public)
                .Where(memberInfo => IgnoredMembers(memberInfo, configuration.IgnoredMembers))
                .Where(memberInfo => IncludeFields(memberInfo, configuration.IncludeFields))
                .ToArray();

        private static bool IgnoredMembers(MemberInfo memberInfo, ICollection<string> ignoredMembers) =>
            !ignoredMembers.Contains(memberInfo.Name);

        private static bool IncludeFields(MemberInfo memberInfo, bool includeFields) =>
            memberInfo.MemberType == MemberTypes.Property
            || includeFields && memberInfo.MemberType == MemberTypes.Field;
    }
}
