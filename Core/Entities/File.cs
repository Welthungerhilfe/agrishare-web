using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Agrishare.Core.Entities
{
    public class File
    {
        private static string _cdnAbsolutePath { get; set; }
        public static string CDNAbsolutePath
        {
            get
            {
                if (_cdnAbsolutePath.IsEmpty())
                    _cdnAbsolutePath = Config.Find(Key: "CDN Absolute Path").Value;
                return _cdnAbsolutePath;
            }
        }

        public static string CreateDirectory(string DirectoryPath)
        {
            DirectoryPath = MapPath(DirectoryPath);

            var pathBuilder = new StringBuilder();

            var folders = Path.GetDirectoryName(DirectoryPath).Split('\\');
            foreach (var folder in folders)
            {
                pathBuilder.Append(folder);
                pathBuilder.Append("\\");
                if (!Directory.Exists(pathBuilder.ToString()))
                    Directory.CreateDirectory(pathBuilder.ToString());
            }

            return DirectoryPath;
        }

        public static string MapPath(string Path)
        {
            if (IsAbsolutePath(Path))
                return Path;

            Path = CDNAbsolutePath + Path;
            Path = Regex.Replace(Path, "/", "\\");
            return Regex.Replace(Path, @"[\\]+", "\\");
        }

        public static bool IsAbsolutePath(string Path)
        {
            return Regex.IsMatch(Path, @"^[a-zA-Z]+:");
        }

        public static string Save(string Filename, byte[] Data)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Filename);
            var filePath = MapPath(fileName);
            System.IO.File.WriteAllBytes(filePath, Data);
            return fileName;
        }

        public static string FormatFileSize(double Bytes)
        {
            if (Bytes < 1024)
                return $"{Bytes}bytes";

            var kb = Bytes / 1024;
            if (kb < 1024)
                return $"{Math.Ceiling(kb)}KB";

            var mb = kb / 1024;
            if (mb < 1024)
                return $"{Math.Ceiling(mb)}MB";

            var gb = mb / 1024;
            return $"{Math.Round(gb, 2)}GB";
        }
    }
}
