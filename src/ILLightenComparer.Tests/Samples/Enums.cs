namespace ILLightenComparer.Tests.Samples
{
    public enum EnumBig : ulong
    {
        First = ulong.MinValue,
        One = 1,
        Two = 2,
        Three = 3,
        All = ulong.MaxValue
    }

    public enum EnumSmall : byte
    {
        First = byte.MinValue,
        One = 1,
        Two = 2,
        Three = 3,
        All = byte.MaxValue
    }
}
