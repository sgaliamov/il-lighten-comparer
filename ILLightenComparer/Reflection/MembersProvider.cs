using System;
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
                .Where(memberInfo => Filter(memberInfo, configuration))
                .ToArray();

        private static bool Filter(MemberInfo memberInfo, CompareConfiguration configuration) =>
            memberInfo.MemberType == MemberTypes.Property
            || configuration.IncludeFields && memberInfo.MemberType == MemberTypes.Field;
    }
}
