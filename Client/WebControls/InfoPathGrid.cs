using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.Design.WebControls;
using System.Web.UI.WebControls;
using dCForm.Client.Properties;
using dCForm.Client.Util;

namespace dCForm.Client.WebControls
{
    [ToolboxData("<{0}:DragDropFileUpload runat=server/>")]
    [Designer(typeof (DataGridDesigner))]
    internal sealed class InfoPathGrid : DataGrid
    {
        public InfoPathGrid()
        {
            AutoGenerateColumns = false;
            DataKeyField = "DataKeyField";
            //IsViewStateEnabled
            ItemDataBound += InfoPathGrid_ItemDataBound;
        }

        public string SearchText { get; set; }

        private void _CheckBoxColumn_CheckedChanged(object sender,
                                                    EventArgs e)
        {
            if (CanApprove && !AuditDocsVisible)
            {
                LightDoc _LightDoc = LightDoc.Parse(DataKeysArray[((DataGridItem) ((CheckBox) sender).Parent.DataItemContainer).ItemIndex].ToString());
                _LightDoc.DocStatus = ((CheckBox) sender).Checked;

                if (ApprovalCommand != null)
                    ApprovalCommand.Invoke(sender,
                        new ApprovalEventArgs(_LightDoc));
            }
        }

        public event ApprovalEventHandler ApprovalCommand;

        /// <summary>
        ///     This entire method was built when I was way to tired
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void InfoPathGrid_ItemCommand(object source,
                                              DataGridCommandEventArgs e)
        {
            if (Items.Count == 0)
                if (NeedsDataSource != null)
                    NeedsDataSource.Invoke(this,
                        new EventArgs());

            string CommandSourceText = ((LinkButton) (e.CommandSource)).Text;
            LightDoc SelectedLightDoc = LightDoc.Parse(DataKeysArray[e.Item.ItemIndex].ToString());

            if (CommandSourceText == Resources.InfoPathGrid_OnDataBinding_Show)
            {
                AuditDocsHeadDoc = SelectedLightDoc;
                DataBind();
            } else
                AjaxHelper.Redirect(this,
                    SelectedLightDoc.DocSrc);
        }

        private void InfoPathGrid_ItemDataBound(object sender,
                                                DataGridItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    e.Item.CssClass = "headrow";
                    break;
                case ListItemType.Item:
                case ListItemType.AlternatingItem:

                    e.Item.HorizontalAlign = HorizontalAlign.Left;
                    e.Item.CssClass += " item";

                    if (e.Item.ItemIndex%2 > 0)
                        e.Item.CssClass += " alt";

                    e.Item.CssClass = e.Item.CssClass.Trim();
                    break;
                case ListItemType.Footer:
                    while (e.Item.Cells.Count > 1)
                    {
                        e.Item.Cells.RemoveAt(1);
                        e.Item.Cells[0].ColumnSpan++;
                    }
                    e.Item.Cells[0].Text = CaptionVisible ? Resources.No_Forms_Found : "";
                    e.Item.CssClass = "footrow";
                    break;
            }
        }

        public event EventHandler NeedsDataSource;

        protected override void OnDataBinding(EventArgs e)
        {
            Results = new DataTable();
            Columns.Clear();

            Columns.Add(new HyperLinkColumn
                        {
                            DataNavigateUrlField = "DocSrc",
                            DataTextField = "DocTitle",
                            HeaderText = string.Empty
                        });

            if (CanAudit)
                Columns.Add(new ButtonColumn
                            {
                                HeaderText = "Change Log",
                                CommandName = "Audit",
                                Text = Resources.InfoPathGrid_OnDataBinding_Show,
                                Visible = !AuditDocsVisible
                            });

            CheckBoxColumn _ApprovedCheckBoxColumn = new CheckBoxColumn(CanApprove && !AuditDocsVisible)
                                                     {
                                                         HeaderText = "Approved",
                                                         DataField = "DocStatus",
                                                         AutoPostBack = true
                                                     };
            _ApprovedCheckBoxColumn.CheckedChanged += _CheckBoxColumn_CheckedChanged;
            Columns.Add(_ApprovedCheckBoxColumn);

            CaptionText = StringTransform.Wordify(DocTypeName);
            CaptionText += AuditDocsVisible
                               ? " Audit Trail of " + AuditDocsHeadDoc.DocTitle
                               : " Submissions";

            DataSource = (AuditDocsVisible
                              ? FormController.Audit(AuditDocsHeadDoc.DocTypeName,
                                  AuditDocsHeadDoc.DocId,
                                  RelayUrl)
                              : FormController.List(FilterDocTypes == null || FilterDocTypes.Count == 0
                                                        ? new List<string>
                                                          {
                                                              NewDocPropVals.GetType().
                                                                             Name
                                                          }
                                                        : FilterDocTypes,
                                  FilterDocKeys != null && FilterDocKeys.Count > 0
                                      ? FilterDocKeys
                                      : null,
                                  FilterPropVals != null && FilterPropVals.Count > 0
                                      ? FilterPropVals
                                      : null,
                                  !string.IsNullOrWhiteSpace(SearchText)
                                      ? SearchText
                                      : null,
                                  150,
                                  0,
                                  RelayUrl)).OrderBy(m => m);


            ItemCommand += InfoPathGrid_ItemCommand;


            base.OnDataBinding(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            ShowHeader = Items.Count != 0;
            ShowFooter = Items.Count == 0;

            if (CaptionVisible)
                if (!string.IsNullOrWhiteSpace(CaptionText))
                    Caption = CaptionText;

            CssClassify.ApplyDefaultCssClass(this);
            base.OnPreRender(e);
        }

        //TODO: Removed InfoPathPanel.Property_XXX references to underlying InfoPathGrid.Property_XXX if the grid never references them for itself

        #region Properties

        internal IDocController FormController { get; set; }

        internal bool AuditDocsVisible
        {
            get { return AuditDocsHeadDoc != null; }
            set
            {
                if (!value)
                    AuditDocsHeadDoc = null;
            }
        }

        /// <summary>
        ///     The current/head document the current list of audit documents represent. This is only set auto documents are
        ///     actually the items bound
        /// </summary>
        internal LightDoc AuditDocsHeadDoc
        {
            get { return (LightDoc) (ViewState["AuditDocsHeadDoc"]) ?? null; }
            set { ViewState["AuditDocsHeadDoc"] = value; }
        }

        /// <summary>
        /// </summary>
        private Type DocType
        {
            get
            {
                return NewDocPropVals == null
                           ? null
                           : NewDocPropVals.GetType();
            }
        }

        internal string DocTypeName
        {
            get
            {
                return DocType == null
                           ? FilterDocTypes.Count() == 1
                                 ? FilterDocTypes.First()
                                 : null
                           : DocType.Name;
            }
        }

        /// <summary>
        ///     Refer to InfoPathPanel.NewValues for more information
        /// </summary>
        internal object NewDocPropVals
        {
            get { return ViewState["NewFormValues"]; }
            set { ViewState["NewFormValues"] = value; }
        }

        internal Dictionary<string, List<string>> FilterDocKeys
        {
            get { return (Dictionary<string, List<string>>) (ViewState["FilterDocKeys"] ?? new Dictionary<string, List<string>>()); }
            set { ViewState["FilterDocKeys"] = value; }
        }

        /// <summary>
        ///     The form object properties; not as a DocKey string value (string-string) pair
        /// </summary>
        internal Dictionary<string, List<string>> FilterPropVals
        {
            get { return (Dictionary<string, List<string>>) (ViewState["FilterProperties"] ?? new Dictionary<string, List<string>>()); }
            set { ViewState["FilterProperties"] = value; }
        }

        internal List<string> FilterDocTypes
        {
            get { return (List<string>) (ViewState["FilterDocTypes"] ?? new List<string>()); }
            set { ViewState["FilterDocTypes"] = value; }
        }

        // public Dictionary<string, string> DataPreFillDocKeys { get; set; }
        //public object ServiceClient { get; set; }
        internal DataTable Results { get; private set; }
        internal bool CanApprove { get; set; }
        internal bool CaptionVisible { get; set; }
        internal string CaptionText { get; set; }
        internal bool CanAudit { get; set; }

        /// <summary>
        ///     Refer to the public property RelayUrl for more information
        /// </summary>
        private string _RelayUrl = DCF_Relay.DCF_Relay.GetRelayUrl();

        /// <summary>
        ///     If the dCForm DCF_Relay\web.config IIS AppRoot folder exists then we know
        ///     the a relay HttpHandler exists; use that. If not then string.empty; don't relay anything,
        ///     communicate directly with the dCForm server for content files, IPB.svc web
        ///     service calls & DocXmlHandler.ashx renderings. Developers may also specify there
        ///     own DCF_Relay (possibly on another server).
        /// </summary>
        internal string RelayUrl
        {
            get { return _RelayUrl; }
            set { _RelayUrl = value; }
        }

        #endregion
    }
}