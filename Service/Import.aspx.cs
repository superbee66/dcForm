using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.UI;
using dCForm.Client.Util;
using dCForm.Core;
using dCForm.Core.Template.Filesystem;
using dCForm.Core.Util;

public partial class _Import : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            Task.Factory.StartNew(() =>
                                  {
                                      ImporterController.ImportDocModelsRunOnce();
                                      ImporterController.ImportFolder();
                                      // make sure all the DOCREV (document templates) have been processed & are known before the rest of the import folder's content are processed as they will depend on there owner doc templates to be processed themselves
                                      foreach (string folderName in Directory.EnumerateDirectories(FilesystemTemplateController.DirectoryPath))
                                          ImporterController.ImportContentFolder(folderName);
                                  });
    }

    protected void UpdateTimer_Tick(object sender, EventArgs e)
    {
        BulletedList1.DataBind();
        BulletedList2.DataBind();
    }
}