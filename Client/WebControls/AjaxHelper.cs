using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace dCForm.Client.WebControls
{
    internal static class AjaxHelper
    {
        private static string ProgressBarWebResourceUrl = string.Empty;
        private static int UpdatePanelCount;

        public static UpdatePanel getUpdatePanel(Control o)
        {
            if (o == null) return null;
            return (o is UpdatePanel) ? ((UpdatePanel) o) : getUpdatePanel(o.Parent);
        }

        /// <summary>
        ///     Utilize UpdatePanel for ClientSide redirects if possible.
        ///     Revert to standard redirect if not.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="url">The target location.</param>
        /// <param name="endResponse">true to terminate the current process, applied only if a traditional redirect occurs</param>
        public static void Redirect(Control o, string url, bool endResponse = true)
        {
            UpdatePanel up = getUpdatePanel(o);

            if (up == null)
                o.Page.Request.RequestContext.HttpContext.Response.Redirect(url,
                    endResponse);
            else
                ScriptManager.RegisterStartupScript(
                    up,
                    up.GetType(),
                    "redirectMe",
                    string.Format("document.location='{0}';",
                        url),
                    true);
        }

        /// <summary>
        /// </summary>
        /// <param name="targetControl">a control you wish to wrap with an UpdatePanel</param>
        /// <param name="tryFindingExisting">ascend the control hierarchy via each parent control </param>
        /// <param name="includeProgress">add an animated gif?</param>
        /// <returns></returns>
        public static UpdatePanel WrapUpdatePanelControl(Control targetControl, bool tryFindingExisting = true, bool includeProgress = true, string includeProgressMessage = null)
        {
            //TODO:Assess if objControl.Parent.Parent is always the best place to an UpdatePanel (should it just be the parent)?
            UpdatePanel updatePanel = targetControl.Parent.Parent is UpdatePanel ?
                                          (UpdatePanel) targetControl.Parent.Parent :
                                          new UpdatePanel
                                          {
                                              UpdateMode = UpdatePanelUpdateMode.Conditional,
                                              ChildrenAsTriggers = true,
                                              RenderMode = UpdatePanelRenderMode.Block,
                                              ID = string.Format("WrapUpdatePanel{0}",
                                                  UpdatePanelCount++)
                                          };

            if (targetControl.Parent != updatePanel.ContentTemplateContainer)
            {
                ScriptManager sm = new ScriptManager {ID = "ScriptManager1", EnablePartialRendering = true, EnablePageMethods = true, EnableHistory = true};
                if (ScriptManager.GetCurrent(targetControl.Page) == null)
                    targetControl.Page.Form.Controls.AddAt(0,
                        sm);
                if (tryFindingExisting)
                    updatePanel = getUpdatePanel(targetControl) ?? updatePanel;

                for (int i = 0; i <= targetControl.Parent.Controls.Count - 1 && updatePanel.Parent == null; i++)
                    if (targetControl.Parent.Controls[i] == targetControl)
                    {
                        targetControl.Parent.Controls.AddAt(i,
                            updatePanel);

                        if (includeProgress)
                        {
                            if (string.IsNullOrEmpty(ProgressBarWebResourceUrl))
                                ProgressBarWebResourceUrl = targetControl.
                                    Page.
                                    ClientScript.
                                    GetWebResourceUrl(
                                        typeof (InfoPathPanel), //TODO:Redesign this GetWebResourceUrl, it will only work with InfoPathPanel atm, this is the reason AjaxHelper is internal
                                        "dCForm.Client.WebControls.images.spinner.gif");

                            UpdateProgress _UpdateProgress = new UpdateProgress
                                                             {
                                                                 AssociatedUpdatePanelID = updatePanel.ID,
                                                                 ID = updatePanel.ID + "_Prog"
                                                             };

                            HtmlGenericControl div = new HtmlGenericControl();
                            div.Attributes.Add("class",
                                "divWaiting");
                            div.Controls.Add(new Image {ImageUrl = ProgressBarWebResourceUrl});
                            div.Controls.Add(new HtmlGenericControl {InnerHtml = "<br>"});
                            div.Controls.Add(new Label {Text = string.IsNullOrWhiteSpace(includeProgressMessage) ? "Please Wait.." : includeProgressMessage});

                            _UpdateProgress.Controls.Add(div);
                            updatePanel.ContentTemplateContainer.Controls.Add(_UpdateProgress);
                        }
                    }
                updatePanel.ContentTemplateContainer.Controls.Add(targetControl);
            }
            return updatePanel;
        }
    }
}