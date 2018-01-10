namespace Mixins
{
    using System;

    /// <summary>
    /// Defines the mixin factory interface.
    /// </summary>
    public interface IMixinTypeGenerator
    {
        /// <summary>
        /// Creates a mixin.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="instances">The instances the mixin uses.</param>
        /// <returns>An instance of the mixin type.</returns>
        T CreateMixin<T>(params object[] instances);

        /// <summary>
        /// Gets or creates a mixin type.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="baseTypes">The mixins base types.</param>
        /// <returns>An instance of the mixin type.</returns>
        Type GetOrCreateMixinType<T>(params Type[] baseTypes);
    }
}
