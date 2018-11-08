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
            throw new NotImplementedException();
        }
    }
}
