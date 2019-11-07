using System;
using System.Linq;
using System.Reflection;

namespace dCForm.Util
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            if (!type.IsGenericType)
                return false;

            return type.GetGenericTypeDefinition() == typeof(Nullable<>);

            //if (!type.IsValueType) return true; // ref-type
            //if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            //return false; // value-type
        }


        /// <summary>
        ///     If the given <paramref name="type" /> is an array or some other collection
        ///     comprised of 0 or more instances of a "subtype", get that type
        /// </summary>
        /// <param name="type">the source type</param>
        /// <returns></returns>
        public static bool isEnumeratedType(this Type type)
        {
            return type.GetEnumeratedType() != null;
        }

        /// <summary>
        ///     If the given <paramref name="type" /> is an array or some other collection
        ///     comprised of 0 or more instances of a "subtype", get that type
        /// </summary>
        /// <param name="type">the source type</param>
        /// <returns></returns>
        public static Type GetEnumeratedType(this Type type)
        {
            // provided by Array
            Type elType = type.GetElementType();
            if (null != elType) return elType;

            // otherwise provided by collection
            Type[] elTypes = type.GetGenericArguments();
            if (elTypes.Length > 0) return elTypes[0];

            // otherwise is not an 'enumerated' type
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The underlying & possibly enumerated type when dealing with collections</returns>
        public static Type GetPrincipleType(this Type type)
        {
            return type.GetEnumeratedType() != null
                   && type != typeof(byte[])
                   && type != typeof(string)
                       ? GetPrincipleType(type.GetEnumeratedType())
                       : Nullable.GetUnderlyingType(type)
                         ?? type;
        }
        public static bool IsCastableTo(this Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
                return true;
            return from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                              .Any(m => m.ReturnType == to &&
                                   (m.Name == "op_Implicit" ||
                                    m.Name == "op_Explicit"));

        }
    }
}