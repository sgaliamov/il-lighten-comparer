using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Reflection.Extensions;
using ILLightenComparer.Reflection.Members;

namespace ILLightenComparer.Shared
{
    internal sealed class MembersProvider
    {
        public Member[] GetMembers(Type type, CompareConfiguration configuration) =>
            type.GetMembers(
                    BindingFlags.Instance
                    //| BindingFlags.FlattenHierarchy
                    | BindingFlags.Public)
                .Where(memberInfo => IgnoredMembers(memberInfo, configuration.IgnoredMembers))
                .Where(memberInfo => IncludeFields(memberInfo, configuration.IncludeFields))
                .Select(Convert)
                .ToArray();

        private static Member Convert(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    break;

                case PropertyInfo property:
                    var compareToMethod = property.PropertyType.GetMethod(
                        nameof(IComparable.CompareTo),
                        new[] { property.PropertyType });

                    if (compareToMethod != null)
                    {
                        return new ComparableProperty
                        {
                            PropertyType = property.PropertyType,
                            Name = property.Name,
                            GetMethod = property.GetGetMethod(),
                            DeclaringType = property.DeclaringType,
                            CompareToMethod = compareToMethod
                        };
                    }

                    break;
            }

            throw new NotSupportedException(
                "Only fields and properties are supported. "
                + $"{memberInfo.MemberType}: {memberInfo.DisplayName()}");
        }

        private static bool IgnoredMembers(MemberInfo memberInfo, ICollection<string> ignoredMembers) =>
            !ignoredMembers.Contains(memberInfo.Name);

        private static bool IncludeFields(MemberInfo memberInfo, bool includeFields) =>
            memberInfo.MemberType == MemberTypes.Property
            || includeFields && memberInfo.MemberType == MemberTypes.Field;
    }
}
