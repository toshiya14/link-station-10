using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMEGo.Sunflower.LinkStation10.Common.FileTree
{
    public enum ItemType
    {
        Context,
        Entry
    }
    public struct MenuItem
    {
        public Image Icon;
        public string Name;
        public ItemType Type;
        public string StartCommand;
    }
    public class FileTree
    {
        private static Image DirIcon {
            get => Image.FromFile(@"../../../Assets/folder.png");
        }

        public Node<MenuItem> BuildFromRootFolder(string rootpath)
        {
            if (!isFolder(rootpath))
            {
                throw new DirectoryNotFoundException("Could not find the directory: " + rootpath);
            }
            var root = new Node<MenuItem>("<root>", new MenuItem());
            FillTree(root, rootpath);
            return root;
        }
        private static void FillTree(Node<MenuItem> node, string refpath)
        {
            if (isFolder(refpath))
            {
                var dirinfo = new DirectoryInfo(refpath);
                foreach (var dir in dirinfo.GetDirectories())
                {
                    if (dir.Name.StartsWith(".") || dir.Attributes.HasFlag(FileAttributes.Hidden) || dir.Attributes.HasFlag(FileAttributes.System))
                    {
                        continue;
                    }
                    node[dir.Name].Value = new MenuItem
                    {
                        Icon = GetDirIcon(dir.FullName),
                        Name = GetDirName(dir.FullName),
                        Type = ItemType.Context
                    };
                    FillTree(node[dir.Name], dir.FullName);
                }

                foreach (var file in dirinfo.GetFiles())
                {
                    if (file.Name.StartsWith(".") || file.Attributes.HasFlag(FileAttributes.Hidden) || file.Attributes.HasFlag(FileAttributes.System))
                    {
                        continue;
                    }
                    node[file.Name].Value = new MenuItem
                    {
                        Icon = GetFileIcon(file.FullName),
                        Name = GetFileName(file.FullName),
                        Type = ItemType.Entry,
                        StartCommand = GetFileCommand(file.FullName)
                    };
                }
            }
        }
        private static bool isFolder(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private static Image GetFileIcon(string filename) {
            var fileinfo = new FileInfo(filename);
            var ext = fileinfo.Extension;
            return IconHandler.IconHandler.IconFromExtension(ext, IconHandler.IconSize.Large).ToBitmap();
        }
        private static string GetFileName(string filename)
        {
            var fileinfo = new FileInfo(filename);
            return fileinfo.Name;
        }
        private static Image GetDirIcon(string filename) {
            throw new NotImplementedException();
        }
        private static string GetDirName(string filename) {
            throw new NotImplementedException();
        }
        private static string GetFileCommand(string filename) {
            throw new NotImplementedException();
        }
    }
}
