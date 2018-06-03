/*
MIT License

Copyright (c) 2018 PCOL

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
        /// <summary>
        /// Gets or sets the service provider to use.
        /// </summary>
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

        /// <summary>
        /// Checks if an object is a mixin.
        /// </summary>
        /// <param name="obj">The object instance to check.</param>
        /// <returns>True if it is; otherwise false</returns>
        public static bool IsMixin(this object obj)
        {
            return typeof(IMixinObject).IsAssignableFrom(obj.GetType());
        }

        /// <summary>
        /// Gets the mixin generator to use.
        /// </summary>
        /// <returns>A <see cref="IMixinTypeGenerator"/> instance.</returns>
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
