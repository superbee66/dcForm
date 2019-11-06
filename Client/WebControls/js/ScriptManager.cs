using System.Web.UI;

[assembly: WebResource("dCForm.Client.WebControls.js.DragDropFileUpload.js", "text/javascript", PerformSubstitution = false)]
[assembly: WebResource("dCForm.Client.WebControls.js.jquery-1.10.2.js", "text/javascript", PerformSubstitution = false)]
[assembly: WebResource("dCForm.Client.WebControls.js.jquery-fileupload.js", "text/javascript", PerformSubstitution = false)]
[assembly: WebResource("dCForm.Client.WebControls.js.jquery-iframe-transport.js", "text/javascript", PerformSubstitution = false)]
[assembly: WebResource("dCForm.Client.WebControls.js.jquery-ui-1.10.4.custom.js", "text/javascript", PerformSubstitution = false)]
[assembly: WebResource("dCForm.Client.WebControls.js.jquery.watermark.js", "text/javascript", PerformSubstitution = false)]
// Markdown -> Markup to make DragDropPanel server side exceptions caught pretty

[assembly: WebResource("dCForm.Client.WebControls.js.jquery.jqprint-0.3.js", "text/javascript", PerformSubstitution = false)]

namespace dCForm.Client.WebControls.js
{
    /// <summary>
    ///     Reduces effort needed to register this assembly's embedded JavaScript with ASP.NET's script manager
    ///     Reference:http://codeverge.com/asp.net.ajax-discussion/scriptmanager.registerclientscriptres/217526
    /// </summary>
    internal class ScriptManager : System.Web.UI.ScriptManager
    {
        /// <summary>
        ///     Loads embedded scripts from the dCForm.Client/WebConstrols/js folder. Remember to register your scripts in
        ///     the AssemblyInfo.cs before calling this
        /// </summary>
        /// <param name="control"></param>
        /// <param name="ScriptFileNames">simple file names not qualified with there .js file extension</param>
        internal static void RegisterClientScriptResources(Control control, params string[] ScriptFileNames)
        {
            //TODO:Script wire compression?
            //TODO:Detect current namespace in order to load embedded scripts properly if namespaces are renamed (dCForm.Client.WebControls.js.whatever.js)
            foreach (string file in ScriptFileNames)
                RegisterClientScriptResource(control,
                    control.GetType(),
                    string.Format("dCForm.Client.WebControls.js.{0}",
                        file));
        }

        /// <summary>
        ///     RegisterStartupScript implementing string.format to build & register the script. Registration
        ///     key combines the give format/string & objects values as a hyphenated HashCode set
        /// </summary>
        /// <param name="control"></param>
        /// <param name="format">script without tags, it will be wrapped automatically</param>
        /// <param name="objects"></param>
        internal static void RegisterStartupScript(Control control, string format, params object[] objects)
        {
            RegisterStartupScript(
                control,
                control.GetType(),
                string.Format("{0}-{1}",
                    format.GetHashCode(),
                    objects.GetHashCode()),
                objects == null || objects.Length == 0 ?
                    format :
                    string.Format(format,
                        objects),
                true);
        }
    }
}