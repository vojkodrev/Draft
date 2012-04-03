using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Draft.Pictures
{
    public class PictureCache
    {
        public const string CACHE_DIR = "cache";

        public static Bitmap GetPicture(string name, string fallbackUrl)
        {
            if (!Directory.Exists(CACHE_DIR))
                Directory.CreateDirectory(CACHE_DIR);

            name = name + ".jpg";
            string fullPath = Path.Combine(CACHE_DIR, name);

            if (File.Exists(fullPath))
                return new Bitmap(fullPath);

            Bitmap downloadedPicture = Http.HttpHelper.DownloadPicture(fallbackUrl);
            downloadedPicture.Save(fullPath);

            return downloadedPicture;
        }
        public static Bitmap GetPictureDirectlyFromCache(string name)
        {
            if (!Directory.Exists(CACHE_DIR))
                Directory.CreateDirectory(CACHE_DIR);

            name = name + ".jpg";
            string fullPath = Path.Combine(CACHE_DIR, name);

            return new Bitmap(fullPath);
        }
        public static bool ContainsPicture(string name)
        {
            if (!Directory.Exists(CACHE_DIR))
                Directory.CreateDirectory(CACHE_DIR);

            name = name + ".jpg";
            string fullPath = Path.Combine(CACHE_DIR, name);

            return File.Exists(fullPath);
        }

    }
}
