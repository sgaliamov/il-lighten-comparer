# Design Solutions

1. *ComparerBuilder.Build* can not be only generic because there will be not enough information to create an instance:

    ``` csharp
    public IComparer CreateComparer(Type type) => (IComparer)_comparerBuilder.Build<T>(type);
    ```

1. *protected* and *private* members are not taken into account for now, because they are not visible for outer code and it will be not possible to define ordering for them.

1. Generated comparer and equality comparer are separate classes because no need generage extra code if only one is needed.
