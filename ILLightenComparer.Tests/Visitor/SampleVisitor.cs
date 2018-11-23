using System;
using System.Diagnostics;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.Visitor
{
    public sealed class SampleVisitor
    {
        public int TestVisit(SampleObject acceptor, int state) => acceptor.KeyField += state;

        public int TestVisit(SampleParentObject acceptor, int state) => acceptor.KeyField += state;

        public int TestVisit(SampleStruct acceptor, int state)
        {
            Debug.WriteLine(acceptor.KeyField);
            return acceptor.KeyField += state;
        }

        public int TestVisit(object acceptor, int state) =>
            throw new InvalidOperationException("Method should not be invoked.");
    }
}
