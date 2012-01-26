using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NContrib.Extensions {

    public static class TypeExtensions {

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for all memebers of type T in the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesOfType<T>(this Type t) {
            return t.GetProperties().Where(p => p.PropertyType == typeof(T));
        }
    }
}
