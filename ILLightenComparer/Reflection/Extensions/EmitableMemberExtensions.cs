namespace ILLightenComparer.Reflection.Extensions
{
    internal static class EmitableMemberExtensions
    {
        public static string DisplayName(this EmitableMember memberInfo) =>
            $"{memberInfo.DeclaringType}.{memberInfo.Name}";
    }
}
