# Fast comparer library

[![Build status](https://ci.appveyor.com/api/projects/status/u9qs6c5v1qbvda2b/branch/master?svg=true)](https://ci.appveyor.com/project/sgaliamov/il-lighten-comparer/branch/master)
[![codecov](https://codecov.io/gh/sgaliamov/il-lighten-comparer/graph/badge.svg)](https://codecov.io/gh/sgaliamov/il-lighten-comparer)
[![NuGet Badge](https://buildstats.info/nuget/ILLightenComparer)](https://www.nuget.org/packages/ILLightenComparer)

**ILLightenComparer** is a library that can generate `IComparer<T>` implementation on runtime using advantages of IL code emission with main focus on **performance**.

## Features

* High performance.
* Support for complex classes and structures.
* Highly configurable.
* Fluent intuitive API.
* Cycle detection.
* Collections comparison (`IEnumerable<T>`, arrays).
* .NET Standard 2.0
* No 3<sup>rd</sup> party dependencies.

## [Benchmarks](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Program.cs)

With regular models like [MovieModel](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Benchmark/MovieModel.cs) generated comparer is criminally close to manual implementation.

| Method                |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
| --------------------- | -------: | --------: | --------: | -------: | ----: | ------: |
| IL Lighten Comparer   | 12.90 ms | 0.2700 ms | 0.3214 ms | 12.77 ms |  1.00 |    0.00 |
| Manual implementation | 12.47 ms | 0.2760 ms | 0.7785 ms | 12.15 ms |  1.00 |    0.09 |
| Nito Comparer         | 16.45 ms | 0.3627 ms | 1.0111 ms | 16.32 ms |  1.33 |    0.07 |

With light optimized structures like [LightStruct](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Benchmarks/Benchmark/LightStruct.cs) `ILLightenComparer` able to give serious performance boost.

| Method                |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD |
| --------------------- | -------: | --------: | --------: | -------: | ----: | ------: |
| IL Lighten Comparer   | 2.151 ms | 0.0862 ms | 0.2473 ms | 2.105 ms |  1.00 |    0.00 |
| Manual implementation | 3.236 ms | 0.0643 ms | 0.0570 ms | 3.225 ms |  1.40 |    0.16 |
| Nito Comparer         | 6.968 ms | 0.2257 ms | 0.6655 ms | 6.647 ms |  3.28 |    0.43 |

## Configuration options

* Cycle detection.
* Ignoring collections order.
* Ignoring members.
* Including fields into comparison.
* Defining order in which members will be compared.
* Defining string comparison type.
* Defining custom comparers by type or instance.

## [Examples](https://github.com/sgaliamov/il-lighten-comparer/blob/master/src/ILLightenComparer.Tests/Examples.cs)

### Basic usage

``` csharp
var comparer = new ComparerBuilder().GetComparer<Tuple<int, string>>();
var result = comparer.Compare(x, y);
```

### Ignore collection order

``` csharp
var x = new[] { 1, 2, 3 };
var y = new[] { 2, 3, 1 };

var comparer = new ComparerBuilder()
                .For<int[]>(c => c.IgnoreCollectionsOrder(true))
                .GetComparer();

var result = comparer.Compare(x, y);
result.Should().Be(0);
```

### Ignore specific members

``` csharp
var x = new Tuple<int, string, double>(1, "value 1", 1.1);
var y = new Tuple<int, string, double>(1, "value 2", 2.2);

var comparer = new ComparerBuilder()
                .For<Tuple<int, string, double>>()
                .Configure(c => c.IgnoreMember(o => o.Item2)
                                 .IgnoreMember(o => o.Item3))
                .GetComparer();

var result = comparer.Compare(x, y);
result.Should().Be(0);
```

### Define custom comparer

``` csharp
var x = _fixture.Create<Tuple<int, string>>();
var y = _fixture.Create<Tuple<int, string>>();
var customComparer = new CustomizableComparer<Tuple<int, string>>((a, b) => 0); // makes all objects always equal

var comparer = new ComparerBuilder()
                .Configure(c => c.SetCustomComparer(customComparer))
                .GetComparer<Tuple<int, string>>();

var result = comparer.Compare(x, y);
result.Should().Be(0);
```

### Define multiple configurations

``` csharp
var builder = new ComparerBuilder(c => c.SetDefaultCyclesDetection(false)); // defines initial configuration

// adds some configuration later
builder.Configure(c => c.SetStringComparisonType(
                            typeof(Tuple<int, string, Tuple<short, string>>),
                            StringComparison.InvariantCultureIgnoreCase)
                        .IgnoreMember<Tuple<int, string, Tuple<short, string>>, int>(o => o.Item1));

// defines configuration for specific types
builder.For<Tuple<short, string>>(c => c.DefineMembersOrder(
    order => order.Member(o => o.Item2)
                  .Member(o => o.Item2)));

// adds additional configuration to existing configuration
builder.For<Tuple<int, string, Tuple<short, string>>>(c => c.IncludeFields(false));
```

## Remarks

* Configuration is fixed for generated comparer. If you want to change configuration you have to request new comparer:
  ``` csharp
  var x = new Tuple<int, string>(1, "text");
  var y = new Tuple<int, string>(2, "TEXT");

  // initially configuration defines case insensitive string comparison
  var builder = new ComparerBuilder()
      .For<Tuple<int, string>>(c => c.SetStringComparisonType(StringComparison.CurrentCultureIgnoreCase)
                                     .DetectCycles(false));

  // in addition, setup to ignore first member
  builder.Configure(c => c.IgnoreMember(o => o.Item1));

  // this version takes in account only case insensitive second member
  var ignoreCaseComparer = builder.GetComparer();

  // override string comparison type with case sensitive setting and build new comparer
  var originalCaseComparer = builder.For<Tuple<int, string>>()
                                    .Configure(c => c.SetStringComparisonType(StringComparison.Ordinal))
                                    .GetComparer();

  // first comparer ignores case for strings still
  ignoreCaseComparer.Compare(x, y).Should().Be(0);

  // second comparer still ignores first member but uses new string comparison type
  var result = originalCaseComparer.Compare(x, y);
  result.Should().Be(string.Compare("text", "TEXT", StringComparison.Ordinal));
  ```
* To help generate more effective code use `sealed` classes and small types (`sbyte`, `byte`, `char`, `short`, `ushort`) when possible.
* For safety reasons cycle detection is enabled by default. But when you are sure that it is not possible you can disable it and get significant performance boost.
* *protected* and *private* members are ignored during comparison.
* [Multidimensional arrays](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/multidimensional-arrays) are not supported now, but [Jagged arrays](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/jagged-arrays) are.
* If a type implements `IComparable<T>` interface and type is a value type or **`sealed`** then this implementations will be used.

## What next

1. Generate implementation for `IEqualityComparer<T>`.
2. Support more types.
3. Add more settings.
4. Improve performance.

In case of unexpected behavior please welcome to create an [issue](https://github.com/sgaliamov/il-lighten-comparer/issues/new) and provide the type that you use.
