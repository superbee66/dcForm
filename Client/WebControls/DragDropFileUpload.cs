using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using dCForm.Client.Util;
using ScriptManager = dCForm.Client.WebControls.js.ScriptManager;

namespace dCForm.Client.WebControls
{
    /// <summary>
    ///     An ajax friendly jquery-fileupload (embed) descendant of System.Web.UI.WebControls.FileUpload that
    ///     enables file upload without full postback on all browsers. On major non-IE browsers the user may
    ///     also drag and drop desktop files for upload without the need for HTML5 or Flash.
    /// </summary>
    public class DragDropFileUpload : FileUpload, INamingContainer
    {
        public Control _ProgressBar = null;

        public DragDropFileUpload(Control dropZone = null, Control progressBar = null)
        {
            PreRender += DragDropFileUpload_PreRender;
            DropZone = dropZone ?? this;
        }

        public Control DropZone { get; set; }
        public string Status { get; set; }

        private void DragDropFileUpload_PreRender(object sender, EventArgs e)
        {
            Attributes["title"] = "Upload completed forms from your desktop";

            CssClass = Enabled ? CssClass.Replace("ui-state-disabled",
                string.Empty) : CssClass + " ui-state-disabled";
            CssClass = CssClass.Trim();
            CssClassify.ApplyDefaultCssClass(this);

            ScriptManager.RegisterClientScriptResources(
                this,
                "jquery-1.10.2.js",
                "jquery-ui-1.10.4.custom.js",
                "jquery-fileupload.js",
                "jquery-iframe-transport.js",
                "jquery.jqprint-0.3.js",
                "DragDropFileUpload.js");

            // This stop must be done in the click event to keep up with an uploadPanel destroying this jQuery every time it posts back
            //Attributes["onclick"] = string.Format("javascript:init{0}('{1}','{2}','{3}','{4}','{5}');",
            // GetType().Name,
            // ClientID,
            // DropZone.ClientID,
            // _ProgressBar == null ? string.Empty : _ProgressBar.ClientID,
            // ClientID,
            // AjaxHelper.getUpdatePanel(this).ClientID);

            System.Web.UI.ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "DragDropFileUpload",
                string.Format(@"init{0}('{1}','{2}','{3}');",
                    GetType().Name,
                    ClientID,
                    DropZone.ClientID,
                    _ProgressBar == null ? string.Empty : _ProgressBar.ClientID),
                true);
        }

        /// <summary>
        ///     The time to process the uploaded file
        /// </summary>
        public event EventHandler FormUploaded;

        protected override void OnLoad(EventArgs e)
        {
            Dictionary<string, string> r = new Dictionary<string, string> {{"CID", ClientID}, {"TS", DateTime.Now.Ticks.ToString()}};

            if (Page.IsPostBack)
                if (Page.Request.Params["__EVENTTARGET"] == ClientID)
                {
                    if (UploadComplete != null)
                        UploadComplete.Invoke(this,
                            new EventArgs());
                } else if (PostedFile != null)
                    if (PostedFile.ContentLength > 0)
                        try
                        {
                            if (FormUploaded != null)
                                FormUploaded.Invoke(this,
                                    new EventArgs());
                        } catch (Exception ex)
                        {
                            r["error"] = string.Format("{0} {1}",
                                ex.Message,
                                ex.InnerException == null ? string.Empty : ex.InnerException.Message).Replace("Exception has been thrown by the target of an invocation.",
                                    "").Trim();
                        } finally
                        {
                            Context.Response.Clear();
                            Context.Response.ContentType = "text/html";
                            Context.Response.Write(Serialize.Json.Serialize(r));
                            Context.Response.End();
                        }

            base.OnLoad(e);
        }

        /// <summary>
        ///     The time to refresh any controls via ajax
        /// </summary>
        public event EventHandler UploadComplete;
    }
}