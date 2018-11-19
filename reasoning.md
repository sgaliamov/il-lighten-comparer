# Design reasons

1. ComparerBuilder.Build can not be only generic because there will be not enough information to create an instance:

    ``` csharp
    public IComparer CreateComparer(Type type) => (IComparer)_comparerBuilder.Build<T>(type);
    ```

1. asdf
