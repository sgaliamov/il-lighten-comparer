namespace ILLightenComparer.Emit.Extensions
{
    internal enum Case : byte
    {
        Unknown = 0,
        String = 1,
        Integral = 2,
        Primitive = 3,
        Comparable = 4,
        Hierarchical = 5,
        Enumerable = 6,
        Array = 7
    }
}
