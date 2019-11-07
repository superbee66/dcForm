using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using dCForm.Client;
using dCForm.Client.Util;
using dCForm.Template.Filesystem;

namespace dCForm
{
    /// <summary>
    ///     Closes the DocExchange ServiceHost & clears all MemoryCache when any changes occur in the
    ///     FilesystemTemplateController.DirectoryPath. The files within that directory influence  This is similar behavior at
    ///     someone editing files in the IIS App_Code directory triggering IIS to recompile. the ServiceKnownType list for our
    ///     ServiceHost.
    /// </summary>
    public class DocExchangeServiceHostFactory : ServiceHostFactory
    {
        /// <summary>
        ///     directory that will be created if not existing then watched
        /// </summary>
        private static readonly FileSystemWatcher[] _FileSystemWatchers =
            new[]
            {
                FilesystemTemplateController.DirectoryPath,
                Directory.Exists(RequestPaths.GetPhysicalApplicationPath("App_Code"))
                    ? RequestPaths.GetPhysicalApplicationPath("App_Code")
                    : string.Empty
            }
                .Where(Directory.Exists)
                .Select(path =>
                        new FileSystemWatcher(new DirectoryInfo(path).mkdir().FullName)
                        {
                            EnableRaisingEvents = false,
                            IncludeSubdirectories = true,
                            NotifyFilter = NotifyFilters.CreationTime |
                                           NotifyFilters.DirectoryName |
                                           NotifyFilters.FileName |
                                           NotifyFilters.LastWrite |
                                           NotifyFilters.Size,
                            Filter = "*.*"
                        }).ToArray();

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHost _ServiceHost = base.CreateServiceHost(serviceType, baseAddresses);

            foreach (FileSystemWatcher _FileSystemWatcher in _FileSystemWatchers)
            {
                _FileSystemWatcher.Changed += (o, args) => Reset(_ServiceHost);
                _FileSystemWatcher.Created += (o, args) => Reset(_ServiceHost);
                _FileSystemWatcher.Deleted += (o, args) => Reset(_ServiceHost);
                _FileSystemWatcher.Renamed += (o, args) => Reset(_ServiceHost);

                _FileSystemWatcher.EnableRaisingEvents = true;
            }

            return _ServiceHost;
        }

        /// <summary>
        ///     force servicehost to rebuild & clear out all caches when something changes on the filesystem
        /// </summary>
        /// <param name="_FileSystemWatcher">Disposes of</param>
        /// <param name="_ServiceHost">Closes</param>
        private static void Reset(ICommunicationObject _ServiceHost)
        {
            // don't need to listen to events now that it's known something has changed, this will be Enabled again with someone taps the CreateServiceHost
            foreach (FileSystemWatcher _FileSystemWatcher in _FileSystemWatchers)
                _FileSystemWatcher.EnableRaisingEvents = false;

            if (_ServiceHost.State != CommunicationState.Closed)
                if (_ServiceHost.State != CommunicationState.Closing)
                    if (_ServiceHost.State != CommunicationState.Faulted)
                        _ServiceHost.Close();

            CacheMan.Clear();
        }
    }

    [ServiceKnownType("DocTypes", typeof (DocExchangeKnowTypeProber))]
    public class DocExchange : IDocExchange
    {
        public List<LightDoc> Audit(string DocTypeName, string DocId, string RelayUrl = null) { return ServiceController.Instance.Audit(DocTypeName, DocId, RelayUrl); }

        public DocTypeInfo Info(string DocTypeName) { return ServiceController.Instance.Info(DocTypeName); }

        public List<LightDoc> List(List<string> DocTypeNames, Dictionary<string, List<string>> DocKeys = null, Dictionary<string, List<string>> DocProperties = null, string KeyWord = null, int PageSize = 150, int PageIndex = 0, string RelayUrl = null) { return List_With_DocSrc(DocTypeNames, DocKeys, DocProperties, KeyWord, PageSize, PageIndex, RelayUrl).ToList(); }

        private static IEnumerable<LightDoc> List_With_DocSrc(List<string> DocTypeNames, Dictionary<string, List<string>> DocKeys = null, Dictionary<string, List<string>> DocProperties = null, string KeyWord = null, int PageSize = 150, int PageIndex = 0, string RelayUrl = null)
        {
            foreach (LightDoc _LightDoc in ServiceController
                .Instance
                .List(DocTypeNames, DocKeys, DocProperties, KeyWord, PageSize, PageIndex, RelayUrl))
            {
                _LightDoc.DocSrc = Nav.ToUrl(_LightDoc.DocTypeName, _LightDoc.DocId, RelayUrl);
                yield return _LightDoc;
            }
            ;
        }

        public BaseDoc Read(out string DocSrc, string DocData, out Dictionary<string, string> DocKeys, string RelayUrl = null) { return (BaseDoc) ServiceController.Instance.Read(out DocSrc, DocData, out DocKeys, RelayUrl); }

        public LightDoc Status(string DocTypeName, string DocId, bool DocStatus, string DocSubmittedBy, string RelayUrl = null) { return ServiceController.Instance.Status(DocTypeName, DocId, DocStatus, DocSubmittedBy, RelayUrl); }

        public LightDoc Submit(string DocData, string DocSubmittedBy, string RelayUrl = null, bool? DocStatus = null, DateTime? SubmittedDate = null, Dictionary<string, string> DocKeys = null, string DocTitle = null) { return ServiceController.Instance.Submit(DocData, DocSubmittedBy, RelayUrl, DocStatus, SubmittedDate, DocKeys, DocTitle); }

        public BaseDoc Create(out string DocSrc, Dictionary<string, string> DocKeys, BaseDoc Doc, string RelayUrl = null) { return (BaseDoc) ServiceController.Instance.Create(out DocSrc, Doc, DocKeys, RelayUrl); }

        public BaseDoc Get(out string DocSrc, out Dictionary<string, string> DocKeysFromDocId, string DocTypeName, Dictionary<string, string> DocKeys = null, string DocId = null, string RelayUrl = null) { return (BaseDoc) ServiceController.Instance.Get(out DocSrc, out DocKeysFromDocId, DocTypeName, DocKeys, DocId, RelayUrl); }
    }

    /// <summary>
    ///     List is built from
    /// </summary>
    public static class DocExchangeKnowTypeProber
    {
        public static IEnumerable<Type> DocTypes(ICustomAttributeProvider provider) { return InfoController.DocTypeServedItems(); }
    }

    [ServiceContract]
    [ServiceKnownType("DocTypes", typeof (DocExchangeKnowTypeProber))]
    public interface IDocExchange
    {
        [OperationContract]
        List<LightDoc> Audit(string DocTypeName, string DocId, string RelayUrl = null);

        [OperationContract]
        BaseDoc Create(out string DocSrc, Dictionary<string, string> DocKeys, BaseDoc Doc, string RelayUrl = null);

        [OperationContract]
        BaseDoc Get(out string DocSrc, out Dictionary<string, string> DocKeysFromDocId, string DocTypeName, Dictionary<string, string> DocKeys = null, string DocId = null, string RelayUrl = null);

        [OperationContract]
        DocTypeInfo Info(string DocTypeName);

        [OperationContract]
        List<LightDoc> List(List<string> DocTypeNames, Dictionary<string, List<string>> DocKeys = null, Dictionary<string, List<string>> DocProperties = null, string KeyWord = null, int PageSize = 150, int PageIndex = 0, string RelayUrl = null);

        [OperationContract]
        BaseDoc Read(out string DocSrc, string DocData, out Dictionary<string, string> DocKeys, string RelayUrl = null);

        [OperationContract]
        LightDoc Status(string DocTypeName, string DocId, bool DocStatus, string DocSubmittedBy, string RelayUrl = null);

        [OperationContract]
        LightDoc Submit(string DocData, string DocSubmittedBy, string RelayUrl = null, bool? DocStatus = null, DateTime? SubmittedDate = null, Dictionary<string, string> DocKeys = null, string DocTitle = null);
    }
}