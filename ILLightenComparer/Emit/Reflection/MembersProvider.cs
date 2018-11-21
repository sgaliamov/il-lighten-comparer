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
                    //| BindingFlags.FlattenHierarchy
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

                    var fieldCompareToMethod = GetCompareToMethod(field.FieldType);
                    return new ComparableField
                    {
                        MemberType = GetUnderlyingType(field.FieldType),
                        Name = field.Name,
                        OwnerType = field.DeclaringType,
                        CompareToMethod = fieldCompareToMethod,
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

                    var propertyCompareToMethod = GetCompareToMethod(property.PropertyType);
                    return new ComparableProperty
                    {
                        MemberType = GetUnderlyingType(property.PropertyType),
                        Name = property.Name,
                        GetterMethod = property.GetGetMethod(),
                        OwnerType = property.DeclaringType,
                        CompareToMethod = propertyCompareToMethod
                    };
            }

            throw new NotSupportedException(
                "Only fields and properties are supported. "
                + $"{memberInfo.MemberType}: {memberInfo.DisplayName()}");
        }

        private static MethodInfo GetCompareToMethod(Type type)
        {
            type = GetUnderlyingType(type);

            return type.GetMethod(
                nameof(IComparable.CompareTo),
                new[] { type });
        }

        private static Type GetUnderlyingType(Type type)
        {
            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            return type;
        }

        private static bool IgnoredMembers(MemberInfo memberInfo, ICollection<string> ignoredMembers) =>
            !ignoredMembers.Contains(memberInfo.Name);

        private static bool IncludeFields(MemberInfo memberInfo, bool includeFields) =>
            memberInfo.MemberType == MemberTypes.Property
            || includeFields && memberInfo.MemberType == MemberTypes.Field;
    }
}
