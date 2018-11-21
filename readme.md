# ILLightenComparer

## To do

### Phase 0 PoC

- [x] comparer test
- [x] flat object properties comparison
- [x] compare structures
- [x] generic comparer
- [x] refactor generic builder
- [x] refactor tests
- [x] benchmarks

### Phase 1 Core features

- [x] compare fields
- [x] compare strings
- [ ] refactor MembersProvider
- [ ] fixture generates nulls
- [ ] compare nullable https://referencesource.microsoft.com/#mscorlib/system/nullable.cs,7fc9e4edf9eff463
- [ ] object field comparison

### Phase 2 Hierarchical objects

- [ ] FlattenHierarchy
- [ ] detect cycles (check graph theory)
- [ ] test when abstract class is replaced after a comparer is created$

### Phase 3 Optimizations

- [ ] optimization for integral types
- [ ] caching for assembly
- [ ] ignored properties
- [ ] support internal classes to compare
- [ ] use short versions of opt codes when possible
- [ ] use call instead callvirt when possible
- [ ] reuse local variables for same types
- [ ] test class with more than 256 properties

### Phase 4 Settings

- [x] string options (case, culture)
- [ ] tests for different cultures
- [ ] float, double, date time precision
- [ ] customization setting to override comparer with specific implementation
- [ ] compare references and the end
- [ ] how to rebuild after set new configuration?

### Phase 5 Collections

- [ ] compare collection, do not iterate IEnumerable
- [ ] IgnoreCollectiOnorder setting
- [ ] define sorting order, emission order affects sorting

### Phase Z

- [ ] compared dynamic?
- [ ] setup CI/CD
