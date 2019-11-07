using System.Text.RegularExpressions;
using System.Web;
using dCForm.Client.DCF_Relay;
using dCForm.Client.Util;
using dCForm.Template;

namespace dCForm.Format.XsnXml
{
    /// <summary>
    ///     Intercepts requests for InfoPath manifest.xsf text file content & replaces it's publishUrl attribute with the
    ///     current request.
    /// </summary>
    public class ManifestRewriter : IHttpHandler
    {
        public bool IsReusable {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // ensure the latest content has been processed & imported
            ImporterController.ImportDocModelsRunOnce();

            Regex regPublishUrl = new Regex("(?<=publishUrl=\")(.*?)(?=\")", RegexOptions.Multiline);

            // The publish URL within this file needs to be updated to the current requested URL for the InfoPath application form to like it
            //string ManifestPath = context.Request.MapPath(new Uri(RequestPaths.AbsoluteUri).LocalPath);
            string UrlReferrer_AbsoluteUri = context.Request.UrlReferrer == null ? "" : context.Request.UrlReferrer.AbsoluteUri;

            string filename;
            string[] lines = TemplateController.Instance.OpenText(context, out filename).Split('\n', '\r');


            // render the publishUrl as the calling request or that of a registered listener
            string publishUrl = UrlReferrer_AbsoluteUri.Contains("/" + DCF_Relay.DirectoryName)
                                    ? UrlReferrer_AbsoluteUri
                                    : RequestPaths.AbsoluteUri;

            context.Response.ClearContent();

            for (int i = 0; i < lines.Length; i++)
                context.Response.Write(
                    regPublishUrl.IsMatch(lines[i]) ?
                        regPublishUrl.Replace(lines[i], publishUrl) :
                        lines[i]);

            context.Response.ContentType = "text/xml";
        }
    }
}