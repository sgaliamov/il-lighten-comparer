# Fast comparer library

[![Build status](https://ci.appveyor.com/api/projects/status/u9qs6c5v1qbvda2b/branch/master?svg=true)](https://ci.appveyor.com/project/sgaliamov/il-lighten-comparer/branch/master)
[![codecov](https://codecov.io/gh/sgaliamov/il-lighten-comparer/branch/master/graph/badge.svg)](https://codecov.io/gh/sgaliamov/il-lighten-comparer)
[![NuGet Badge](https://buildstats.info/nuget/ILLightenComparer)](https://www.nuget.org/packages/ILLightenComparer)

**ILLightenComparer** is a flexible library that can generate very effective and comprehensive `IComparer<T>` and `IEqualityComparer<T>` implementations on runtime using advantages of `IL` code emission.

## Features

* Support for classes and structures any complexity and nesting.
* Highly configurable.
* Fluent intuitive API.
* Cycle detection.
* Collections comparison (`IEnumerable<T>`, `T[]`, `T[][]`).
* .NET Standard 2.0
* High performance.
* No 3<sup>rd</sup> party dependencies.

## Configuration options

* Cycle detection.
* Ignoring collections order.
* Ignoring members.
* Including fields into comparison.
* Defining order in which members will be compared.
* Defining string comparison type.
* Defining custom comparers by type or instance.

## Examples

### Basic usage

``` csharp
var comparer = ComparerBuilder.Default.GetComparer<Tuple<int, string>>();
var compareResult = comparer.Compare(x, y);

var equalityComparer = ComparerBuilder.Default.GetEqualityComparer<Tuple<int, string>>();
var equalityResult = equalityComparer.Equals(x, y);
var hashResult = equalityComparer.GetHashCode(x);
```

And it "just works", no need complex configuration.

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
builder.Configure(c => c
    .SetStringComparisonType(
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
      .For<Tuple<int, string>>(c => c
          .SetStringComparisonType(StringComparison.CurrentCultureIgnoreCase)
          .DetectCycles(false));

  // in addition, setup to ignore first member
  builder.Configure(c => c.IgnoreMember(o => o.Item1));

  // this version takes in account only case insensitive second member
  var ignoreCaseComparer = builder.GetComparer();

  // override string comparison type with case sensitive setting and build new comparer
  var originalCaseComparer = builder
      .For<Tuple<int, string>>()
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
* If a type implements `IComparable<T>` interface then this implementations will be used.
* [Benchmarks](https://github.com/sgaliamov/il-lighten-comparer/blob/master/docs/benchmarks.md)

In case of an unexpected behavior, please welcome to create an [issue](https://github.com/sgaliamov/il-lighten-comparer/issues/new) and provide the type and data that you use.
