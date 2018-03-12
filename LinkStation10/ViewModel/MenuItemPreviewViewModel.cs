using GalaSoft.MvvmLight;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GDI = System.Drawing;

namespace RMEGo.Sunflower.LinkStation10.ViewModel
{
    public class MenuItemPreviewViewModel : ViewModelBase
    {
        public string IconPath {
            set {
                var b = new ImageBrush();
                var uri = new Uri(@"C:\Users\v-zhica\Desktop\firefox-logo-300x310.png");
                var img = new BitmapImage(uri);
                //b.TileMode = TileMode.None;
                //b.Stretch = Stretch.Uniform;
                //b.ImageSource = img;
                //icon = b;
                icon = img;
            }
        }
        public ImageSource Icon {
            set {
                var b = new ImageBrush();
                var uri = new Uri(@"C:\Users\v-zhica\Desktop\firefox-logo-300x310.png");
                var img = new BitmapImage(uri);
                //b.TileMode = TileMode.None;
                //b.Stretch = Stretch.Uniform;
                //b.ImageSource = img;
                //icon = b;
                icon = img;
            }
            get { return icon; }
        }
        private ImageSource icon;

        public string Title { set { Set(ref title, value); } get { return title; } }
        private string title;

        public string Description { set { Set(ref description, value); } get { return description; } }
        private string description;

        public MenuItemPreviewViewModel()
        {
            var b = new ImageBrush();
            var uri = new Uri(@"C:\Users\v-zhica\Desktop\firefox-logo-300x310.png");
            var img = new BitmapImage(uri);
            //b.TileMode = TileMode.None;
            //b.Stretch = Stretch.Uniform;
            //b.ImageSource = img;
            //icon = b;
            icon = img;
            Title = "Mozilla Firefox";
            Description = "使用 Quantum 内核的全新火狐浏览器。";
        }
    }
}