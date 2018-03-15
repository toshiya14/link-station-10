using GalaSoft.MvvmLight;
using RMEGo.Sunflower.LinkStation10.Common;
using System.Collections.Generic;
using System.Linq;

namespace RMEGo.Sunflower.LinkStation10.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public List<MenuItem> TileList {
            get => _tilelist;
            set => Set(ref _tilelist, value); }
        private List<MenuItem> _tilelist;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            var refpath = @"C:\Users\v-zhica\Documents\TLB\APP";
            var ft = new FileTree().BuildFromRootFolder(refpath);
            TileList = ft.SubNode.Select(x => x.Value).ToList();
            
        }
    }
}