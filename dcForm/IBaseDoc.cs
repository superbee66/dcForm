using System.Collections.Generic;
using System.Reflection;


namespace dCForm
{
    public interface IBaseDoc
    {
        int DocChecksum { get; set; }

        string DocId { get; set; }

        List<DocKey> DocKey { get; set; }

        bool? DocStatus { get; set; }

        string DocTitle { get; set; }

        string GetDocData(string href = null);
        PropertyInfo[] GetFormFillableProperties(bool filled = false);
        PropertyInfo[] GetFormObjectMappedProperties(bool filled = false);
        PropertyInfo[] GetFormObjectNavProperties(bool filled = false);
        LightDoc ToLightDoc(string DocSrc = null);
    }
}