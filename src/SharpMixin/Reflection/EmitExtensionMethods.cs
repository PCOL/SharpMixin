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

namespace SharpMixin.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using FluentIL;

    /// <summary>
    /// IL Emit Extension methods
    /// </summary>
    internal static class EmitExtensionMethods
    {
        /// <summary>
        /// Emits optimized IL to load parameters.
        /// </summary>
        /// <param name="methodIL">A <see cref="IEmitter"/> to use.</param>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to emit the parameters for.</param>
        /// <returns>The <see cref="IEmitter"/> to use.</returns>
        public static IEmitter EmitLoadParameters(this IEmitter methodIL, MethodInfo methodInfo)
        {
            return methodIL.EmitLoadParameters(methodInfo.GetParameters());
        }

        /// <summary>
        /// Emits optimized IL to load parameters.
        /// </summary>
        /// <param name="methodIL">A <see cref="IEmitter"/> to use.</param>
        /// <param name="parameters">The parameters loads to emit.</param>
        /// <returns>The <see cref="IEmitter"/> to use.</returns>
        public static IEmitter EmitLoadParameters(this IEmitter methodIL, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                methodIL.EmitLdArg(i);
            }

            return methodIL;
        }

        /// <summary>
        /// Emits optimized IL to load an argument.
        /// </summary>
        /// <param name="methodIL">A <see cref="IEmitter"/> to use.</param>
        /// <param name="index">The arguement index.</param>
        /// <returns>The <see cref="IEmitter"/> to use.</returns>
        public static IEmitter EmitLdArg(this IEmitter methodIL, int index)
        {
            if (index == 0)
            {
                methodIL.LdArg1();
            }
            else if (index == 1)
            {
                methodIL.LdArg2();
            }
            else if (index == 2)
            {
                methodIL.LdArg3();
            }
            else
            {
                methodIL.LdArg(index + 1);
            }

            return methodIL;
        }

        /// <summary>
        /// Implements the interface and any of it descendent interfaces.
        /// </summary>
        /// <param name="typeBuilder">A type builder.</param>
        /// <param name="interfaceType">The interface type.</param>
        /// <returns>The type builder.</returns>
        public static ITypeBuilder ImplementsInterfaces(this ITypeBuilder typeBuilder, Type interfaceType)
        {
            typeBuilder.Implements(interfaceType);

            Type[] implementedInterfaces = interfaceType.GetInterfaces();
            if (implementedInterfaces.IsNullOrEmpty() == false)
            {
                foreach (Type type in implementedInterfaces)
                {
                    typeBuilder.ImplementsInterfaces(type);
                }
            }

            return typeBuilder;
        }
    }
}