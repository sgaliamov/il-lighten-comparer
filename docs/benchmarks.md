# Benchmarks

Benchmarks are executed with disabled cycle detections.

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.836 (1909/November2018Update/19H2)
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.202
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
```

## IComparer\<T\>

### MovieModel

With regular classes like [MovieModel](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Models/MovieModel.cs) generated comparer is quite close to manual implementation.

|                  Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |
|------------------------ |----------:|----------:|----------:|----------:|------:|--------:|
| IL Lighten Comparer     |  7.605 ms | 0.1437 ms | 0.1538 ms |  7.601 ms |  1.14 |    0.03 |
| Manual implementation   |  6.470 ms | 0.1285 ms | 0.2413 ms |  6.390 ms |  1.00 |    0.00 |
| Nito Comparer           | 27.233 ms | 0.5294 ms | 0.5200 ms | 26.985 ms |  4.07 |    0.12 |

### LightStruct

With light optimized structures like [LightStruct](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Models/LightStruct.cs) `ILLightenComparer` able to give serious performance boost.


## IEqualityComparer\<T\>

### Equals

#### MovieModel

|                  Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |
|------------------------ |----------:|----------:|----------:|----------:|------:|--------:|
|   'IL Lighten Comparer' |  5.855 ms | 0.1152 ms | 0.1078 ms |  5.821 ms |  1.50 |    0.08 |
| 'Manual implementation' |  3.812 ms | 0.0751 ms | 0.1373 ms |  3.755 ms |  1.00 |    0.00 |
|         'Nito Comparer' | 23.129 ms | 0.4955 ms | 1.3812 ms | 22.850 ms |  6.06 |    0.45 |

#### LightStruct

### GetHashCode

#### MovieModel

|                  Method |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |
|------------------------ |---------:|---------:|---------:|---------:|------:|--------:|
|   'IL Lighten Comparer' | 53.93 ms | 0.993 ms | 1.182 ms | 54.27 ms |  1.00 |    0.00 |
| 'Manual implementation' | 14.92 ms | 0.126 ms | 0.118 ms | 14.88 ms |  0.28 |    0.01 |
|         'Nito Comparer' | 75.34 ms | 1.503 ms | 1.900 ms | 75.16 ms |  1.40 |    0.04 |

#### LightStruct
