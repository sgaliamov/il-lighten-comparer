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
| Manual implementation   |  6.470 ms | 0.1285 ms | 0.2413 ms |  6.390 ms |  1.00 |    0.00 |
| IL Lighten Comparer     |  7.605 ms | 0.1437 ms | 0.1538 ms |  7.601 ms |  1.14 |    0.03 |
| Nito Comparer           | 27.233 ms | 0.5294 ms | 0.5200 ms | 26.985 ms |  4.07 |    0.12 |

### LightStruct

With light optimized structures like [LightStruct](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Models/LightStruct.cs) `ILLightenComparer` able to give serious performance boost.

|                  Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
|------------------------ |---------:|----------:|----------:|---------:|------:|--------:|
|    IL Lighten Comparer  | 2.864 ms | 0.0361 ms | 0.0320 ms | 2.861 ms |  0.71 |    0.01 |
|  Manual implementation  | 4.029 ms | 0.0537 ms | 0.0503 ms | 4.027 ms |  1.00 |    0.00 |
|          Nito Comparer  | 6.919 ms | 0.0756 ms | 0.0670 ms | 6.926 ms |  1.72 |    0.03 |

## IEqualityComparer\<T\>

### Equals

#### MovieModel

|                  Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |
|------------------------ |----------:|----------:|----------:|----------:|------:|--------:|
|  Manual implementation  |  3.812 ms | 0.0751 ms | 0.1373 ms |  3.755 ms |  1.00 |    0.00 |
|    IL Lighten Comparer  |  5.855 ms | 0.1152 ms | 0.1078 ms |  5.821 ms |  1.50 |    0.08 |
|          Nito Comparer  | 23.129 ms | 0.4955 ms | 1.3812 ms | 22.850 ms |  6.06 |    0.45 |

#### LightStruct

|                  Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
|------------------------ |---------:|----------:|----------:|---------:|------:|--------:|
|  Manual implementation  | 1.731 ms | 0.0151 ms | 0.0134 ms | 1.729 ms |  1.00 |    0.00 |
|    IL Lighten Comparer  | 2.232 ms | 0.0428 ms | 0.0420 ms | 2.211 ms |  1.29 |    0.03 |
|          Nito Comparer  | 6.969 ms | 0.1380 ms | 0.2452 ms | 6.921 ms |  3.98 |    0.16 |

### GetHashCode

#### MovieModel

|                  Method |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |
|------------------------ |---------:|---------:|---------:|---------:|------:|--------:|
|  Manual implementation  | 14.93 ms | 0.077 ms | 0.065 ms | 14.92 ms |  1.00 |    0.00 |
|    IL Lighten Comparer  | 52.70 ms | 0.710 ms | 0.664 ms | 52.69 ms |  3.53 |    0.04 |
|          Nito Comparer  | 73.54 ms | 1.309 ms | 1.225 ms | 73.33 ms |  4.92 |    0.09 |

#### LightStruct

|                  Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |
|------------------------ |----------:|----------:|----------:|----------:|------:|--------:|
|  Manual implementation  |  1.082 ms | 0.0206 ms | 0.0220 ms |  1.073 ms |  1.00 |    0.00 |
|    IL Lighten Comparer  |  3.406 ms | 0.0467 ms | 0.0436 ms |  3.393 ms |  3.16 |    0.07 |
|          Nito Comparer  | 12.452 ms | 0.1535 ms | 0.1360 ms | 12.412 ms | 11.54 |    0.27 |
