# Roadmap

## Phase 0 PoC

- [x] comparer test
- [x] flat object properties comparison
- [x] compare structures
- [x] generic comparer
- [x] refactor generic builder
- [x] refactor tests
- [x] benchmarks

## Phase 1 Core features

- [x] compare fields
- [x] compare strings
- [x] compare enums
- [x] optimization for integral types
- [x] refactor MembersProvider
- [x] refactor visitors, introduce interfaces for member classes
- [x] refactor MemberConverter
- [x] fixture generates Max and Min values
- [X] fixture generates nulls
- [x] compare nullable
- [x] ~~normalize result to -1, 0, 1~~ - no need to do it
- [x] ~~try to implement auto visitor~~ - moved to a separate project, to mush extra complexity

## Phase 2 Hierarchical objects

- [x] FlattenHierarchy
- [x] nested members sort configuration
- [x] compare fields
- [x] use existing `CompareTo` method
- [x] nested object
- [x] abstract member comparison
- [x] object type member comparison
- [x] test when abstract class or not sealed is replaced after a comparer is created
- [x] multi-threading for `GetComparerType` method
- [x] detect cycles for objects
- [x] detect cycles for nested objects in structs
- [x] compare nullable complex structs
- [x] nested structure
- [ ] how to split visiting `INullableAcceptor`?
- [ ] comparable nullable struct
- [ ] replace not sealed comparable
- [ ] test with interface member

## Phase 3 Collections

- [ ] compare collection, do not iterate `IEnumerable`
- [ ] `IgnoreCollectionOrder` setting
- [ ] refactor tests - ensure all branch execution

## Phase 4 Settings

- [x] separate settings for each comparable type
- [x] string options (case, culture)
- [ ] smart configuration builder
- [ ] define order for members using expressions, order of generated code affects sorting
- [ ] use expressions to define ignored properties
- [ ] tests for different cultures
- [ ] float, double, date time precision
- [ ] customization setting to override comparer with specific implementation
- [ ] ignore existing `IComparable` implementation
- [ ] use `DetectCycles` setting
- [ ] compare references and the end?
- [ ] how to rebuild after set new configuration?
- [ ] include protected (BindingFlags.NonPublic)?

## Phase 5 Optimizations

- [x] reuse local variables for same types
- [x] caching for assembly
- [x] use short versions of opt codes when possible
- [x] use call instead callvirt when possible
- [ ] create unified interface `IComparer<>: IComparer<>, IComparer`
- [ ] cache instances by type and configuration in `Context.GetComparerType`
- [ ] optimization for a last member - just return its result
- [ ] change behavior then types are not matched but castable
- [ ] do reference comparison only once
- [ ] test class with more than 256 properties?
- [ ] support internal classes to compare?

## Phase 6

- [x] move checks to member classes to make constructors safe, it's possible to create invalid member instances now
- [ ] compare `IntPtr`
- [ ] setup CI/CD
- [ ] add documentation comments
- [ ] prepare presentation
- [ ] compared dynamic?
- [ ] compare private and protected members?
