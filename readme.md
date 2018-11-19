# ILLightenComparer

## To do

### Phase 0 PoC

- [x] Comparer test
- [ ] Flat object properties comparison
- [ ] Generic comparer
- [ ] Benchmarks

### Phase 1 Core features

- [ ] Compare strings
- [ ] Compare fields
- [ ] Fixture generates nulls
- [ ] Compare nullable https://referencesource.microsoft.com/#mscorlib/system/nullable.cs,7fc9e4edf9eff463
- [ ] Object field comparison

### Phase 2 Hierarchical objects

- [ ] Detect cycles (check graph theory)
- [ ] Compare structures
- [ ] Test when abstract class is replaced after a comparer is created

### Phase 3 Optimizations

- [ ] Optimization for integral types
- [ ] Caching for assembly
- [ ] Ignored properties
- [ ] String options (case, culture)
- [ ] Support internal classes to compare

### Phase 4 Settings

- [ ] Float, double, date time precision
- [ ] Customization setting to override comparer with specific implementation

### Phase 5 Collections

- [ ] Compare collection, do not iterate IEnumerable
- [ ] IgnoreCollectionOrder setting

### Phase Z

- [ ] Compared dynamic?
- [ ] Setup CI/CD
