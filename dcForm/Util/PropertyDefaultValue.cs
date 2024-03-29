﻿using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace dCForm.Util
{
    public static class PropertyDefaultValue
    {
        /// <summary>
        ///     Observed default values for any object utilizing this extension
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> defaults = new ConcurrentDictionary<Type, ConcurrentDictionary<string, string>>();

        public static bool IsDefaultValue(this object o, PropertyInfo Property, object PropertyValue = null) { return IsDefaultValue(o, Property.Name, PropertyValue); }

        /// <summary>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="PropertyName">Property that will have its value converted to a string before comparison test</param>
        /// <param name="PropertyValue"></param>
        /// <returns></returns>
        public static bool IsDefaultValue(this object o, string PropertyName, object PropertyValue = null)
        {
            Type t = o.GetType();
            if (!defaults.ContainsKey(t) || !defaults[t].ContainsKey(PropertyName))
            {
                defaults[t] = new ConcurrentDictionary<string, string>();
                object _CreateInstance = Activator.CreateInstance(t, null);
                foreach (PropertyInfo p in t.GetProperties())
                    if (p.CanRead)
                        if (p.CanWrite)
                            defaults[t][p.Name] = string.Format("{0}", p.PropertyType.GetDefault());
            }


            // HACK: It was observed that a compare of Int32 values comparing, both set to zero where not equal for some reason. Since we know anything to compare will translate into a string easily this proves to be the bast option to compare them considering we are dealing with nulls
            // (strangely enough there GetHashCode() did return equal values)
            return
                defaults[t][PropertyName]
                    .Equals(
                        string.Format("{0}", PropertyValue ?? t.GetProperty(PropertyName).GetValue(o, null)));
        }

        /// <summary>
        ///     [ <c>public static object GetDefault(this Type type)</c> ]
        ///     <para></para>
        ///     Retrieves the default value for a given Type
        /// </summary>
        /// <param name="type">The Type for which to get the default value</param>
        /// <returns>The default value for <paramref name="type" /></returns>
        /// <remarks>
        ///     If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value
        ///     type
        ///     is supplied which is not publicly visible or which contains generic parameters, this method will fail with an
        ///     exception.
        /// </remarks>
        /// <example>
        ///     To use this method in its native, non-extension form, make a call like:
        ///     <code>
        ///     object Default = DefaultValue.GetDefault(someType);
        /// </code>
        ///     To use this method in its Type-extension form, make a call like:
        ///     <code>
        ///     object Default = someType.GetDefault();
        /// </code>
        /// </example>
        /// <seealso cref="GetDefault&lt;T&gt;" />
        public static object GetDefault(this Type type)
        {
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !type.IsValueType || type == typeof (void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (type.ContainsGenericParameters)
                throw new ArgumentException(
                    "{" + MethodBase.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                    "> contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a 
            //  default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
                try
                {
                    return Activator.CreateInstance(type);
                } catch (Exception e)
                {
                    throw new ArgumentException(
                        "{" + MethodBase.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
                        "create a default instance of the supplied value type <" + type +
                        "> (Inner Exception message: \"" + e.Message + "\")", e);
                }

            // Fail with exception
            throw new ArgumentException("{" + MethodBase.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                                        "> is not a publicly-visible type, so the default value cannot be retrieved");
        }
    }
}