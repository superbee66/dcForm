using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using dCForm.Core;
using dCForm.Core.Format;
using dCForm.Core.Storage.Sql;
using dCForm.Client;
using dCForm.Client.Util;
using dCForm.Client.DCF_Relay;

public partial class _LucToSql : Page
{
    private static readonly SqlController _SqlController = new SqlController();

    private static readonly string[] DocTypeNameExclusions =
    {
        //"FORM1613A",
        //"FORM1617",
        //"FORM1623A",
        //"FORM1625A",
        //"FORM500",
        //"DOCREV",
        //"ISP_Client_SEP_Beta_Participants"
    };

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string RelayUrl = DCF_Relay.GetRelayUrl();
            Task.Factory.StartNew(() =>
                                  {
                                      lock (_SqlController)
                                      {
                                          string DocSrc = string.Empty, DocData = string.Empty;

                                          var SamplesTodo = ServiceController
                                            .Instance
                                            .List(new List<string> { "DOCREV" }, null, null, null, int.MaxValue, 0)
                                            .Select(m => new { Name = m.GetTargetDocName(), Rev = m.GetTargetDocVer() })
                                            .Distinct()
                                            .ToList();

                                          List<string> DocTypeNames = SamplesTodo.Select(m => m.Name).Distinct().ToList();

                                          foreach (LightDoc src in ServiceController.LuceneController.List(DocTypeNames, null, null, null, int.MaxValue))
                                          {
                                              DocData = ServiceController.LuceneController.GetDocData(out DocSrc, src.DocTypeName, src.DocId);

                                              string DocRev = "???", DocTypeName = "????";
                                              try
                                              {
                                                  DocRev = DocInterpreter.Instance.ReadRevision(DocData);
                                                  DocTypeName = DocInterpreter.Instance.ReadDocTypeName(DocData);
                                                  src.DocTitle += ", rev " + DocRev;

                                                  var sample = SamplesTodo.FirstOrDefault(m => m.Name == DocTypeName && DocRev == m.Rev);
                                                  if (sample != null)
                                                  {
                                                      _SqlController.Submit(DocData, "noone@nowhere.com", RelayUrl);
                                                      _List_ImporterLightDoc.Insert(0, src);
                                                  }
                                              }
                                              catch (SubmitDeniedException) { }
                                              catch (Exception ex)
                                              {
                                                  string filename = string.Format(@"{0}\fail\{1}\{2}", RequestPaths.PhysicalApplicationPath, GetType().Name, DocDataHandler.GetFilename(DocData));

                                                  FileInfo _FileInfo = new FileInfo(filename);

                                                  _FileInfo.Directory.mkdir();

                                                  File.WriteAllText(_FileInfo.FullName, DocData);
                                                  File.WriteAllText(_FileInfo.FullName + ".dump", string.Format("==start exception==\n\n\n\n\n{0}\n\n\n\n\n{1}", ex.Message, ex.StackTrace));

                                                  src.DocTitle += ", " + ex.Message;
                                                  _List_ImporterLightDoc.Insert(0, src);
                                              }
                                              //finally
                                              //{
                                              //    var sample = SamplesTodo.FirstOrDefault(m => m.Name == DocTypeName && DocRev == m.Rev);
                                              //    if (sample != null)
                                              //        SamplesTodo.Remove(SamplesTodo.FirstOrDefault(m => m.Name == DocTypeName && DocRev == m.Rev));
                                              //}
                                          }



                                          //foreach (var sample in DocTypeNameRevSamples)
                                          //{
                                          //    var DocTypeNameLightDocList = ;

                                          //}

                                          //    foreach (string DocTypeName in DocTypeNames)
                                          //        ProcessLightDocList(ServiceController.LuceneController.List(new List<string> { DocTypeName }, null, null, null, int.MaxValue), RelayUrl);


                                          //List<string> DocTypeNames = ServiceController
                                          //    .Instance
                                          //    .List(new List<string> { "DOCREV" }, null, null, null, int.MaxValue, 0)
                                          //    .Select(lightdoc => lightdoc.GetTargetDocName())
                                          //    .Where(DocTypeName => !DocTypeNameExclusions.Any(m => m.ToLower() == DocTypeName.ToLower()))
                                          //    .Distinct()
                                          //    .ToList();

                                          //foreach (string DocTypeName in DocTypeNames)
                                          //    ProcessLightDocList(ServiceController.LuceneController.List(new List<string> { DocTypeName }, null, null, null, int.MaxValue), RelayUrl);
                                      }
                                  });
        }
    }

    private void ProcessLightDocList(List<LightDoc> LightDocList, string RelayUrl)
    {

    }

    private static readonly List<LightDoc> _List_ImporterLightDoc = new List<LightDoc>();

    public static List<LightDoc> List_ImporterLightDoc {
        get { return _List_ImporterLightDoc; }
    }

    protected void UpdateTimer_Tick(object sender, EventArgs e) { BulletedList1.DataBind(); }
}