using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace dCForm.Client.Util
{
    public static class StringInject
    {
        /// <summary>
        ///     Creates a HashTable based on current object state.
        ///     <remarks>Copied from the MVCToolkit HtmlExtensionUtility class</remarks>
        /// </summary>
        /// <param name="properties">The object from which to get the properties</param>
        /// <returns>A <see cref="Hashtable" /> containing the object instance's property names and their values</returns>
        public static Hashtable GetPropertyHash(object properties, string formatString)
        {
            Hashtable values = null;
            if (properties != null)
            {
                values = new Hashtable();
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(properties);
                foreach (PropertyDescriptor prop in props)
                {
                    Regex attributeRegex = new Regex("{(" + prop.Name + ")(?:}|(?::(.[^}]*)}))");
                    if (attributeRegex.IsMatch(formatString))
                        values.Add(prop.Name,
                            prop.GetValue(properties));
                }
            }
            return values;
        }

        /// <summary>
        ///     Extension method that replaces keys in a string with the values of matching object properties.
        ///     <remarks>Uses <see cref="string.Format()" /> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="injectionObject">The object whose properties should be injected in the string</param>
        /// <returns>A version of the formatString string with keys replaced by (formatted) settingsKey values.</returns>
        public static string Inject(this string formatString, object injectionObject)
        {
            string[] badXmlns =
            {
                "xmlns:w=\"http://schemas.microsoft.com/office/word/2003/wordml\"",
                "xmlns:o=\"urn:schemas-microsoft-com:office:office\"",
                "xmlns:v=\"urn:schemas-microsoft-com:vml\"",
                "xmlns:wx=\"http://schemas.microsoft.com/office/word/2003/auxHint\"",
                "xmlns:aml=\"http://schemas.microsoft.com/aml/2001/core\"",
                "xmlns:w10=\"urn:schemas-microsoft-com:office:word\"",
                "xmlns:st=\"urn:schemas-microsoft-com:office:smarttags\""
            };

            return formatString.Inject(GetPropertyHash(injectionObject,
                formatString));
        }

        /// <summary>
        ///     Extension method that replaces keys in a string with the values of matching dictionary entries.
        ///     <remarks>Uses <see cref="string.Format()" /> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="dictionary">An <see cref="IDictionary" /> with keys and values to inject into the string</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) settingsKey values.</returns>
        public static string Inject(this string formatString, IDictionary dictionary) { return formatString.Inject(new Hashtable(dictionary)); }

        /// <summary>
        ///     Extension method that replaces keys in a string with the values of matching hashtable entries.
        ///     <remarks>Uses <see cref="string.Format()" /> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="attributes">A <see cref="Hashtable" /> with keys and values to inject into the string</param>
        /// <returns>A version of the formatString string with hastable keys replaced by (formatted) settingsKey values.</returns>
        public static string Inject(this string formatString, Hashtable attributes)
        {
            string result = formatString;
            if (attributes == null || formatString == null)
                return result;

            foreach (string attributeKey in attributes.Keys)
                result = result.InjectSingleValue(attributeKey,
                    attributes[attributeKey]);
            return result;
        }

        /// <summary>
        ///     Replaces all instances of a 'settingsKey' (e.g. {foo} or {foo:SomeFormat}) in a string with an optionally formatted
        ///     value, and returns the result.
        /// </summary>
        /// <param name="formatString">The string containing the settingsKey; unformatted ({foo}), or formatted ({foo:SomeFormat})</param>
        /// <param name="settingsKey">The settingsKey name (foo)</param>
        /// <param name="replacementValue">The replacement value; if null is replaced with an empty string</param>
        /// <returns>The input string with any instances of the settingsKey replaced with the replacement value</returns>
        public static string InjectSingleValue(this string formatString, string key, object replacementValue)
        {
            string result = formatString;
            //regex replacement of settingsKey with value, where the generic settingsKey format is:
            //Regex foo = new Regex("{(foo)(?:}|(?::(.[^}]*)}))");
            Regex attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))"); //for settingsKey = foo, matches {foo} and {foo:SomeFormat}

            //loop through matches, since each settingsKey may be used more than once (and with a different format string)
            foreach (Match m in attributeRegex.Matches(formatString))
            {
                string replacement = m.ToString();
                if (m.Groups[2].Length > 0) //matched {foo:SomeFormat}
                {
                    //do a double string.Format - first to build the proper format string, and then to format the replacement value
                    string attributeFormatString = string.Format(CultureInfo.InvariantCulture,
                        "{{0:{0}}}",
                        m.Groups[2]);
                    replacement = string.Format(CultureInfo.CurrentCulture,
                        attributeFormatString,
                        replacementValue);
                } else //matched {foo}
                    replacement = (replacementValue ?? string.Empty).ToString();
                //perform replacements, one match at a time
                result = result.Replace(m.ToString(),
                    replacement); //attributeRegex.Replace(result, replacement, 1);
            }
            return result;
        }
    }
}