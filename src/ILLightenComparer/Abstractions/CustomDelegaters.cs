using System;
using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Abstractions
{
    public delegate ILEmitter EmitterDelegate(ILEmitter il, Label next);

    public delegate ILEmitter EmitReferenceComparisonDelegate(ILEmitter il, Func<ILEmitter, ILEmitter> loadX, Func<ILEmitter, ILEmitter> loadY, Label ifEqual);

    public delegate ILEmitter EmitCheckIfLoopsAreDoneDelegate(ILEmitter il, LocalBuilder xDone, LocalBuilder yDone, Label gotoNext);
}
