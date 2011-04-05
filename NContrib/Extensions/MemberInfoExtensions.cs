using System;
using System.Reflection;

namespace NContrib.Extensions {

    public static class MemberInfoExtensions {

        /// <summary>
        /// Returns a single attribute of type T
        /// </summary>
        /// <typeparam name="T">Type of the attribute to find and return</typeparam>
        /// <param name="m"></param>
        /// <param name="inherit">Specifies whether to search this member's inheritance chain to find the attribute.</param>
        /// <returns></returns>
        public static T GetSingleAttribute<T>(this MemberInfo m, bool inherit = true) where T : Attribute {
            return (T)m.GetCustomAttributes(typeof(T), inherit)[0];
        }

        public static bool HasAttributeOfType<T>(this MemberInfo memberInfo, bool inherit = true) {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Length > 0;
        }
    }
}
