using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Agrishare.Core.Entities
{
    public class File
    {
        public string Name { get; set; }
        public string Extension { get; set; }

        public string Filename => $"{Name}{Extension}";
        public string ThumbName => $"{Name}-thumb{Extension}";
        public string ZoomName => $"{Name}-zoom{Extension}";

        public File(string Filepath)
        {
            Name = Path.GetFileNameWithoutExtension(Filepath);
            Extension = Path.GetExtension(Filepath);
        }

        public object JSON()
        {
            return new
            {
                Filename,
                Thumb = $"{Config.CDNURL}/{ThumbName}",
                Zoom = $"{Config.CDNURL}/{ZoomName}"
            };
        }

        #region Image 

        private string FilePath => $"{CDNAbsolutePath}{Filename}";

        private string MimeType
        {
            get
            {
                switch (Extension.ToLower())
                {
                    case ".tif":
                    case ".tiff": return "image/tiff";
                    case ".png": return "image/png";
                    default: return "image/jpeg";
                }
            }
        }

        private ImageCodecInfo Codec
        {
            get
            {
                var encoders = ImageCodecInfo.GetImageEncoders();
                for (var j = 0; j < encoders.Length; ++j)
                    if (encoders[j].MimeType == MimeType)
                        return encoders[j];
                return null;
            }
        }

        public bool Resize(int Width, int Height, string Destination, long Compression = 50L)
        {
            Image image = null;
            Bitmap bitmap = null;
            Graphics graphics = null;
            EncoderParameters encoder = null;
            FileStream stream = null;

            bool success = false;
            try
            {
                stream = new FileStream(MapPath(FilePath), FileMode.Open);
                image = Image.FromStream(stream, false, false);
                if (image == null)
                    return false;

                var imageWidth = Width;
                var imageHeight = (int)(((double)imageWidth / (double)image.Width) * image.Height);                
                if (imageHeight > Height)
                {
                    imageHeight = Height;
                    imageWidth = (int)(((double)imageHeight / (double)image.Height) * image.Width);
                }

                var offsetX = (imageWidth - Width) / 2;
                var offsetY = (imageHeight - Height) / 2;

                var attributes = new ImageAttributes();
                attributes.SetWrapMode(WrapMode.TileFlipXY);
                bitmap = new Bitmap(imageWidth, imageHeight);
                graphics = Graphics.FromImage(bitmap);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.FillRectangle(Brushes.White, 0, 0, imageWidth, imageHeight);
                graphics.DrawImage(image, new Rectangle(0, 0, imageWidth, imageHeight), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

                encoder = new EncoderParameters(1);
                encoder.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Compression);
                bitmap.Save(MapPath(Destination), Codec, encoder);
                success = true;
            }
            finally
            {
                if (stream != null) stream.Dispose();
                if (encoder != null) encoder.Dispose();
                if (graphics != null) graphics.Dispose();
                if (bitmap != null) bitmap.Dispose();
                if (image != null) image.Dispose();
            }

            return success;
        }

        public static string GetImageAsBase64(string Filename)
        {
            var path = CDNAbsolutePath + Filename;
            if (System.IO.File.Exists(path))
                using (var image = System.Drawing.Image.FromFile(path))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        return base64String;
                    }
                }
            return string.Empty;
        }

        public static string SaveBase64Image(string Input)
        {
            string[] parts = Input.Split(';');
            if (parts.Length != 2)
                return string.Empty;

            var extension = parts[0].Contains("png") ? ".png" : ".jpg";
            var fileName = Guid.NewGuid() + extension;
            var path = CDNAbsolutePath + fileName;
            try
            {
                var base64 = parts[1].Split(',')[1];
                System.IO.File.WriteAllBytes(path, Convert.FromBase64String(base64));
                return fileName;
            }
            catch (Exception ex)
            {
                Log.Error("Save Base64 Image", ex);
                return string.Empty;
            }

        }

        #endregion

        #region Utils

        private static string _cdnAbsolutePath { get; set; }
        private static string CDNAbsolutePath
        {
            get
            {
                if (_cdnAbsolutePath.IsEmpty())
                    _cdnAbsolutePath = Config.Find(Key: "CDN Absolute Path").Value;
                return _cdnAbsolutePath;
            }
        }

        private static string CreateDirectory(string DirectoryPath)
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

        private static string MapPath(string Path)
        {
            if (IsAbsolutePath(Path))
                return Path;

            Path = CDNAbsolutePath + Path;
            Path = Regex.Replace(Path, "/", "\\");
            return Regex.Replace(Path, @"[\\]+", "\\");
        }

        private static bool IsAbsolutePath(string Path)
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

        #endregion
    }
}
