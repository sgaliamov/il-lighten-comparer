# Design Solutions

## General

1. *ComparerBuilder.Build* can not be only generic because there will be not enough information to create an instance:
    ``` csharp
    public IComparer CreateComparer(Type type) => (IComparer)_comparerBuilder.Build<T>(type);
    ```
1. *protected* and *private* members are not taken into account for now, because they are not visible for outer code and it will be not possible to define ordering for them.
1. Generated comparer and equality comparer are separate classes because no need generate extra code if only one is needed.
1. *GenericProxy* is not cached because it do almost nothing and have no state.
1. Configurations collection is *ConcurrentDictionary* because while one thread may build a type, another can try to update the collection.
1. *ComparersBuilder* does not use *Lazy* to cache instances because creation is relatively cheap. But *Context* uses it because the code emission is slow.
1. *GetCompareToMethod* will not support typeless *IComparable*.
1. Generated methods should follow a possible manual implementation as much as possible. So, if types of arguments are not matched it should fail with ArgumentException. If one of types is castable to another, an object with more members should be considered as bigger object, because if base parts are same, extra parts of child class add additional weight.

## Cycle detection

1. Use *HashSet* instead *ObjectIDGenerator* to detect member cycles because users may want to override the way how to compare a member.
1. Check for *null* before add to a cycle detections set because *null* is kind a value and should be compared on a next iteration.
1. We need two separate sets to be able to go as deep as we can to behave same way when we don't have a cycle.
