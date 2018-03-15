using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMEGo.Sunflower.LinkStation10.Common
{
    public class CachedDesktopIniLoader
    {
        private class IconConfig
        {
            public string IconPath;
            public int IconIndex;
            public string InfoTip;
            public string LocalizedResourceName;
        }
        private static Dictionary<string, IconConfig> cache = new Dictionary<string, IconConfig>();
        public static string GetIconPathConfig(string path)
        {
            if (cache.ContainsKey(path))
            {
                return cache[path].IconPath;
            }
            else
            {
                return GetConfigAndCache(path)?.IconPath;
            }
        }
        public static int GetIconIndexConfig(string path)
        {
            if (cache.ContainsKey(path))
            {
                return cache[path].IconIndex;
            }
            else
            {
                var index = GetConfigAndCache(path)?.IconIndex;
                if (index.HasValue)
                {
                    return index.Value;
                }
                else {
                    return 0;
                }
            }
        }
        public static string GetInfoTipConfig(string path)
        {
            if (cache.ContainsKey(path))
            {
                return cache[path].InfoTip;
            }
            else
            {
                return GetConfigAndCache(path)?.InfoTip;
            }
        }
        public static string GetLocalizedResourceNameConfig(string path)
        {
            if (cache.ContainsKey(path))
            {
                return cache[path].LocalizedResourceName;
            }
            else
            {
                return GetConfigAndCache(path)?.LocalizedResourceName;
            }
        }
        private static IconConfig GetConfigAndCache(string path) {
            try
            {
                var parser = new IniDataParser();
                var data = parser.Parse(File.ReadAllText(Path.Combine(path, "desktop.ini")));
                var res = data[".ShellClassInfo"]["IconResource"];
                var file = "";
                var index = "";
                if (string.IsNullOrWhiteSpace(res))
                {
                    file = data[".ShellClassInfo"]["IconFile"];
                    index = data[".ShellClassInfo"]["IconIndex"];
                }
                else {
                    var lasti = res.LastIndexOf(',');
                    file = res.Substring(0, lasti);
                    index = res.Substring(lasti + 1).Trim();
                }
                var result = new IconConfig {
                    IconPath = file,
                    IconIndex = Convert.ToInt32(index),
                    InfoTip = data[".ShellClassInfo"]["InfoTip"],
                    LocalizedResourceName = data[".ShellClassInfo"]["LocalizedResourceName"]
                };
                cache[path] = result;
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
