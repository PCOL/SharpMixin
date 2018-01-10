/*
MIT License

Copyright (c) 2017 P Collyer

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
    /// Property extension methods.
    /// </summary>
    internal static class PropertyExtensionMethods
    {
        /// <summary>
        /// The get property methods name prefix.
        /// </summary>
        private const string GetMethodPrefix = "get_";

        /// <summary>
        /// The set property methods name prefix.
        /// </summary>
        private const string SetMethodPrefix = "set_";

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="MethodInfo"/> is a property method.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>True if the <see cref="MethodInfo"/> is a property method; otherwise false.</returns>
        public static bool IsProperty(this MethodInfo methodInfo)
        {
            return methodInfo.IsPropertyGet() || methodInfo.IsPropertySet();
        }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="MethodInfo"/> is a property get method.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>True if the <see cref="MethodInfo"/> is a property get method; otherwise false.</returns>
        public static bool IsPropertyGet(this MethodInfo methodInfo)
        {
            return methodInfo != null && methodInfo.Name.StartsWith(GetMethodPrefix);
        }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="MethodInfo"/> is a property set method.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>True if the <see cref="MethodInfo"/> is a property set method; otherwise false.</returns>
        public static bool IsPropertySet(this MethodInfo methodInfo)
        {
            return methodInfo != null && methodInfo.Name.StartsWith(SetMethodPrefix);
        }

        /// <summary>
        /// Returns the name of the get method if the given <see cref="MemberInfo"/> is a property.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns>The name of the get method if the <see cref="MemberInfo"/> is a property; otherwise null.</returns>
        public static string PropertyGetName(this MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                if (memberInfo is PropertyInfo)
                {
                    return $"{GetMethodPrefix}{memberInfo.Name}";
                }
                else if (memberInfo is MethodInfo &&
                    ((MethodInfo)memberInfo).IsPropertyGet())
                {
                    return memberInfo.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the set method if the given <see cref="MemberInfo"/> is a property.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns>The name of the set method if the <see cref="MemberInfo"/> is a property; otherwise null.</returns>
        public static string PropertySetName(this MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                if (memberInfo is PropertyInfo)
                {
                    return $"{SetMethodPrefix}{memberInfo.Name}";
                }
                else if (memberInfo is MethodInfo &&
                    ((MethodInfo)memberInfo).IsPropertySet())
                {
                    return memberInfo.Name;
                }
            }

            return null;
        }

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

        /// <summary>
        /// Emits IL to load the contents of a property onto the evaluation stack.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="local">The <see cref="LocalBuilder"/> that contains the property to read.</param>
        /// <returns>The <see cref="ILGenerator"/>.</returns>
        public static ILGenerator EmitGetProperty(this ILGenerator ilGen, string propertyName, LocalBuilder local)
        {
            ilGen.Emit(OpCodes.Ldloc, local);
            ilGen.EmitGetProperty(propertyName, local.LocalType);
            return ilGen;
        }

        /// <summary>
        /// Emits IL to load the contents of a property onto the evaluation stack.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The <see cref="ILGenerator"/>.</returns>
        public static ILGenerator EmitGetProperty(this ILGenerator ilGen, string propertyName, Type propertyType)
        {
            ilGen.Emit(OpCodes.Callvirt, propertyType.GetProperty(propertyName).GetGetMethod());
            return ilGen;
        }

        /// <summary>
        /// Emits IL to pass the value on the top of set evaluation stack to a property set method.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="local">The <see cref="LocalBuilder"/> that contains the property to set.</param>
        /// <returns>The <see cref="ILGenerator"/>.</returns>
        public static ILGenerator EmitSetProperty(this ILGenerator ilGen, string propertyName, LocalBuilder local)
        {
            MethodInfo setMethod = local.LocalType.GetMethod($"{SetMethodPrefix}{propertyName}");
            ilGen.Emit(OpCodes.Ldloc_S, local);
            ilGen.Emit(OpCodes.Callvirt, setMethod);
            return ilGen;
        }
    }
}