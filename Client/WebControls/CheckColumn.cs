using System;
using System.Web.UI.WebControls;

namespace dCForm.Client.WebControls
{
    internal sealed class CheckBoxColumn : TemplateColumn
    {
        /// <summary>
        ///     Internal storage of the CheckBoxItem that is to be used for the edit state.
        /// </summary>
        private readonly CheckBoxItem editItem;

        /// <summary>
        ///     Internal storage of the CheckBoxItem that is to be used for the view state.
        /// </summary>
        private readonly CheckBoxItem viewItem;

        /// <summary>
        ///     Initialize our CheckBoxColumn.
        /// </summary>
        public CheckBoxColumn()
        {
            // set the view one as read-only
            viewItem = new CheckBoxItem(false); // SAW was false
            ItemTemplate = viewItem;

            // let the edit check box be editable
            editItem = new CheckBoxItem(true);
            EditItemTemplate = editItem;
        }

        /// <summary>
        ///     Initialize our CheckBoxColumn with an optional ImmediatePostback capability.
        /// </summary>
        /// <param name="ImmediatePostback">
        ///     If true then each change of state of the CheckBox item
        ///     will cause an event to be fired immediately on the server.
        /// </param>
        public CheckBoxColumn(bool ImmediatePostback)
        {
            // set the view one as read-only
            viewItem = new CheckBoxItem(ImmediatePostback);
            ItemTemplate = viewItem;

            // let the edit check box be editable
            editItem = new CheckBoxItem(true);
            EditItemTemplate = editItem;

            AutoPostBack = ImmediatePostback;
        }

        /// <summary>
        ///     If true then then each click on a CheckBox will cause an event to be fired on the server.
        /// </summary>
        public bool AutoPostBack
        {
            set
            {
                viewItem.AutoPostBack = value;
                editItem.AutoPostBack = value;
            }
            get { return viewItem.AutoPostBack; }
        }

        /// <summary>
        ///     The DataField that we wish our control to bind to.
        /// </summary>
        public string DataField
        {
            get { return viewItem.DataField; }
            set
            {
                viewItem.DataField = value;
                editItem.DataField = value;
            }
        }

        /// <summary>
        ///     Occurs when the value of the Checked property changes between posts to the server.
        /// </summary>
        /// <remarks>
        ///     The <b>CheckedChanged</b> event is raised when the value of the Checked property changes between posts to the
        ///     server.
        ///     <b>Note</b>   This event does not post the page back to the server unless the AutoPostBack property is set to true.
        ///     <b>Note</b>   The control must have viewstate enabled for the <b>CheckedChanged</b> event to work correctly.
        /// </remarks>
        public event EventHandler CheckedChanged
        {
            add
            {
                viewItem.CheckedChanged += value;
                editItem.CheckedChanged += value;
            }
            remove
            {
                viewItem.CheckedChanged -= value;
                editItem.CheckedChanged -= value;
            }
        }
    }
}