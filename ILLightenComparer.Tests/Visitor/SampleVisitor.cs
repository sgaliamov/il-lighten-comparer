using System;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.Visitor
{
    public sealed class SampleVisitor : IVisitor<SampleObject, int>
    {
        public int Do(SampleObject acceptor, int state) => acceptor.KeyField + state;

        public int Do(SampleParentObject acceptor, int state) => acceptor.KeyField + state;

        public int Do(SampleStruct acceptor, int state) => acceptor.KeyField + state;

        public int Do(object acceptor, int state) =>
            throw new InvalidOperationException("Method should not be invoked.");
    }
}
