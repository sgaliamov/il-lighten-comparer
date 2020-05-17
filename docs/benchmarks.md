## [Benchmarks](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Program.cs)

With regular classes like [MovieModel](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Models/MovieModel.cs) generated comparer is criminally close to manual implementation.

| Method                |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
| --------------------- | -------: | --------: | --------: | -------: | ----: | ------: |
| IL Lighten Comparer   | 12.90 ms | 0.2700 ms | 0.3214 ms | 12.77 ms |  1.00 |    0.00 |
| Manual implementation | 12.47 ms | 0.2760 ms | 0.7785 ms | 12.15 ms |  1.00 |    0.09 |

With light optimized structures like [LightStruct](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Models/LightStruct.cs) `ILLightenComparer` able to give serious performance boost.

| Method                |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
| --------------------- | -------: | --------: | --------: | -------: | ----: | ------: |
| IL Lighten Comparer   | 2.151 ms | 0.0862 ms | 0.2473 ms | 2.105 ms |  1.00 |    0.00 |
| Manual implementation | 3.236 ms | 0.0643 ms | 0.0570 ms | 3.225 ms |  1.40 |    0.16 |

\* Benchmarks are executed with disabled cycle detections.
