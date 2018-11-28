namespace ILLightenComparer
{
    public interface IBuilderContext : IComparerProvider, IConfigurationSetter
    {
        /// <summary>
        ///     Sugar to convert the builder to generic version.
        /// </summary>
        /// <typeparam name="T">
        ///     The type whose instances need to compare.
        ///     Defines context for the following methods.
        /// </typeparam>
        IBuilderContext<T> For<T>();
    }

    public interface IBuilderContext<in T> : IComparerProvider<T>, IConfigurationSetter<T>
    {
        /// <summary>
        ///     Starts another builder context.
        /// </summary>
        IBuilderContext<TOther> For<TOther>();
    }
}
