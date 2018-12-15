namespace ILLightenComparer.Emit.Extensions
{
    internal enum ComparisonType : byte
    {
        NotSupported = 0,
        Strings = 1,
        Integrals = 2,
        Primitives = 3,
        Comparables = 4,
        Hierarchicals = 5,
        Enumerables = 6,
        Arrays = 7
    }
}
