using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ItemStruct = RMEGo.Sunflower.LinkStation10.Common.MenuItem;

namespace RMEGo.Sunflower.LinkStation10.Controls
{
    /// <summary>
    /// TileItem.xaml 的交互逻辑
    /// </summary>
    public partial class TileItem : UserControl
    {
        public ItemStruct ItemContent {
            get { return (ItemStruct)GetValue(ItemContentProperty); }
            set { SetValue(ItemContentProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemContentProperty =
            DependencyProperty.Register("ItemContent", typeof(ItemStruct), typeof(TileItem), new PropertyMetadata(default(ItemStruct), ItemContentChangedCallback));

        public TileItem()
        {
            InitializeComponent();
        }

        private static void ItemContentChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (args.NewValue != null)
            {
                var e = sender as TileItem;
                var value = args.NewValue as ItemStruct;
                using (var ms = new MemoryStream())
                {
                    var bitmap = new BitmapImage();
                    value.Icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    e.IconImage.Source = bitmap;
                }
            }
        }
    }
}
