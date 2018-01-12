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
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Represents a type factory
    /// </summary>
    internal class TypeFactory
    {
        /// <summary>
        /// The default <see cref="TypeFactory"/> instance.
        /// </summary>
        private static Lazy<TypeFactory> instance = new Lazy<TypeFactory>(() => new TypeFactory("Default", "Default"), true);

        /// <summary>
        /// The assemlby cache.
        /// </summary>
        private AssemblyBuilderCache assemblyCache;

        /// <summary>
        /// The assembly builder.
        /// </summary>
        private AssemblyBuilder assemblyBuilder;

        /// <summary>
        /// Initialises a new instance of the <see cref=""/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="moduleName">The module name.</param>
        public TypeFactory(string assemblyName, string moduleName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            if (string.IsNullOrEmpty(assemblyName) == true)
            {
                throw new ArgumentException("Value cannot be empty.", nameof(assemblyName));
            }

            if (moduleName == null)
            {
                throw new ArgumentNullException(nameof(moduleName));
            }

            if (string.IsNullOrEmpty(moduleName) == true)
            {
                throw new ArgumentException("Value cannot be empty.", nameof(moduleName));
            }

            this.assemblyCache = new AssemblyBuilderCache();
            this.assemblyBuilder =  this.assemblyCache.GetOrCreateAssemblyBuilder(assemblyName);
            this.ModuleBuilder = this.assemblyBuilder.DefineDynamicModule(moduleName);
        }

        /// <summary>
        /// Gets the default type factory.
        /// </summary>
        public static TypeFactory Default
        {
            get
            {
                return instance.Value;
            }
        }

        /// <summary>
        /// Gets the module builder
        /// </summary>
        public ModuleBuilder ModuleBuilder { get;}

        /// <summary>
        /// Creates the global functions in a module.
        /// </summary>
        public void CreateGlobalFunctions()
        {
            this.ModuleBuilder.CreateGlobalFunctions();
        }

        /// <summary>
        /// Gets a method from the type factory.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <returns>A <see cref="MethodInfo"/> instance if found; otherwise null.</returns>
        public MethodInfo GetMethod(string methodName)
        {
            return this.ModuleBuilder.GetMethod(methodName);
        }

                /// <summary>
        /// Gets a type by name from the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="dynamicOnly">A value indicating whether only dynamic assemblies should be checked or not.</param>
        /// <returns>A <see cref="Type"/> representing the type if found; otherwise null.</returns>
        public Type GetType(string typeName, bool dynamicOnly)
        {
            var list = this.assemblyCache.GetAssemblies()
                .Union(AssemblyCache.GetAssemblies())
                .Where(a => dynamicOnly == false || a.IsDynamic == true);

            foreach (var ass in list)
            {
                Type type = ass.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}