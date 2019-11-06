using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace dCForm.Client
{
    /// <summary>
    ///     Implemented at every tier of the dCForm architecture.
    ///     [x] ClientBaseDocController -> WCF-Client-Proxy -> ServiceController
    ///     [x] ServiceController -> Memory -> Entity Framework Code First
    ///     [0] Entity Framework Code First-> Database
    /// </summary>
    [ServiceContract]
    public interface IDocController
    {
        /// <summary>
        ///     Any & all submissions accepted for a given DocTypeName/DocId combination should be persisted. Pull up all those
        ///     submissions with this.
        /// </summary>
        /// <param name="DocTypeName"></param>
        /// <param name="DocId"></param>
        /// <param name="RelayUrl"></param>
        /// <returns></returns>
        [OperationContract]
        List<LightDoc> Audit(string DocTypeName, string DocId, string RelayUrl = null);

        object Create(out string DocSrc, object Doc, Dictionary<string, string> DocKeys = null, string RelayUrl = null);

        object Get(out string DocSrc, out Dictionary<string, string> DocKeysFromDocId, string DocTypeName, Dictionary<string, string> DocKeys = null, string DocId = null, string RelayUrl = null);

        /// <summary>
        ///     Microsoft's published manifest.xsf schema as an object. A manifest.xsf contains
        ///     is a document proprietary to Microsoft Office InfoPath. It bare's meta data
        ///     about the form's themselves. One useful property being the form's description
        /// </summary>
        /// <param name="DocTypeName"></param>
        /// <returns>object-form of the manifest.xsf</returns>
        [OperationContract]
        DocTypeInfo Info(string DocTypeName);

        [OperationContract]
        List<LightDoc> List(List<string> DocTypeNames, Dictionary<string, List<string>> DocKeys = null, Dictionary<string, List<string>> DocProperties = null, string KeyWord = null, int PageSize = 150, int PageIndex = 0, string RelayUrl = null);

        object Read(out string DocSrc, string DocData, out Dictionary<string, string> DocKeys, string RelayUrl = null);

        [OperationContract]
        LightDoc Status(string DocTypeName, string DocId, bool DocStatus, string DocSubmittedBy, string RelayUrl = null);

        [OperationContract]
        LightDoc Submit(string DocData, string DocSubmittedBy, string RelayUrl = null, bool? DocStatus = null, DateTime? SubmittedDate = null, Dictionary<string, string> DocKeys = null, string DocTitle = null);
    }
}