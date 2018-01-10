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
    using System.Reflection.Emit;

    /// <summary>
    /// Generates the IL for handling an array.
    /// </summary>
    internal class ArrayBuilder
    {
        /// <summary>
        /// The <see cref="ILGenerator"/> to use.
        /// </summary>
        private readonly ILGenerator ilGen;

        /// <summary>
        /// A local varaible to hold the array.
        /// </summary>
        private LocalBuilder localArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayBuilder"/> class.
        /// </summary>
        /// <param name="ilGen">The IL generator.</param>
        /// <param name="arrayType">The type of array.</param>
        /// <param name="length">The length of the array.</param>
        /// <param name="localArray">Optional local variable.</param>
        public ArrayBuilder(ILGenerator ilGen, Type arrayType, int length, LocalBuilder localArray = null)
        {
            if (localArray != null &&
                localArray.LocalType.IsArray == false)
            {
                throw new InvalidProgramException("The local array type is not an array");
            }

            this.ilGen = ilGen;
            this.localArray = localArray;
            if (this.localArray == null)
            {
                this.localArray = this.ilGen.DeclareLocal(arrayType.MakeArrayType());
            }

            // Create the argument types array.
            this.ilGen.Emit(OpCodes.Ldc_I4, length);
            this.ilGen.Emit(OpCodes.Newarr, arrayType);
            this.ilGen.Emit(OpCodes.Stloc_S, this.localArray);
        }

        /// <summary>
        /// Emits the IL to start a set operation for the given index.
        /// </summary>
        /// <param name="index">The array index to set.</param>
        public void SetStart(int index)
        {
            this.ilGen.Emit(OpCodes.Ldloc_S, this.localArray);
            this.ilGen.Emit(OpCodes.Ldc_I4, index);
        }

        /// <summary>
        /// Emits the IL to store the element.
        /// </summary>
        public void SetEnd()
        {
            this.ilGen.Emit(OpCodes.Stelem_Ref);
        }

        /// <summary>
        /// Emits the IL to set an element of an array.
        /// </summary>
        /// <param name="index">The index of the element to set.</param>
        /// <param name="action">The action to call to emit the set code.</param>
        public void Set(int index, Action action)
        {
            this.SetStart(index);

            if (action != null)
            {
                action();
            }

            this.SetEnd();
        }

        /// <summary>
        /// Emits the IL to set an element of an array.
        /// </summary>
        /// <param name="index">The index of the element to set.</param>
        /// <param name="action">The action to call to emit the set code.</param>
        public void Set(int index, Action<int> action)
        {
            this.SetStart(index);

            if (action != null)
            {
                action(index);
            }

            this.SetEnd();
        }

        /// <summary>
        /// Emits the IL to load the given array element onto the evaluation stack.
        /// </summary>
        /// <param name="index">The index of the element to load.</param>
        public void Get(int index)
        {
            this.ilGen.Emit(OpCodes.Ldloc_S, this.localArray);
            this.ilGen.Emit(OpCodes.Ldc_I4, index);
            this.ilGen.Emit(OpCodes.Ldelem_Ref);
        }

        /// <summary>
        /// Emits the IL to load the array onto the evaluation stack.
        /// </summary>
        public void Load()
        {
            this.ilGen.Emit(OpCodes.Ldloc_S, this.localArray);
        }
    }
}
