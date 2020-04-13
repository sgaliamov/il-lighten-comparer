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
- [x] ~~normalize result to -1, 0, 1~~ -> no need to do it
- [x] ~~try to implement auto visitor~~ -> moved to a separate project, to mush extra complexity

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
- [x] split visiting `INullableAcceptor`
- [x] test with interface member
- [x] replace not sealed comparable
- [x] comparable nullable struct

## Phase 3 Collections

- [x] create type only at the end, use builders when cycle is possible
- [x] refactor visiting `IComparableAcceptor`
- [x] compare arrays
- [x] compare array of `ComparableStruct`
- [x] refactor variables loading
- [x] compare `IEnumarable`

## Phase 4 Settings

- [x] separate settings for each comparable type
- [x] string options (case, culture)
- [x] nullable comparison class
- [x] generate comparers for simple members
- [x] `IgnoreCollectionOrder` setting
- [x] use `DetectCycles` setting
- [x] refactor tests - ensure all branch execution
- [x] refactor visitors
- [x] introduce variables scope to not have to track variable buckets
- [x] smart `IComparerBuilder`
- [x] customization setting to override comparer with specific implementation
- [x] create separate context after set new configuration?
- [x] define order for members using expressions, order of generated code affects sorting
- [x] use expressions to define ignored properties

## Phase 5 Release One

- [x] benchmarks
- [x] add documentation comments
- [x] documentation and examples
- [x] setup CI/CD
- [x] code coverage
- [x] review *todo*
- [x] review reasoning
- [x] publish

## Phase 6 GetHashCode

- [x] support for .net core 3
- [x] create reusable traversing
- [x] implement `GetHashCode`
- [x] implement `Equals`
- [ ] implementation for `IEnumarable`
- [ ] create tests

## Phase 7 Improvements

- [x] move checks to member classes to make constructors safe, it's possible to create invalid member instances now
- [x] reuse local variables for same types
- [x] caching for assembly
- [x] use short versions of opt codes when possible
- [x] use call instead callvirt when possible
- [x] cache instances by type and **configuration** in `Context.GetComparerType`
- [x] ~~create unified interface `IComparer<>: IComparer<>, IComparer`~~
- [x] ~~possible to merge `MembersComparison` and `HierarchicalsComparison` but need to create `LocalVariable` class~~
- [x] ~~compare references and the end?~~ no, because objects are equal semantically
- [ ] fix *todo* list
- [ ] ignore existing `IComparable` implementation
- [ ] maybe move logic for `IComparable` to separate static method to simplify logic with variables loading - no need to have deal with addresses?
- [ ] add `IgnoreAll()` method to be able simplify comparison setup on specific fields or props only
- [ ] add `Include(string member)` method in pair with `IgnoreAll`
- [ ] custom comparison expression for a member to simplify comparison setup
- [ ] optimization for a last member - just return its result
- [ ] same for simple types, when arguments are compared directly
- [ ] change behavior then types are not matched but cast-able
- [ ] do reference comparison only once? done?
- [ ] use Br_S when possible?
- [ ] test class with more than 256 properties?
- [ ] support internal classes to compare?
- [ ] compare `IntPtr` and `UIntPtr`
- [ ] compare dictionary
- [ ] compare complex collection
- [ ] compared dynamic?
- [ ] compare private and protected members?
- [ ] helper wrappers for not typed compares
- [ ] include protected (BindingFlags.NonPublic)?
- [ ] tests for different cultures
- [ ] float, double, date time precision
- [ ] `checked` subs
- [ ] automatic cycle detection when needed
