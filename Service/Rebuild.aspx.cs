using System;
using System.Threading.Tasks;
using System.Web.UI;
using dCForm.Storage.Nosql;

public partial class _Rebuild : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            Task.Factory.StartNew(() => { new LuceneController().Rebuild(); });
    }
}