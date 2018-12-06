using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class StackEmitter
    {
        public ILEmitter Visit(ICallableField member, ILEmitter il) =>
            il.LoadFieldAddress(member, Arg.X)
              .LoadField(member, Arg.Y);

        public ILEmitter Visit(ICallableProperty member, ILEmitter il) =>
            il.LoadProperty(member, Arg.X)
              .Store(member.MemberType.GetUnderlyingType(), out var local)
              .LoadAddress(local)
              .LoadProperty(member, Arg.Y);

        public ILEmitter Visit(ITwoArgumentsField member, ILEmitter il) =>
            il.LoadField(member, Arg.X)
              .LoadField(member, Arg.Y);

        public ILEmitter Visit(ITwoArgumentsProperty member, ILEmitter il) =>
            il.LoadProperty(member, Arg.X)
              .LoadProperty(member, Arg.Y);
    }
}
