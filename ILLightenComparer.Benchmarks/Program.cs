using System;
using ILLightenComparer.Benchmarks.Models;

namespace ILLightenComparer.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<CompareIntegral>();

            var target = new ComparersBuilder().CreateComparer(typeof(SimpleObject));

            var one = new SimpleObject
            {
                Integer = 1
            };

            var other = new SimpleObject
            {
                Integer = -1
            };

            var result = target.Compare(one, other);

            Console.WriteLine(result);
        }
    }
}
