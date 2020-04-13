﻿using System.Reflection;
using System.Reflection.Emit;
using Illuminator;

namespace ILLightenComparer.Abstractions
{
    public delegate ILEmitter EmitterDelegate(ILEmitter il, Label next);

    public delegate ILEmitter EmitReferenceComparisonDelegate(ILEmitter il, LocalVariableInfo x, LocalVariableInfo y, Label ifEqual);

    public delegate ILEmitter EmitCheckIfLoopsAreDoneDelegate(ILEmitter il, LocalBuilder xDone, LocalBuilder yDone, Label gotoNext);

    public delegate ILEmitter EmitCheckIfArrayLoopsAreDoneDelegate(ILEmitter il, LocalBuilder index, LocalBuilder countX, LocalBuilder countY, Label afterLoop);
}
