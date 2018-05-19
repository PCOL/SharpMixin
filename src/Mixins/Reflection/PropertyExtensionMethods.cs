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

namespace Mixins.Reflection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using FluentIL;

    /// <summary>
    /// Property extension methods.
    /// </summary>
    internal static class PropertyExtensionMethods
    {
        /// <summary>
        /// Gets the property for a property method.
        /// </summary>
        /// <param name="methodInfo">A <see cref="MethodInfo"/>.</param>
        /// <returns>A <see cref="PropertyInfo"/> if found; otherwise null.</returns>
        public static PropertyInfo GetProperty(this MethodInfo methodInfo)
        {
            Type[] types = Type.EmptyTypes;
            Type returnType = methodInfo.ReturnType;
            var parms = methodInfo.GetParameters();
            if (parms.Length > 0)
            {
                var parmTypes = parms.Select(p => p.ParameterType);

                if (methodInfo.IsPropertySet() == true)
                {
                    returnType = parmTypes.Last();
                    parmTypes = parmTypes.Take(parms.Length - 1);
                }

                types = parmTypes.ToArray();
            }

            return methodInfo.DeclaringType.GetProperty(
                methodInfo.Name.Substring(4),
                returnType,
                types);
        }
    }
}