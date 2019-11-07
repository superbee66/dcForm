using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Caching;
using dCForm.Client;
using dCForm.Client.DCF_Relay;
using dCForm.Client.Util;
using dCForm.Util.Zip;

namespace dCForm
{
    /// <summary>
    ///     Reads & writes DocSrc URLs
    /// </summary>
    public static class Nav
    {
        public static string FromQueryParameters(NameValueCollection queryStringFieldValues)
        {
            string r = string.Empty;

            if (queryStringFieldValues.AllKeys.Any(m => m == Parm.DocId.ToString()))
                r = ServiceController.LuceneController.GetDocData(out r,
                    queryStringFieldValues[Parm.DocTypeName],
                    queryStringFieldValues[Parm.DocId],
                    HttpUtility.UrlDecode(queryStringFieldValues[Parm.RelayUrl]),
                    long.Parse(queryStringFieldValues[Parm.LogSequenceNumber] ?? "0"));
            else
                r = ((BaseDoc) (queryStringFieldValues.AllKeys.Any(m => m == Parm.DocCache)
                                    ? HttpRuntime.Cache[queryStringFieldValues[Parm.DocCache]]
                                    : Compressor.DecompressFromBase64String(HttpUtility.UrlDecode(queryStringFieldValues[Parm.DocBin])).FromBytes<object>()))
                    .GetDocData(HttpUtility.UrlDecode(queryStringFieldValues[Parm.RelayUrl]));

            return r;
        }

        /// <summary>
        ///     Generates the correctly formatted url
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="url">The url to format.</param>
        /// <returns>The formatted (resolved) url</returns>
        private static string ResolveUrl(string url)
        {
            // Needs to operate on regular request & service requests
            string ApplicationPath = RequestPaths.ApplicationPath,
                   ResultPath = string.Empty;

            if (Uri.IsWellFormedUriString(ApplicationPath,
                UriKind.Absolute))
                if (new Uri(ApplicationPath).IsFile)
                    ApplicationPath = ApplicationPath.Substring(0,
                        ApplicationPath.LastIndexOf("/")); // This may be true if the application is a file such as IPB_WCF.svc for a WCF request

            // String is Empty, just return Url
            if (url.Length == 0)
                ResultPath = url;

            // String does not contain a ~, so just return Url
            if (url.StartsWith("~") == false)
                ResultPath = url;

            // There is just the ~ in the Url, return the appPath
            if (url.Length == 1)
                ResultPath = ApplicationPath;

            if ((url.ToCharArray()[1] == '/' || url.ToCharArray()[1] == '\\'))
                // Url looks like ~/ or ~\
                if (ApplicationPath.Length > 1)
                    ResultPath = ApplicationPath + "/" + url.Substring(2);
                else
                    ResultPath = "/" + url.Substring(2);
            else

            // Url look like ~something
                if (ApplicationPath.Length > 1)
                    ResultPath = ApplicationPath + "/" + url.Substring(1);
                else
                    ResultPath = ApplicationPath + url.Substring(1);

            if (Uri.IsWellFormedUriString(ResultPath,
                UriKind.Absolute))
            {
                UriBuilder _UriBuilder = new UriBuilder(ResultPath);

                if (_UriBuilder.Scheme != Uri.UriSchemeHttp
                    || _UriBuilder.Scheme != Uri.UriSchemeHttps)
                {
                    _UriBuilder.Scheme = Uri.UriSchemeHttp;
                    _UriBuilder.Port = 80;
                }
                ResultPath = _UriBuilder.ToString();
            }

            return ResultPath;
        }

        public static string ToUrl(string DocTypeName, string DocId, string RelayUrl = "~", long LogSequenceNumber = 0)
        {
            if (string.IsNullOrWhiteSpace(RelayUrl))
                RelayUrl = DCF_Relay.GetRelayUrl();

            string _Url = string.Format("{0}/DocDataHandler.ashx?{6}={4}&DocTypeName={1}&{2}={3}&LogSequenceNumber={5}",
                RelayUrl,
                DocTypeName,
                Parm.DocId,
                DocId,
                HttpUtility.UrlEncode(RelayUrl),
                LogSequenceNumber,
                Parm.RelayUrl);

            return RelayUrl != "~"
                       ? _Url
                       : ResolveUrl(_Url); //TODO:Clean this slop up... In fact, clean the whole FormHandlerNavigation up; it sucks
        }

        /// <summary>
        ///     if the DocData is less then 2083 characters long (that is internet explorer's address bar limit), inline the actual
        ///     data to the URL. When that can't be achieved the document will be stored in MemoryCache and a quazi-ticket-number
        ///     will URL parameter will be used guaranteeing availability of anything that requests it within 10 minutes of this
        ///     method being called.
        /// </summary>
        /// <param name="BaseDoc"></param>
        /// <param name="RelayUrl"></param>
        /// <returns></returns>
        public static string ToUrl(BaseDoc BaseDoc, string RelayUrl = null)
        {
            if (string.IsNullOrWhiteSpace(RelayUrl))
                RelayUrl = DCF_Relay.GetRelayUrl();

            string DocTypeName = BaseDoc.DocTypeName;

            string _Url = string.IsNullOrWhiteSpace(RelayUrl)
                              ? string.Format("{0}/DocDataHandler.ashx?DocTypeName={1}&{2}={3}",
                                  RequestPaths.ApplicationPath,
                                  DocTypeName,
                                  Parm.DocBin,
                                  HttpUtility.UrlEncode(Compressor.CompressToBase64String(BaseDoc.ToBytes())))
                              : string.Format("{0}/DocDataHandler.ashx?DocTypeName={1}&{2}={3}&{4}={5}",
                                  RelayUrl,
                                  DocTypeName,
                                  Parm.DocBin,
                                  HttpUtility.UrlEncode(Compressor.CompressToBase64String(BaseDoc.ToBytes())),
                                  Parm.RelayUrl,
                                  HttpUtility.UrlEncode(RelayUrl)
                                    );

            //REF:http://support.microsoft.com/kb/208427
            if (_Url.Length > 2083)
            {
                string _CacheKey = HttpUtility.UrlEncode(string.Format("{0}.{1}", DocTypeName, _Url.GetHashCode()));

                if (HttpRuntime.Cache[_CacheKey] == null)
                    HttpRuntime.Cache.Insert(_CacheKey,
                        BaseDoc,
                        null,
                        Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(10));

                _Url = string.IsNullOrWhiteSpace(RelayUrl)
                           ? string.Format("{0}/DocDataHandler.ashx?DocTypeName={1}&{2}={3}",
                               RequestPaths.ApplicationPath,
                               DocTypeName,
                               Parm.DocCache,
                               _CacheKey)
                           : string.Format("{0}/DocDataHandler.ashx?DocTypeName={1}&{2}={3}&{4}={5}",
                               RelayUrl,
                               DocTypeName,
                               Parm.DocCache,
                               _CacheKey,
                               Parm.RelayUrl,
                               HttpUtility.UrlEncode(RelayUrl));
            }
            return RelayUrl != "~"
                       ? _Url
                       : ResolveUrl(_Url); //TODO:Clean this slop up... In fact, clean the whole FormHandlerNavigation up; it sucks
        }
    }
}