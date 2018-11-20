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
                .OrderBy(x => x.Name)
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

                    var compareToMethod1 = field.FieldType.GetMethod(
                        nameof(IComparable.CompareTo),
                        new[] { field.FieldType });

                    return new ComparableField
                    {
                        MemberType = field.FieldType,
                        Name = field.Name,
                        OwnerType = field.DeclaringType,
                        CompareToMethod = compareToMethod1,
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

                    var compareToMethod = property.PropertyType.GetMethod(
                        nameof(IComparable.CompareTo),
                        new[] { property.PropertyType });

                    if (compareToMethod != null)
                    {
                        return new ComparableProperty
                        {
                            MemberType = property.PropertyType,
                            Name = property.Name,
                            GetterMethod = property.GetGetMethod(),
                            OwnerType = property.DeclaringType,
                            CompareToMethod = compareToMethod
                        };
                    }

                    return null;
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
