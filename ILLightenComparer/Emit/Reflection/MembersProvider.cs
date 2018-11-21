using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MembersProvider
    {
        public Member[] GetMembers(Type type, CompareConfiguration configuration) =>
            type.GetMembers(
                    BindingFlags.Instance
                    | BindingFlags.FlattenHierarchy
                    | BindingFlags.Public)
                .Where(memberInfo => IgnoredMembers(memberInfo, configuration.IgnoredMembers))
                .Where(memberInfo => IncludeFields(memberInfo, configuration.IncludeFields))
                .OrderBy(x => x.MemberType) // todo: use functor from settings
                .ThenBy(x => x.Name)
                .Select(Convert)
                .ToArray();

        private static Member Convert(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    if (field.FieldType == typeof(string))
                    {
                        return new StringFiledMember
                        {
                            MemberType = field.FieldType,
                            Name = field.Name,
                            OwnerType = field.DeclaringType,
                            FieldInfo = field
                        };
                    }

                    return new FieldMember
                    {
                        MemberType = GetUnderlyingType(field.FieldType),
                        Name = field.Name,
                        OwnerType = field.DeclaringType,
                        FieldInfo = field
                    };

                case PropertyInfo property:
                    if (property.PropertyType == typeof(string))
                    {
                        return new StringPropertyMember
                        {
                            MemberType = property.PropertyType,
                            Name = property.Name,
                            OwnerType = property.DeclaringType,
                            GetterMethod = property.GetMethod
                        };
                    }

                    return new PropertyMember
                    {
                        MemberType = GetUnderlyingType(property.PropertyType),
                        Name = property.Name,
                        GetterMethod = property.GetGetMethod(),
                        OwnerType = property.DeclaringType
                    };
            }

            throw new NotSupportedException(
                "Only fields and properties are supported. "
                + $"{memberInfo.MemberType}: {memberInfo.DisplayName()}");
        }

        private static Type GetUnderlyingType(Type type) =>
            type.IsEnum
                ? Enum.GetUnderlyingType(type)
                : type;

        private static bool IgnoredMembers(MemberInfo memberInfo, ICollection<string> ignoredMembers) =>
            !ignoredMembers.Contains(memberInfo.Name);

        private static bool IncludeFields(MemberInfo memberInfo, bool includeFields) =>
            memberInfo.MemberType == MemberTypes.Property
            || includeFields && memberInfo.MemberType == MemberTypes.Field;
    }
}
