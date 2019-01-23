using System;
using System.Reflection;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.v2.Variables
{
    internal static class VariableFactory
    {
        public static IVariable Create(MemberInfo memberInfo)
        {
            return PropertyVariable.Create(memberInfo)
                   ?? FieldVariable.Create(memberInfo)
                   ?? throw new ArgumentException(
                       $"Can't create factory from {memberInfo.DisplayName()}",
                       nameof(memberInfo));
        }
    }
}
