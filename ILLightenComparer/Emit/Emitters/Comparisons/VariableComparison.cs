﻿using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Emitters.Visitors;
using ILLightenComparer.Emit.Shared;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal sealed class VariableComparison : IComparison
    {
        public VariableComparison(IVariable itemVariable, IVisitorsAcceptor itemAcceptor)
        {
            Variable = itemVariable ?? throw new ArgumentNullException(nameof(itemVariable));
            Acceptor = itemAcceptor ?? throw new ArgumentNullException(nameof(itemAcceptor));
        }

        public IVisitorsAcceptor Acceptor { get; }
        public IVariable Variable { get; }

        public ILEmitter Accept(CompareVisitor visitor, ILEmitter il)
        {
            return Acceptor.Accept(visitor, il);
        }

        public ILEmitter Accept(CompareEmitter visitor, ILEmitter il)
        {
            return visitor.Visit(this, il);
        }

        public ILEmitter LoadVariables(StackVisitor visitor, ILEmitter il, Label gotoNext)
        {
            return Acceptor.LoadVariables(visitor, il, gotoNext);
        }
    }
}
