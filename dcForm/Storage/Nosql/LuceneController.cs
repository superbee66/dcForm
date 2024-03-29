﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using dCForm.Util;
using dCForm.Format;
using dCForm.Util.Zip;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace dCForm.Storage.Nosql
{
    public class LuceneController : BaseController, IDocController, IDocData
    {
        /// <summary>
        ///     should the index be optimized when some other outside method requests us to?
        ///     TODO:research a less invasive runtime optimization strategy. When resources run low NativeFSLocks exceptions seem
        ///     to occur much more frequently
        ///     example:
        ///     Lock obtain timed out: NativeFSLock@C:\inetpub\wwwroot\dCForm\db\write.lock
        ///     Description: An unhandled exception occurred during the execution of the current web request. Please review the
        ///     stack trace for more information about the error and where it originated in the code.
        ///     Exception Details: System.ServiceModel.FaultException`1[[System.ServiceModel.ExceptionDetail, System.ServiceModel,
        ///     Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]: Lock obtain timed out:
        ///     NativeFSLock@C:\inetpub\wwwroot\dCForm\db\write.lock
        ///     Source Error:
        ///     An unhandled exception was generated during the execution of the current web request. Information regarding the
        ///     origin and location of the exception can be identified using the exception stack trace below.
        ///     Stack Trace:
        ///     [FaultException`1: Lock obtain timed out: NativeFSLock@C:\inetpub\wwwroot\dCForm\db\write.lock]
        ///     System.Runtime.Remoting.Proxies.RealProxy.HandleReturnMessage(IMessage reqMsg, IMessage retMsg) +14579646
        ///     System.Runtime.Remoting.Proxies.RealProxy.PrivateInvoke(MessageData& msgData, Int32 type) +622
        ///     ISP.InfoPathServices.IService.Info(String DocTypeName) +0
        ///     [TargetInvocationException: Exception has been thrown by the target of an invocation.]
        ///     System.RuntimeMethodHandle.InvokeMethod(Object target, Object[] arguments, Signature sig, Boolean constructor) +0
        ///     System.Reflection.RuntimeMethodInfo.UnsafeInvokeInternal(Object obj, Object[] parameters, Object[] arguments) +76
        ///     System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters,
        ///     CultureInfo culture) +211
        ///     dCForm.<>c__DisplayClass2.
        ///     <Info>
        ///         b__1() +164
        ///         dCForm.Util.CacheMan.Cache(Func`1 itemFactory, Boolean forcedRefresh, String cacheKey) +133
        ///         dCForm.ClientBaseDocController`1.Info(String DocTypeName) +219
        ///         ISP.ISPFormsList.
        ///         <get_DataSource>
        ///             b__0(String m) +192
        ///             System.Linq.WhereSelectEnumerableIterator`2.MoveNext() +270
        ///             System.Linq.Buffer`1..ctor(IEnumerable`1 source) +488
        ///             System.Linq.
        ///             <GetEnumerator>
        ///                 d__0.MoveNext() +252
        ///                 System.Web.UI.WebControls.ListControl.PerformDataBinding(IEnumerable dataSource) +760
        ///                 System.Web.UI.WebControls.ListControl.PerformSelect() +51
        ///                 System.Web.UI.Control.DataBindChildren() +12397719
        ///                 System.Web.UI.Control.DataBind(Boolean raiseOnDataBinding) +321
        ///                 System.Web.UI.Control.DataBindChildren() +12397719
        ///                 System.Web.UI.Control.DataBind(Boolean raiseOnDataBinding) +321
        ///                 System.Web.UI.Control.DataBindChildren() +12397719
        ///                 System.Web.UI.Control.DataBind(Boolean raiseOnDataBinding) +321
        ///                 System.Web.UI.Control.DataBindChildren() +12397719
        ///                 System.Web.UI.Control.DataBind(Boolean raiseOnDataBinding) +321
        ///                 System.Web.UI.Control.DataBindChildren() +12397719
        ///                 System.Web.UI.Control.DataBind(Boolean raiseOnDataBinding) +321
        ///                 ISP.ISPFormsList.Page_Load(Object sender, EventArgs e) +396
        ///                 ISP.BasePage.OnLoad(EventArgs e) +31
        ///                 System.Web.UI.Control.LoadRecursive() +71
        ///                 System.Web.UI.Page.ProcessRequestMain(Boolean includeStagesBeforeAsyncPoint, Boolean
        ///                 includeStagesAfterAsyncPoint) +3178
        /// </summary>
        public const bool OPTIMIZE_WHEN_REQUESTED = true;

        public const Version LUCENE_VERSION = Version.LUCENE_30;

        internal const int PAGE_SIZE_DEFAULT = 150;

        /// <summary>
        ///     defaults to RequestPaths\db\PhysicalApplicationPath when running in an IIS apppool
        /// </summary>
        public string DirectoryPath { get; set; }

        public LightDoc Status(string DocTypeName, string DocId, bool DocStatus, string DocSubmittedBy, string RelayUrl = null)
        {
            string DocSrc;
            return
                Submit(
                    GetDocData(out DocSrc,
                        DocTypeName,
                        DocId),
                    DocSubmittedBy,
                    RelayUrl,
                    DocStatus);
        }

        public List<LightDoc> Audit(string DocTypeName, string DocId, string RelayUrl = null)
        {
            string DocSrc = string.Empty;

            List<LightDoc> _Audit = new List<LightDoc>();

            foreach (LightDoc _LightDoc in GetDoc(out DocSrc, DocTypeName, DocId, RelayUrl).AsDocSubmissions().Keys)
            {
                //TODO:Move _LightDoc.DocSrc to DocExchange (closer to the surface)
                _LightDoc.DocSrc = Nav.ToUrl(DocTypeName,
                    DocId,
                    RelayUrl,
                    _LightDoc.LogSequenceNumber);
                _LightDoc.DocTitle += string.Format(" (modified {0})",
                    _LightDoc.DocSubmitDate.ToString("g"));
                _Audit.Add(_LightDoc);
            }

            return _Audit;
        }

        public List<LightDoc> List(List<string> DocTypeNames, Dictionary<string, List<string>> DocKeys = null, Dictionary<string, List<string>> DocProperties = null, string KeyWord = null, int PageSize = PAGE_SIZE_DEFAULT, int PageIndex = 0, string RelayUrl = null)
        {
            return ListDocuments(
                DocTypeNames,
                DocKeys,
                DocProperties,
                KeyWord,
                PageSize,
                PageIndex,
                RelayUrl,
                DirectoryPath).Select(m => m.GetBinaryValue(Parm.LightDoc).FromBytes<LightDoc>()).ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="DocSrc"></param>
        /// <param name="DocKeysFromDocId"></param>
        /// <param name="DocTypeName"></param>
        /// <param name="DocKeys">have precedence over DocId when is not null</param>
        /// <param name="DocId"></param>
        /// <param name="RelayUrl"></param>
        /// <returns></returns>
        public object Get(out string DocSrc, out Dictionary<string, string> DocKeysFromDocId, string DocTypeName, Dictionary<string, string> DocKeys = null, string DocId = null, string RelayUrl = null)
        {
            DocSrc = null;
            DocKeysFromDocId = new Dictionary<string, string>();
            BaseDoc _BaseDoc = null;
            Dictionary<string, List<string>> _RequiredDocKeys = new Dictionary<string, List<string>>();
            DocKeys = DocKeys ?? DocKey.DocIdToKeys(DocId);

            foreach (KeyValuePair<string, string> _Item in DocKeys)
                _RequiredDocKeys[_Item.Key] = new List<string> { _Item.Value };

            //BUG:DocStatus is not persisted by in the DocData; this band-aid gets it from the LightDoc in order to return it to the calling DataContract method
            foreach (Document _Document in ListDocuments(new List<string> { DocTypeName }, _RequiredDocKeys, null, null, 1, 0, RelayUrl, DirectoryPath))
            {
                _BaseDoc = DocInterpreter.Instance.Read(_Document.AsDocSubmissions().Last().Value);
                if (_BaseDoc.DocIdKeys.Count == DocKeys.Count)
                {
                    DocSrc = Nav.ToUrl(DocTypeName, DocId, RelayUrl);
                    DocKeysFromDocId = _BaseDoc.DocIdKeys;
                    // there is a chance the DocStatus may not be set when it comes to items like DOCREV BaseDocType(s)
                    _BaseDoc.DocStatus = bool.Parse(_Document.Get(Parm.DocStatus) ?? Boolean.FalseString);
                    break;
                }
            }

            return _BaseDoc;
        }

        public string GetDocData(out string DocSrc, string DocTypeName, string DocId = null, string RelayUrl = null, long LogSequenceNumber = 0)
        {
            Document _Document = GetDoc(out DocSrc,
                DocTypeName,
                DocId,
                RelayUrl);

            return _Document == null
                       ? null
                       : _Document.AsDocSubmissions()
                                  .Last(m => LogSequenceNumber == 0 || m.Key.LogSequenceNumber == LogSequenceNumber)
                                  .Value;
        }

        #region private

        /// <summary>
        ///     ListDocuments always returns results in reverse-chronological order
        /// </summary>
        private static readonly Sort _ListDocumentsSort = new Sort(new SortField(Parm.LogSequenceNumber, SortField.LONG, true));

        internal List<Document> ListDocuments(List<string> DocTypeNames, Dictionary<string, List<string>> DocKeys = null, Dictionary<string, List<string>> DocProperties = null, string KeyWord = null, int PageSize = 150, int PageIndex = 0, string RelayUrl = null, string IndexDirectory = null)
        {
            if (CreateNeeded())
                return new List<Document>();

            //TODO:need to build the query bypassing Parse method of the lucene API (currently we stringbuild the query only for the underlying QueryParser.Parse to take it appart
            string _DocTypeNames = KeysToPredicate(Parm.DocTypeName, DocTypeNames);
            string _DocKeyFields = KeysToPredicate(DocKeys);
            string _DocPropertyFields = KeysToPredicate(DocProperties);
            string _keywords = string.IsNullOrWhiteSpace(KeyWord)
                                   ? string.Empty
                                   : string.Format("+{0}:{1}*",
                                   Parm.DocData,
                                   KeyWord.Trim(' ', '*'));

            if (_keywords.Length > 0)
                if (_keywords.IndexOf(' ') > 0)
                    _keywords = _keywords.Replace("*", "");


            List<string> _Fields = new List<string>();
            if (!string.IsNullOrWhiteSpace(_DocTypeNames))
                _Fields.Add(Parm.DocTypeName);

            if (!string.IsNullOrWhiteSpace(_DocKeyFields))
                _Fields.AddRange(DocKeys.Keys.ToList());

            if (!string.IsNullOrWhiteSpace(_DocPropertyFields))
                _Fields.AddRange(DocProperties.Keys.ToList());

            string _QueryString = string.Join(" ", new[]
            {
                _DocTypeNames,
                _DocKeyFields,
                _DocPropertyFields,
                _keywords
            }.Where(m => !string.IsNullOrWhiteSpace(m) && m != "+()").Select(m => string.Format("{0}", m)).ToArray());

            using (KeywordAnalyzer _KeywordAnalyzer = new KeywordAnalyzer())
            using (PerFieldAnalyzerWrapper _PerFieldAnalyzerWrapper = new PerFieldAnalyzerWrapper(_KeywordAnalyzer))
            using (IndexSearcher _Searcher = new IndexSearcher(FSDirectory.Open(DirectoryPath), true))
            {
                ScoreDoc[] _ScoreDoc = _Searcher.Search(new MultiFieldQueryParser(LUCENE_VERSION,
                    _Fields.ToArray(),
                    _PerFieldAnalyzerWrapper)
                {
                    AllowLeadingWildcard = false
                }.Parse(_QueryString),
                    null,
                    PageIndex * PageSize + PageSize,
                    _ListDocumentsSort).ScoreDocs;

                return PageIndex * PageSize < _ScoreDoc.Length
                           ? _ScoreDoc.Skip(PageIndex * PageSize).Select(m => _Searcher.Doc(
                               m.Doc,
                               //BANDAID:for some reason binary fields don't seem to lazy load; they throw an IO exception
                               // until this is resolved it will be assumed the full document (all byte[] fields) will be
                               // needed thus avoiding any lazy loading here
                               PageSize == 1
                               ? null
                               : LazyLoadFieldSelector.Instance)).ToList()
                           : new List<Document>();
            }
        }

        private class LazyLoadFieldSelector : FieldSelector
        {
            public static readonly LazyLoadFieldSelector Instance = new LazyLoadFieldSelector();

            private static readonly string[] LAZY_LOAD_FIELDS =
            {
                Parm.Submissions,
                Parm.DocStatus,
                Parm.LogSequenceNumber,
                Parm.DocTypeName,
                Parm.DocChecksum
            };

            public FieldSelectorResult Accept(string fieldName)
            {
                return fieldName == Parm.LightDoc
                           ? FieldSelectorResult.LOAD
                           : LAZY_LOAD_FIELDS.Contains(fieldName)
                                 ? FieldSelectorResult.LAZY_LOAD
                                 : FieldSelectorResult.NO_LOAD;
            }
        }

        private Document GetDoc(out string DocSrc, string DocTypeName, string DocId = null, string RelayUrl = null, string IndexDirectory = null)
        {
            Dictionary<string, List<string>> _RequiredDocKeys = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<string, string> _Item in DocKey.DocIdToKeys(DocId))
                _RequiredDocKeys[_Item.Key] = new List<string>
                {
                    _Item.Value
                };

            //TODO:GetDoc needs to query by the exact key, not that a subset of DocKeys exist
            Document _Document = ListDocuments(new List<string>
            {
                DocTypeName
            },
                _RequiredDocKeys,
                null,
                null,
                1,
                0,
                RelayUrl,
                IndexDirectory).FirstOrDefault();

            DocSrc = _Document == null
                         ? null
                         : Nav.ToUrl(DocTypeName,
                             DocId,
                             RelayUrl);

            return _Document;
        }

        public LightDoc Submit(string DocData, string DocSubmittedBy, string RelayUrl = null, bool? DocStatus = null, DateTime? SubmittedDate = null, Dictionary<string, string> DocKeys = null, string DocTitle = null)
        {
            if (SubmittedDate != null)
                throw new NotImplementedException("SubmittedDate can't be specified explicitly in submit method as it's not implemented yet");

            BaseDoc _BaseDoc = DocInterpreter.Instance.Read(DocData, true);
            DocProcessingInstructions _DocProcessingInstructions = DocInterpreter.Instance.ReadDocPI(DocData);

            _DocProcessingInstructions.DocStatus = DocStatus ?? _BaseDoc.DocStatus ?? false;
            _DocProcessingInstructions.DocTitle = DocTitle ?? _BaseDoc.DocTitle;
            _DocProcessingInstructions.DocIdKeys = DocKeys ?? _BaseDoc.DocIdKeys;

            string DocSrc;
            Document _Document = GetDoc(out DocSrc, _BaseDoc.DocTypeName, _BaseDoc.DocId, RelayUrl);
            Dictionary<LightDoc, string> _DocSubmissions = new Dictionary<LightDoc, string>();

            if (_Document != null)
            {
                if (_DocProcessingInstructions.DocChecksum == int.Parse(_Document.Get(Parm.DocChecksum)))
                    throw new NoChangesSinceLastSubmitException();

                if ((_Document.Get(Parm.DocStatus) ?? bool.FalseString) == bool.TrueString)
                    throw new NoOverwriteOfPreviouslyApproveException();

                _DocSubmissions = _Document.AsDocSubmissions();
                _DocProcessingInstructions.DocTitle = _DocSubmissions.Keys.First().DocTitle;

            }

            LightDoc _LightDoc = _BaseDoc.ToLightDoc();
            _LightDoc.DocSubmitDate = DateTime.Now;

            // adjust PI before adding this submission; in particular, the DocChecksum was recalculated within the scope of this method
            // to reflect what is really being submitted. when additional submissions occur there after they will be compared to this
            // current submission checksum to see if there are any changes to persist
            _DocSubmissions.Add(
                _LightDoc,
                DocInterpreter.Instance.WritePI(DocData, _DocProcessingInstructions));

            using (StandardAnalyzer _StandardAnalyzer = new StandardAnalyzer(LUCENE_VERSION))
            using (IndexWriter _CurrentIndexWriter = new IndexWriter(FSDirectory.Open(DirectoryPath), _StandardAnalyzer, CreateNeeded(), IndexWriter.MaxFieldLength.UNLIMITED))
            {
                if (_DocSubmissions.Count > 1)
                    _CurrentIndexWriter.UpdateDocument(
                        _BaseDoc.docTermFromBaseDoc(),
                        _DocSubmissions.AsDocument());
                else
                    _CurrentIndexWriter.AddDocument(
                        _DocSubmissions.AsDocument());

                _CurrentIndexWriter.Commit();
            }

            return _LightDoc;
        }

        public LuceneController() { CreateNeeded(); }

        private bool CreateNeeded()
        {
            DirectoryPath = RequestPaths.GetPhysicalApplicationPath("App_Data\\nosql");

            // ensure the import folder actually exists
            if (!Directory.Exists(DirectoryPath))
            {
                new DirectoryInfo(DirectoryPath)
                    .mkdir()
                    .Attributes = FileAttributes.NotContentIndexed | FileAttributes.Hidden;
                return true;
            }

            return !Directory.EnumerateFiles(DirectoryPath).Any();
        }

        private static string KeysToPredicate(string FieldName, List<string> DocTypes)
        {
            return DocTypes == null && DocTypes.Count > 0
                       ? string.Empty
                       : string.Format(DocTypes.Count == 1
                                           ? "+{0}"
                                           : "+({0})",
                           string.Join(" OR ",
                               DocTypes.Select(m => string.Format("{0}:\"{1}\"",
                                   FieldName,
                                   m))));
        }

        private static string KeysToPredicate(Dictionary<string, List<string>> Keys)
        {
            return Keys == null
                       ? string.Empty
                       : string.Join(" ",
                           Keys.Keys.Cast<string>().Select(m => string.Format("{0}",
                               KeysToPredicate(m,
                                   Keys[m]))).ToArray());
        }

        #endregion
    }
}