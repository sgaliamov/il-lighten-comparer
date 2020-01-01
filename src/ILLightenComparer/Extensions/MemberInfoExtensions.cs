using System.Reflection;

namespace ILLightenComparer.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static string DisplayName(this MemberInfo memberInfo) {
            var name = $"{memberInfo.MemberType} {memberInfo.DeclaringType}::{memberInfo.Name}";
#if DEBUG
            name = name
                   .Replace("\\, System.Private.CoreLib\\, Version=4.0.0.0\\, Culture=neutral\\, PublicKeyToken=7cec85d7bea7798e", "")
                   .Replace("\\, ILLightenComparer.Tests\\, Version=1.0.0.0\\, Culture=neutral\\, PublicKeyToken=null", "")
                   .Replace("\\", "");
#endif
            return name;
        }
    }
}
