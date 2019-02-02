using ILLightenComparer.Config;

namespace ILLightenComparer
{
    public interface IContextBuilder : IConfigurationBuilder, IComparerProvider
    {
        /// <summary>
        ///     Sugar to convert the builder to generic version.
        /// </summary>
        /// <typeparam name="T">
        ///     The type whose instances need to compare.
        ///     Defines context for the following methods.
        /// </typeparam>
        IContextBuilder<T> For<T>();
    }

    public interface IContextBuilder<T> : IConfigurationBuilder<T>, IComparerProvider<T> { }
}
