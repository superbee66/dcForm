using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Deployment.Compression;
using Microsoft.Deployment.Compression.Cab;

namespace dCForm.Util.Cab
{
    public class ArchiveMemoryStreamContext : ArchiveFileStreamContext, IDisposable
    {
        private Dictionary<string, MemoryStream> _DictionaryStringMemoryStream = new Dictionary<string, MemoryStream>();

        public Dictionary<string, MemoryStream> DictionaryStringMemoryStream {
            get { return _DictionaryStringMemoryStream; }

            set { _DictionaryStringMemoryStream = value; }
        }

        public ArchiveMemoryStreamContext(string archiveFile)
            : base(archiveFile)
        { }

        public ArchiveMemoryStreamContext(string archiveFile, string directory, IDictionary<string, string> files)
            : base(archiveFile, directory, files)
        { }

        public ArchiveMemoryStreamContext(IList<string> archiveFiles, string directory, IDictionary<string, string> files)
            : base(archiveFiles, directory, files)
        { }

        public override void CloseArchiveWriteStream(int archiveNumber, string archiveName, Stream stream)
        {
            //don't want to close the stream as someone will want it when this is done.
            //disposable will eventually close it
        }

        public override Stream OpenArchiveWriteStream(int archiveNumber, string archiveName, bool truncate, CompressionEngine compressionEngine)
        {
            if (string.IsNullOrEmpty(archiveName))
                throw new ArgumentNullException("archiveName");
            string path = Path.Combine(Path.GetDirectoryName(ArchiveFiles[0]), archiveName);

            if (truncate)
                if (!DictionaryStringMemoryStream.ContainsKey(path))
                    DictionaryStringMemoryStream[path] = new MemoryStream();

            Stream stream = DictionaryStringMemoryStream[path];
            if (EnableOffsetOpen)
            {
                long num = compressionEngine.FindArchiveOffset(new DuplicateStream(stream));
                if (num < 0L)
                    num = stream.Length;
                if (num > 0L)
                    stream = new OffsetStream(stream, num);
                stream.Seek(0L, SeekOrigin.Begin);
            }
            if (truncate)
                stream.SetLength(0L);
            return stream;
        }

        public override Stream OpenArchiveReadStream(int archiveNumber, string archiveName, CompressionEngine compressionEngine)
        {
            if (archiveNumber >= ArchiveFiles.Count)
                return null;

            string path = ArchiveFiles[archiveNumber];
            Stream stream = DictionaryStringMemoryStream[path];
            if (EnableOffsetOpen)
            {
                long num = compressionEngine.FindArchiveOffset(new DuplicateStream(stream));
                if (num > 0L)
                    stream = new OffsetStream(stream, num);
                else
                    stream.Seek(0L, SeekOrigin.Begin);
            }
            return stream;
        }

        public void Dispose()
        {
            foreach (MemoryStream _MemoryStream in DictionaryStringMemoryStream.Values)
                _MemoryStream.Dispose();
        }
    }


    public static class CabInfoExtensions
    {
        private static IList<string> GetRelativeFilePathsInDirectoryTree(string dir, bool includeSubdirectories)
        {
            IList<string> list = new List<string>();
            RecursiveGetRelativeFilePathsInDirectoryTree(dir, string.Empty, includeSubdirectories, list);
            return list;
        }

        private static void RecursiveGetRelativeFilePathsInDirectoryTree(string dir, string relativeDir, bool includeSubdirectories, IList<string> fileList)
        {
            string[] files = Directory.GetFiles(dir);
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                string fileName = Path.GetFileName(path);
                fileList.Add(Path.Combine(relativeDir, fileName));
            }
            if (includeSubdirectories)
            {
                string[] directories = Directory.GetDirectories(dir);
                for (int j = 0; j < directories.Length; j++)
                {
                    string path2 = directories[j];
                    string fileName2 = Path.GetFileName(path2);
                    RecursiveGetRelativeFilePathsInDirectoryTree(Path.Combine(dir, fileName2), Path.Combine(relativeDir, fileName2), includeSubdirectories, fileList);
                }
            }
        }

        public static void Pack(this ArchiveInfo o, string sourceDirectory, IPackStreamContext ipackstreamcontext) { o.Pack(sourceDirectory, false, CompressionLevel.Max, null); }

        public static void Pack(this ArchiveInfo o, string sourceDirectory, bool includeSubdirectories, CompressionLevel compLevel, EventHandler<ArchiveProgressEventArgs> progressHandler, IPackStreamContext ipackstreamcontext)
        {
            IList<string> relativeFilePathsInDirectoryTree = GetRelativeFilePathsInDirectoryTree(sourceDirectory, includeSubdirectories);
            o.PackFiles(sourceDirectory, relativeFilePathsInDirectoryTree, relativeFilePathsInDirectoryTree, compLevel, progressHandler);
        }

        public static void Pack(this ArchiveInfo o, string sourceDirectory, IList<string> sourceFileNames, IList<string> fileNames, IPackStreamContext ipackstreamcontext) { o.PackFiles(sourceDirectory, sourceFileNames, fileNames, CompressionLevel.Max, null); }

        private static IDictionary<string, string> CreateStringDictionary(IList<string> keys, IList<string> values)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            checked
            {
                for (int i = 0; i < keys.Count; i++)
                    dictionary.Add(keys[i], values[i]);
                return dictionary;
            }
        }

        public static void Pack(this ArchiveInfo o, string sourceDirectory, IList<string> sourceFileNames, IList<string> fileNames, CompressionLevel compLevel, EventHandler<ArchiveProgressEventArgs> progressHandler, IPackStreamContext ipackstreamcontext)
        {
            if (sourceFileNames == null)
                throw new ArgumentNullException("sourceFileNames");
            checked
            {
                if (fileNames == null)
                {
                    string[] array = new string[sourceFileNames.Count];
                    for (int i = 0; i < sourceFileNames.Count; i++)
                        array[i] = Path.GetFileName(sourceFileNames[i]);
                    fileNames = array;
                }
                else if (fileNames.Count != sourceFileNames.Count)
                    throw new ArgumentOutOfRangeException("fileNames");
                using (CompressionEngine compressionEngine = new CabEngine())
                {
                    compressionEngine.Progress += progressHandler;
                    IDictionary<string, string> files = CreateStringDictionary(fileNames, sourceFileNames);
                    ArchiveFileStreamContext archiveFileStreamContext = new ArchiveFileStreamContext(o.FullName, sourceDirectory, files);
                    archiveFileStreamContext.EnableOffsetOpen = true;
                    compressionEngine.CompressionLevel = compLevel;
                    compressionEngine.Pack(archiveFileStreamContext, fileNames);
                }
            }
        }
    }
}