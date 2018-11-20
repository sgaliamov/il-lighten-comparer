using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class MemberExtensions
    {
        public static string DisplayName(this Member memberInfo) =>
            $"{memberInfo.OwnerType}.{memberInfo.Name}";
    }
}
