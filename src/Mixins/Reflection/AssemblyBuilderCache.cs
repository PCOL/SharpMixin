/*
MIT License

Copyright (c) 2018 P Collyer

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

namespace Mixins.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Represents an <see cref="AssemblyBuilder"/> cache.
    /// </summary>
    internal class AssemblyBuilderCache
    {
        /// <summary>
        /// The cache.
        /// </summary>
        private Dictionary<string, AssemblyBuilder> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBuilderCache"/> class.
        /// </summary>
        public AssemblyBuilderCache()
        {
            this.cache = new Dictionary<string, AssemblyBuilder>();
        }

        /// <summary>
        /// Gets or creates an <see cref="AssemblyBuilder"/> and <see cref="ModuleBuilder"/> pair.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly builder.</param>
        /// <returns>An assembly builder instance.</returns>
        public AssemblyBuilder GetOrCreateAssemblyBuilder(string assemblyName)
        {
            AssemblyBuilder builder;
            if (this.cache.TryGetValue(assemblyName, out builder) == false)
            {
                AssemblyName name = new AssemblyName(assemblyName);
                builder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
                this.cache.Add(assemblyName, builder);
            }

            return builder;
        }

        /// <summary>
        /// Removes an assembly builder and all of its module builders.
        /// </summary>
        /// <param name="name">The name of the assembly builder.</param>
        /// <returns>True if removed; otherwise false.</returns>
        public bool RemoveAssemblyBuilder(string name)
        {
            return this.cache.Remove(name);
        }

        /// <summary>
        /// Gets a list of assemblies.
        /// </summary>
        /// <returns>A list of assemblies.</returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            return this.cache.Values;
        }
    }
}
