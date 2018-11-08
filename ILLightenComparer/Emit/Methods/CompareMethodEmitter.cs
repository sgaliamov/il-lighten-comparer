using System;
using System.Reflection.Emit;
using ILLightenComparer.Reflection;

namespace ILLightenComparer.Emit.Methods
{
    internal sealed class CompareMethodEmitter
    {
        private readonly MembersProvider _membersProvider = new MembersProvider();

        public void Emit(Type objectType, CompareConfiguration configuration, MethodBuilder method)
        {
            var members = _membersProvider.GetMembers(objectType);

            var il = method.GetILGenerator();

            foreach (var member in members) { }
        }
    }
}
