using System.Text.RegularExpressions;
using System.Web;

namespace dCForm.Core.Util
{
    public static class Html
    {
        /// <summary>
        ///     Clean removes any HTML Tags, Entities (and optionally any punctuation) from
        ///     a string
        /// </summary>
        /// <remarks>
        ///     Encoded Tags are getting decoded, as they are part of the content!
        /// </remarks>
        /// <param name="HTML">The Html to clean</param>
        /// <param name="RemovePunctuation">A flag indicating whether to remove punctuation</param>
        /// <returns>The cleaned up string</returns>
        public static string Clean(string HTML, bool RemovePunctuation)
        {
            //First remove any HTML Tags ("<....>")
            HTML = StripTags(HTML, true);
            //Second replace any HTML entities (&nbsp; &lt; etc) through their char symbol
            HTML = HttpUtility.HtmlDecode(HTML);
            //Thirdly remove any punctuation
            if (RemovePunctuation)
                HTML = StripPunctuation(HTML, true);
            //Finally remove extra whitespace
            HTML = StripWhiteSpace(HTML, true);
            return HTML;
        }

        /// <summary>
        ///     Formats String as Html by replacing linefeeds by <br />
        /// </summary>
        /// <param name="strText">Text to format</param>
        /// <returns>The formatted html</returns>
        public static string ConvertToHtml(string strText)
        {
            string strHtml = strText;
            if (!string.IsNullOrEmpty(strHtml))
            {
                strHtml = strHtml.Replace("\n", "");
                strHtml = strHtml.Replace("\r", "<br />");
            }
            return strHtml;
        }

        /// <summary>
        ///     StripPunctuation removes the Punctuation from the content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Indicates whether to replace the Punctuation by a space (true) or nothing (false)</param>
        /// <returns>The cleaned up string</returns>
        public static string StripPunctuation(string HTML, bool RetainSpace)
        {
            //Create Regular Expression objects
            string punctuationMatch = "[~!#\\$%\\^&*\\(\\)-+=\\{\\[\\}\\]\\|;:\\x22'<,>\\.\\?\\\\\\t\\r\\v\\f\\n]";
            var afterRegEx = new Regex(punctuationMatch + "\\s");
            var beforeRegEx = new Regex("\\s" + punctuationMatch);
            //Define return string
            string retHTML = HTML + " "; //Make sure any punctuation at the end of the String is removed
            //Set up Replacement String
            string RepString;
            if (RetainSpace)
                RepString = " ";
            else
                RepString = "";
            while (beforeRegEx.IsMatch(retHTML))
                retHTML = beforeRegEx.Replace(retHTML, RepString);
            while (afterRegEx.IsMatch(retHTML))
                retHTML = afterRegEx.Replace(retHTML, RepString);
            // Return modified string after trimming leading and ending quotation marks
            return retHTML.Trim('"');
        }

        /// <summary>
        ///     StripTags removes the HTML Tags from the content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Indicates whether to replace the Tag by a space (true) or nothing (false)</param>
        /// <returns>The cleaned up string</returns>
        public static string StripTags(string HTML, bool RetainSpace)
        {
            //Set up Replacement String
            string RepString;
            if (RetainSpace)
                RepString = " ";
            else
                RepString = "";
            //Replace Tags by replacement String and return mofified string
            return Regex.Replace(HTML, "<[^>]*>", RepString);
        }

        /// <summary>
        ///     StripWhiteSpace removes the WhiteSpace from the content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="HTML">The HTML content to clean up</param>
        /// <param name="RetainSpace">Indicates whether to replace the WhiteSpace by a space (true) or nothing (false)</param>
        /// <returns>The cleaned up string</returns>
        public static string StripWhiteSpace(string HTML, bool RetainSpace)
        {
            //Set up Replacement String
            string RepString;
            if (RetainSpace)
                RepString = " ";
            else
                RepString = "";
            //Replace Tags by replacement String and return modified string
            if (string.IsNullOrWhiteSpace(HTML))
                return string.Empty;
            return Regex.Replace(HTML, "\\s+", RepString);
        }
    }
}