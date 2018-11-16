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
                .Select(memberInfo =>
                {
                    switch (memberInfo)
                    {
                        case FieldInfo field:
                            return (MemberInfo)field;

                        case PropertyInfo property:
                            return (MemberInfo)property;

                        default:
                            throw new NotSupportedException(
                                "Only fields and properties are supported. "
                                + $"{memberInfo.MemberType}: {memberInfo.DeclaringType}.{memberInfo.Name}");
                    }
                })
                .ToArray();

        private static bool Filter(MemberInfo memberInfo, CompareConfiguration configuration) =>
            memberInfo.MemberType == MemberTypes.Property
            || configuration.IncludeFields && memberInfo.MemberType == MemberTypes.Field;
    }
}
