using System;
using System.Reflection;

namespace ILLightenComparer.Reflection
{
    internal sealed class MembersProvider
    {
        public MemberInfo[] GetMembers(Type type) =>
            type.GetMembers(
                BindingFlags.GetProperty
                //| BindingFlags.GetField
                //| BindingFlags.FlattenHierarchy
                | BindingFlags.Public);
    }
}
