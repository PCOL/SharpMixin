namespace Mixins
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Mixin extension methods.
    /// </summary>
    public static class MixinExtensionMethods
    {
        public static IServiceProvider Services { get; set; }

        /// <summary>
        /// Creates a mixin.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="instance">The instance to create the mixin from.</param>
        /// <returns>An instance of the mixin type or null if the instances to be 'mixed in' are null or empty.</returns>
        public static T CreateMixin<T>(this object instance)
        {
            if (instance == null)
            {
                return default(T);
            }

            return new object[] { instance }.CreateMixin<T>();
        }

        /// <summary>
        /// Creates a mixin.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="instances">The instances to create the mixin from.</param>
        /// <returns>An instance of the mixin type or null if the instances to be 'mixed in' are null or empty.</returns>
        public static T CreateMixin<T>(this object[] instances)
        {
            if (instances == null ||
                instances.Any() == false)
            {
                return default(T);
            }

            return (T)GetGenerator()
                .CreateMixin<T>(instances);
        }

        /// <summary>
        /// Creates a mixin type.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="baseTypes">The mixins base types.</param>
        /// <returns>A <see cref="Type"/> representing the mixin.</returns>
        public static Type CreateMixinType<T>(this Type[] baseTypes)
        {
            return GetGenerator()
                .GetOrCreateMixinType<T>(baseTypes);
        }

        private static IMixinTypeGenerator GetGenerator()
        {
            IMixinTypeGenerator generator = null;

            if (Services != null)
            {
                generator = Services.GetService<IMixinTypeGenerator>();
            }

            return generator ?? new MixinTypeGenerator();
        }
    }
}
