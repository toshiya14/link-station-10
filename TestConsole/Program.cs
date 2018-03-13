using Microsoft.Win32;
using RMEGo.Sunflower.LinkStation10.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RMEGo.Sunflower.LinkStation10.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new FileTree();
            var nodes = tree.BuildFromRootFolder(@"C:\Users\v-zhica\Documents\TLB\APP");
            Console.Read();
        }
        
    }
}
