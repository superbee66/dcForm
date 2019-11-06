using System.IO;
using System.Web;
using dCForm.Core.Format.XsnXml;
using dCForm.Core.Template;
using dCForm.Core.Util;

namespace dCForm.Core
{
    public class DocRevHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     locate versions of what were once physical files utilizing form/DocTypeName/VersionNumber/*.*
        ///     from archives of what was once seen & now compressed as cab in the document database.
        ///     This allows older documents in the field to request resources that deployed a long time ago
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            // ensure the latest content has been processed & imported
            ImporterController.ImportDocModelsRunOnce();

            //TODO:move ~/web.config ManifestRewrite_Integrated & ManifestRewrite_Classic structures to ~/form/web.config so there is no need for this manifest.xsf httphandler switching logic section to 
            if (context.Request.Url.ToString().ToLower().EndsWith("manifest.xsf"))
                new ManifestRewriter().ProcessRequest(context);
            else
            {
                string filename;
                using (MemoryStream _MemoryStream = TemplateController.Instance.OpenRead(context, out filename))
                {
                    context.Response.DisableKernelCache();
                    context.Response.Clear();
                    context.Response.ClearContent();
                    context.Response.ClearHeaders();

                    _MemoryStream.CopyTo(context.Response.OutputStream);

                    context.Response.ContentType = MimeExtensionHelper.GetMimeType(filename);
                    context.Response.AddHeader("content-disposition", "attachment; filename=\"" + filename + "\";");
                }
            }
        }
    }
}