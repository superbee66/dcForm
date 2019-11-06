using System;
using dCForm.Client.Util;
using dCForm.Core.Format.Json;
using dCForm.Core.Format.XsnXml;
using dCForm.Core.Template;

namespace dCForm.Core.Format
{
    public class DocInterpreter : IDocTextInterpreter
    {
        private static readonly Lazy<DocInterpreter> _Instance = new Lazy<DocInterpreter>(() => new DocInterpreter());

        private static readonly IDocTextInterpreter[] _otherInstances =
        {
            new XsnXmlInterpreter(), new JsonInterpreter()
        };

        public static DocInterpreter Instance = new DocInterpreter();

        public BaseDoc Read(string DocData, bool DocRevStrict = false) { return LocateInstance(DocData).Read(DocData, DocRevStrict); }

        public DocProcessingInstructions ReadDocPI(string srcDocData) { return LocateInstance(srcDocData).ReadDocPI(srcDocData); }

        public string WritePI(string srcDocData, DocProcessingInstructions pi) { return LocateInstance(srcDocData).WritePI(srcDocData, pi); }

        public string GetDescription(string DocTypeName) { return "GetDescription not implemented, the NotImplementedException() did not suite the cutover well."; }

        public string ReadDocTypeName(string DocData) { return LocateInstance(DocData).ReadDocTypeName(DocData); }

        public string ReadRevision(string DocData) { return LocateInstance(DocData).ReadRevision(DocData); }

        public string Write<T>(T source, bool includeProcessingInformation = true) where T : DocProcessingInstructions
        {
            string DocTypeName, DocRev;
            if (!RuntimeTypeNamer.TryParseDocNameAndRev(source.GetType().Namespace, out DocTypeName, out DocRev))
                throw new Exception("Can't determine DocTypeName/DocRev");

            return InstanceLocatorByName(DocTypeName, DocRev).Write(source, includeProcessingInformation);
        }

        public void Validate(string DocData) { LocateInstance(DocData).Validate(DocData); }

        public BaseDoc Create(string DocTypeName)
        {
            return InstanceLocatorByName(
                DocTypeName,
                TemplateController.Instance.TopDocRev(DocTypeName))
                .Create(DocTypeName);
        }

        public bool Processable(string DocTypeName, string DocRev) { return InstanceLocatorByName(DocTypeName, DocRev).Processable(DocTypeName, DocRev); }

        public string ContentType
        {
            get { return "application/text"; }
        }

        public string ContentFileExtension
        {
            get { return "dat"; }
        }

        public string HrefVirtualFilename(string DocTypeName, string DocRev) { return InstanceLocatorByName(DocTypeName, DocRev).HrefVirtualFilename(DocTypeName, DocRev); }

        internal static IDocTextInterpreter InstanceLocatorByName(string DocTypeName, string DocRev = null)
        {
            return CacheMan.Cache(() =>
                                  {
                                      if (DocTypeName == "DOCREV")
                                          return _otherInstances[0];
                                      foreach (IDocTextInterpreter _IDocDataInterpreter in _otherInstances)
                                          //TODO:Need a better way of discovering what IDocDataInterpreter can process the given document; it needs to consider the DocRev also
                                          if (_IDocDataInterpreter.Processable(DocTypeName, DocRev))
                                              return _IDocDataInterpreter;
                                      throw new Exception(string.Format("{0} could not locate a DocDataInterpreter to process the data passed", typeof (DocInterpreter).Name));
                                  }, false, "InstanceLocatorByName", DocTypeName, DocRev ?? string.Empty);
        }

        internal static IDocTextInterpreter LocateInstance(string DocData)
        {
            foreach (IDocTextInterpreter _IDocDataInterpreter in _otherInstances)
                if (!string.IsNullOrWhiteSpace(_IDocDataInterpreter.ReadDocTypeName(DocData)) && !string.IsNullOrWhiteSpace(_IDocDataInterpreter.ReadRevision(DocData)))
                    return _IDocDataInterpreter;

            throw new InterpreterLocationException();
        }
    }
}