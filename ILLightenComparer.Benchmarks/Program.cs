using System;
using System.Collections;
using System.Diagnostics;
using ILLightenComparer.Benchmarks.Models;

namespace ILLightenComparer.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<CompareIntegral>();

            Console.WriteLine("build");
            //Console.ReadLine();

            var target = new ComparersBuilder().CreateComparer(typeof(SimpleObject));

            var flatObject = new SimpleObject();

            Console.WriteLine("compare");

            var result = Result(target, flatObject);

            Console.WriteLine("done");
            Console.WriteLine(result);
        }

        private static int Result(IComparer target, SimpleObject flatObject)
        {
            GC.WaitForFullGCComplete(10000);

            //Console.ReadLine();

            return target.Compare(flatObject, flatObject);
        }
    }
}
