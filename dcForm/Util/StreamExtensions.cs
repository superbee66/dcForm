using System.IO;

namespace dCForm.Client.Util
{
    public static class StreamExtensions
    {
        public static byte[] AsBytes(this Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using (MemoryStream _MemoryStream = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    _MemoryStream.Write(buffer, 0, read);
                return _MemoryStream.ToArray();
            }
        }
    }
}