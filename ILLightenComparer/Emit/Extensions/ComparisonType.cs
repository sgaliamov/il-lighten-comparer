namespace ILLightenComparer.Emit.Extensions
{
    internal enum ComparisonType : byte
    {
        NotSupported = 0,
        Strings = 1,
        Integrals = 2,
        Comparables = 3,
        Hierarchicals = 4,
        Enumerables = 5,
        Arrays = 6
    }
}
