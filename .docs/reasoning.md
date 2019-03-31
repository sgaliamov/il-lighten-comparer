# Design Solutions

## General

1. `protected` and `private` members are not taken into account for now because they are not visible for outer code and it will be not possible to define ordering for them.
2. Generated comparer and equality comparer are separate classes because no need generate extra code if only one is needed.
3. `Proxy` classes is not cached because it do almost nothing and have no state.
4. Configurations collection is `ConcurrentDictionary` because while one thread may build a type, another can try to update the collection.
5. `ComparerBuilder` does not use cache because creation is relatively cheap. But `ContextBuilder` uses it because the code emission is slow.
6. Typeless `IComparable` is not supported for sake of simplicity. It's easy to create wrapper if need be.
7. Generated methods should follow a possible manual implementation as much as possible. So, if types of arguments are not matched it should fail with `ArgumentException`. If one of types is cast-able to another, an object with more members should be considered as bigger object, because if base parts are same, extra parts of child class add additional weight.
8. Visitor Pattern can be applied when decision can be done on design stage.
9. Generated `Compare` method should not do any reflection on runtime.

## Cycle detection

1. Use `ConcurrentDictionary` instead `ObjectIDGenerator` to detect member cycles because users may want to override the way how to compare a member.
2. Check for `null` before add to a cycle detections set because `null` is kind a value and should be compared on a next iteration.
3. We need two separate sets to be able to go as deep as we can to behave same way when we don't have a cycle.
4. Cycle detection is not applied on value types because it does not have a sense. Nested value could be equal to an owner object only because of wrong equality implementation, but it can not be considered as a cycle.
