using RMEGo.Sunflower.LinkStation10.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RMEGo.Sunflower.LinkStation10
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Window
    {
        public static string DebugText;
        public Launch()
        {
            InitializeComponent();
            NotifyIcon.ShowIcon();
            this.ShowInTaskbar = false;

            NamedPipeHelper.AddProcessor(x => true, x => DebugText = x);
            NamedPipeHelper.StartListenThread();

            NotifyIcon.RegisterDoubleClick(e =>
            {
                TileBar.Expand();
                e.Visible = false;
            });

            TileBar.RegisterClickCollapse(e =>
            {
                NotifyIcon.ShowIcon();
            });

            TileBar.Expand(e => { e.Collapsed(); });
        }
    }
}
