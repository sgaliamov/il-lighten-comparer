using System;
using System.Reflection.Emit;
using ILightenComparer.Reflection;

namespace ILightenComparer.Emit.Methods
{
    internal sealed class CompareMethodEmitter
    {
        private readonly MembersProvider _membersProvider = new MembersProvider();

        public void Emit(Type objectType, CompareConfiguration configuration, MethodBuilder method)
        {
            var members = _membersProvider.GetMembers(objectType);

            var il = method.GetILGenerator();

            foreach (var member in members)
            {
                
            }
        }
    }
}
