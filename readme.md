# IL Lighten Comparer

**ILLightenComparer** is a library that can generate implementation for `IComparer\<T\>` on runtime using advantages of IL code emission.

## Features

* High performance.
* Support for complex classes and structures.
* Configurable.
* Cycle detection.
* Collections comparison.

## Benchmarks

``` ini
BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.379 (1809/October2018Update/Redstone5)
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.103
  [Host]     : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT
```

``` c
|          Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD | Rank |
|---------------- |---------:|----------:|----------:|---------:|------:|--------:|-----:|
|     IL_Comparer | 63.23 ms | 1.2523 ms | 2.3827 ms | 63.57 ms |  4.72 |    0.18 |    3 |
|   Nito_Comparer | 22.49 ms | 0.1738 ms | 0.1626 ms | 22.48 ms |  1.70 |    0.05 |    2 |
| Native_Comparer | 13.32 ms | 0.2583 ms | 0.3536 ms | 13.21 ms |  1.00 |    0.00 |    1 |
```

## Remarks

* *protected* and *private* members are ignored during comparison.

### Links

* [Activity diagram](./docs/activity-diagram.html).
* [Implementation details](./docs/reasoning.md).
* [Roadmap](./docs/roadmap.md).
