﻿using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface INullableAcceptor : IAcceptor
    {
        MethodInfo GetValueMethod { get; }
        MethodInfo HasValueMethod { get; }
        Type MemberType { get; }
    }
}