using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.Visitor
{
    public struct SampleStructVisitor : IVisitor<SampleStruct, int>
    {
        public int Do(SampleStruct acceptor, int state) => acceptor.KeyField += state;
    }
}
