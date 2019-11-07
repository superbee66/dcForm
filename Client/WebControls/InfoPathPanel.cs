using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dCForm.Client.Properties;
using dCForm.Client.Util;

[assembly: TagPrefix("dCForm.Client.WebControls", "IPB")]

namespace dCForm.Client.WebControls
{
    public enum MessageState
    {
        Normal,
        Highlight,
        Error
    }

    [ToolboxData("<{0}:InfoPathPanel runat=server></{0}:InfoPathPanel")]
    public class InfoPathPanel : WebControl
    {
        /// <summary>
        ///     Used to decode XML file content from uploaded zip files
        /// </summary>
        private static readonly UTF8Encoding _UTF8Encoding = new UTF8Encoding();

        private readonly Panel _commandPanel = new Panel();
        private readonly DragDropFileUpload _DragDropFileUpload = new DragDropFileUpload();
        private readonly InfoPathGrid _grid = new InfoPathGrid();
        private readonly Label _message = new Label { EnableViewState = false };

        private readonly Button
            _newButton = new Button { ToolTip = Resources.NewFormText },
            _auditButton = new Button { Text = Resources.ExitAuditText, CssClass = "auditButton" };

        //TODO: Create "need" events for properties to make it easier for developers to figure out what is needed when

        //TODO:Move labeling into Resource.* for all controls
        private readonly CheckBox _preFillCheckBox = new CheckBox
                                                     {
                                                         Text = Resources.preFillCheckBox_Text,
                                                         ToolTip = Resources.preFillCheckBox_Title,
                                                         Visible = false
                                                     };

        private readonly SearchTextBox _searchTextBox = new SearchTextBox
                                                        {
                                                            AutoPostBack = true,
                                                            WatermarkText =
                                                                Resources.SearchBoxWatermark_Text
                                                        };

        private object _CopyLatestApproved;
        private object _postedDoc = new object();
        private Dictionary<string, string> _postedDocKeys = new Dictionary<string, string>();
        private string _postedDocSrc;

        public InfoPathPanel()
        {
            NewFormDocKeys = new Dictionary<string, string>();

            _DragDropFileUpload = new DragDropFileUpload(_commandPanel);
            _newButton.Text = Resources.textFillOutForm;

            //if (HttpContext.Current.Handler != null)
            //    ((Page)HttpContext.Current.Handler).Init += TryUpdatePanelInject;
            //else if (Page != null)
            //    Page.Init += TryUpdatePanelInject;

            _DragDropFileUpload.FormUploaded += uploader_FormUploaded;
            _DragDropFileUpload.UploadComplete += uploader_UploadComplete;
            _newButton.Click += newButton_Click;
            _auditButton.Click += exitButton_Click;
            _grid.ApprovalCommand += grid_ApprovalCommand;
            _grid.NeedsDataSource += grid_NeedsDataSource;
            // PreRender += InfoPathPanel_PreRender;


            _grid.ShowFooter = false;
            _grid.ShowHeader = true;

            _grid.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            _grid.AllowSorting = true;
            _grid.BorderWidth = 0;
            _grid.GridLines = GridLines.None;
            _grid.Width = Unit.Percentage(100);

            _commandPanel.Style[HtmlTextWriterStyle.Display] = "block";

            _commandPanel.Controls.Add(_newButton);
            _commandPanel.Controls.Add(_preFillCheckBox);
            _commandPanel.Controls.Add(_searchTextBox);
            _commandPanel.Controls.Add(_DragDropFileUpload);
            _commandPanel.Controls.Add(_auditButton);
            _commandPanel.Controls.Add(_message);

            Controls.Add(_commandPanel);
            Controls.Add(_grid);
        }

        [Bindable(true)]
        [Category("Features")]
        [DefaultValue(true)]
        public virtual bool CanApprove
        {
            get { return _grid.CanApprove; }
            set { _grid.CanApprove = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool CanAudit
        {
            get { return _grid.CanAudit; }
            set { _grid.CanAudit = value; }
        }

        /// <summary>
        ///     Transfers values from the more recent approved document to a new document when the requested by the
        ///     user. Required/Not-Null field values do not transfer as there would be no point in sending out
        ///     someone with a form to capture something that was just filled in for them.
        /// </summary>
        [Bindable(true)]
        [Category("Features")]
        [DefaultValue(true)]
        public bool CanCopyLatestApproved
        {
            get { return _preFillCheckBox.Visible; }
            set { _preFillCheckBox.Visible = value; }
        }

        [Bindable(true)]
        [Category("Features")]
        [DefaultValue(true)]
        public bool CanCreate
        {
            get { return _newButton.Visible; }
            set { _newButton.Visible = value; }
        }

        [Bindable(true)]
        [Category("Features")]
        [DefaultValue(true)]
        public bool CanList
        {
            get { return _grid.Visible; }
            set { _grid.Visible = value; }
        }

        [Bindable(true)]
        [Category("Features")]
        [DefaultValue(true)]
        public bool CanSearch
        {
            get { return _searchTextBox.Visible; }
            set { _searchTextBox.Visible = value; }
        }

        [Bindable(true)]
        [Category("Features")]
        [DefaultValue(true)]
        public bool CanSubmit
        {
            get { return _DragDropFileUpload.Visible; }
            set { _DragDropFileUpload.Visible = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        public string Caption
        {
            get { return _grid.CaptionText; }
            set { _grid.CaptionText = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool CaptionVisible
        {
            get { return _grid.CaptionVisible; }
            set { _grid.CaptionVisible = value; }
        }

        /// <summary>
        ///     Copies values from the most recent approved form found. Required properties are not filled from this return as when
        ///     the user clicks the checkbox to perform a similar operation
        /// </summary>
        /// <returns></returns>
        [Bindable(false)]
        [Category("Document")]
        public object CopyOfLatestApproved
        {
            get
            {
                EnsureDataSource();

                // The default order of items is chronological; newest item first
                if (_CopyLatestApproved == null && Items.Count > 0)
                {
                    DataRow r = _grid.Results.Rows.Cast<DataRow>().FirstOrDefault(m => (bool)m["Approved"]);
                    if (r != null)
                        _CopyLatestApproved = DataSource.Get(
                            out _postedDocSrc,
                            out _postedDocKeys,
                            DocTypeName,
                            null,
                            string.Format("{0}",
                                r["DocKey"]),
                            RelayUrl);
                }

                return _CopyLatestApproved;
            }
        }

        [Bindable(true)]
        [Category("Document")]
        public IDocController DataSource
        {
            get { return _grid.FormController; }
            set { _grid.FormController = value; }
        }

        [Bindable(true)]
        [Category("Document")]
        public string DocTypeName
        {
            get { return _grid.DocTypeName; }
        }

        /// <summary>
        ///     List items are filtered on this & items posted must have matching keys & optional matching values if they are
        ///     defined
        /// </summary>
        [Bindable(true)]
        [Category("Document")]
        public Dictionary<string, List<string>> FilterDocKeys
        {
            get { return _grid.FilterDocKeys; }
            set { _grid.FilterDocKeys = value; }
        }

        [Bindable(true)]
        [Category("Document")]
        public List<string> FilterDocTypes
        {
            get { return _grid.FilterDocTypes; }
            set { _grid.FilterDocTypes = value; }
        }

        /// <summary>
        ///     The form object properties; not as a DocKey string value (string-string) pair
        /// </summary>
        [Bindable(true)]
        [Category("Document")]
        public Dictionary<string, List<string>> FilterPropVals
        {
            get { return _grid.FilterPropVals; }
            set { _grid.FilterPropVals = value; }
        }

        /// <summary>
        ///     Direct access to the DataGridItemCollection of the underlying DataGrid
        /// </summary>
        [Bindable(false)]
        [Category("Features")]
        public DataGridItemCollection Items
        {
            get { return _grid.Items; }
        }

        private string Message
        {
            get { return _message.Text; }
            set { _message.Text = value; }
        }

        private MessageState MessageState
        {
            set
            {
                _message.CssClass = string.Format("ui-corner-all ui-state-{0}",
                    value).ToLower();
            }
        }

        [Bindable(true)]
        [Category("Document")]
        public Dictionary<string, string> NewFormDocKeys
        {
            set { ViewState["NewFormDocKeys"] = value; }
            get { return (Dictionary<string, string>)(ViewState["NewFormDocKeys"] ?? new Dictionary<string, string>()); }
        }

        /// <summary>
        ///     New forms created for user will have there field values fill with the given
        /// </summary>
        [Bindable(true)]
        [Category("Document")]
        public object NewFormPropVals
        {
            get { return _grid.NewDocPropVals; }
            set { _grid.NewDocPropVals = value; }
        }

        [Bindable(true)]
        [Category("Features")]
        [DefaultValue(10)]
        public int PageSize
        {
            get { return _grid.PageSize; }
            set { _grid.PageSize = value; }
        }

        [Bindable(false)]
        [Category("Document")]
        public object PostedDoc
        {
            get { return _postedDoc; }
        }

        public string PostedDocId { get; private set; }

        [Bindable(false)]
        [Category("Document")]
        public Dictionary<string, string> PostedDocKeys
        {
            get { return _postedDocKeys; }
        }

        /// <summary>
        ///     The DocSrc Href rendered by the WCF IPB service suitable for redirecting the client or requesting the DocData
        /// </summary>
        public string PostedDocSrc
        {
            get { return _postedDocSrc; }
        }

        public bool PostedDocStatus { get; private set; }

        [Bindable(false)]
        [Category("Document")]
        public string PostedDocXml { get; set; }

        [Bindable(false)]
        [Category("Document")]
        internal HttpPostedFile PostedFile
        {
            get { return _DragDropFileUpload.PostedFile; }
        } //TODO: Relocate Posted* properties to FormChange & FormApprovalChange even arguments

        /// <summary>
        ///     If the dCForm DCF_Relay\web.config IIS AppRoot folder exists then we know
        ///     the a relay HttpHandler exists; use that. If not then string.empty; don't relay anything,
        ///     communicate directly with the dCForm server for content files, IPB.svc web
        ///     service calls & DocXmlHandler.ashx renderings. Developers may also specify there
        ///     own DCF_Relay (possibly on another server).
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        public string RelayUrl
        {
            get { return _grid.RelayUrl; }
            set { _grid.RelayUrl = value; }
        }

        /// <summary>
        ///     Search text applied toward table item(s) filter
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        public string SearchText
        {
            get { return _searchTextBox.Text; }
            set { _searchTextBox.Text = value; }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        protected override string TagName
        {
            get { return TagKey.ToString(); }
        }

        private static void ApplyDisabledCssClass(WebControl c)
        {
            c.CssClass = c.Enabled
                             ? c.CssClass.Replace("ui-state-disabled",
                                 string.Empty)
                             : c.CssClass + " ui-state-disabled";
            c.CssClass = c.CssClass.Trim();

            foreach (WebControl cc in c.Controls.OfType<WebControl>())
                ApplyDisabledCssClass(cc);
        }

        private static string ConcatExceptionMessages(Exception ex)
        {
            return
                string.Format(
                    "\n\n  - {0}{1}",
                    ex.Message,
                    ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message)
                        ? ConcatExceptionMessages(ex.InnerException)
                        : string.Empty
                    );
        }

        /// <summary>
        ///     Fires of NeedsDataSource to whatever is listening if there are no items in the underlying _grid
        /// </summary>
        private void EnsureDataSource()
        {
            if (Items.Count == 0)
            {
                if (!(DataSource is IDocController))
                    if (NeedsDataSource != null)
                        NeedsDataSource.Invoke(this,
                            new EventArgs());

                DataBind();
            }
        }

        //void tmrLateLoad_Tick(object sender, EventArgs e) { tmrLateLoad.Enabled = false; }
        private void exitButton_Click(object sender, EventArgs e)
        {
            _grid.AuditDocsVisible = false;
            _grid.DataBind();
        }

        private static UpdateProgress findUpdateProgress(Control o)
        {
            if (o is UpdateProgress)
                return (UpdateProgress)o;
            foreach (Control c in o.Controls)
                return findUpdateProgress(c);
            return null;
        }

        public event EventHandler FormApprovalChange;

        /// <summary>
        ///     The time to process the uploaded file
        /// </summary>
        public event EventHandler FormChange;

        /// <summary>
        ///     Copied from BaseDoc, needs to be consolidated & placed in one spot somewhere
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetFormObjectNavProperties(object o)
        {
            Type t = o.GetType();
            Assembly a = t.Assembly;
            PropertyInfo[] _PropertyInfos =
                t.GetProperties(
                    BindingFlags.IgnoreCase
                    | BindingFlags.Public
                    | BindingFlags.Instance
                    | BindingFlags.SetProperty)
                 .Where(m =>
                        m.CanWrite
                        && !m.Name.EndsWith("Specified")
                        && !m.Name.Equals("checksum")
                        && m.PropertyType.IsPublic
                        && m.PropertyType.IsSerializable
                        && !m.PropertyType.IsArray
                        && ExpressionParser.IsPredefinedType(m.PropertyType)
                        && !ExpressionParser.GetNonNullableType(m.PropertyType).IsAbstract
                        && !ExpressionParser.GetNonNullableType(m.PropertyType).IsGenericType
                    )
                 .ToArray();
            return _PropertyInfos;
        }

        //"~/SomeFolder/SomePage.aspx"
        public string GetFullURL(string relativePath)
        {
            if (!relativePath.ToLower().StartsWith("http"))
            {
                string sRelative = Page.ResolveUrl(relativePath);
                string sAbsolute = Page.Request.Url.AbsoluteUri.Replace(Page.Request.Url.PathAndQuery,
                    sRelative);
                relativePath = sAbsolute;
            }
            return relativePath;
        }

        private void grid_ApprovalCommand(object sender, ApprovalEventArgs e)
        {
            EnsureDataSource();
            if (CanApprove)
            {
                _postedDoc = DataSource.Get(out _postedDocSrc,
                    out _postedDocKeys,
                    e.LightDoc.DocTypeName,
                    null,
                    e.LightDoc.DocId,
                    RelayUrl);
                PostedDocId = e.LightDoc.DocId;
                PostedDocStatus = e.LightDoc.DocStatus ?? false;
                if (FormApprovalChange != null)
                {
                    FormApprovalChange.Invoke(this,
                        new EventArgs());
                    DataBind();
                }
            }
        }

        private void grid_NeedsDataSource(object sender, EventArgs e) { EnsureDataSource(); }

        /// <summary>
        ///     InfoPathPanel.DataSource needs a valid IDocController
        /// </summary>
        public event EventHandler NeedsDataSource;

        /// <summary>
        ///     NewFormDocKeys & NewFormPropVals are needed to satisfy a user's request for a new form
        /// </summary>
        public event EventHandler NeedsNewFormData;

        private void newButton_Click(object sender, EventArgs e)
        {
            EnsureDataSource();

            if (NewFormDocKeys == null || NewFormDocKeys.Count == 0 || NewFormPropVals == null)
                if (NeedsNewFormData != null)
                    NeedsNewFormData.Invoke(this,
                        new EventArgs());

            object formvals = NewFormPropVals;

            // Copy values from other corm only if they are "non-required"
            if (_preFillCheckBox.Checked)
                if (_preFillCheckBox.Enabled)
                    if (_preFillCheckBox.Visible)
                        if (CopyOfLatestApproved != null)
                            foreach (
                                PropertyInfo _PropertyInfo in
                                    GetFormObjectNavProperties(CopyOfLatestApproved)
                                        .Where(
                                            x =>
                                            !CopyOfLatestApproved.IsDefaultValue(x.Name) &&
                                            ExpressionParser.GetNonNullableType(x.PropertyType) != x.PropertyType))
                                _PropertyInfo.SetValue(formvals,
                                    _PropertyInfo.GetValue(CopyOfLatestApproved,
                                        null),
                                    null);

            // Copy all the values that are explicitly set (probably by the application using this control)
            if (formvals != null)
                foreach (
                    PropertyInfo _PropertyInfo in
                        GetFormObjectNavProperties(NewFormPropVals).Where(x => !NewFormPropVals.IsDefaultValue(x.Name)))
                    _PropertyInfo.SetValue(formvals,
                        _PropertyInfo.GetValue(NewFormPropVals,
                            null),
                        null);

            DataSource.Create(out _postedDocSrc,
                formvals,
                NewFormDocKeys,
                RelayUrl);
            AjaxHelper.Redirect(_newButton,
                _postedDocSrc);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            _CopyLatestApproved = null;
            if (FilterDocKeys != null && FilterDocKeys.Count > 0)
                if (string.IsNullOrWhiteSpace(Caption))
                    Caption = StringTransform.Wordify(DocTypeName);

            _grid.SearchText = _searchTextBox.Text;
            base.OnDataBinding(e);
        }

        /// <summary>
        ///     underlying datagrid button events don't work if the _grid is not loaded with data first
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (Page.IsPostBack)
                EnsureDataSource();
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            EnsureDataSource();

            _newButton.Enabled = !_grid.AuditDocsVisible && FilterDocTypes.Count == 1;
            _DragDropFileUpload.Enabled = !_grid.AuditDocsVisible;
            _auditButton.Visible = _grid.AuditDocsVisible && CanList;
            _preFillCheckBox.Enabled = !_grid.AuditDocsVisible;
            _searchTextBox.Enabled = !_grid.AuditDocsVisible && CanList;

            if (_grid.Results != null)
            {
                if (_preFillCheckBox.Enabled)
                    _preFillCheckBox.Enabled = _grid.Results.Rows.Cast<DataRow>().Any(m => (bool)m["Approved"]);

                if (_preFillCheckBox.Enabled)
                    _preFillCheckBox.Text = string.Format("Copy values from \"{0}\"",
                        _grid.Results.Rows.Cast<DataRow>().FirstOrDefault(m => (bool)m["Approved"])["Title"]);
                else
                    _preFillCheckBox.ToolTip = Resources.FormNotFoundText;
            }

            if (!_auditButton.Visible && !_newButton.Visible && !_DragDropFileUpload.Visible && !_searchTextBox.Visible)
                _commandPanel.Visible = false;

            ApplyDisabledCssClass(_commandPanel);

            base.OnPreRender(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="FileName">
        ///     the filename to associate & rewrite an exception using the FileName.zip as the prefix of
        ///     exception message
        /// </param>
        private void ProcessFormChange(string FileName)
        {
            try
            {
                EnsureDataSource();

                List<string> exceptionMessages = new List<string>();
                _postedDoc = DataSource.Read(out _postedDocSrc,
                       PostedDocXml,
                       out _postedDocKeys,
                       RelayUrl);
                string _PostedDocTypeName = _postedDoc.GetType().Name;

                if (!string.IsNullOrWhiteSpace(_PostedDocTypeName))
                {
                   


                    if ((FilterDocTypes.Count > 0 && !FilterDocTypes.Any(m => m == _PostedDocTypeName)) ||
                        (!string.IsNullOrWhiteSpace(DocTypeName) && DocTypeName != _PostedDocTypeName))
                        exceptionMessages.Add(string.Format(@"""{0}"" form type may not be posted here",
                            _PostedDocTypeName));


                    // Ensure given DocKey's values are positive
                    if (FilterDocKeys != null && FilterDocKeys.Count > 0)
                        foreach (KeyValuePair<string, string> _postedDocKey in _postedDocKeys)
                            if (FilterDocKeys.ContainsKey(_postedDocKey.Key))
                                if (FilterDocKeys[_postedDocKey.Key] != null)
                                    if (FilterDocKeys[_postedDocKey.Key].Count > 0)
                                        if (!FilterDocKeys[_postedDocKey.Key].Contains(_postedDocKey.Value))
                                            exceptionMessages.Add(string.Format(@"""{0}"" is invalid, this form can't be posted here",
                                                _postedDocKey.Key));


                    if (FormChange == null)
                        exceptionMessages.Add("No listeners to alert about this FormChange");

                    //if (exceptionMessages.Count > 0)
                    //    throw new Exception(string.Join("\n", exceptionMessages));
                    FormChange.Invoke(this, new EventArgs());
                }
            } catch (Exception ex)
            {
                throw new Exception(
                    string.Format(
                        "{0}{1}",
                        FileName,
                        ConcatExceptionMessages(ex)
                        ));
            }
        }

        private void up_PreRender(object sender, EventArgs e) { ((UpdatePanel)sender).Attributes["Class"] = CssClassify.GetDefaultCssClass(this); }

        /// <summary>
        ///     handles forms uploaded as simple xml files or a collection of xml files
        ///     zipped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploader_FormUploaded(object sender, EventArgs e)
        {
            List<string> ConfirmationMessages = new List<string>();

            if (!_DragDropFileUpload.PostedFile.FileName.EndsWith(".zip",
                StringComparison.InvariantCultureIgnoreCase))
                using (StreamReader _InputStreamReader = new StreamReader(_DragDropFileUpload.PostedFile.InputStream))
                    try
                    {
                        PostedDocXml = _InputStreamReader.ReadToEnd();
                        ProcessFormChange(_DragDropFileUpload.PostedFile.FileName);
                    }
                    catch (Exception ex)
                    {
                        ConfirmationMessages.Add(ex.Message);
                    }
            else
                using (
                    ZipStorer _ZipStorer = ZipStorer.Open(_DragDropFileUpload.PostedFile.InputStream,
                        FileAccess.Read))
                    foreach (
                        ZipStorer.ZipFileEntry _Entry in
                            _ZipStorer.ReadCentralDir()
                                      .Where(
                                          m =>
                                          m.FilenameInZip.EndsWith(".xml",
                                              StringComparison.CurrentCultureIgnoreCase))
                        )
                        using (MemoryStream _MemoryStream = new MemoryStream())
                            try
                            {
                                _ZipStorer.ExtractFile(_Entry,
                                    _MemoryStream);
                                PostedDocXml = _UTF8Encoding.GetString(_MemoryStream.GetBuffer().ToArray());
                                ProcessFormChange(_Entry.FilenameInZip);
                            }
                            catch (Exception ex)
                            {
                                ConfirmationMessages.Add(ex.Message);
                            }

            if (ConfirmationMessages.Count > 0)
                throw new Exception(Serialize.Json.Serialize(ConfirmationMessages));
        }

        private void uploader_UploadComplete(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                MessageState = MessageState.Normal;
                Message = "Successfully submitted";
            }
        }
    }
}