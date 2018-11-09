/* Title: Gloo Framework
 * Author: Bradley Searle (C2 Digital)
 * Source: www.c2.co.zw
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/legalcode) */

using Agrishare.Core;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Agrishare.Web
{
    public class HttpModule : IHttpModule
    {
        private HttpContext Context;

        public void Init(HttpApplication Context)
        {
            Context.PostResolveRequestCache += new EventHandler(this.PostResolveRequestCache);
        }

        private void PostResolveRequestCache(Object sender, EventArgs e)
        {
            Context = HttpContext.Current;

            if (Regex.IsMatch(Context.Request.Path, @"^/(script|styles)-([\d]+).(js|css)$"))
                BundleMinifyCompress(Context.Request.Path);

            if (Context.Request.Path == "/")
                Context.RewritePath("/Pages/Homepage.aspx", false);
            else if (Regex.IsMatch(Context.Request.Path, @"^/(about|account)"))
                Context.RewritePath($"/Pages{Context.Request.Path}.aspx", false);
        }

        private void BundleMinifyCompress(string Path)
        {
            var debugMode = false;
            #if DEBUG
                debugMode = true;
            #endif

            var key = Regex.Replace(Path, @"^/(script|styles)-([\d]+).(js|css)$", "$1");
            var version = Convert.ToInt32(Regex.Replace(Path, @"^/(script|styles)-([\d]+).(js|css)$", "$2"));
            var extension = Regex.Replace(Path, @"^/(script|styles)-([\d]+).(js|css)$", "$3");
            var acceptEncoding = Context.Request.Headers["Accept-Encoding"];
            var useGzip = !acceptEncoding.IsEmpty() && acceptEncoding.ToLower().Contains("gzip");
            var cacheKey = $"{key}{useGzip}{version}";
            var compressedBytes = Context.Cache[cacheKey] as byte[];
            var contentType = extension.Equals("js") ? "application/javascript" : "text/css";

            if (debugMode || compressedBytes == null)
            {
                var output = string.Empty;
                switch (key)
                {
                    case "script":
                        {
                            output = Bundle("/Resources/Javascript", "js");
                            break;
                        }
                    case "styles":
                        {
                            output = Bundle("/Resources/Styles/", "css");
                            break;
                        }
                }
                
                output = Minify(output);

                if (useGzip && output.Length > 8192)
                    compressedBytes = Compress(output);
                else
                {
                    compressedBytes = Encoding.UTF8.GetBytes(output);
                    useGzip = false;
                }

                Context.Cache.Add(cacheKey, compressedBytes, null, DateTime.UtcNow.AddDays(90), TimeSpan.Zero, CacheItemPriority.High, null);
            }

            Context.Response.ContentType = contentType;

            if (useGzip)
                Context.Response.AppendHeader("Content-Encoding", "gzip");

            if (!debugMode)
            {
                Context.Response.ExpiresAbsolute = DateTime.UtcNow.AddDays(1);
                Context.Response.Cache.SetLastModified(DateTime.UtcNow);
                Context.Response.Cache.SetCacheability(HttpCacheability.Public);
            }

            Context.Response.BinaryWrite(compressedBytes);
            Context.Response.End();
        }

        public static string Bundle(string Folder, string Extension)
        {
            var builder = new StringBuilder();
            
            var path = Folder.StartsWith("/") ? HostingEnvironment.MapPath("/") + Folder : Folder;
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)                    
                    if (file.EndsWith("." + Extension))
                        using (var stream = File.OpenText(file))
                            builder.AppendLine(stream.ReadToEnd());

                var folders = Directory.GetDirectories(path);
                foreach (var folder in folders)
                    builder.AppendLine(Bundle(folder, Extension));
            }

            return builder.ToString();
        }

        public static string Minify(string Input)
        {
            Input = Input.Replace("\r\n", "\n");
            var lines = Input.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();
            foreach (var line in lines)
                if (!line.Trim().StartsWith("//"))
                    builder.Append(line.Trim() + "\n");
            return Regex.Replace(builder.ToString(), @"(\s)+", @" ");
        }

        private static byte[] Compress(string Input)
        {
            var buffer = Encoding.UTF8.GetBytes(Input);
            using (var stream = new MemoryStream())
            {
                using (var zip = new GZipStream(stream, CompressionMode.Compress))
                    zip.Write(buffer, 0, buffer.Length);
                return stream.ToArray();
            }            
        }

        /// <summary>
        /// REQUIRED - do not delete
        /// </summary>
        public void Dispose() { }
    }
    
}
