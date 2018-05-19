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
    using System.Reflection;
    using System.Reflection.Emit;
    using FluentIL;

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
        /// <param name="baseTypes">The base types being built on.</param>
        /// <param name="serviceProvider">The current dependency injection service provider.</param>
        /// <param name="baseObjectField">The <see cref="FieldBuilder"/> that holds the base type instance.</param>
        /// <param name="serviceProviderField">The <see cref="FieldBuilder"/> that holds the <see cref="IServiceProvider"/> instance.</param>
        /// <param name="ctorBuilder">The <see cref="ConstructorBuilder"/> for the Itypes constructor.</paramI>
        public TypeFactoryContext(
            ITypeBuilder typeBuilder,
            Type newType,
            Type[] baseTypes,
            IServiceProvider serviceProvider,
            IFieldBuilder baseObjectField,
            IFieldBuilder serviceProviderField,
            IConstructorBuilder ctorBuilder = null)
        {
            this.TypeBuilder = typeBuilder;
            this.NewType = newType;
            this.BaseTypes = baseTypes;
            this.ServiceProvider = serviceProvider;
            this.BaseObjectField = baseObjectField;
            this.ServiceProviderField = serviceProviderField;
            this.ConstructorBuilder = ctorBuilder;
        }

        /// <summary>
        ///  Gets the <see cref="TypeBuilder"/>
        /// </summary>
        public ITypeBuilder TypeBuilder { get; }

        /// <summary>
        /// Gets the new type.
        /// </summary>
        public Type NewType { get; }

        /// <summary>
        /// Gets the base types.
        /// </summary>
        public Type[] BaseTypes { get; }

        /// <summary>
        /// Gets the current dependency injection service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the <see cref="FieldBuilder"/> which will contain the base object instance.
        /// </summary>
        public IFieldBuilder BaseObjectField { get; }

        /// <summary>
        /// Gets the <see cref="FieldBuilder"/> which will contain the dependency injection resolver.
        /// </summary>
        public IFieldBuilder ServiceProviderField { get; }

        /// <summary>
        /// Gets the <see cref="ConstructorBuilder"/> used to construct the new object.
        /// </summary>
        public IConstructorBuilder ConstructorBuilder { get; }

        /// <summary>
        /// Creates a new <see cref="TypeFactoryContext"/> instance for a new interface type.
        /// </summary>
        /// <param name="interfaceType">The mixin <see cref="Type"/>.</param>
        /// <returns>The new <see cref="TypeFactoryContext"/> instance.</returns>
        public TypeFactoryContext CreateTypeFactoryContext(Type interfaceType)
        {
            return new TypeFactoryContext(
                this.TypeBuilder,
                interfaceType,
                this.BaseTypes,
                this.ServiceProvider,
                this.BaseObjectField,
                this.ServiceProviderField,
                this.ConstructorBuilder);
        }
    }
}
