using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace dCForm.Client.WebControls
{
    public class SearchTextBox : TextBox
    {
        [DefaultValue("")]
        [Themeable(false)]
        [Category("Appearance")]
        public virtual string WatermarkText
        {
            get
            {
                return string.Format("{0}",
                    ViewState["WatermarkText"]);
            }
            set { ViewState["WatermarkText"] = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            CssClass = GetType().Name;

            // watermark does not work in internet explorer for some reason... I'm thinking there is a conflict between multiple versions of jquery on the site it was tested
//            js.ScriptManager.RegisterClientScriptResources(
//               this,
//               "jquery-1.10.2.js",
//               "jquery-ui-1.10.4.custom.js",
//               "jquery.watermark.js");

//            js.ScriptManager.RegisterStartupScript(
//                this,
//                @"
//    jQuery(document).ready(
//        function() {{
//            jQuery( '#{0}' ).watermark( '{1}' ); }} );
//                ",
//                ClientID,
//                WatermarkText);


            base.OnPreRender(e);
        }
    }
}