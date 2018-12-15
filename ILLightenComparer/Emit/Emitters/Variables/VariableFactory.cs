using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Variables
{
    internal static class VariableFactory
    {
        public static IVariable Create(MemberInfo memberInfo)
        {
            return PropertyVariable.Create(memberInfo)
                   ?? FieldVariable.Create(memberInfo);
        }
    }
}
