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
- [x] ~~normalize result to -1, 0. 1~~ - no need to do it
- [x] refactor visitors, introduce interfaces for member classes
- [x] refactor MemberConverter
- [x] fixture generates Max and Min values
- [X] fixture generates nulls
- [x] ~~try to implement auto visitor~~ - moved to a separate project, to mush extra complexity
- [x] compare nullable

## Phase 2 Hierarchical objects

- [ ] FlattenHierarchy
- [ ] nested object
- [ ] nested struct
- [ ] detect cycles (check graph theory)
- [ ] abstract member comparison
- [ ] object member comparison?
- [ ] test when abstract class is replaced after a comparer is created

## Phase 3 Collections

- [ ] compare collection, do not iterate IEnumerable
- [ ] IgnoreCollectiOnorder setting
- [ ] define sorting order, emission order affects sorting

## Phase 4 Settings

- [ ] string options (case, culture)
- [ ] separate settings for each comparable type
- [ ] ignored properties
- [ ] tests for different cultures
- [ ] float, double, date time precision
- [ ] customization setting to override comparer with specific implementation
- [ ] compare references and the end?
- [ ] how to rebuild after set new configuration?

## Phase 5 Optimizations

- [x] reuse local variables for same types
- [ ] caching for assembly
- [ ] support internal classes to compare
- [ ] use short versions of opt codes when possible
- [ ] use call instead callvirt when possible
- [ ] test class with more than 256 properties

## Phase 6

- [ ] setup CI/CD
- [ ] prepare presentation
- [ ] move checks to member classes to make constructors safe, it's possible to create invalid member instances now
- [ ] compared dynamic?
- [ ] compare private and protected members?
