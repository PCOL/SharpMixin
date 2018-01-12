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
    /// IL Emit Extension methods
    /// </summary>
    internal static class EmitExtensionMethods
    {
        /// <summary>
        /// A <see cref="ConstructorInfo"/> for the <c>object</c> default constructor.
        /// </summary>
        /// <returns></returns>
        private readonly static ConstructorInfo Object_Ctor = typeof(object).GetConstructor(Type.EmptyTypes);

        /// <summary>
        /// A <see cref="MethodInfo"/> for the <c>Type.GetTypeFromHandle()</c> method.
        /// </summary>
        private readonly static MethodInfo Type_GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");

        private readonly static MethodInfo MethodBase_GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });

        private readonly static MethodInfo MethodBase_GetMethodFromHandleGeneric = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });

        /// <summary>
        /// A <see cref="MethodInfo"/> for the <c>Object.GetType()</c> method.
        /// </summary>
        private readonly static MethodInfo Object_GetType = typeof(object).GetMethod("GetType");

        /// <summary>
        /// A <see cref="MethodInfo"/> for the <c>Type.GetType()</c> method.
        /// </summary>
        private readonly static MethodInfo Type_GetType = typeof(Type).GetMethod("GetType", new[] { typeof(string), typeof(bool) });

        /// <summary>
        /// A <see cref="MethodInfo"/> for the <c>Type.IsAssignableFrom()</c> method.
        /// </summary>
        private readonly static MethodInfo Type_IsAssignableFrom = typeof(Type).GetMethod("IsAssignableFrom");

        /// <summary>
        /// <see cref="IDisposable"/> Dispose <see cref="MethodInfo"/>
        /// </summary>
        private static readonly MethodInfo IDisposable_Dispose = typeof(IDisposable).GetMethod("Dispose");

        /// <summary>
        /// <see cref="string"/> Format <see cref="MethodInfo"/>
        /// </summary>
        private readonly static MethodInfo String_Format = typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object[]) });

        /// <summary>
        /// <see cref="Type"/> Get custom attributes
        /// </summary>
        private readonly static MethodInfo Type_GetCustomAttributes = typeof(Type).GetMethod("GetCustomAttributes", new Type[] { typeof(Type), typeof(bool) });

        /// <summary>
        /// <see cref="CustomAttributeExtensions.GetCustomAttribute(MemberInfo, Type)"/>
        /// </summary>
        private readonly static MethodInfo Type_GetCustomAttribute = typeof(CustomAttributeExtensions).GetMethod("GetCustomAttribute", new[] { typeof(MemberInfo), typeof(Type) });

        /// <summary>
        /// Perform a 'typeof()' style operation.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to emit the 'typeof()' for.</typeparam>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static LocalBuilder DeclareLocal<T>(this ILGenerator ilGen)
        {
            return ilGen.DeclareLocal(typeof(T));
        }

        /// <summary>
        /// Emits IL to call the object base constructor.
        /// </summary>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitCallBaseCtor(this ILGenerator ilGen)
        {
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Call, Object_Ctor);
            ilGen.Emit(OpCodes.Nop);
            return ilGen;
        }

        /// <summary>
        /// Perform a 'typeof()' style operation.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to emit the 'typeof()' for.</typeparam>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitTypeOf<T>(this ILGenerator ilGen)
        {
            return ilGen.EmitTypeOf(typeof(T));
        }

        /// <summary>
        /// Perform a 'typeof()' style operation.
        /// </summary>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <param name="type">The <see cref="Type"/> to emit the 'typeof()' for.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitTypeOf(this ILGenerator ilGen, Type type)
        {
            ilGen.Emit(OpCodes.Ldtoken, type);
            ilGen.Emit(OpCodes.Call, Type_GetTypeFromHandle);
            return ilGen;
        }

        /// <summary>
        /// Emits the IL to perform an 'IsAssignableFrom' operation.
        /// </summary>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <param name="from">The <see cref="Type"/> to check is assignable from.</param>
        /// <param name="local">A <see cref="LocalBuilder"/> to check.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitIsAssignableFrom<T>(this ILGenerator ilGen, LocalBuilder local)
        {
            return ilGen.EmitIsAssignableFrom(typeof(T), local);
        }

        /// <summary>
        /// Emits the IL to perform an 'IsAssignableFrom' operation.
        /// </summary>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <param name="from">The <see cref="Type"/> to check is assignable from.</param>
        /// <param name="local">A <see cref="LocalBuilder"/> to check.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitIsAssignableFrom(this ILGenerator ilGen, Type from, LocalBuilder local)
        {
            ilGen.EmitTypeOf(from);
            ilGen.Emit(OpCodes.Ldloca, local);
            ilGen.Emit(OpCodes.Constrained, local.LocalType);
            ilGen.Emit(OpCodes.Callvirt, Object_GetType);
            ilGen.Emit(OpCodes.Callvirt, Type_IsAssignableFrom);

            return ilGen;
        }

        /// <summary>
        /// Emits the IL to perform an 'IsAssignableFrom' operation.
        /// </summary>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <param name="from">The <see cref="Type"/> to check is assignable from.</param>
        /// <param name="to">A <see cref="Type"/> to check.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitIsAssignableFrom(this ILGenerator ilGen, Type from, Type to)
        {
            ilGen.EmitTypeOf(from);
            ilGen.EmitTypeOf(to);
            ilGen.Emit(OpCodes.Callvirt, Type_IsAssignableFrom);
            return ilGen;
        }

        /// <summary>
        /// Emit IL to get method.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to emit the 'typeof()' for.</typeparam>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitMethod(this ILGenerator ilGen, MethodInfo methodInfo)
        {
            ilGen.Emit(OpCodes.Ldtoken, methodInfo);
            ilGen.Emit(OpCodes.Call, MethodBase_GetMethodFromHandle);
            return ilGen;
        }

        /// <summary>
        /// Emit IL to get method.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to emit the 'typeof()' for.</typeparam>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitMethod(this ILGenerator ilGen, MethodInfo methodInfo, Type declaringType)
        {
            ilGen.Emit(OpCodes.Ldtoken, methodInfo);
            ilGen.Emit(OpCodes.Ldtoken, declaringType);
            ilGen.Emit(OpCodes.Call, MethodBase_GetMethodFromHandleGeneric);
            return ilGen;
        }

        /// <summary>
        /// Emits IL for 'using' pattern.
        /// </summary>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <param name="disposableObj">The disposable object.</param>
        /// <param name="generateBlock">The code block inside the using block.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator Using(this ILGenerator ilGen, LocalBuilder disposableObj, Action generateBlock)
        {
            Label endFinally = ilGen.DefineLabel();

            // Try
            Label beginBlock =  ilGen.BeginExceptionBlock();

            generateBlock();

            // Finally
            ilGen.BeginFinallyBlock();
            ilGen.Emit(OpCodes.Ldloc_S, disposableObj);
            ilGen.Emit(OpCodes.Brfalse_S, endFinally);
            ilGen.Emit(OpCodes.Ldloc_S, disposableObj);
            ilGen.Emit(OpCodes.Callvirt, IDisposable_Dispose);
            ilGen.Emit(OpCodes.Nop);
            ilGen.MarkLabel(endFinally);
            ilGen.EndExceptionBlock();

            return ilGen;
        }

        /// <summary>
        /// Emits IL to call the static Format method on the <see cref="string"/> object.
        /// </summary>
        /// <param name="ilGen">A <see cref="IEmitter"/> instance.</param>
        /// <param name="format">The format to use.</param>
        /// <param name="locals">An array of <see cref="LocalBuilder"/> to use.</param>
        /// <returns>The <see cref="IEmitter"/> instance.</returns>
        public static ILGenerator EmitStringFormat(this ILGenerator ilGen, string format, params LocalBuilder[] locals)
        {
            LocalBuilder localArray = ilGen.DeclareLocal<object>();

            ilGen.EmitArray(
                localArray,
                locals.Length,
                (index) =>
                {
                    ilGen.Emit(OpCodes.Ldloc_S, locals[index]);
                    if (locals[index].LocalType.IsValueType == true)
                    {
                        ilGen.Emit(OpCodes.Box, locals[index].LocalType);
                    }
                });

            ilGen.Emit(OpCodes.Ldstr, format);
            ilGen.Emit(OpCodes.Ldloc_S, localArray);
            ilGen.Emit(OpCodes.Call, String_Format);

            return ilGen;
        }

        /// <summary>
        /// Emits the IL to allocate and fill an array.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="localArray">The local to store the array in.</param>
        /// <param name="length">The size of the array.</param>
        /// <param name="action">The action to execute for each index in the array.</param>
        public static ILGenerator EmitArray(this ILGenerator ilGen, LocalBuilder localArray, int length, Action<int> action)
        {
            if (localArray.LocalType.IsArray == false)
            {
                throw new InvalidProgramException("The local array type is not an array");
            }

            ArrayBuilder arrayBuilder = new ArrayBuilder(ilGen, localArray.LocalType.GetElementType(), length, localArray);
            for (int i = 0; i < length; i++)
            {
                arrayBuilder.Set(i, action);
            }

            return ilGen;
        }

        /// <summary>
        /// Emits the IL to allocate and fill an array.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="arrayType">The <see cref="Type"/> to array to emit.</param>
        /// <param name="localArray">The local to store the array in.</param>
        /// <param name="length">The size of the array.</param>
        /// <param name="action">The action to execute for each index in the array.</param>
        public static ILGenerator EmitArray(this ILGenerator ilGen, Type arrayType, LocalBuilder localArray, int length, Action<int> action)
        {
            if (localArray.LocalType.IsArray == false)
            {
                throw new InvalidProgramException("The local array type is not an array");
            }

            ArrayBuilder arrayBuilder = new ArrayBuilder(ilGen, arrayType, length, localArray);
            for (int i = 0; i < length; i++)
            {
                arrayBuilder.Set(i, action);
            }

            return ilGen;
        }

        /// <summary>
        /// Emits the IL to allocate and fill an array.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="localArray">The local to store the array in.</param>
        /// <param name="localTypes">The local variables to add to the array.</param>
        public static ILGenerator EmitTypeArray(this ILGenerator ilGen, LocalBuilder localArray, params LocalBuilder[] localTypes)
        {
            if (localArray.LocalType.IsArray == false)
            {
                throw new InvalidProgramException("The local array type is not an array");
            }

            ArrayBuilder arrayBuilder = new ArrayBuilder(ilGen, typeof(Type), localTypes.Length, localArray);
            for (int i = 0; i < localTypes.Length; i++)
            {
                arrayBuilder.Set(i, () => ilGen.Emit(OpCodes.Ldloc_S, localTypes[i]));
            }

            return ilGen;
        }

        /// <summary>
        /// Emits the IL to allocate and fill an array.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="localArray">The local to store the array in.</param>
        /// <param name="types">The types to add to the array.</param>
        public static ILGenerator EmitTypeArray(this ILGenerator ilGen, LocalBuilder localArray, params Type[] types)
        {
            if (localArray.LocalType.IsArray == false)
            {
                throw new InvalidProgramException("The local array type is not an array");
            }

            ArrayBuilder arrayBuilder = new ArrayBuilder(ilGen, typeof(Type), types.Length, localArray);
            for (int i = 0; i < types.Length; i++)
            {
                arrayBuilder.Set(i, () => ilGen.EmitTypeOf(types[i]));
            }

            return ilGen;
        }

        /// <summary>
        /// Emits optimized IL to load parameters.
        /// </summary>
        /// <param name="methodIL">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to emit the parameters for.</param>
        public static void EmitLoadParameters(this ILGenerator methodIL, MethodInfo methodInfo)
        {
            methodIL.EmitLoadParameters(methodInfo.GetParameters());
        }

        /// <summary>
        /// Emits optimized IL to load parameters.
        /// </summary>
        /// <param name="methodIL">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="parameters">The parameters loads to emit.</param>
        public static void EmitLoadParameters(this ILGenerator methodIL, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                methodIL.EmitLdArg(i);
            }
        }

        /// <summary>
        /// Emits optimized IL to load an argument.
        /// </summary>
        /// <param name="methodIL">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="index">The arguement index.</param>
        public static void EmitLdArg(this ILGenerator methodIL, int index)
        {
            if (index == 0)
            {
                methodIL.Emit(OpCodes.Ldarg_1);
            }
            else if (index == 1)
            {
                methodIL.Emit(OpCodes.Ldarg_2);
            }
            else if (index == 2)
            {
                methodIL.Emit(OpCodes.Ldarg_3);
            }
            else
            {
                methodIL.Emit(OpCodes.Ldarg, index + 1);
            }
        }

        /// <summary>
        /// Emits the IL to get a custom attribute from a type.
        /// </summary>
        /// <typeparam name="T">The type of custom attribute to get.</typeparam>
        /// <param name="ilGen">The methods <see cref="ILGenerator"/>.</param>
        /// <param name="type">The type to get the attribute from.</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator EmitGetCustomAttribute<T>(
            this ILGenerator ilGen,
            Type type)
        {
            return ilGen.EmitGetCustomAttribute(type, typeof(T));
        }

        /// <summary>
        /// Emits the IL to get a custom attribute from a type.
        /// </summary>
        /// <param name="ilGen">The methods <see cref="ILGenerator"/>.</param>
        /// <param name="type">The type to get the attribute from.</param>
        /// <param name="attributeType">The type of custom attribute to get.</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator EmitGetCustomAttribute(
            this ILGenerator ilGen,
            Type type,
            Type attributeType)
        {
            ilGen.EmitTypeOf(type);
            ilGen.EmitTypeOf(attributeType);
            ilGen.Emit(OpCodes.Call, typeof(CustomAttributeExtensions).GetMethod("GetCustomAttribute", new[] { typeof(MemberInfo), typeof(Type) }));
            return ilGen;
        }

        /// <summary>
        /// Gets the custom attributes for an object instance.
        /// </summary>
        /// <typeparam name="T">The type of custom attribute to get.</typeparam>
        /// <param name="ilGen">The methods <see cref="ILGenerator"/>.</param>
        /// <param name="local">A <see cref="LocalBuilder"/> containg the object instance.</param>
        /// <param name="inherited">A value indicating whether to get inherite attributes.</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator EmitGetCustomAttributes<T>(
            this ILGenerator ilGen,
            LocalBuilder local,
            bool inherited)
            where T : Attribute
        {
            ilGen.Emit(OpCodes.Ldloc, local);
            ilGen.EmitTypeOf<T>();
            ilGen.Emit(inherited == true ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Callvirt, Type_GetCustomAttributes);
            return ilGen;
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <param name="methodIL">An <see cref="ILGenerator"/>> instance.</param>
        /// <param name="message">The exception message</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator ThrowException<TException>(
            this ILGenerator methodIL,
            string message)
            where TException : Exception
        {
            ConstructorInfo ctor =
                typeof(TException)
                .GetConstructor(new[] { typeof(string) });

            if (ctor == null)
            {
                throw new ArgumentException("Type TException does not have a public constructor that takes a string argument");
            }

            methodIL.Emit(OpCodes.Ldstr, message);
            methodIL.Emit(OpCodes.Newobj, ctor);
            methodIL.Emit(OpCodes.Throw);

            return methodIL;
        }

        /// <summary>
        /// Emits IL to load the type for a given type onto the evaluation stack.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator EmitGetType(this ILGenerator ilGen)
        {
            ilGen.Emit(OpCodes.Callvirt, Object_GetType);
            return ilGen;
        }

        /// <summary>
        /// Emits IL to load the type for a given type name onto the evaluation stack.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="typeName">The <see cref="LocalBuilder"/> containing the type name.</param>
        /// <param name="dynamicOnly">A value indicating whether or not to only check for dynamically generated types.</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator EmitGetType(this ILGenerator ilGen, LocalBuilder typeNameLocal, bool dynamicOnly = false)
        {
            ilGen.Emit(OpCodes.Ldloc_S, typeNameLocal);
            ilGen.Emit(OpCodes.Ldc_I4, dynamicOnly == false ? 0 : 1);
            ilGen.Emit(OpCodes.Call, Type_GetType);
            return ilGen;
        }

        /// <summary>
        /// Emits IL to load the type for a given type name onto the evaluation stack.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="dynamicOnly">A value indicating whether or not to only check for dynamically generated types.</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator EmitGetType(
            this ILGenerator ilGen,
            string typeName,
            bool dynamicOnly = false)
        {
            ilGen.Emit(OpCodes.Ldstr, typeName);
            ilGen.Emit(OpCodes.Ldc_I4, dynamicOnly == false ? 0 : 1);
            ilGen.Emit(OpCodes.Call, Type_GetType);
            return ilGen;
        }

        /// <summary>
        /// Emits IL to box a value type.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="boxType">The box type.</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator EmitBox(this ILGenerator ilGen, Type boxType)
        {
            if (boxType.IsValueType == true)
            {
                ilGen.Emit(OpCodes.Box, boxType);
            }

            return ilGen;
        }

        /// <summary>
        /// Emits IL to unbox a value type.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="boxType">The box type.</param>
        /// <returns>The <see cref="ILGenerator"/> instance</returns>
        public static ILGenerator EmitUnbox(this ILGenerator ilGen, Type boxType)
        {
            if (boxType.IsValueType == true)
            {
                ilGen.Emit(OpCodes.Unbox, boxType);
            }

            return ilGen;
        }

        /// <summary>
        /// Emits IL to store a value in an out/ref parameter
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="arg">The argument to store the value in.</param>
        /// <param name="localValue">A <see cref="LocalBuilder"/> containing the value to store.</param>
        /// <param name="conversion">Option action to put value conversion code.</param>
        /// <returns>The passed in <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitStoreByRefArg(
            this ILGenerator ilGen,
            int arg,
            LocalBuilder localValue,
            Action conversion = null)
        {
            ilGen.Emit(OpCodes.Ldarg, arg);
            ilGen.Emit(OpCodes.Ldloc, localValue);
            conversion?.Invoke();
            ilGen.Emit(OpCodes.Stind_Ref);
            return ilGen;
        }

        /// <summary>
        /// Emits IL to convert one type to another.
        /// </summary>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <param name="sourceType">The source type.</param>
        /// <param name="targetType">The destination type.</param>
        /// <param name="isAddress">A value indicating whether or not the convert is for an address.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitConv(
            this ILGenerator ilGen,
            Type sourceType,
            Type targetType,
            bool isAddress)
        {
            if (sourceType != targetType)
            {
                if (sourceType.IsByRef == true)
                {
                    Type elementType = sourceType.GetElementType();
                    ilGen.EmitLdInd(elementType);
                    ilGen.EmitConv(elementType, targetType, isAddress);
                }
                else if (targetType.IsValueType == true)
                {
                    if (sourceType.IsValueType == true)
                    {
                        ilGen.EmitConv(targetType);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Unbox, targetType);
                        if (isAddress == false)
                        {
                            ilGen.EmitLdInd(targetType);
                        }
                    }
                }
                else if (targetType.IsAssignableFrom(sourceType) == true)
                {
                    if (sourceType.IsValueType == true)
                    {
                        if (isAddress == true)
                        {
                            ilGen.EmitLdInd(sourceType);
                        }

                        ilGen.Emit(OpCodes.Box, sourceType);
                    }
                }
                else if (targetType.IsGenericParameter == true)
                {
                    ilGen.Emit(OpCodes.Unbox_Any, targetType);
                }
                else
                {
                    ilGen.Emit(OpCodes.Castclass, targetType);
                }
            }

            return ilGen;
        }

        /// <summary>
        /// Emits IL to convert a type.
        /// </summary>
        /// <param name="ilGen">A <see cref="ILGenerator"/> instance.</param>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="ILGenerator"/> instance.</returns>
        public static ILGenerator EmitConv(this ILGenerator ilGen, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                    ilGen.Emit(OpCodes.Conv_I1);
                    break;

                case TypeCode.Char:
                case TypeCode.Int16:
                    ilGen.Emit(OpCodes.Conv_I2);
                    break;

                case TypeCode.Byte:
                    ilGen.Emit(OpCodes.Conv_U2);
                    break;

                case TypeCode.Int32:
                    ilGen.Emit(OpCodes.Conv_I4);
                    break;

                case TypeCode.UInt32:
                    ilGen.Emit(OpCodes.Conv_U4);
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    ilGen.Emit(OpCodes.Conv_I8);
                    break;

                case TypeCode.Single:
                    ilGen.Emit(OpCodes.Conv_R4);
                    break;

                case TypeCode.Double:
                    ilGen.Emit(OpCodes.Conv_R8);
                    break;

                default:
                    ilGen.Emit(OpCodes.Nop, type);
                    break;
            }

            return ilGen;
        }

        /// <summary>
        /// Emits the IL to indirectly load a value onto the evaluation stack.
        /// </summary>
        /// <param name="ilGen">A <see cref="IEmitter"/> instance.</param>
        /// <param name="type">The type to load.</param>
        /// <returns>The <see cref="IEmitter"/> instance.</returns>
        public static ILGenerator EmitLdInd(this ILGenerator ilGen, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                    ilGen.Emit(OpCodes.Ldind_I1);
                    break;

                case TypeCode.Char:
                case TypeCode.Int16:
                    ilGen.Emit(OpCodes.Ldind_I2);
                    break;

                case TypeCode.Byte:
                    ilGen.Emit(OpCodes.Ldind_U2);
                    break;

                case TypeCode.Int32:
                    ilGen.Emit(OpCodes.Ldind_I4);
                    break;

                case TypeCode.UInt32:
                    ilGen.Emit(OpCodes.Ldind_U4);
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    ilGen.Emit(OpCodes.Ldind_I8);
                    break;

                case TypeCode.Single:
                    ilGen.Emit(OpCodes.Ldind_R4);
                    break;

                case TypeCode.Double:
                    ilGen.Emit(OpCodes.Ldind_R8);
                    break;

                default:
                    ilGen.Emit(OpCodes.Ldobj, type);
                    break;
            }

            return ilGen;
        }

        /// <summary>
        /// Emits IL to check if the object on the top of the evaluation stack is not null, executing the emitted body if not.
        /// </summary>
        /// <param name="ilGen">The <see cref="ILGenerator"/> to use.</param>
        /// <param name="emitBody">A function to emit the IL to be executed if the object is not null.</param>
        /// <param name="emitElse">A function to emit the IL to be executed if the object is null.</param>
        public static ILGenerator EmitIfNotNull(
            this ILGenerator ilGen,
            Action emitBody,
            Action emitElse = null)
        {
            Label endIf = ilGen.DefineLabel();

            if (emitElse != null)
            {
                Label notNull = ilGen.DefineLabel();
                ilGen.Emit(OpCodes.Brtrue, notNull);
                ilGen.Emit(OpCodes.Nop);
                emitElse();
                ilGen.Emit(OpCodes.Br, endIf);
                ilGen.MarkLabel(notNull);
            }
            else
            {
                ilGen.Emit(OpCodes.Brfalse, endIf);
            }

            emitBody();
            ilGen.MarkLabel(endIf);

            return ilGen;
        }

        /// <summary>
        /// Emits IL to perform a for loop over an array without element loading.
        /// </summary>
        /// <param name="ilGen">An IL generator.</param>
        /// <param name="localLength">The local variable holding the length.</param>
        /// <param name="action">An action to allow the injecting of the loop code.</param>
        public static ILGenerator EmitFor(
            this ILGenerator ilGen,
            LocalBuilder localLength,
            Action<LocalBuilder> action)
        {
            Label beginLoop = ilGen.DefineLabel();
            Label loopCheck = ilGen.DefineLabel();

            LocalBuilder index = ilGen.DeclareLocal(typeof(int));

            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Stloc, index);
            ilGen.Emit(OpCodes.Br, loopCheck);
            ilGen.MarkLabel(beginLoop);

            action(index);

            ilGen.Emit(OpCodes.Nop);
            ilGen.Emit(OpCodes.Ldloc, index);
            ilGen.Emit(OpCodes.Ldc_I4_1);
            ilGen.Emit(OpCodes.Add);
            ilGen.Emit(OpCodes.Stloc, index);

            ilGen.MarkLabel(loopCheck);
            ilGen.Emit(OpCodes.Ldloc, index);
            ilGen.Emit(OpCodes.Ldloc, localLength);
            ilGen.Emit(OpCodes.Blt, beginLoop);

            return ilGen;
        }

        /// <summary>
        /// Emits IL to perform a for loop over an array with element loading.
        /// </summary>
        /// <param name="ilGen">An IL generator.</param>
        /// <param name="localArray">The local variable holding the array.</param>
        /// <param name="action">An action to allow the injecting of the loop code.</param>
        public static ILGenerator EmitFor(
            this ILGenerator ilGen,
            LocalBuilder localArray,
            Action<LocalBuilder, LocalBuilder> action)
        {
            LocalBuilder itemLocal = ilGen.DeclareLocal(localArray.LocalType.GetElementType());
            LocalBuilder lengthLocal = ilGen.DeclareLocal(typeof(int));

            ilGen.Emit(OpCodes.Ldloc, localArray);
            ilGen.Emit(OpCodes.Ldlen);
            ilGen.Emit(OpCodes.Conv_I4);
            ilGen.Emit(OpCodes.Stloc_S, lengthLocal);

            return ilGen.EmitFor(
                lengthLocal,
                (index) =>
                {
                    ilGen.Emit(OpCodes.Ldloc, localArray);
                    ilGen.Emit(OpCodes.Ldloc, index);
                    ilGen.Emit(OpCodes.Ldelem_Ref);
                    ilGen.Emit(OpCodes.Stloc, itemLocal);
                    ilGen.Emit(OpCodes.Nop);

                    action(index, itemLocal);
                });
        }
    }
}