using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dCForm.Client.WebControls
{
    internal class CheckBoxItem : ITemplate
    {
        /// <summary>
        ///     Internal storage for the readOnly flag.
        /// </summary>
        private readonly bool readOnly = true;

        /// <summary>
        ///     The CheckBoxItem constructor
        /// </summary>
        /// <param name="editable">true if the item is to be in its editable state, false for the item to be disabled.</param>
        public CheckBoxItem(bool editable) { readOnly = !editable; }

        /// <summary>
        ///     Set the AutoPostBack flag. If this is true then each time a CheckBox is clicked
        ///     in the Column that contains this item then an event is raised on the server.
        /// </summary>
        public bool AutoPostBack { set; get; }

        /// <summary>
        ///     Used to set the DataField that we wish to represent with this CheckBox.
        /// </summary>
        public string DataField { get; set; }

        /// <summary>
        ///     Instantiates the CheckBox that we wish to represent in this column.
        /// </summary>
        /// <param name="container">The container into which the control or controls are added.</param>
        void ITemplate.InstantiateIn(Control container)
        {
            CheckBox box = new CheckBox();
            box.DataBinding += BindData;
            box.AutoPostBack = AutoPostBack;
            box.CheckedChanged += OnCheckChanged;
            container.Controls.Add(box);
        }

        /// <summary>
        ///     Handler for the DataBinding event where we bind the data for a specific row
        ///     to the CheckBox.
        /// </summary>
        /// <param name="sender">The raiser of the event.</param>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        private void BindData(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox) sender;
            DataGridItem container = (DataGridItem) box.NamingContainer;
            box.Enabled = !readOnly;
            bool Checked = false;
            box.Checked = bool.TryParse(
                string.Format("{0}",
                    container.DataItem.GetType().GetProperty(DataField).GetValue(container.DataItem,
                        null)),
                out Checked) && Checked;
        }

        /// <summary>
        ///     Our CheckChanged event
        /// </summary>
        public event EventHandler CheckedChanged;

        /// <summary>
        ///     This is a common handler for all the Checkboxes.
        /// </summary>
        /// <param name="sender">The raiser of this event a CheckBox.</param>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        private void OnCheckChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged(sender,
                    e);
        }
    }
}