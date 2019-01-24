using System;
using System.Reflection;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.v2.Variables.Members
{
    internal static class MemberVariableFactory
    {
        public static IVariable Create(MemberInfo memberInfo)
        {
            return PropertyMemberVariable.Create(memberInfo)
                   ?? FieldMemberVariable.Create(memberInfo)
                   ?? throw new ArgumentException(
                       $"Can't create factory from {memberInfo.DisplayName()}",
                       nameof(memberInfo));
        }
    }
}
