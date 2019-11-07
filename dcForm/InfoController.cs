using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dCForm.Client;
using dCForm.Client.Util;
using dCForm.Format;
using dCForm.Format.XsnXml;
using dCForm.Template;
using dCForm.Template.Filesystem;

namespace dCForm
{
    public class InfoController
    {
        /// <summary>
        /// </summary>
        /// <returns>current DocTypeNames known to this system</returns>
        public static List<string> DocTypeNames()
        {
            return CacheMan.Cache(() => DocTypeServedItems().Select(typ => typ.Name).ToList(), false, "DocTypeNames");
        }

        /// <summary>
        ///     Types that can be actively served via WCF as "new" documents. There types must have a folder representation in the
        ///     file system. This list is cached internally. Before the list is constructed models and other contents are processed
        ///     & imported to the nosql database.
        /// </summary>
        /// <returns>current DocTypeNames known to this system</returns>
        public static List<Type> DocTypeServedItems()
        {
            return CacheMan.Cache(() =>
                                  {
                                      ImporterController.ImportDocModelsRunOnce();

                                      return Directory
                                          .EnumerateDirectories(FilesystemTemplateController.DirectoryPath)
                                          .Select(path => new DirectoryInfo(path).Name)
                                          .Select(DocTypeName => DocInterpreter.Instance.Create(DocTypeName).GetType())
                                          .ToList();
                                  }, false, "DocTypes");
        }

        public virtual DocTypeInfo Info(string DocDataOrTypeName)
        {
            //TODO:performance tune
            string DocTypeName = DocDataOrTypeName.IndexOf(' ') == -1 && DocTypeNames().Any(s => s == DocDataOrTypeName)
                                     ? DocDataOrTypeName
                                     : DocInterpreter.Instance.ReadDocTypeName(DocDataOrTypeName);

            string DocTypeVer = DocTypeName == DocDataOrTypeName
                                    ? TemplateController.Instance.TopDocRev(DocTypeName)
                                    : DocInterpreter.Instance.ReadRevision(DocDataOrTypeName);

            return Info(DocTypeName, DocTypeVer);
        }

        public virtual DocTypeInfo Info(string DocTypeName, string DocTypeVer)
        {
            return new DocTypeInfo
            {
                DocTypeName = DocTypeName,
                DocTypeVer = DocTypeVer,
                Description = DocInterpreter.Instance.GetDescription(DocTypeName),
                //TODO:source IsSignable from DocDataInterpreter.Instance
                IsSignable = DocInterpreter.Instance.Create(DocTypeName).IsSignable()
            };
        }
    }
}