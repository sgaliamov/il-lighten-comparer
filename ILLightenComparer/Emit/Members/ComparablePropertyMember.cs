﻿using System.Reflection;
using ILLightenComparer.Emit.Members.Base;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Members
{
    internal sealed class ComparablePropertyMember : PropertyMember
    {
        public MethodInfo CompareToMethod { get; set; }

        public override void Accept(IVisitor visitor, ILEmitter il)
        {
            visitor.Visit(this, il);
        }
    }
}
