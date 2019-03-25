# IL Lighten Comparer

**ILLightenComparer** is a library that can generate implementation of `IComparer<T>` on runtime using advantages of IL code emission with main focus on **performance**.

Why yet another comparer? Because I can ¯_(ツ)_/¯.

## Features

* High performance.
* Support for complex classes and structures.
* Highly configurable.
* Fluent intuitive API.
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

With regular models like [MovieObject](src/ILLightenComparer.Benchmarks/Benchmark/MovieObject.cs) generated comparer criminally close to manual implementation.

``` c
|                  Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
|------------------------ |---------:|----------:|----------:|---------:|------:|--------:|
| 'Manual implementation' | 12.47 ms | 0.2760 ms | 0.7785 ms | 12.15 ms |  1.00 |    0.09 |
|   'IL Lighten Comparer' | 12.90 ms | 0.2700 ms | 0.3214 ms | 12.77 ms |  1.00 |    0.00 |
|         'Nito Comparer' | 16.45 ms | 0.3627 ms | 1.0111 ms | 16.32 ms |  1.33 |    0.07 |
```

With light optimized structures like [LightStruct](src/ILLightenComparer.Benchmarks/Benchmark/LightStruct.cs) `ILLightenComparer` able to give serious performance boost.

``` c
|                  Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
|------------------------ |---------:|----------:|----------:|---------:|------:|--------:|
| 'Manual implementation' | 3.236 ms | 0.0643 ms | 0.0570 ms | 3.225 ms |  1.40 |    0.16 |
|   'IL Lighten Comparer' | 2.151 ms | 0.0862 ms | 0.2473 ms | 2.105 ms |  1.00 |    0.00 |
|         'Nito Comparer' | 6.968 ms | 0.2257 ms | 0.6655 ms | 6.647 ms |  3.28 |    0.43 |
```

## Configuration options

* Cycle detection.
* Ignoring collections order.
* Ignoring members.
* Including fields into comparison.
* Defining order in which members will be compared.
* Defining string comparison type.
* Defining custom comparers.

## Examples

### Basic usage

### Ignoring collection order

### Ignoring specific members

### Defining custom comparer

In some cases you may want use your own implementation.

## Remarks

* *protected* and *private* members are ignored during comparison.
* Configuration is fixed for generated comparer. If you want to change configuration you need request new comparer:
* For safety reasons cycle detection is enabled by default. But when you are sure that it is not possible you can disable it and get significant performance boost.

## What next

1. Generate implementation for `IEqualityComparer<T>`.
2. Support more types.
3. Add more settings.
4. Improve performance.

## Links

* [Activity diagram](./docs/activity-diagram.html).
* [Implementation details](./docs/reasoning.md).
* [Roadmap](./docs/roadmap.md).
