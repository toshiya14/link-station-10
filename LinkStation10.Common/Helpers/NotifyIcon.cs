using System;
using System.Windows.Forms;
using NICon = System.Windows.Forms.NotifyIcon;

namespace RMEGo.Sunflower.LinkStation10.Common
{
    public class NotifyIcon
    {
        private static NICon _icon;
        private static object locker = new object();
        public static NICon Icon {
            get {
                if (_icon == null)
                {
                    lock (locker)
                    {
                        if (_icon == null)
                        {
                            _icon = new NICon();
                            _icon.Text = "Link Station 10 后台养咸鱼中……";
                            _icon.Visible = false;
                            _icon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
                        }
                    }
                }
                return _icon;
            }
        }

        public static void ShowIcon()
        {
            Icon.Visible = true;
            Icon.ShowBalloonTip(1000, "Link Station 10 放在后台啦", "你可以点击通知栏上的小图标再次激活我哦。", ToolTipIcon.Info);
        }

        public static void HideIcon()
        {
            Icon.Visible = false;
        }

        public static void RegisterDoubleClick(Action<NICon> ac) {
            Icon.DoubleClick += (s, a) => {
                ac(Icon);
            };
        }
    }
}
