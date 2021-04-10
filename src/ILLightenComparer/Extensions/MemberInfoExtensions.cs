using System.Reflection;

namespace ILLightenComparer.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static string DisplayName(this MemberInfo memberInfo) =>
            $"{memberInfo.MemberType} {memberInfo.DeclaringType?.Name}::{memberInfo.Name}";
    }
}
