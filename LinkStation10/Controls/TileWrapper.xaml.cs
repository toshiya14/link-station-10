using RMEGo.Sunflower.LinkStation10.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RMEGo.Sunflower.LinkStation10.Controls
{
    /// <summary>
    /// TileWrapper.xaml 的交互逻辑
    /// </summary>
    public partial class TileWrapper : UserControl
    {
        public TileWrapper()
        {
            InitializeComponent();
            var popout = TryFindResource("popout") as Storyboard;
            popout.Completed += (s, a) => {
                PopoutCompletedCallback?.Invoke(this);
            };
            var popin = TryFindResource("popin") as Storyboard;
            popin.Completed += (s, a) => {
                PopinCompletedCallback?.Invoke(this);
            };
        }

        private Action<TileWrapper> OnCollapsedClick = null;
        private Action<TileWrapper> PopoutCompletedCallback = null;
        private Action<TileWrapper> PopinCompletedCallback = null;

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnCollapsedClick?.Invoke(this);
            Collapsed();
        }
       
        public void Expand(Action<TileWrapper> callback = null) {
            var popout = TryFindResource("popout") as Storyboard;
            PopoutCompletedCallback = callback;
            popout.Begin();
        }

        public void Collapsed(Action<TileWrapper> callback = null) {
            var popin = TryFindResource("popin") as Storyboard;
            PopinCompletedCallback = callback;
            popin.Begin();
        }

        public void RegisterClickCollapse(Action<TileWrapper> action) {
            OnCollapsedClick = action;
        }
    }
}
