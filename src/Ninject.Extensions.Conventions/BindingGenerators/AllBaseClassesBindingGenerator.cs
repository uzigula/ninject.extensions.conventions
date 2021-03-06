﻿//-------------------------------------------------------------------------------
// <copyright file="AbstractClassWithManyInterfaces.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2013 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Extensions.Conventions.BindingGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
	using Ninject.Syntax;

    /// <summary>
    /// Binds a type to any non-Object base types in its inheritance tree.
    /// </summary>
    public class AllBaseClassesBindingGenerator : IBindingGenerator
    {
        /// <summary>
        /// Creates the bindings for a type.
        /// </summary>
        /// <param name="type">The type for which the bindings are created.</param>
        /// <param name="bindingRoot">The binding root that is used to create the bindings.</param>
        /// <returns>
        /// The syntaxes for the created bindings to configure more options.
        /// </returns>
        public IEnumerable<IBindingWhenInNamedWithOrOnSyntax<object>> CreateBindings(Type type, IBindingRoot bindingRoot)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (bindingRoot == null)
            {
                throw new ArgumentNullException("bindingRoot");
            }
            
            if (type.IsInterface || type.IsAbstract)
            {
                return Enumerable.Empty<IBindingWhenInNamedWithOrOnSyntax<object>>();
            }
            
            var bindings = RecursivelyBindToBaseTypes(type, bindingRoot);
            return bindings;
        }

        private IEnumerable<IBindingWhenInNamedWithOrOnSyntax<object>> RecursivelyBindToBaseTypes(Type type, IBindingRoot bindingRoot)
        {
            Type baseType = type.BaseType;
            var bindings = new List<IBindingWhenInNamedWithOrOnSyntax<object>>();
            if (baseType != null)
            {
                bindings.Add(bindingRoot.Bind(baseType).To(type));
                if (ShouldBeBound(baseType))
                {
                    var ancestor = baseType.BaseType;
                    while (ancestor != null && ShouldBeBound(ancestor))
                    {
                        bindings.Add(bindingRoot.Bind(ancestor).To(type));
                        ancestor = ancestor.BaseType;
                    }
                }
            }
            return bindings;
        }

        private static bool ShouldBeBound(Type type)
        {
            return type.IsClass && type != typeof(object);
        }
    }
}