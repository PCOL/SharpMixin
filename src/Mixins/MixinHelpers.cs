namespace Mixins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Mixin helper functions.
    /// </summary>
    public static class MixinHelpers
    {
        /// <summary>
        /// Creates an adapter.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="inst">The instance being adapted</param>
        /// <returns>An instance of the mixin type or null if the instances to be 'mixed in' are null or empty.</returns>
        public static T CreateMixin<T>(params object[] inst)
        {
            return inst.CreateMixin<T>();
        }

        /// <summary>
        /// Creates an adapter type.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="baseTypes">The mixins base types.</param>
        /// <returns>A <see cref="Type"/> representing the mixin.</returns>
        public static Type CreateAdapterType<T>(params Type[] baseTypes)
        {
            return baseTypes.CreateMixinType<T>();
        }
    }
}
