using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using dCForm.Client.Util;

namespace dCForm.Client.DCF_Relay
{
    /// <summary>
    ///     dCForm Relay
    ///     Forwards requests for InfoPath form content files & the form XML itself
    ///     to the principle service. This facilities the common
    ///     WWW -> WCF -> DB
    ///     tier pattern utilized by many enterprises
    /// </summary>
    public class DCF_Relay : IHttpHandler
    {
        public static readonly string DirectoryName = typeof (DCF_Relay).Name;

        /// <summary>
        ///     Parses service URL from anywhere in a config file by looking for the IPB.svc or DocExchange.svc filename
        /// </summary>
        private static readonly Regex FormHandlerUrlParse = new Regex(@"(?<="")(.*)(?=/(DocExchange)\.svc"")", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Service URL straight from IPB folder specific configuration
        /// </summary>
        private static string FormHandlerUrl;

        /// <summary>
        ///     Empty means we need to go look for it, null means we didn't find it
        /// </summary>
        private static string _GetRelayUrlFound = string.Empty;

        /// <summary>
        ///     URL path to the virtual directory running this
        /// </summary>
        private string baseAbsoluteUri;

        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        ///     Forwards GET & HEAD requests to back-end server hosting the WCF & content files
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod != "GET" && context.Request.HttpMethod != "HEAD")
            {
                context.Response.ClearHeaders();
                context.Response.ClearContent();
                context.Response.Status = "405 Method Not Allowed";
                context.Response.StatusCode = 405;
                context.Response.StatusDescription = string.Format("A request was made of a resource using a request method not supported by that resource; {0} supports on HEAD & GET verbs.", DirectoryName);
                context.Response.TrySkipIisCustomErrors = true;
            } else
            {
                //EXAMPLE:http://form417439-sp.azdes.gov/dCForm/Dev/Service
                string relay_svc_app_url = ResolveFormHandlerUrl();

                //EXAMPLE:http://form417439-sp/ISPClient/Dev/Src/IntranetPresentationLayer/ISP_Web
                baseAbsoluteUri =
                    string.Format("{0}/{1}", context.Request.Url.AbsoluteUri.Replace(
                        context.Request.Url.AbsoluteUri.Substring(
                            context.Request.Url.AbsoluteUri.ToLower().IndexOf(context.Request.ApplicationPath.ToLower()) + context.Request.ApplicationPath.Length)
                        , string.Empty), DirectoryName); //TODO:DCF_Relay folder dynamically gleamed

                // http://form417439-sp.azdes.gov/dCForm/Dev/Service/DocDataHandler.ashx?FormName=FORM1512A&MEMBER_NAME=WU9VU0VGIEFMS0FSS0hJ&LOCATION_OF_MEETING=Ly9UT0RPOkpBRz8gQWxzbywgaXMgdGhlIGRhdGUgdGhlIGRheSB0aGV5IGRvd25sb2FkZWQ_&DATE=MTEvMTcvMjAxNCA0OjAyOjM5IFBN&DocId=MGQyU2dxMUhvY1VfNGU5dTdsdXM3WVRSdi1yTDk4TEItb1o0VnhuVFNvWi1lX3MtS2EtM1RNSnJKcV8wbUxnTG55T0l2bXUtdkpzVF9Ydmp1Zm1MOGclM2QlM2Q$&FormListener=http%3a%2f%2fform417439-sp%2fISPClient%2fDev%2fSrc%2fIntranetPresentationLayer%2fISP_Web
                string _HttpWebRequest_Url =
                    relay_svc_app_url
                    + context.Request.Url.AbsoluteUri.Substring(
                        context.Request.Url.AbsoluteUri.ToLower().IndexOf(context.Request.ApplicationPath.ToLower()) + (context.Request.ApplicationPath + "/" + DirectoryName).Length);

                HttpWebRequest _HttpWebRequest = (HttpWebRequest) WebRequest.Create(_HttpWebRequest_Url);
                _HttpWebRequest.Proxy = null;
                _HttpWebRequest.AllowAutoRedirect = true;
                _HttpWebRequest.Referer = context.Request.Url.AbsoluteUri;

                using (HttpWebResponse _HttpWebResponse = (HttpWebResponse) _HttpWebRequest.GetResponse())
                {
                    _HttpWebResponse.GetResponseStream().CopyTo(context.Response.OutputStream);
                    context.Response.ContentType = _HttpWebResponse.ContentType;
                    context.Response.StatusCode = (int) _HttpWebResponse.StatusCode;
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    // header must be cloned in this manner in order to support integrated & classic IIS modes,
                    // context.Response.Headers.Add((NameValueCollection)_HttpWebResponse.Headers) will fail
                    NameValueCollection _NameValueCollection = _HttpWebResponse.Headers;
                    foreach (string key in _NameValueCollection.AllKeys)
                        context.Response.AddHeader(key, _NameValueCollection[key]);
                }
            }
        }

        /// <summary>
        ///     When operating on the client, a search is performed on the ~/MyName/web.config
        ///     & the configuration is read or path is assumed simply by the ~/MyName
        /// </summary>
        /// <returns></returns>
        public static string GetRelayUrl()
        {
            if (_GetRelayUrlFound == string.Empty && HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                //TODO:check this logic as it will not work outside a web context
                //string IpbWebConfigFileName = HttpContext.Current.Request.MapPath("~/MyName/web.config");

                //if (!File.Exists(IpbWebConfigFileName))
                //    throw new Exception(
                //        string.Format(
                //        Properties.Resources.MyName_Not_Found_Exception_Message,
                //        Properties.Resources.web));


                if (File.Exists(HttpContext.Current.Request.MapPath(string.Format("~/{0}/web.config", DirectoryName))))
                    _GetRelayUrlFound = string.Format("{0}://{1}:{2}{3}/{4}", //TODO:Detect if a relay is needed at all
                        HttpContext.Current.Request.Url.Scheme,
                        HttpContext.Current.Request.Url.Host.ToLower() == "localhost"
                            ? Environment.MachineName
                            : HttpContext.Current.Request.Url.Host /* avoid using localhost so that other machine can access the URI this may be used to generate */,
                        HttpContext.Current.Request.Url.Port,
                        HttpContext.Current.Request.ApplicationPath,
                        DirectoryName);

                string appRoot = HttpContext.Current.Request.MapPath("~");
                string path = Directory.EnumerateDirectories(
                    HttpContext.Current.Request.MapPath("~"),
                    "*",
                    SearchOption.AllDirectories).FirstOrDefault(m => m.ToLower().EndsWith(DirectoryName.ToLower()) && File.Exists(string.Format(@"{0}\web.config", m)));

                if (string.IsNullOrWhiteSpace(path))
                    _GetRelayUrlFound = null;
                else
                {
                    path = path.Replace(appRoot, string.Empty).Replace(@"\", "/");
                    _GetRelayUrlFound = string.Format("{0}://{1}:{2}{3}/{4}", //TODO:Detect if a relay is needed at all
                        HttpContext.Current.Request.Url.Scheme,
                        HttpContext.Current.Request.Url.Host,
                        HttpContext.Current.Request.Url.Port,
                        HttpContext.Current.Request.ApplicationPath,
                        DirectoryName);
                }
            }
            return _GetRelayUrlFound;
        }

        /// <summary>
        ///     Attempts to get the dCForm web service URL from an appsetting set explicitly in the MyName folder
        ///     or by looking for a web service entry in the application main web.config
        /// </summary>
        /// <returns></returns>
        private static string ResolveFormHandlerUrl()
        {
            if (string.IsNullOrWhiteSpace(FormHandlerUrl))
            {
                // try parse from explicit set in the MyName\web.config
                if (string.IsNullOrWhiteSpace(FormHandlerUrl))
                    FormHandlerUrl = string.Format("{0}", ConfigurationManager.AppSettings["ServiceUrl"]);

                // try parse from WCF service address in the App root by search for something that references IPB.svc

                string webconfigXml = File.ReadAllText(RequestPaths.GetPhysicalApplicationPath("web.config"));
                if (string.IsNullOrWhiteSpace(FormHandlerUrl))
                    FormHandlerUrl = string.Format("{0}", FormHandlerUrlParse.Match(webconfigXml).Value);


                // make sure the URL is pretty and escaped
                if (!string.IsNullOrWhiteSpace(FormHandlerUrl))
                    // are we dealing with a file?
                    if (FormHandlerUrl.LastIndexOf('.') > FormHandlerUrl.LastIndexOf('/'))
                        FormHandlerUrl = new Uri(FormHandlerUrl.Substring(0, FormHandlerUrl.LastIndexOf("/") - 1), UriKind.RelativeOrAbsolute).ToString();
            }

            return FormHandlerUrl;
        }
    }
}