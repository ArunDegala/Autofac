﻿// This software is part of the Autofac IoC container
// Copyright (c) 2007 - 2009 Autofac Contributors
// http://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Autofac.Core;
using Autofac.Util;

namespace Autofac.Builder
{
    /// <summary>
    /// Used with the WithExtendedProperties configuration method to
    /// associate key-value pairs with an <see cref="IComponentRegistration"/>.
    /// </summary>
    /// <typeparam name="TMetadata">Interface with properties whose names correspond to
    /// the property keys.</typeparam>
    /// <remarks>This feature was suggested by OJ Reeves (@TheColonial).</remarks>
    public class ExtendedPropertyConfiguration<TMetadata>
    {
        readonly IDictionary<string, object> _properties = new Dictionary<string, object>();

        internal IEnumerable<KeyValuePair<string, object>> Properties { get { return _properties; } }

        /// <summary>
        /// Set one of the property values.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyAccessor">An expression that accesses the property to set.</param>
        /// <param name="value">The property value to set.</param>
        public void For<TProperty>(Expression<Func<TMetadata, TProperty>> propertyAccessor, TProperty value)
        {
            Enforce.ArgumentNotNull(propertyAccessor, "propertyAccessor");
            var pn = ReflectionExtensions.GetProperty(propertyAccessor).Name;
            _properties.Add(pn, value);
        }
    }
}
