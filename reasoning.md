# Design reasons

1. *ComparerBuilder.Build* can not be only generic because there will be not enough information to create an instance:

    ``` csharp
    public IComparer CreateComparer(Type type) => (IComparer)_comparerBuilder.Build<T>(type);
    ```

1. *TVisitor* can't be *IVisitor* becasue it forces implementation follow a call signature:

    ``` c#
    _autoVisitor.Accept(member, _visitor, il);
    ```
    
     Class of *_visitor* will have to implement method that depends on type of *method* parameter.

1. *AutoVisitor* don't depend on *IVisitor* to be able to name the *Visit* method differently. *AutoVisitor* uses duck typing, thus methods must follow signature of *IVisitor*.
