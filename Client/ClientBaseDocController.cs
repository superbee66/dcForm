using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using dCForm.Client.Util;

namespace dCForm.Client
{
    /// <summary>
    ///     The only means of communications to the Core. No in memory/direct reference is every made
    ///     by dCForm.Client to dCForm.Core. All communication is over WCF. The WCF client proxy should
    ///     be generated & wrapped by this. It's the developer responsibility to do this.
    ///     When out parameters are present... Invokes underlying WCF proxied data operation utilizing named parameters as
    ///     VS2015 seems to sequentially reorder parameters when "out"(s) are present. It always places them at the end of the
    ///     parameter set.
    /// </summary>
    /// <typeparam name="TClientBaseT">Visual studio generated WCF client pointing to the Core's service URL</typeparam>
    public class ClientBaseDocController<TClientBaseT> : IDocController where TClientBaseT : ICommunicationObject
    {
        private readonly Type _UnderlyingControllerType;
        private readonly TClientBaseT _UnderlyingWSClient;
        private string _defaultRelayUrl = DCF_Relay.DCF_Relay.GetRelayUrl();

        public ClientBaseDocController(TClientBaseT ClientBaseT)
        {
            _UnderlyingWSClient = ClientBaseT;
            _UnderlyingControllerType = _UnderlyingWSClient.GetType();
        }

        public string DefaultRelayUrl {
            get { return _defaultRelayUrl; }
            set { _defaultRelayUrl = value; }
        }

        public TClientBaseT UnderlyingWSClient {
            get { return _UnderlyingWSClient; }
        }

        public LightDoc Submit(string DocData, string DocSubmittedBy, string RelayUrl = null, bool? DocStatus = null, DateTime? SubmittedDate = null, Dictionary<string, string> DocKeys = null, string DocTitle = null)
        {
            RelayUrl = string.IsNullOrWhiteSpace(RelayUrl)
                           ? DefaultRelayUrl
                           : RelayUrl;

            return (LightDoc)GetMethodInfo(DocCmd.Submit)
                                  .Invoke(UnderlyingWSClient,
                                      new object[]
                                      {
                                          DocData,
                                          DocSubmittedBy,
                                          RelayUrl,
                                          DocStatus,
                                          SubmittedDate,
                                          DocKeys,
                                          DocTitle
                                      });
        }

        public LightDoc Status(string DocTypeName, string DocId, bool DocStatus, string DocSubmittedBy, string RelayUrl = null)
        {
            return (LightDoc)GetMethodInfo(DocCmd.Status)
                                  .Invoke(UnderlyingWSClient,
                                      new object[]
                                      {
                                          DocTypeName,
                                          DocId,
                                          DocStatus,
                                          DocSubmittedBy,
                                          RelayUrl
                                      });
        }


        public object Read(out string DocSrc, string DocData, out Dictionary<string, string> DocKeys, string RelayUrl = null)
        {
            RelayUrl = string.IsNullOrWhiteSpace(RelayUrl)
                           ? DefaultRelayUrl
                           : RelayUrl;
            DocSrc = string.Empty;
            DocKeys = new Dictionary<string, string>();

            Dictionary<string, object> _Parms = new Dictionary<string, object>
            {
                {Parm.DocSrc, DocSrc},
                {Parm.DocData, DocData},
                {Parm.DocKeys, DocKeys},
                {Parm.RelayUrl, RelayUrl}
            };

            object Doc = GetMethodInfo(DocCmd.Read, Info(DocData).DocTypeName).Invoke(UnderlyingWSClient, _Parms);

            DocSrc = (string)_Parms[Parm.DocSrc];
            DocKeys = (Dictionary<string, string>)_Parms[Parm.DocKeys];

            return Doc;
        }

        public object Create(out string DocSrc, object Doc, Dictionary<string, string> DocKeys, string RelayUrl = null)
        {
            RelayUrl = string.IsNullOrWhiteSpace(RelayUrl)
                           ? DefaultRelayUrl
                           : RelayUrl;

            DocSrc = string.Empty;

            Dictionary<string, object> _Parms = new Dictionary<string, object>
            {
                {Parm.DocSrc, DocSrc},
                {Parm.Doc, Doc},
                {Parm.DocKeys, DocKeys},
                {Parm.RelayUrl, RelayUrl}
            };

            object DocReturn = GetMethodInfo(DocCmd.Create, Doc).Invoke(UnderlyingWSClient, _Parms);
            DocSrc = (string)_Parms[Parm.DocSrc];

            return DocReturn;
        }

        public object Get(out string DocSrc, out Dictionary<string, string> DocKeysFromDocId, string DocTypeName, Dictionary<string, string> DocKeys = null, string DocId = null, string RelayUrl = null)
        {
            RelayUrl = string.IsNullOrWhiteSpace(RelayUrl)
                           ? DefaultRelayUrl
                           : RelayUrl;

            DocKeysFromDocId = null;
            DocSrc = string.Empty;

            Dictionary<string, object> parms = new Dictionary<string, object>
            {
                {Parm.DocSrc, DocSrc},
                {"DocKeysFromDocId", DocKeysFromDocId},
                {Parm.DocTypeName, DocTypeName},
                {Parm.DocKeys, DocKeys},
                {Parm.DocId, DocId},
                {Parm.RelayUrl, RelayUrl}
            };

            object DocReturn = GetMethodInfo(DocCmd.Get, DocTypeName).Invoke(UnderlyingWSClient, parms);

            DocSrc = (string)parms[Parm.DocSrc];
            DocKeysFromDocId = (Dictionary<string, string>)parms["DocKeysFromDocId"];

            return DocReturn;
        }

        public List<LightDoc> List(List<string> DocTypes, Dictionary<string, List<string>> DocKeys = null, Dictionary<string, List<string>> DocProperties = null, string KeyWord = null, int PageSize = 150, int PageIndex = 0, string RelayUrl = null)
        {
            RelayUrl = string.IsNullOrWhiteSpace(RelayUrl)
                           ? DefaultRelayUrl
                           : RelayUrl;


            return (List<LightDoc>)GetMethodInfo(DocCmd.List)
                                        .Invoke(
                                            UnderlyingWSClient,
                                            new object[]
                                            {
                                                DocTypes,
                                                DocKeys,
                                                DocProperties,
                                                KeyWord,
                                                PageSize,
                                                PageIndex,
                                                RelayUrl
                                            });
        }

        public List<LightDoc> Audit(string DocTypeName, string DocId, string RelayUrl = null)
        {
            return (List<LightDoc>)GetMethodInfo(DocCmd.Audit)
                                        .Invoke(UnderlyingWSClient,
                                            new object[]
                                            {
                                                DocTypeName,
                                                DocId,
                                                RelayUrl
                                            });
        }

        public DocTypeInfo Info(string DocDataOrTypeName)
        {
            // don't cache the call if it appears to be a DocData payload being inquired about
            return DocDataOrTypeName.IndexOf(' ') != -1
                       ? (DocTypeInfo)GetMethodInfo(DocCmd.Info).Invoke(UnderlyingWSClient, new object[] { DocDataOrTypeName })
                       : CacheMan.Cache(() => (DocTypeInfo)GetMethodInfo(DocCmd.Info).Invoke(UnderlyingWSClient, new object[] { DocDataOrTypeName }), false, "DocTypeInfo", DocDataOrTypeName, DocCmd.Info);
        }

        /// <summary>
        ///     Download the actual InfoPath XML Document the desktop application would open. This
        ///     is performed over an System.Net.HttpWebRequest utilizing the same DocSrc (URL)
        ///     used to redirect browsers to download the InfoPath document.
        /// </summary>
        /// <param name="DocSrc"></param>
        /// <returns>Xml suitable for opening in Office InfoPath</returns>
        [Obsolete("Use the non-static GetDocText(string DocSrc)")]
        public static string GetDocXml(string DocSrc)
        {
            //TODO:Relocate this to a more appropriate class
            HttpWebRequest _HttpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(DocSrc));
            _HttpWebRequest.Proxy = new WebProxy { BypassList = new[] { _HttpWebRequest.RequestUri.ToString() }, BypassProxyOnLocal = true };
            using (HttpWebResponse _HttpWebResponse = (HttpWebResponse)_HttpWebRequest.GetResponse())
            using (Stream _Stream = _HttpWebResponse.GetResponseStream())
            using (StreamReader _StreamReader = new StreamReader(_Stream))
                return _StreamReader.ReadToEnd();
        }

        /// <summary>
        ///     Download the actual InfoPath XML Document the desktop application would open. This
        ///     is performed over an System.Net.HttpWebRequest utilizing the same DocSrc (URL)
        ///     used to redirect browsers to download the InfoPath document.
        /// </summary>
        /// <param name="DocSrc"></param>
        /// <returns>Xml suitable for opening in Office InfoPath</returns>
        public string GetDocText(string DocSrc)
        {
            //TODO:Relocate this to a more appropriate class
            HttpWebRequest _HttpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(DocSrc));
            _HttpWebRequest.Proxy = new WebProxy { BypassList = new[] { _HttpWebRequest.RequestUri.ToString() }, BypassProxyOnLocal = true };
            using (HttpWebResponse _HttpWebResponse = (HttpWebResponse)_HttpWebRequest.GetResponse())
            using (Stream _Stream = _HttpWebResponse.GetResponseStream())
            using (StreamReader _StreamReader = new StreamReader(_Stream))
                return _StreamReader.ReadToEnd();
        }

        public byte[] GetDocBytes(string DocSrc) { throw new NotImplementedException("future support for byte[] based documents (pdf/doc/docx) not implemented"); }

        private MethodInfo GetMethodInfo(DocCmd MethodPrefix, object Doc)
        {
            return GetMethodInfo(MethodPrefix, Doc.GetType().Name)
                   ?? GetMethodInfo(MethodPrefix);
        }

        /// <summary>
        ///     Get
        /// </summary>
        /// <param name="MethodPrefix"></param>
        /// <param name= NavKey.DocTypeName></param>
        /// <param name="DocTypeName"></param>
        /// <returns></returns>
        private MethodInfo GetMethodInfo(DocCmd MethodPrefix, string DocTypeName)
        {
            return _UnderlyingControllerType.GetMethod(string.Format("{0}{1}", MethodPrefix, DocTypeName))
                   ?? _UnderlyingControllerType.GetMethod(string.Format("{0}", MethodPrefix));
        }

        private MethodInfo GetMethodInfo(DocCmd MethodName) { return _UnderlyingControllerType.GetMethod(string.Format("{0}", MethodName)); }
    }
}