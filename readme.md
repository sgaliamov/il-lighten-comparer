# IL Lighten Comparer

**ILLightenComparer** is a library that can generate implementation for *IComparer\<T\>*, *IComparer*, *IEqualityComparer\<T\>* and *IEqualityComparer* on runtime using advantages of IL code emission.

## Features

* High performance.
* Support for complex classes and structures.
* Cycle detection.
* Collections comparison.
* 100% code coverage.

## Benchmarks

``` ini
BenchmarkDotNet=v0.11.2, OS=Windows 10.0.17134.407 (1803/April2018Update/Redstone4)
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=2531248 Hz, Resolution=395.0620 ns, Timer=TSC
.NET Core SDK=2.1.500
  [Host]     : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.6 (CoreCLR 4.6.27019.06, CoreFX 4.6.27019.05), 64bit RyuJIT
```

``` c
|          Method |     Mean |    Error |   StdDev |   Median | Ratio | Rank |
|---------------- |---------:|---------:|---------:|---------:|------:|-----:|
|     IL_Comparer | 240.3 us | 2.676 us | 2.503 us | 240.4 us |  0.70 |    1 |
| Native_Comparer | 343.2 us | 3.218 us | 2.853 us | 343.7 us |  1.00 |    2 |
```

## Limitations

* *protected* and *private* members are ignored during comparison.

### Links

* [Roadmap](./roadmap.md).
* [Implementation details](./reasoning.md).
