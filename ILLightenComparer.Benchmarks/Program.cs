using System;
using ILLightenComparer.Benchmarks.Models;

namespace ILLightenComparer.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<CompareIntegral>();

            var target = new ComparersBuilder().CreateComparer(typeof(FlatObject));

            var one = new FlatObject
            {
                IntegerProperty = 1,
                Double = 2
            };

            var other = new FlatObject
            {
                //IntegerProperty = 2,
                Double = 2
            };

            var result = target.Compare(one, other);

            Console.WriteLine(result);
        }
    }
}
