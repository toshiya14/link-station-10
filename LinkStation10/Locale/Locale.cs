using Newtonsoft.Json;
using RMEGo.Sunflower.LinkStation10;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RMEGo.Localization
{
    public class Locale
    {
        public static Locale GetLocale(string langname) {
            var path = Path.Combine(App.AppRoot, "lang", langname + ".locale");
            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<Locale>(File.ReadAllText(path));
            }
            else {
                return null;
            }
        }

        #region NotifyIcon
        public string NI_Text = "Link Station 10 正在后台养咸鱼";
        public string NI_MinTip = "Link Station 10 已经放置到后台了，你可以通过双击状态栏上的小图标再次激活。";
        #endregion
    }
}
