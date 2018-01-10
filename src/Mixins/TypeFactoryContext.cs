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

namespace Mixins
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Represent contextual data used by <see cref="TypeFactory"/> implementations.
    /// </summary>
    internal class TypeFactoryContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFactoryContext"/> class.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> being use to create the type.</param>
        /// <param name="newType">The new type being built.</param>
        /// <param name="baseType">The base type being built on.</param>
        /// <param name="dependencyScope">The current dependency injection scope</param>
        /// <param name="baseObjectField">The <see cref="FieldBuilder"/> that holds the base type instance.</param>
        /// <param name="serviceProviderField">The <see cref="FieldBuilder"/> that holds the <see cref="IDependencyResolver"/> instance.</param>
        /// <param name="ctorBuilder">The <see cref="ConstructorBuilder"/> for the types constructor.</param>
        public TypeFactoryContext(
            TypeBuilder typeBuilder,
            Type newType,
            Type baseType,
            IServiceProvider dependencyScope,
            FieldBuilder baseObjectField,
            FieldBuilder serviceProviderField,
            ConstructorBuilder ctorBuilder = null)
            : this(typeBuilder,
            newType,
            new Type[] { baseType },
            dependencyScope,
            baseObjectField,
            serviceProviderField,
            ctorBuilder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFactoryContext"/> class.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> being use to create the type.</param>
        /// <param name="newType">The new type being built.</param>
        /// <param name="baseTypes">The base types being built on.</param>
        /// <param name="dependencyScope">The current dependency injection scope</param>
        /// <param name="baseObjectField">The <see cref="FieldBuilder"/> that holds the base type instance.</param>
        /// <param name="serviceProviderField">The <see cref="FieldBuilder"/> that holds the <see cref="IDependencyResolver"/> instance.</param>
        /// <param name="ctorBuilder">The <see cref="ConstructorBuilder"/> for the Itypes constructor.</paramI>
        public TypeFactoryContext(
            TypeBuilder typeBuilder,
            Type newType,
            Type[] baseTypes,
            IServiceProvider dependencyScope,
            FieldBuilder baseObjectField,
            FieldBuilder serviceProviderField,
            ConstructorBuilder ctorBuilder = null)
        {
            this.TypeBuilder = typeBuilder;
            this.NewType = newType;
            this.BaseTypes = baseTypes;
            this.ServiceProvider = dependencyScope;
            this.BaseObjectField = baseObjectField;
            this.ServiceProviderField = serviceProviderField;
            this.ConstructorBuilder = ctorBuilder;
        }

        /// <summary>
        ///  Gets the <see cref="TypeBuilder"/>
        /// </summary>
        public TypeBuilder TypeBuilder { get; }

        /// <summary>
        /// Gets the new type.
        /// </summary>
        public Type NewType { get; }

        /// <summary>
        /// Gets the base type.
        /// </summary>
        public Type BaseType
        {
            get
            {
                return this.BaseTypes[0];
            }
        }

        /// <summary>
        /// Gets the base types.
        /// </summary>
        public Type[] BaseTypes { get; }

        /// <summary>
        /// Gets the current dependency injection scope.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the <see cref="FieldBuilder"/> which will contain the base object instance.
        /// </summary>
        public FieldBuilder BaseObjectField { get; }

        /// <summary>
        /// Gets the <see cref="FieldBuilder"/> which will contain the dependency injection resolver.
        /// </summary>
        public FieldBuilder ServiceProviderField { get; }

        /// <summary>
        /// Gets the <see cref="ConstructorBuilder"/> used to construct the new object.
        /// </summary>
        public ConstructorBuilder ConstructorBuilder { get; }

        /// <summary>
        /// Creates a new <see cref="TypeFactoryContext"/> instance for a new interface type.
        /// </summary>
        /// <param name="interfaceType">The duck <see cref="Type"/>.</param>
        /// <returns>The new <see cref="TypeFactoryContext"/> instance.</returns>
        public TypeFactoryContext CreateTypeFactoryContext(Type interfaceType)
        {
            var context = new TypeFactoryContext(this.TypeBuilder, interfaceType, this.BaseTypes, this.ServiceProvider, this.BaseObjectField, this.ServiceProviderField, this.ConstructorBuilder);
            return context;
        }

        /// <summary>
        /// Does the type builder implement a given interface type
        /// </summary>
        /// <param name="ifaceType">Interface type.</param>
        /// <returns>True if it does; otherwise false.</returns>
        public bool DoesTypeBuilderImplementInterface(Type ifaceType)
        {
            return this.TypeBuilder.GetInterfaces().FirstOrDefault((type) => ifaceType == type) != null;
        }

        // /// <summary>
        // /// Gets the adapter constructor.
        // /// </summary>
        // /// <param name="targetType">The target type.</param>
        // /// <param name="adpaterType">The adapter type</param>
        // /// <returns></returns>
        // public ConstructorInfo GetAdapterConstructor(Type targetType, Type adpaterType)
        // {
        //     if (this.DoesTypeBuilderImplementInterface(adpaterType) == true)
        //     {
        //         return this.ConstructorBuilder;
        //     }

        //     // We need to create a new adapted object.
        //     Type adapterType = targetType.CreateDuckType()
        //         adpaterType,
        //         this.ServiceProvider);

        //     return adapterType.GetConstructor(new[] { targetType, typeof(IServiceProvider) });
        // }
    }
}
