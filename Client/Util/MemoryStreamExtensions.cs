using System.IO;

namespace dCForm.Client.Util
{
    public static class MemoryStreamExtensions
    {
        public static string AsString(this MemoryStream _MemoryStream)
        {
            string s = null;

            if (_MemoryStream != null)
            {
                _MemoryStream.Position = 0;
                using (StreamReader _StreamReader = new StreamReader(_MemoryStream, true))
                    s = _StreamReader.ReadToEnd();
            }

            return s;
        }
    }
}