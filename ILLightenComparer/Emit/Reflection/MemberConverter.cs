using System;
using System.Reflection;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Reflection
{
    internal sealed class MemberConverter
    {
        public Member Convert(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    return Convert(field);

                case PropertyInfo property:
                    return Convert(property);
            }

            throw new NotSupportedException(
                "Only fields and properties are supported. "
                + $"{memberInfo.MemberType}: {memberInfo.DisplayName()}");
        }

        private static Member Convert(PropertyInfo property)
        {
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

        private static Member Convert(FieldInfo field)
        {
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
        }

        private static Type GetUnderlyingType(Type type) =>
            type.IsEnum
                ? Enum.GetUnderlyingType(type)
                : type;
    }
}
