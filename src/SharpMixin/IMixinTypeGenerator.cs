﻿/*
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

namespace SharpMixin
{
    using System;

    /// <summary>
    /// Defines the mixin factory interface.
    /// </summary>
    public interface IMixinTypeGenerator
    {
        /// <summary>
        /// Creates a mixin.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="instances">The instances the mixin uses.</param>
        /// <returns>An instance of the mixin type.</returns>
        T CreateMixin<T>(params object[] instances);

        /// <summary>
        /// Gets or creates a mixin type.
        /// </summary>
        /// <typeparam name="T">The mixin type.</typeparam>
        /// <param name="baseTypes">The mixins base types.</param>
        /// <returns>An instance of the mixin type.</returns>
        Type GetOrCreateMixinType<T>(params Type[] baseTypes);
    }
}
