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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading.Tasks;
    using FluentIL;
    using Reflection;

    /// <summary>
    /// A factory class for creating "mixin" types.
    /// </summary>
    public class MixinTypeGenerator
        : IMixinTypeGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MixinTypeGenerator"/> class.
        /// </summary>
        public MixinTypeGenerator()
        {
        }

        /// <summary>
        /// Gets the name of a dynamic mixin type.
        /// </summary>
        /// <param name="mixinType">The mixin type.</param>
        /// <param name="baseTypes">The mixins base types.</param>
        /// <returns>The type name.</returns>
        public static string TypeName(Type mixinType, Type[] baseTypes)
        {
            return $"Dynamic.Mixins.{mixinType.Name}_{string.Join("_", baseTypes.Select(t => t.Name))}";
        }

        /// <summary>
        /// Creates a mixin.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="instances">The instances the mixin uses.</param>
        /// <returns>An instance of the mixin.</returns>
        public T CreateMixin<T>(params object[] instances)
        {
            Type[] baseTypes = new Type[instances.Length];
            for (int i = 0; i < instances.Length; i++)
            {
                baseTypes[i] = instances[i].GetType();
            }

            Type mixinType = this.CreateMixinType(typeof(T), baseTypes, null);

            return (T)Activator.CreateInstance(mixinType, new object[] { instances, null });
        }

        /// <summary>
        /// Creates a mixin type.
        /// </summary>
        /// <typeparam name="T">The mixin type to create.</typeparam>
        /// <param name="baseTypes">The base types that the mixin is made from.</param>
        /// <returns>A <see cref="Type"/> that represents the mixin.</returns>
        public Type GetOrCreateMixinType<T>(params Type[] baseTypes)
        {
            return this.CreateMixinType(typeof(T), baseTypes, null);
        }

        /// <summary>
        /// Creates an mixin type.
        /// </summary>
        /// <param name="mixinType">The interface type to implement on the mixin type.</param>
        /// <param name="baseTypes">The base types the mixin represents.</param>
        /// <param name="serviceProvider">A <see cref="IServiceProvider"/>.</param>
        /// <returns>A new mixin type.</returns>
        private Type CreateMixinType(
            Type mixinType,
            Type[] baseTypes,
            IServiceProvider serviceProvider)
        {
            Type actualType = TypeFactory
                .Default
                .GetType(TypeName(mixinType, baseTypes), true);

            if (actualType == null)
            {
                actualType = this.GenerateMixinType(mixinType, baseTypes, serviceProvider);
            }

            return actualType;
        }

        /// <summary>
        /// Generates the mixin type.
        /// </summary>
        /// <param name="mixinType">The interface type to implement.</param>
        /// <param name="baseTypes">The base types The mixin uses.</param>
        /// <param name="serviceProvider">The dependency injection scope.</param>
        /// <returns>A <see cref="Type"/> that represents the mixin.</returns>
        private Type GenerateMixinType(Type mixinType, Type[] baseTypes, IServiceProvider serviceProvider)
        {
            var typeBuilder = TypeFactory
                .Default
                .NewType(TypeName(mixinType, baseTypes))
                .Class()
                .Public()
                .Implements<IMixinObject>()
                .ImplementsInterfaces(mixinType);

            var instancesField = typeBuilder
                .NewField<object[]>("baseTypes")
                .Private();

            var serviceProviderField = typeBuilder
                .NewField<IServiceProvider>("serviceProvider")
                .Private();

            var context = new TypeFactoryContext(
                typeBuilder,
                mixinType,
                baseTypes,
                serviceProvider,
                instancesField,
                serviceProviderField);

            this.EmitMixinObjectInterface(typeBuilder, instancesField);

            this.ImplementInterfaces(context);

            // Add a constructor to the type.
            this.EmitConstructor(
                typeBuilder,
                mixinType,
                instancesField,
                serviceProviderField);

            // Create the type.
            return typeBuilder
                .CreateType();
        }

        /// <summary>
        /// Adds a constructor to the mixin type.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> use to construct the type.</param>
        /// <param name="mixinType">The mixin <see cref="Type"/> being created.</param>
        /// <param name="baseTypesField">The <see cref="FieldBuilder"/> which will hold the instances of the base types.</param>
        /// <param name="serviceProviderField">The <see cref="FieldBuilder"/> which will hold the instance of the dependency injection resolver.</param>
        private void EmitConstructor(
            ITypeBuilder typeBuilder,
            Type mixinType,
            IFieldBuilder baseTypesField,
            IFieldBuilder serviceProviderField)
        {
            var constructorBuilder = typeBuilder
                .NewConstructor()
                .Public()
                .HideBySig()
                .SpecialName()
                .RTSpecialName()
                .CallingConvention(CallingConventions.HasThis)
                .Param<object[]>("instances")
                .Param<IServiceProvider>("serviceProvider");

            constructorBuilder
                .Body()
                .LdArg0()
                .LdArg1()
                .StFld(baseTypesField)
                .LdArg0()
                .LdArg2()
                .StFld(serviceProviderField)
                .Ret();
        }

        /// <summary>
        /// Implements the <see cref="IMixinObject"/> interface on the mixin type.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> use to construct the type.</param>
        /// <param name="baseTypeField">The <see cref="FieldBuilder"/> which will hold the instances of the types that make up the mixin.</param>
        private void EmitMixinObjectInterface(
            ITypeBuilder typeBuilder,
            IFieldBuilder baseTypeField)
        {
            var propertyMixinObjects = typeBuilder
                .NewProperty<object[]>("MixinObjects")
                .Getter(m => m
                    .Public()
                    .Virtual()
                    .HideBySig()
                    .NewSlot()
                    .CallingConvention(CallingConventions.HasThis)
                    .Body(il => il
                        .LdArg0()
                        .LdFld(baseTypeField)
                        .Ret()));
        }

        /// <summary>
        /// Implements the mixin types interfaces on the new type.
        /// </summary>
        /// <param name="context">The current type factory context.</param>
        private void ImplementInterfaces(TypeFactoryContext context)
        {
            var propertyMethods = new Dictionary<string, IMethodBuilder>();

            foreach (var memberInfo in context.NewType.GetMembers())
            {
                if (memberInfo.MemberType == MemberTypes.Method)
                {
                    var methodInfo = (MethodInfo)memberInfo;
                    var methodArgs = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

                    if (methodInfo.ContainsGenericParameters == true)
                    {
                        var genericArguments = methodInfo.GetGenericArguments();

                        var methodBuilder = context
                            .TypeBuilder
                            .NewMethod(methodInfo.Name)
                            .Public()
                            .Virtual()
                            .Params(methodArgs)
                            .Returns(methodInfo.ReturnType);

                        methodBuilder
                            .NewGenericParameters(
                                genericArguments.Select(t => t.Name).ToArray(),
                                (gps) =>
                                {
                                    for (int m = 0; m < gps.Length; m++)
                                    {
                                        gps[m].Attributes = genericArguments[m].GetTypeInfo().GenericParameterAttributes;
                                    }
                                });

                        var methodIL = methodBuilder.Body();

                        if (context.NewType.GetMethod(methodInfo.Name, methodInfo.GetGenericArguments()) == null)
                        {
                            // Throw NotImplementedException
                            methodIL.ThrowException<NotImplementedException>("Not Implemented");
                            continue;
                        }

                        ILocal methodReturn = null;
                        if (methodInfo.ReturnType != typeof(void))
                        {
                            methodIL.DeclareLocal(methodInfo.ReturnType, out methodReturn);
                        }

                        methodIL
                            .LdArg0()
                            .LdFld(context.BaseObjectField)
                            .EmitLoadParameters(methodInfo);

                        MethodInfo callMethod1 = context.BaseObjectField.FieldType.GetMethod(memberInfo.Name, genericArguments);
                        MethodInfo callMethod = context.BaseObjectField.FieldType.GetMethod(memberInfo.Name, methodArgs).MakeGenericMethod(genericArguments);
                        methodIL
                            .CallVirt(callMethod1);

                        if (methodReturn != null)
                        {
                            methodIL
                                .StLoc(methodReturn)
                                .LdLoc(methodReturn);
                        }

                        methodIL.Ret();
                    }
                    else
                    {
                        string name = methodInfo.Name;
                        MethodAttributes attrs = methodInfo.Attributes & ~MethodAttributes.Abstract;
                        var methodBuilder = context
                            .TypeBuilder
                            .NewMethod(
                                methodInfo.Name,
                                attrs,
                                CallingConventions.HasThis,
                                methodInfo.ReturnType)
                            .Params(methodArgs);

                        var methodIL = methodBuilder.Body();

                        this.BuildMethod(context, methodInfo, methodIL, methodArgs);

                        if (methodInfo.IsProperty() == true)
                        {
                            propertyMethods.Add(methodInfo.Name, methodBuilder);
                        }
                    }
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    var propertyBuilder = context
                        .TypeBuilder
                        .NewProperty(memberInfo.Name, ((PropertyInfo)memberInfo).PropertyType)
                        .Attributes(PropertyAttributes.SpecialName);

                    if (propertyMethods.TryGetValue(memberInfo.PropertyGetName(), out IMethodBuilder getMethod) == true)
                    {
                        propertyBuilder.GetMethod = getMethod;
                    }

                    if (propertyMethods.TryGetValue(memberInfo.PropertySetName(), out IMethodBuilder setMethod) == true)
                    {
                        propertyBuilder.SetMethod = setMethod;
                    }
                }
            }
        }

        /// <summary>
        /// Implements a method.
        /// </summary>
        /// <param name="context">The mixin factories current context.</param>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> of the method being implemented.</param>
        /// <param name="methodIL">The methods <see cref="ILGenerator"/>.</param>
        /// <param name="methodArgs">An array containing the methods argument types.</param>
        private void BuildMethod(TypeFactoryContext context, MethodInfo methodInfo, IEmitter methodIL, Type[] methodArgs)
        {
            string targetMemberName = methodInfo.Name;
            Type targetStaticType = null;

            // Are we proxying a property?
            PropertyInfo propertyInfo = methodInfo.GetProperty();
            if (propertyInfo != null)
            {
                MixinImplAttribute implAttr = propertyInfo.GetCustomAttribute<MixinImplAttribute>();
                if (implAttr != null)
                {
                    targetMemberName = implAttr.TargetMemberName.IsNullOrEmpty() == false ? methodInfo.Name.Substring(0, 4) + implAttr.TargetMemberName : targetMemberName;
                    targetStaticType = implAttr.TargetStaticType;
                }
            }
            else
            {
                // The is a method.
                MixinImplAttribute implAttr = methodInfo.GetCustomAttribute<MixinImplAttribute>();
                if (implAttr != null)
                {
                    targetMemberName = implAttr.TargetMemberName.IsNullOrEmpty() == false ? implAttr.TargetMemberName : targetMemberName;
                    targetStaticType = implAttr.TargetStaticType;
                }
            }

            // Get the method being proxied.
            int index = -1;
            MethodInfo proxiedMethod = null;
            if (targetStaticType == null)
            {
                for (int i = 0; i < context.BaseTypes.Length; i++)
                {
                    proxiedMethod = context.BaseTypes[i].GetMethodWithParameters(targetMemberName, BindingFlags.Public | BindingFlags.Instance, methodInfo.GetParameters());
                    if (proxiedMethod != null)
                    {
                        index = i;
                        break;
                    }
                }
            }
            else
            {
                proxiedMethod = targetStaticType.GetMethodWithParameters(targetMemberName, BindingFlags.Public | BindingFlags.Static, methodInfo.GetParameters());
            }

            // Was the method found?
            if (proxiedMethod == null)
            {
                // No method found
                FieldInfo field = null;

                // Is the desired method is a property getter and there is public field with same name?
                if (methodInfo.IsPropertyGet() == true &&
                    (field = context.NewType.GetField(methodInfo.Name.Substring(4))) != null)
                {
                    methodIL
                        .DeclareLocal(proxiedMethod.ReturnType, out ILocal sourceValue)
                        .DeclareLocal(field.FieldType, out ILocal fieldValue)
                        .DeclareLocal(methodInfo.ReturnType, out ILocal returnValue)

                        .LdArg0()
                        .LdFld(context.BaseObjectField)
                        .LdcI4(index)
                        .LdElemRef()
                        .StLoc(sourceValue)

                        .LdLoc(sourceValue)
                        .LdFld(field)
                        .StLoc(fieldValue);

                    // Are the return types different?
                    if (field.FieldType != methodInfo.ReturnType)
                    {
                        // try casting...
                        methodIL
                            .LdLoc(fieldValue)
                            .CastClass(methodInfo.ReturnType)
                            .StLoc(returnValue);
                    }

                    methodIL
                        .LdLoc(fieldValue)
                        .Ret();
                }

                // Is the desired method is a property setter and there is public field with same name?
                else if (methodInfo.IsPropertySet() == true &&
                    (field = context.NewType.GetField(methodInfo.Name.Substring(4))) != null)
                {
                    methodIL
                        .LdArg0()
                        .LdFld(context.BaseObjectField)
                        .LdcI4(index)
                        .LdElemRef()

                        .LdArg1()
                        .StFld(field)
                        .Ret();
                }
                else
                {
                    // Unable to implement the desired method.
                    methodIL.ThrowException(typeof(NotImplementedException));
                }

                return;
            }

            ILocal methodReturn = null;
            if (methodInfo.ReturnType != typeof(void))
            {
                // Do the return types match?
                if (methodInfo.ReturnType != proxiedMethod.ReturnType)
                {
                    throw new MixinGenerationException("The returns types do not match or cannot be adapted");
                }

                // Declare locals
                methodIL.DeclareLocal(methodInfo.ReturnType, out methodReturn);

                // hmm
                // If this is a generic method changes it to a generic method of return type.
                // This is NOT robust enough needs to only change the signature if the proxied return
                // type is actual required in the generic definition.
                if (proxiedMethod.IsGenericMethodDefinition == true)
                {
                    proxiedMethod = proxiedMethod.MakeGenericMethod(methodInfo.ReturnType);
                }
            }

            // Is the proxied method static?
            if (proxiedMethod.IsStatic == true)
            {
                methodIL
                    .EmitLoadParameters(methodInfo)
                    .Call(proxiedMethod);
            }
            else
            {
                // Is the adapted type a class?
                if (context.BaseTypes[index].IsClass == true)
                {
                    methodIL
                        .LdArg0()
                        .LdFld(context.BaseObjectField)
                        .LdcI4(index)
                        .LdElemRef()
                        .EmitLoadParameters(methodInfo)
                        .CallVirt(proxiedMethod);
                }

                // Is the adapted type a value type?
                else if (context.BaseTypes[index].IsValueType == true)
                {
                    methodIL
                        .LdArg0()
                        .LdFlda(context.BaseObjectField)
                        .LdcI4(index)
                        .LdElemRef()
                        .EmitLoadParameters(methodInfo)
                        .Call(proxiedMethod);
                }
            }

            // Does the method expect a return value?
            if (methodReturn != null)
            {
                methodIL
                    .StLoc(methodReturn)
                    .LdLoc(methodReturn);
            }

            methodIL.Emit(OpCodes.Ret);
        }
    }
}
