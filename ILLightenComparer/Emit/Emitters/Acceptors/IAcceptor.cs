using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IAcceptor : IVariable
    {
        ILEmitter LoadVariables(StackEmitter visitor, ILEmitter il, Label gotoNext);
        ILEmitter Load(VariableLoader visitor, ILEmitter il, ushort arg);
        ILEmitter LoadAddress(VariableLoader visitor, ILEmitter il, ushort arg);
        ILEmitter Accept(CompareEmitter visitor, ILEmitter il);
        ILEmitter Accept(CompareCallVisitor visitor, ILEmitter il);
    }
}
