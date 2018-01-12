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
    using Microsoft.DotNet.PlatformAbstractions;
    using Microsoft.Extensions.DependencyModel;

    /// <summary>
    /// Implementation of an assembly cache.
    /// </summary>
    internal static class AssemblyCache
    {
        /// <summary>
        /// Gets a type by name from the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="dynamicOnly">A value indicating whether only dynamic assemblies should be checked or not.</param>
        /// <returns>A <see cref="Type"/> representing the type if found; otherwise null.</returns>
        public static Type GetType(string typeName, bool dynamicOnly)
        {
            foreach (var ass in AssemblyCache.GetAssemblies())
            {
                if (dynamicOnly == false ||
                    ass.IsDynamic == true)
                {
                    Type type = ass.GetType(typeName);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a list of loaded assemblies.
        /// </summary>
        /// <returns>A list of assemblies.</returns>
        public static IEnumerable<Assembly> GetAssemblies(bool dynamicOnly = false)
        {
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var compiled =
                from lib in DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId)
                let ass = Assembly.Load(lib)
                select ass;

            return FilterAssemblies(compiled, dynamicOnly);
        }

        private static IEnumerable<Assembly> FilterAssemblies(
            IEnumerable<Assembly> list,
            bool dynamicOnly = false)
        {
            if (list == null)
            {
                yield break;
            }

            foreach (var assembly in list)
            {
                if (dynamicOnly == false ||
                    assembly.IsDynamic == true)
                {
                    yield return assembly;
                }
            }
        }
    }
}