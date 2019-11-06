using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using dCForm.Client;
using dCForm.Core.Format;
using dCForm.Core.Storage.Nosql;
using dCForm.Core.Storage.Sql;

namespace dCForm.Core
{
    /// <summary>
    ///     utilizes LuceneController as primary means of persisting objects. The SqlController
    ///     is also called for Status & Submit in order to record our objects for reporting
    ///     purposes in the enterprise/organization.
    /// </summary>
    public class ServiceController : BaseController, IDocController
    {
        //TODO:Build service from POCOs
        public static readonly SqlController SqlController = new SqlController();
        public static readonly LuceneController LuceneController = new LuceneController();
        public static readonly Lazy<ServiceController> _Instance = new Lazy<ServiceController>(() => new ServiceController());

        /// <summary>
        ///     singleton instance safe for multithreading
        /// </summary>
        public static ServiceController Instance {
            get { return _Instance.Value; }
        }


        protected virtual void Optimize() { Task.Factory.StartNew(() => { LuceneController.Rebuild(); }); }

        #region IDocController Methods

        /// <summary>
        ///     Persists changes to LuceneController & SqlController
        /// </summary>
        /// <param name="DocTypeName"></param>
        /// <param name="DocId"></param>
        /// <param name="DocStatus"></param>
        /// <param name="RelayUrl"></param>
        /// <returns></returns>
        public virtual LightDoc Status(string DocTypeName, string DocId, bool DocStatus, string DocSubmittedBy, string RelayUrl = null)
        {
            LightDoc _LightDoc = LuceneController.Status(DocTypeName, DocId, DocStatus, DocSubmittedBy, RelayUrl);

            StartNewTask(() => SqlController.Status(DocTypeName, DocId, DocStatus, DocSubmittedBy, RelayUrl));
            return _LightDoc;
        }

        private static void StartNewTask<TResult>(Func<TResult> a)
        {
#if DEBUG
            a.Invoke();
#else
        
            Task.Factory.StartNew(
                a,
                CancellationToken.None,
                TaskCreationOptions.PreferFairness,
                TaskScheduler.Default);
#endif
        }


        /// <summary>
        ///     Persists changes to LuceneController & SqlController
        /// </summary>
        /// <param name="DocData"></param>
        /// <param name="DocSubmittedBy"></param>
        /// <param name="RelayUrl"></param>
        /// <param name="SubmittedDate"></param>
        /// <param name="DocKeys"></param>
        /// <param name="DocTitle"></param>
        /// <returns></returns>
        public LightDoc Submit(string DocData, string DocSubmittedBy, string RelayUrl = null, bool? DocStatus = null, DateTime? SubmittedDate = null, Dictionary<string, string> DocKeys = null, string DocTitle = null)
        {

            DocProcessingInstructions _DocProcessingInstructions = DocInterpreter.Instance.ReadDocPI(DocData);
            //TODO:Before production, need to implemented proper way of including signature(s) to calc
            int DocChecksum = CalcDocChecksum(DocData, DocStatus);

            // make sure something has changed since this doc was served up
            if (_DocProcessingInstructions.DocChecksum == DocChecksum)
                throw new NoChangesSinceRenderedException();
            else
                _DocProcessingInstructions.DocChecksum = DocChecksum;

            //TODO:Rethink this logic & relocate it somewhere better
            if (DocKeys != null || !string.IsNullOrWhiteSpace(DocTitle) || DocStatus != null || !string.IsNullOrWhiteSpace(DocSubmittedBy))
            {
                if (DocKeys != null)
                    _DocProcessingInstructions.DocIdKeys = DocKeys;

                if (DocStatus != null)
                    _DocProcessingInstructions.DocStatus = DocStatus;

                if (!string.IsNullOrWhiteSpace(DocTitle))
                    _DocProcessingInstructions.DocTitle = DocTitle;

                if (!string.IsNullOrWhiteSpace(DocSubmittedBy))
                    _DocProcessingInstructions.DocSubmittedBy = DocSubmittedBy;
            }

            DocData = DocInterpreter.Instance.WritePI(DocData, _DocProcessingInstructions);



            // validate the content against it's XSD if it's being "approved" as good captured information for the organization
            // now is a good time to do this as the exception we want the user to see first would have hacazd there chance
            DocInterpreter.Instance.Validate(DocData);
            LightDoc _LightDoc = LuceneController.Submit(DocData, DocSubmittedBy, RelayUrl, DocStatus, SubmittedDate, DocKeys, DocTitle);
            StartNewTask(() => SqlController.Submit(DocData, DocSubmittedBy, RelayUrl, DocStatus, SubmittedDate, DocKeys, DocTitle));
            return _LightDoc;
        }

        /// <summary>
        ///     Persists changes to LuceneController & SqlController without running validation checks
        /// </summary>
        /// <param name="DocData"></param>
        /// <param name="DocSubmittedBy"></param>
        /// <param name="RelayUrl"></param>
        /// <param name="SubmittedDate"></param>
        /// <param name="DocKeys"></param>
        /// <param name="DocTitle"></param>
        /// <returns></returns>
        public LightDoc Import(string DocData, string DocSubmittedBy = null, string RelayUrl = null, bool? DocStatus = null, DateTime? SubmittedDate = null, Dictionary<string, string> DocKeys = null, string DocTitle = null)
        {
            //DocData = PIRewrite(DocData, DocStatus, SubmittedDate, DocKeys, DocTitle);
            LightDoc _LightDoc = LuceneController.Submit(DocData, DocSubmittedBy, RelayUrl, DocStatus, SubmittedDate, DocKeys, DocTitle);

            //when not in debug mode perform SQL operations on another thread since they seem to be costly
            SqlController.Submit(DocData, DocSubmittedBy, RelayUrl, DocStatus, SubmittedDate, DocKeys, DocTitle);
            if (_LightDoc.DocTypeName == "DOCREV")
            {
                BaseDoc _BaseDoc = DocInterpreter.Instance.Create(_LightDoc.GetTargetDocName());
                _BaseDoc.DocIdKeys = new Dictionary<string, string> { { "DefaultAsOfDate", DateTime.UtcNow.ToShortDateString() } };
                SqlController.Submit(_BaseDoc.GetDocData(), "system@nowhere.com");
            }
            return _LightDoc;
        }

        public virtual object Get(out string DocSrc, out Dictionary<string, string> DocKeysFromDocId, string DocTypeName, Dictionary<string, string> DocKeys = null, string DocId = null, string RelayUrl = null) { return LuceneController.Get(out DocSrc, out DocKeysFromDocId, DocTypeName, DocKeys, DocId, RelayUrl); }

        public virtual List<LightDoc> List(List<string> DocTypes, Dictionary<string, List<string>> DocKeys = null, Dictionary<string, List<string>> DocProperties = null, string KeyWord = null, int PageSize = 150, int PageIndex = 0, string RelayUrl = null) { return LuceneController.List(DocTypes, DocKeys, DocProperties, KeyWord, PageSize, PageIndex, RelayUrl); }

        public virtual List<LightDoc> Audit(string DocTypeName, string DocId, string RelayUrl = null) { return LuceneController.Audit(DocTypeName, DocId, RelayUrl); }

        #endregion IDocController Methods
    }
}