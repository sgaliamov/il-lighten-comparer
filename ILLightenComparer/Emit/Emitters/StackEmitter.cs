using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class StackEmitter
    {
        public ILEmitter Visit(ICallableField member, ILEmitter il) =>
            il.LoadFieldAddress(member, 1)
              .LoadField(member, 2);

        public ILEmitter Visit(ICallableProperty member, ILEmitter il) =>
            il.LoadProperty(member, 1)
              .DeclareLocal(member.MemberType.GetUnderlyingType(), out var local)
              .Store(local)
              .LoadAddress(local)
              .LoadProperty(member, 2);

        public ILEmitter Visit(ITwoArgumentsField member, ILEmitter il) =>
            il.LoadField(member, 1)
              .LoadField(member, 2);

        public ILEmitter Visit(ITwoArgumentsProperty member, ILEmitter il) =>
            il.LoadProperty(member, 1)
              .LoadProperty(member, 2);
    }
}
