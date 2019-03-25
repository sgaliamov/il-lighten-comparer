using System;

namespace ILLightenComparer.Examples
{
    internal sealed class Program
    {
        private static void Main()
        {
            var compare = new ComparerBuilder(c =>
                              c.SetDefaultCyclesDetection(false)
                               .SetDefaultFieldsInclusion(false))
                          .GetComparer<Tuple<int, string>>()
                          .Compare(
                              new Tuple<int, string>(0, "string 1"),
                              new Tuple<int, string>(0, "string 2"));

            Console.WriteLine(compare);
        }
    }
}
