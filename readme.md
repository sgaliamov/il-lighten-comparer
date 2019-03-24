# IL Lighten Comparer

**ILLightenComparer** is a library that can generate implementation for `IComparer\<T\>` on runtime using advantages of IL code emission.

## Features

* High performance.
* Support for complex classes and structures.
* High configurable.
* Fluent API.
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
| Native_Comparer | 1.121 ms | 0.0220 ms | 0.0343 ms | 1.111 ms |  1.00 |    0.00 |    1 |
|     IL_Comparer | 1.209 ms | 0.0241 ms | 0.0338 ms | 1.207 ms |  1.08 |    0.03 |    2 |
|   Nito_Comparer | 1.795 ms | 0.0153 ms | 0.0143 ms | 1.793 ms |  1.58 |    0.05 |    3 |
```

## Remarks

* *protected* and *private* members are ignored during comparison.

### Links

* [Activity diagram](./docs/activity-diagram.html).
* [Implementation details](./docs/reasoning.md).
* [Roadmap](./docs/roadmap.md).
