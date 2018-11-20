using System.Reflection;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static string DisplayName(this MemberInfo memberInfo) =>
            $"{memberInfo.DeclaringType}::{memberInfo.Name}";
    }
}
