using System.IO;

namespace dCForm.Util
{
    public static class StringExtensions
    {
        public static MemoryStream AsStream(this string s)
        {
            MemoryStream _MemoryStream = new MemoryStream();
            StreamWriter _StreamWriter = new StreamWriter(_MemoryStream)
            {
                AutoFlush = true
            };
            _StreamWriter.Write(s);
            _StreamWriter.Flush();
            _MemoryStream.Position = 0;
            return _MemoryStream;
        }
    }
}