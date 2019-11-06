using System;
using System.Linq;
using System.Reflection;
using dCForm.Client.Util;

namespace dCForm.Core.Format.XsnXml
{
    /// <summary>
    ///     Supports written signature detection & extraction from BaseDoc objects.
    /// </summary>
    internal static class BaseDocExtensions
    {
        /// <summary>
        ///     attempts to screen properties that follow the naming pattern & inner property structure InfoPath xsd signature
        ///     elements
        ///     converted to when processed by xsd.exe (_XsnToCSharp.tt) to yield the C# class.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetFormObjectSignatureProperties(BaseDoc o)
        {
            return CacheMan.Cache(() =>
                                  {
                                      return o
                                          .GetFormObjectMappedProperties()
                                          .Where(m => m.Name.StartsWith("signatures", StringComparison.InvariantCultureIgnoreCase))
                                          .ToArray();
                                  },
                false,
                o.DocTypeName,
                o.solutionVersion,
                "GetFormWetSignatureProperties");
        }

        /// <summary>
        /// </summary>
        /// <param name="o"></param>
        /// <returns>true of Doc object has properties to meant for written signature capture</returns>
        internal static bool IsSignable(this BaseDoc o) { return GetFormObjectSignatureProperties(o).Any(); }
    }
}