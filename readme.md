# IL Lighten Comparer

**ILLightenComparer** is a library that can generate implementation for `IComparer<T>` on runtime using advantages of IL code emission.

## Features

* High performance.
* Support for complex classes and structures.
* Highly configurable.
* Fluent API.
* Cycle detection.
* Collections comparison.

## Benchmarks

On 1000000 iterations with [MovieSampleObject](src/ILLightenComparer.Benchmarks/Benchmark/MovieSampleObject.cs).

``` ini
BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.379 (1809/October2018Update/Redstone5)
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.103
  [Host]     : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT
```

``` c
|          Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
|---------------- |---------:|----------:|----------:|---------:|------:|--------:|
| Native_Comparer | 16.05 ms | 0.7101 ms | 2.0937 ms | 14.80 ms |  1.00 |    0.00 |
|     IL_Comparer | 16.36 ms | 0.3667 ms | 0.9530 ms | 16.07 ms |  1.03 |    0.14 |
|   Nito_Comparer | 26.40 ms | 0.8624 ms | 2.5428 ms | 25.06 ms |  1.67 |    0.26 |
```

``` c
|          Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
|---------------- |---------:|----------:|----------:|---------:|------:|--------:|
| Native_Comparer | 3.024 ms | 0.0588 ms | 0.0933 ms | 3.012 ms |  1.00 |    0.00 |
|     IL_Comparer | 1.924 ms | 0.0156 ms | 0.0146 ms | 1.925 ms |  0.63 |    0.02 |
|   Nito_Comparer | 6.109 ms | 0.0487 ms | 0.0456 ms | 6.094 ms |  2.00 |    0.06 |
```

## Remarks

* *protected* and *private* members are ignored during comparison.

### Links

* [Activity diagram](./docs/activity-diagram.html).
* [Implementation details](./docs/reasoning.md).
* [Roadmap](./docs/roadmap.md).
