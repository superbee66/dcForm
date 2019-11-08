using System;
using System.Web;

using dCForm.Util;
using dCForm.Format;
using dCForm.Template.Filesystem;
using dCForm.Util;

namespace dCForm
{
    /// <summary>
    ///     XML contents of the response generated will usually contain business information specific to the form type request.
    ///     Static content files that might replace this HttpHandler would look something like "FormA John Doe 1/1/2015.xml" or
    ///     "FormB Bob Smith 1/1/2014.xml"
    /// </summary>
    public class DocDataHandler : IHttpHandler
    {
        public static string BuildHref(HttpContext context, IDocTextInterpreter _IDocDataInterpreter, string DocTypeName, string solutionVersion)
        {
            string ashxFilename = context.Request.Url.AbsoluteUri.Substring(context.Request.Url.AbsoluteUri.LastIndexOf('/') + 1).Replace(context.Request.Url.Query, "");
            string href = string.Empty;
            href = new Uri(string.Format("{0}/{1}/{2}/{3}/{4}",
                context
                    .Request
                    .Url
                    .AbsoluteUri
                    .Replace(context.Request.Url.Query, "")
                    .Replace(ashxFilename, "")
                    .TrimEnd('/'),
                FilesystemTemplateController.DirectoryName,
                DocTypeName,
                solutionVersion,
                _IDocDataInterpreter.HrefVirtualFilename(DocTypeName, solutionVersion))).ToString();

            // Is this request coming in from a "proxying listener"?
            if (!string.IsNullOrWhiteSpace(context.Request.Params[Parm.RelayUrl]))
                href =
                    context.Request.Params[Parm.RelayUrl]
                    + href.Substring(
                        href.ToLower().IndexOf(context.Request.ApplicationPath.ToLower())
                        + context.Request.ApplicationPath.Length);

            return href;
        }

        /// <summary>
        ///     DocFileName changes the filename of the response
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // ensure the latest content has been processed & imported
                ImporterController.ImportDocModelsRunOnce();

                string docData = Nav.FromQueryParameters(context.Request.Params);

                IDocTextInterpreter _IDocDataInterpreter = DocInterpreter.LocateInstance(docData);
                DocProcessingInstructions _DocProcessingInstructions = _IDocDataInterpreter.ReadDocPI(docData);
                _DocProcessingInstructions.href = BuildHref(context, _IDocDataInterpreter, _DocProcessingInstructions.DocTypeName, _DocProcessingInstructions.solutionVersion);

                docData = _IDocDataInterpreter.WritePI(docData, _DocProcessingInstructions);

                context.Response.DisableKernelCache();
                context.Response.Clear();
                context.Response.ClearContent();
                context.Response.ClearHeaders();
                context.Response.ContentType = _IDocDataInterpreter.ContentType;
                context.Response.AddHeader(
                    "content-disposition",
                    string.Format(
                        "attachment; filename=\"{0}\";",
                        GetFilename(
                            _DocProcessingInstructions,
                            _IDocDataInterpreter,
                            context.Request.Params["ContentFileExtension"])));

                context.Response.Write(docData);
            } catch (Exception ex)
            {
                context.Response.ClearHeaders();
                context.Response.ClearContent();
                context.Response.Status = "500 Internal Server Error";
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = string.Format("500 Internal Server Error:\n{0}", ex.AsString());
                context.Response.TrySkipIisCustomErrors = true;
            }
        }

        public static string GetFilename(string docData)
        {
            IDocTextInterpreter _IDocDataInterpreter = DocInterpreter.LocateInstance(docData);
            DocProcessingInstructions _DocProcessingInstructions = _IDocDataInterpreter.ReadDocPI(docData);

            return GetFilename(_DocProcessingInstructions, _IDocDataInterpreter);
        }

        private static string GetFilename(DocProcessingInstructions _DocProcessingInstructions, IDocTextInterpreter _IDocDataInterpreter, string ContentFileExtension = null)
        {
            return string.Format(
                "{0}.{1}",
                FileSystem.CleanFileName(_DocProcessingInstructions.DocTitle).Trim(),
                string.IsNullOrWhiteSpace(ContentFileExtension)
                    ? _IDocDataInterpreter.ContentFileExtension
                    : ContentFileExtension);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}